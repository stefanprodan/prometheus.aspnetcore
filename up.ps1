# build image
docker pull microsoft/dotnet:latest
docker build -t eventlog .

# create network
$network = "intranet"
if(!(docker network ls --filter name=$network -q)){
	docker network create $network
}

# run containers
docker run -d --network intranet -p 5200:5000 --name eventlog --restart unless-stopped eventlog
docker run -d --network intranet -p 5100:5000 --name eventlog-proxy --restart unless-stopped -e PROXY_FOR='http://eventlog:5200' eventlog