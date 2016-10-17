using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Text;

namespace Prometheus.Demo.Controllers
{
    public class IngestController : Controller
    {
        private ILogger _logger;
        private Settings _settings;
        public IngestController(ILogger<IngestController> logger, IOptions<Settings> settings)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public string Index()
        {
            return $"Running on {Environment.MachineName}";
        }

        [HttpPost]
        public IActionResult Event([FromBody]Payload payload)
        {
            //var data = JsonConvert.DeserializeObject<dynamic>(payload.Data);

            if (!string.IsNullOrEmpty(_settings.ProxyFor))
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(_settings.ProxyFor);
                    var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                    var result = client.PostAsync("ingest/data", content).Result;
                    result.RequestMessage.Dispose();
                    result.Dispose();
                }
            }

            return new EmptyResult();
        }

        [HttpPost]
        public IActionResult Data([FromBody]Payload payload)
        {
            dynamic data = JsonConvert.DeserializeObject<dynamic>(payload.Data);

            return new EmptyResult();
        }
    }

    public class Payload
    {
        public string Data { get; set; }
    }
}
