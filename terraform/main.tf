provider "azurerm" {
  subscription_id = "override-this"
  features {}
}

variable "resource_group_name" {
  default = "cosmosdb-studying"
}

variable "cosmos_account_name" {
  default = "windpowersystem"
}

variable "cosmos_database_name" {
  default = "WIND-POWER-SYSTEM-DB"
}

# Create new Cosmos DB NoSQL container
resource "azurerm_cosmosdb_sql_container" "new_container" {
  name                = "TurbineCharacteristics"
  resource_group_name = var.resource_group_name
  account_name        = var.cosmos_account_name
  database_name       = var.cosmos_database_name
  partition_key_paths  = ["/turbineId"]

  indexing_policy {
    indexing_mode = "consistent"

    included_path {
      path = "/*"
    }

    excluded_path {
      path = "/\"_etag\"/?"
    }
  }
}
