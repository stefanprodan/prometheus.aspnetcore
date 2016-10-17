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
            if (payload != null && !string.IsNullOrEmpty(payload.Log))
            {
                dynamic log = JsonConvert.DeserializeObject<dynamic>(payload.Log);

                if(!string.IsNullOrEmpty(_settings.ProxyFor))
                {
                    // Disposing the HttpClient had no effect on the API receiving a ECONNECT error
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(_settings.ProxyFor);
                        var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                        var result = client.PostAsync("Ingest/EventLog", content).Result;

                        // possible fix 
                        result.RequestMessage.Dispose();
                        result.Dispose();
                    }
                }
            }

            return new EmptyResult();
        }

        [HttpPost]
        public IActionResult EventLog([FromBody]Payload payload)
        {
            if (payload != null && !string.IsNullOrEmpty(payload.Log))
            {
                dynamic log = JsonConvert.DeserializeObject<dynamic>(payload.Log);
            }

            return new EmptyResult();
        }
    }

    public class Payload
    {
        public string Log { get; set; }
    }
}
