terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  # The "feature" block is required for AzureRM provider 2.x. 
  # If you are using version 1.x, the "features" block is not allowed.
  # version = "~>2.0"
  features {}
  # Use Azure CLI to authencation
  subscription_id = "ffc7fbc7-3840-4835-ad88-4eb5015d7dac"

}

locals {
  group_name  = "DemoTf-${formatdate("MMDDHHmm", timestamp())}"
  location    = "eastasia"
  random_name = "__random__"
  # random_name             = random_string.rid.result
}

resource "random_string" "rid" {
  length  = 3
  special = false
  number  = false
}

resource "azurerm_resource_group" "demotf" {
  name     = local.group_name
  location = local.location

  tags = {
    environment = local.group_name
  }
}

resource "azurerm_service_plan" "demotf" {
  name                = "plan${local.random_name}"
  location            = azurerm_resource_group.demotf.location
  resource_group_name = azurerm_resource_group.demotf.name
  os_type             = "Windows"
  sku_name            = "S1"

  tags = {
    environment = local.group_name
  }
}

resource "azurerm_windows_web_app" "demotf" {
  name                = "web${local.random_name}"
  location            = azurerm_resource_group.demotf.location
  resource_group_name = azurerm_resource_group.demotf.name
  service_plan_id     = azurerm_service_plan.demotf.id

  site_config {
    application_stack {
      current_stack  = "dotnet"
      dotnet_version = "v10.0"
    }
  }

  app_settings = {
    "WEBSITE_TIME_ZONE" = "Asia/Taipei"
  }

  tags = {
    environment = local.group_name
  }
}
