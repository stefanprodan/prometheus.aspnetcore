$ErrorActionPreference = "Stop"

# build image
docker pull microsoft/dotnet:latest
docker build -t prom-demo .

# run container
docker run --name prom-demo -d -p 5100:5000 -t prom-demo