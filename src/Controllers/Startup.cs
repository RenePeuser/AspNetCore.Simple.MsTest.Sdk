using AspNetCore.Simple.Sdk.Startups;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Controllers
{
    public class Startup(IConfiguration configuration,
                         IWebHostEnvironment webHostEnvironment) : SimpleStartup(configuration, webHostEnvironment, new PathString("/api"));
}