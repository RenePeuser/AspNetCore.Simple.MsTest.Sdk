using AspNetCore.Simple.Sdk.Startups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Versioning;
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

            // Url segment for the classic routes, query string and header for the versioning samples
            services.Configure<ApiVersioningOptions>(options => options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                                                                                     new QueryStringApiVersionReader("api-version"),
                                                                                                                     new HeaderApiVersionReader("x-api-version")));
        }
    }
}