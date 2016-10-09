using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Prometheus.Demo
{
    public class PingService
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;
        private readonly Counter _counter;
        private Task pingTask;

        public PingService(IOptions<Settings> settings, ILogger<PingService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            _counter = Metrics.CreateCounter("dotnet_ping_requests_total", "Ping Requests Total", labelNames: new[] { "target", "status" });
        }

        private void Ping(string baseUrl)
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);
                var result = client.GetStringAsync("Home/Ping").Result;
                _counter.Labels(baseUrl, "200").Inc();
            }
            catch (Exception)
            {
                _counter.Labels(baseUrl, "500").Inc();
            }
        }

        public void Start()
        {
            if (_settings.PingTargets != null && _settings.PingTargets.Any())
            {
                int interval = string.IsNullOrEmpty(_settings.PingInterval) ? 1000 : Convert.ToInt32(_settings.PingInterval);

                try
                {
                    if (pingTask == null || pingTask.Status != TaskStatus.Running)
                    {
                        pingTask = Task.Factory.StartNew(() =>
                        {
                            while (true)
                            {
                                Thread.Sleep(interval);

                                Parallel.ForEach(_settings.PingTargets, target =>
                                {
                                    if (!string.IsNullOrEmpty(target))
                                    {
                                        Ping(target);
                                    }
                                });
                            }
                        }, TaskCreationOptions.LongRunning);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(1001, ex, ex.Message);
                }
            }
        }
    }
}
