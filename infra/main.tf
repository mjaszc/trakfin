terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.1.0"
    }
  }
}

variable "subscription_id" {}
variable "api_url" {}
variable "mssql_admin_login" {}
variable "mssql_admin_password" {}
variable "mssql_connection_string" {}
variable "publisher_email" {}
variable "publisher_name" {}

provider "azurerm" {
  subscription_id = var.subscription_id
  features {}
}

resource "azurerm_resource_group" "rg" {
  name     = "Trakfin-RG"
  location = "westeurope"
}


resource "azurerm_container_registry" "acr_api" {
  name                = "TrakfinAPI"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true

  provisioner "local-exec" {
    command = "sh ../scripts/push_api_container_to_acr.sh"
  }
}

resource "azurerm_container_registry" "acr_web" {
  name                = "TrakfinWeb"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = true

  provisioner "local-exec" {
    command = "sh ../scripts/push_web_container_to_acr.sh"
  }
}

resource "azurerm_user_assigned_identity" "assigned_id" {
  name                = "jasmar2"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
}


resource "azurerm_mssql_server" "trakfin" {
  name                         = "trakfinserver"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = "polandcentral"
  version                      = "12.0"
  administrator_login          = var.mssql_admin_login
  administrator_login_password = var.mssql_admin_password
  minimum_tls_version          = "1.2"
}


resource "azurerm_mssql_database" "db" {
  name                 = "TrakfinDB"
  server_id            = azurerm_mssql_server.trakfin.id
  collation            = "SQL_Latin1_General_CP1_CI_AS"
  max_size_gb          = 2
  sku_name             = "Basic"
  zone_redundant       = false
  storage_account_type = "Local"

  # prevent the possibility of accidental data loss
  lifecycle {
    prevent_destroy = true
  }
}


data "azurerm_client_config" "current" {}

# Create a key vault with access policies which allow for the current user to get, list, create, delete, update, recover, purge and getRotationPolicy for the key vault key and also add a key vault access policy for the Microsoft Sql Server instance User Managed Identity to get, wrap, and unwrap key(s)
resource "azurerm_key_vault" "kv" {
  name                       = "TrakfinKV"
  location                   = azurerm_resource_group.rg.location
  resource_group_name        = azurerm_resource_group.rg.name
  tenant_id                  = data.azurerm_client_config.current.tenant_id
  sku_name                   = "standard"
  soft_delete_retention_days = 7

  access_policy {
    tenant_id = data.azurerm_client_config.current.tenant_id
    object_id = data.azurerm_client_config.current.object_id

    key_permissions = [
      "Create",
      "Get",
    ]

    secret_permissions = [
      "Set",
      "Get",
      "Delete",
      "Purge",
      "Recover",
      "List"
    ]
  }
}

resource "azurerm_api_management" "api" {
  name                = "TrakfinAPI"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  publisher_email     = var.publisher_email
  publisher_name      = var.publisher_name
  sku_name            = "Developer_1"
}

resource "azurerm_api_management_api" "api_management" {
  name                = "TrakfinAPI"
  resource_group_name = azurerm_resource_group.rg.name
  api_management_name = azurerm_api_management.api.name
  revision            = "1"
  display_name        = "Trakfin API"
  protocols           = ["https"]

  import {
    content_format = "openapi"
    content_value  = file("${path.module}/swagger.json")
  }
}

resource "azurerm_service_plan" "sp" {
  name                = "TrakfinServicePlan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  os_type             = "Linux"
  sku_name            = "F1"
}

resource "azurerm_linux_web_app" "api_app" {
  name                = "TrakfinAPI"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_service_plan.sp.location
  service_plan_id     = azurerm_service_plan.sp.id

  site_config {
    always_on = false
    # Link the api management straight to the api
    api_management_api_id = azurerm_api_management_api.api_management.id

    application_stack {
      docker_image_name        = "trakfinapi:latest"
      docker_registry_url      = "https://${azurerm_container_registry.acr_api.login_server}"
      docker_registry_username = azurerm_container_registry.acr_api.admin_username
      docker_registry_password = azurerm_container_registry.acr_api.admin_password
    }
  }

  app_settings = {
    "TrakfinContext" = var.mssql_connection_string
  }
}

resource "azurerm_linux_web_app" "web_app" {
  name                = "TrakfinWeb"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_service_plan.sp.location
  service_plan_id     = azurerm_service_plan.sp.id

  site_config {
    always_on = false

    application_stack {
      docker_image_name        = "trakfinweb:latest"
      docker_registry_url      = "https://${azurerm_container_registry.acr_web.login_server}"
      docker_registry_username = azurerm_container_registry.acr_web.admin_username
      docker_registry_password = azurerm_container_registry.acr_web.admin_password
    }
  }

  app_settings = {
    "API_URL" = var.api_url
    "TrakfinContext" = var.mssql_connection_string
  }

  # Retrieve TrakfinContext variable from secrets
}

resource "azurerm_kubernetes_cluster" "aks_web" {
  name                = "trakfin-web-aks"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  dns_prefix          = "trakfinweb"

  default_node_pool {
    name       = "default"
    node_count = 1
    vm_size    = "Standard_D2_v2"
  }

  identity {
    type = "SystemAssigned"
  }
}

resource "azurerm_role_assignment" "aks_web_ra" {
  principal_id                     = azurerm_kubernetes_cluster.aks.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                            = azurerm_container_registry.acr_web.id
  skip_service_principal_aad_check = true
}

resource "azurerm_kubernetes_cluster" "aks_api" {
  name                = "trakfin-api-aks"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  dns_prefix          = "trakfinapi"

  default_node_pool {
    name       = "default"
    node_count = 1
    vm_size    = "Standard_D2_v2"
  }

  identity {
    type = "SystemAssigned"
  }
}

resource "azurerm_role_assignment" "aks_api_ra" {
  principal_id                     = azurerm_kubernetes_cluster.aks.kubelet_identity[0].object_id
  role_definition_name             = "AcrPull"
  scope                            = azurerm_container_registry.acr_api.id
  skip_service_principal_aad_check = true
}
