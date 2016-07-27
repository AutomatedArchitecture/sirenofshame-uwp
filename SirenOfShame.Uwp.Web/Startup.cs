using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace SirenOfShame.Uwp.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddMvc(options =>
            {
                var formatter = new JsonOutputFormatter
                {
                    SerializerSettings = { ContractResolver = new CamelCasePropertyNamesContractResolver() }
                };
                options.OutputFormatters.Insert(0, formatter);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            var pages = new List<string> {"showoff", "server", "settings"};

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                pages.ForEach(page =>
                {
                    routes.MapRoute(
                        name: page,
                        template: page,
                        defaults: new {controller = "Home", action = "Index"}
                        );
                });
                routes.MapRoute(
                    name: "vendorsmin",
                    template: "vendors.min.js",
                    defaults: new { controller = "Home", action = "Vendors" }
                    );
                routes.MapRoute(
                    name: "bootmin",
                    template: "boot.min.js",
                    defaults: new { controller = "Home", action = "Boot" }
                    );
                routes.MapRoute(
                    name: "appmin",
                    template: "app.min.js",
                    defaults: new { controller = "Home", action = "App" }
                    );
            });
        }
    }
}
