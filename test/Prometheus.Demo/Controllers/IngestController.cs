using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Prometheus.Demo.Controllers
{
    //[ServiceFilter(typeof(PrometheusHttpFilter))]
    public class IngestController : Controller
    {
        private ILogger _logger;

        public IngestController(ILogger<IngestController> logger)
        {
            _logger = logger;
        }

        public string Index()
        {
            return $"Running on {Environment.MachineName}";
        }

        [HttpPost]
        public IActionResult Event([FromBody]Payload payload)
        {
            if (payload != null && !string.IsNullOrEmpty(payload.Log))
            {
                dynamic log = JsonConvert.DeserializeObject<dynamic>(payload.Log);

                _logger.LogInformation(payload.Log);
            }

            return new EmptyResult();
        }
    }

    public class Payload
    {
        public string Log { get; set; }
    }
}
