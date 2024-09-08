az acr update -n trakfinweb --admin-enabled true
az acr login --name trakfinweb --expose-token

TOKEN=$(az acr login --name trakfinweb --expose-token --output tsv --query accessToken)

# Log in to Docker
docker login trakfinweb.azurecr.io -u 00000000-0000-0000-0000-000000000000 -p $TOKEN

docker tag trakfinweb:latest trakfinweb.azurecr.io/trakfinweb:latest
docker push trakfinweb.azurecr.io/trakfinweb:latest