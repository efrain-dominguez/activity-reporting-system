\# Azure Deployment Guide



Quick reference for deploying to Azure.



\## Prerequisites



\- Azure subscription

\- Docker Desktop

\- Azure CLI



\## Initial Deployment



```bash

\# 1. Login

az login



\# 2. Create resources

az group create --name ActivityReportingRG --location eastus

az cosmosdb create --name activityreporting-db --kind MongoDB

az acr create --name activityreportingacr --sku Basic



\# 3. Build and push

docker build -t activityreportingacr.azurecr.io/activityreporting-api:v1 .

az acr login --name activityreportingacr

docker push activityreportingacr.azurecr.io/activityreporting-api:v1



\# 4. Deploy

az containerapp env create --name activityreporting-env

az containerapp create --name activityreporting-api \[with env vars]



\# 5. Get URL

az containerapp show --query properties.configuration.ingress.fqdn

```



\## Update Deployment



```bash

\# 1. Build new version

docker build -t activityreportingacr.azurecr.io/activityreporting-api:v2 .



\# 2. Push

docker push activityreportingacr.azurecr.io/activityreporting-api:v2



\# 3. Update

az containerapp update \\

&#x20; --name activityreporting-api \\

&#x20; --image activityreportingacr.azurecr.io/activityreporting-api:v2

```



\## View Logs



```bash

az containerapp logs show \\

&#x20; --name activityreporting-api \\

&#x20; --follow

```



\## Costs



\- Cosmos DB: Free tier (400 RU/s)

\- Container Registry: \~$5/month

\- Container Apps: \~$0-10/month

\- Total: \~$5-15/month



\## Live URL



https://YOUR\_API\_URL.azurecontainerapps.io

