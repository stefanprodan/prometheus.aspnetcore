#!/bin/bash
set -e

image="eventlog"

docker service scale $image=0
docker service scale ${image}-proxy=0
docker service scale ${image}-ping=0

docker service rm $image ${image}-proxy ${image}-ping

docker rmi -f $image
