using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Prometheus.Demo
{
    public class Startup
    {
        private IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            _env = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddScoped<PrometheusHttpFilter>();

            var proxyFor = Environment.GetEnvironmentVariable("PROXY_FOR");
            var pingInterval = Environment.GetEnvironmentVariable("PING_INTERVAL");
            var pingTargets = Environment.GetEnvironmentVariable("PING_TARGETS");

            services.Configure<Settings>(s =>
            {
                s.ProxyFor = proxyFor;
                s.PingInterval = pingInterval;
                if (!string.IsNullOrEmpty(pingTargets))
                {
                    s.PingTargets = pingTargets.Split(',').ToList();
                }
            });

            services.AddSingleton<PingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, PingService pingService)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //app.UseDeveloperExceptionPage();

            app.UseInjestEventsMiddleware();

            app.UsePrometheusMiddleware();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            pingService.Start();

            //Advanced.DefaultCollectorRegistry.Instance.RegisterOnDemandCollectors(new[] { new Advanced.DotNetStatsCollector() });
        }
    }
}
