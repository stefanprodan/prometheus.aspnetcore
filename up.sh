# build image
docker pull microsoft/dotnet:latest
docker build -t eventlog .

# run containers
docker run -d --network dockerprom_monitor-net --network intranet -p 5200:5000 --name eventlog --restart unless-stopped eventlog
docker run -d --network dockerprom_monitor-net --network intranet -p 5100:5000 --name eventlog-proxy --restart unless-stopped -e PROXY_FOR='http://eventlog:5000' eventlog
docker run -d --network dockerprom_monitor-net --network intranet -p 5300:5000 --name eventlog-ping --restart unless-stopped -e PING_INTERVAL="100" -e PING_TARGETS='http://eventlog:5000,http://eventlog-proxy:5000' eventlog