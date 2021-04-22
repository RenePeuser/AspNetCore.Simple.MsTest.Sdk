using AspNetCore.Simple.MsTest.Sdk.TestCreator;
using AspNetCore.Simple.Sdk.Startups;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Api
{
    public class Startup : SimpleStartup
    {
        public Startup(IConfiguration configuration,
                       IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment, typeof(Startup).Assembly, new PathString("/api/tests"), "Test API for AspNetCore.Simple.MsTest.Sdk")
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddTestCreator();
            base.ConfigureServices(services);
        }

        public override void Configure(IApplicationBuilder app)
        {
            app.UseTestCreator();
            base.Configure(app);
        }
    }
}
