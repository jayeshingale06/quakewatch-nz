#!/usr/bin/env bash
# Shows EVERY Azure resource this project has created, so you can
# confirm nothing unexpected exists. Run it whenever you want
# reassurance. It only reads, it never changes anything.

set -euo pipefail

RESOURCE_GROUP="quakewatch-rg"

echo "============================================================"
echo "Resources in $RESOURCE_GROUP"
echo "============================================================"

if ! az group exists --name "$RESOURCE_GROUP" | grep -q true; then
  echo "The resource group does not exist."
  echo "Nothing of yours is running in Azure, so nothing can cost money."
  exit 0
fi

az resource list --resource-group "$RESOURCE_GROUP" \
  --query "[].{Name:name, Type:type, Location:location}" -o table

echo ""
echo "============================================================"
echo "Container apps and their replica counts"
echo "============================================================"

az containerapp list --resource-group "$RESOURCE_GROUP" \
  --query "[].{Name:name, MinReplicas:properties.template.scale.minReplicas, MaxReplicas:properties.template.scale.maxReplicas, Url:properties.configuration.ingress.fqdn}" \
  -o table

echo ""
echo "MinReplicas should be 0 for both. That is what makes them free when idle."
echo ""
echo "You should see ONLY:"
echo "  - one Microsoft.App/managedEnvironments"
echo "  - two Microsoft.App/containerApps"
echo ""
echo "If you see a Log Analytics workspace, a virtual network, a load"
echo "balancer, or a database server, something was created that this"
echo "project does not need. Tell Claude before leaving it running."
