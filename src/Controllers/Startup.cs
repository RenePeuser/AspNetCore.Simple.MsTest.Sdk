using AspNetCore.Simple.Sdk.Startups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Controllers
{
    public class Startup(IConfiguration configuration,
                         IWebHostEnvironment webHostEnvironment) : SimpleStartup(configuration, webHostEnvironment, new PathString("/api"))
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            services.AddMvc(config => config.Filters.Clear());
        }
    }
}