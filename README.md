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
 ab -k -l -p payload.json -T application/json -c 50 -n 10000 http://<HOST-IP>:5100/ingest/event
```

payload.json

```json
{
	"Log": "{Data:'mock data'}"
}
```

Prometheus targets:

```yaml
  - job_name: 'aspnetcore'
    scrape_interval: 10s
    static_configs:
      - targets: ['app:5000', 'app-proxy:5000', 'app-ping:5000']
```

Proxy metrics:

```
# HELP dotnet_http_requests_total HTTP Requests Total
# TYPE dotnet_http_requests_total COUNTER
dotnet_http_requests_total{path="/ingest/eventlog",method="POST",status="200"} 347000
dotnet_http_requests_total{path="/Home/Ping",method="GET",status="200"} 487393
dotnet_http_requests_total{path="/favicon.ico",method="GET",status="404"} 1
dotnet_http_requests_total{path="/",method="GET",status="200"} 2
dotnet_http_requests_total{path="/home/ping",method="GET",status="200"} 10000
dotnet_http_requests_total{path="/ingest/event",method="POST",status="200"} 226798
```

Ping metrics:

```
# HELP dotnet_ping_requests_total Ping Requests Total
# TYPE dotnet_ping_requests_total COUNTER
dotnet_ping_requests_total{target="http://app:5000",status="200"} 1467989
dotnet_ping_requests_total{target="http://app-proxy:5000",status="500"} 57
dotnet_ping_requests_total{target="http://app-proxy:5000",status="200"} 1467932
```
