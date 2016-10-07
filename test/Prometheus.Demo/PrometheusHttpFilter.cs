using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prometheus.Demo
{
    public class PrometheusHttpFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var method = context.HttpContext.Request.Method;
            var controller = ((ControllerActionDescriptor)context.ActionDescriptor).ControllerName;
            var action = ((ControllerActionDescriptor)context.ActionDescriptor).ActionName;

            var counter = Metrics.CreateCounter("dotnet_http_requests_total", "HTTP Requests Total", labelNames: new[] { "controller", "action", "method" });
            counter.Labels(controller, action, method).Inc();

            await next();
        }
    }
}
