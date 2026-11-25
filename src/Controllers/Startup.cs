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

            // Url segment for the classic routes; query string, header and media type for the versioning samples.
            // "ver", "x-custom-version" and ";v=" are no common names - the sdk has to detect them from this reader
            services.Configure<ApiVersioningOptions>(options => options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                                                                                     new QueryStringApiVersionReader("api-version", "ver"),
                                                                                                                     new HeaderApiVersionReader("x-api-version", "x-custom-version"),
                                                                                                                     new MediaTypeApiVersionReader("v")));
        }
    }
}