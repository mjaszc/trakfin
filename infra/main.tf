terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.1.0"
    }
  }
}

variable "subscription_id" {}
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
  admin_enabled       = false

  provisioner "local-exec" {
    command = "sh ../scripts/push_api_container_to_acr.sh"
  }
}

resource "azurerm_container_registry" "acr_web" {
  name                = "TrakfinWeb"
  resource_group_name = azurerm_resource_group.rg.name
  location            = azurerm_resource_group.rg.location
  sku                 = "Basic"
  admin_enabled       = false

  provisioner "local-exec" {
    command = "sh ../scripts/push_web_container_to_acr.sh"
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

resource "azurerm_api_management_api" "example" {
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

