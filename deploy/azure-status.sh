#!/usr/bin/env bash
# Lists every Azure resource this project creates, so you can confirm nothing
# unexpected exists. Read-only.

set -euo pipefail

RESOURCE_GROUP="quakewatch-rg"

echo "Resources in $RESOURCE_GROUP"

if ! az group exists --name "$RESOURCE_GROUP" | grep -q true; then
  echo "The resource group does not exist."
  echo "Nothing of yours is running in Azure, so nothing can cost money."
  exit 0
fi

az resource list --resource-group "$RESOURCE_GROUP" \
  --query "[].{Name:name, Type:type, Location:location}" -o table

echo "Container apps and their replica counts"

az containerapp list --resource-group "$RESOURCE_GROUP" \
  --query "[].{Name:name, MinReplicas:properties.template.scale.minReplicas, MaxReplicas:properties.template.scale.maxReplicas, Url:properties.configuration.ingress.fqdn}" \
  -o table

echo "MinReplicas should be 0 for both. That is what makes them free when idle."
echo ""
echo "You should see ONLY:"
echo "  - one Microsoft.App/managedEnvironments"
echo "  - two Microsoft.App/containerApps"
echo ""
echo "If you see a Log Analytics workspace, a virtual network, a load"
echo "balancer, or a database server, something was created that this"
echo "project does not need. Tell Claude before leaving it running."
