using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Prometheus.Demo.Controllers
{
    //[ServiceFilter(typeof(PrometheusHttpFilter))]
    public class HomeController : Controller
    {
        public string Index()
        {
            return $"Running on {Environment.MachineName}";
        }

        public string Error()
        {
            throw new Exception("Test");
        }
    }
}
