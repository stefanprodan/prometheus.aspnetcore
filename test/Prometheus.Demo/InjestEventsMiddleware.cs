using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Prometheus.Demo.Controllers;

namespace Prometheus.Demo
{
    public class InjestEventsMiddleware
    {
        private readonly HttpClient _client = new HttpClient(new HttpClientHandler
        {
            MaxConnectionsPerServer = int.MaxValue,
            UseProxy = false, // Perf sensitive
            UseCookies = false
        });

        private readonly Uri _proxyEndpointUri;

        private readonly JsonSerializer _serializer = new JsonSerializer();

        public InjestEventsMiddleware(RequestDelegate next, IOptions<Settings> settings)
        {
            if (!string.IsNullOrEmpty(settings.Value.ProxyFor))
            {
                _proxyEndpointUri = new Uri(settings.Value.ProxyFor + "/ingest/data");
            }
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (_proxyEndpointUri == null)
                return;

            Payload payload;
            using (var sr = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
            {
                payload = (Payload) _serializer.Deserialize(sr, typeof(Payload));
            }

            if (string.IsNullOrEmpty(payload.Data))
            {
                return;
            }

            var data = JsonConvert.DeserializeObject<dynamic>(payload.Data);

            using (var message = new HttpRequestMessage(HttpMethod.Post, _proxyEndpointUri))
            {
                message.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

                using (var response = await _client.SendAsync(message))
                {
                    response.EnsureSuccessStatusCode();
                }
            }
        }
    }

    public static class InjestEventsMiddlewareExtensions
    {
        public static IApplicationBuilder UseInjestEventsMiddleware(this IApplicationBuilder builder)
        {
            builder
                .Map("/injest/event", app => app.UseMiddleware<InjestEventsMiddleware>());
            return builder;
        }
    }
}