using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Prometheus.Advanced;

namespace Prometheus.Demo.Controllers
{
    public class MetricsController : Controller
    {
        public MetricsController()
        {
            //var counter = Metrics.CreateCounter("myCounter", "help text", labelNames: new[] { "method", "endpoint" });
            //counter.Labels("GET", "/").Inc();
            //counter.Labels("POST", "/cancel").Inc();

        }

        public void Index()
        {
            var response = HttpContext.Response;
            var request = HttpContext.Request;
            response.StatusCode = 200;

            var acceptHeader = request.Headers["Accept"];
            var contentType = ScrapeHandler.GetContentType(acceptHeader);
            response.ContentType = contentType;

            using (var outputStream = response.Body)
            {
                var collected = DefaultCollectorRegistry.Instance.CollectAll();
                ScrapeHandler.ProcessScrapeRequest(collected, contentType, outputStream);
            };
        }
    }
}
