# prometheus.netcore

Services:

* app-proxy (receives the json payload)
* app (receives the json payload from app-proxy)
* app-ping (pings each service every 100ms)

Running the stack:

```bash
docker pull microsoft/dotnet:latest
docker build -t prom-app .

docker network create promapp-net

docker run -d --network promapp-net -p 5200:5000 --name app --restart unless-stopped prom-app
docker run -d --network promapp-net -p 5100:5000 --name app-proxy --restart unless-stopped -e PROXY_FOR='http://app:5000' prom-app
docker run -d --network promapp-net -p 5300:5000 --name app-ping --restart unless-stopped -e PING_INTERVAL="100" -e PING_TARGETS='http://app:5000,http://app-proxy:5000' prom-app

```

Load test:

```
 ab -k -l -p log.json -T application/json -c 50 -n 10000 http://<HOST-IP>:5100/ingest/event
```
