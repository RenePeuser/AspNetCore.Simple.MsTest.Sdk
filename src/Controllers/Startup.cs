using AspNetCore.Simple.Sdk.Startups;

namespace Controllers
{
    public class Startup(IConfiguration configuration,
                         IWebHostEnvironment webHostEnvironment) : SimpleStartup(configuration, webHostEnvironment, new PathString("/api"));
}