terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "4.1.0"
    }
  }
}

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
