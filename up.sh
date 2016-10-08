# build image
docker pull microsoft/dotnet:latest
docker build -t eventlog .

# run containers
docker run -d --network dockerprom_monitor-net --network intranet -p 5200:5000 --name eventlog --restart unless-stopped eventlog
docker run -d --network dockerprom_monitor-net --network intranet -p 5100:5000 --name eventlog-proxy --restart unless-stopped -e PROXY_FOR='http://188.241.146.52:5200' eventlog