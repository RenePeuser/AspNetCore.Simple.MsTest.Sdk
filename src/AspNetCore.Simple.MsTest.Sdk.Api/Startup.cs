using AspNetCore.Simple.Sdk.Startups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace AspNetCore.Simple.MsTest.Sdk.Api
{
    public class Startup : SimpleStartup
    {
        public Startup(IConfiguration configuration,
                       IWebHostEnvironment webHostEnvironment) : base(configuration, webHostEnvironment, new PathString("/api/tests"), "Test API for AspNetCore.Simple.MsTest.Sdk")
        {
        }
    }
}
