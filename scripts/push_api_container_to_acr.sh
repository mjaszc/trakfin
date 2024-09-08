az acr update -n trakfinapi --admin-enabled true
az acr login --name trakfinapi --expose-token

TOKEN=$(az acr login --name trakfinapi --expose-token --output tsv --query accessToken)

# Log in to Docker
docker login trakfinapi.azurecr.io -u 00000000-0000-0000-0000-000000000000 -p $TOKEN

docker tag trakfinapi:latest trakfinapi.azurecr.io/trakfinapi:latest
docker push trakfinapi.azurecr.io/trakfinapi:latest