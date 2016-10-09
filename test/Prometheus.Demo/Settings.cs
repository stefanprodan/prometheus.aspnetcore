using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prometheus.Demo
{
    public class Settings
    {
        public string ProxyFor { get; set; }

        public List<string> PingTargets { get; set; }
        public string PingInterval { get; set; }
    }
}
