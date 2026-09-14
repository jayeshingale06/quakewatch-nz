#!/usr/bin/env bash
# Deletes everything this project created in Azure.
# Run it when the project no longer needs to be online.

set -euo pipefail

RESOURCE_GROUP="quakewatch-rg"

echo "This permanently deletes the resource group: $RESOURCE_GROUP"
read -r -p "Type the group name to confirm: " CONFIRM

if [ "$CONFIRM" != "$RESOURCE_GROUP" ]; then
  echo "Not confirmed. Nothing deleted."
  exit 1
fi

az group delete --name "$RESOURCE_GROUP" --yes --no-wait
echo "Deletion started."
