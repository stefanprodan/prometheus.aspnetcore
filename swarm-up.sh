#!/bin/bash
set -e

image="eventlog"
network="eventlog-net"

# build image
if [ ! "$(docker images -q  $image)" ];then
	docker pull microsoft/dotnet:latest
    docker build -t $image .
fi

# create network
if [ ! "$(docker network ls --filter name=$network -q)" ];then
    docker network create --driver overlay $network
fi

# run containers
docker service create --network $network -p 5020:5000 --name ${image} --restart unless-stopped $image
docker service create --network $network -p 5010:5000 --name ${image}-proxy --restart unless-stopped -e PROXY_FOR='http://eventlog:5000' $image
docker service create --network $network -p 5030:5000 --name ${image}-ping --restart unless-stopped -e PING_INTERVAL="100" -e PING_TARGETS='http://eventlog:5000,http://eventlog-proxy:5000' $image
