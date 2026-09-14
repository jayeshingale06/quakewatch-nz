#!/usr/bin/env bash
# One-time Azure setup. Deployments after this are done by GitHub Actions.
# Cost: Container Apps has a monthly free grant and both apps scale to zero
# when idle. Images live on GitHub Container Registry rather than a paid Azure
# registry. Check your own billing; prices and free grants change.

set -euo pipefail

# Change these to your own subscription details.
GITHUB_USER="your-github-username"

RESOURCE_GROUP="quakewatch-rg"
LOCATION="australiaeast"
ENVIRONMENT="quakewatch-env"
API_APP="quakewatch-api"
WEB_APP="quakewatch-web"

echo "==> Signing in"
az login

echo "==> Making sure the Container Apps extension is installed"
az extension add --name containerapp --upgrade
az provider register --namespace Microsoft.App --wait
az provider register --namespace Microsoft.OperationalInsights --wait

echo "==> Creating the resource group (a folder for everything)"
az group create --name "$RESOURCE_GROUP" --location "$LOCATION"

echo "==> Creating the Container Apps environment (the shared network)"
# --logs-destination none means NO Log Analytics workspace is created.
# Log Analytics is a separately billed service and is the only part of
# this setup that could quietly accrue cost. Live log streaming still
# works without it; only the searchable log history is given up.
az containerapp env create \
  --name "$ENVIRONMENT" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --logs-destination none

echo "==> Creating the API container app"
# No connection string is passed, so the API uses SQLite inside the
# container. That is deliberate: the database is only a cache of
# GeoNet, so losing it on restart costs nothing and saves the
# monthly price of a managed PostgreSQL server.
az containerapp create \
  --name "$API_APP" \
  --resource-group "$RESOURCE_GROUP" \
  --environment "$ENVIRONMENT" \
  --image "ghcr.io/$GITHUB_USER/quakewatch-api:latest" \
  --target-port 8080 \
  --ingress external \
  --min-replicas 0 \
  --max-replicas 1 \
  --cpu 0.25 --memory 0.5Gi

API_URL="https://$(az containerapp show --name "$API_APP" --resource-group "$RESOURCE_GROUP" --query properties.configuration.ingress.fqdn -o tsv)"
echo "==> API is at: $API_URL"

echo "==> Creating the web container app"
# The web image must be built with VITE_API_BASE set to the API URL
# above, because Vite bakes it in at build time. The GitHub Actions
# workflow does that for you.
az containerapp create \
  --name "$WEB_APP" \
  --resource-group "$RESOURCE_GROUP" \
  --environment "$ENVIRONMENT" \
  --image "ghcr.io/$GITHUB_USER/quakewatch-web:latest" \
  --target-port 80 \
  --ingress external \
  --min-replicas 0 \
  --max-replicas 1 \
  --cpu 0.25 --memory 0.5Gi

WEB_URL="https://$(az containerapp show --name "$WEB_APP" --resource-group "$RESOURCE_GROUP" --query properties.configuration.ingress.fqdn -o tsv)"
echo "==> Web is at: $WEB_URL"

echo "==> Telling the API to allow the web app through CORS"
az containerapp update \
  --name "$API_APP" \
  --resource-group "$RESOURCE_GROUP" \
  --set-env-vars "Cors__AllowedOrigins__0=$WEB_URL"

echo "Done."
echo "  Web:  $WEB_URL"
echo "  API:  $API_URL/api/quakes"
echo ""
echo "Put these two values into GitHub as repository VARIABLES:"
echo "  AZURE_RESOURCE_GROUP = $RESOURCE_GROUP"
echo "  API_BASE_URL         = $API_URL"
