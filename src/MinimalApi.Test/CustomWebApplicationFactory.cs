using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MinimalApi.ErrorHandling;
using StrategyPattern.Evolution;

namespace MinimalApi.Test
{
    public class CustomWebApplicationFactory : ApiTestBase<Program>
    {
        public CustomWebApplicationFactory() : base("Development",
                                                     (services, configuration) =>
                                                     {
                                                         services.AddAssertableHttpClient(configuration);
                                                     })
        {
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            // Configure the app to map endpoints
            builder.Configure(app =>
            {
                app.UseErrorHandling();
                app.UseRouting();
                
                app.UseEndpoints(endpoints =>
                {
                    var apiV1 = endpoints.MapGroup("api/v1");
                    var registerEndpoints = app.ApplicationServices.GetRequiredService<RegisterEndpoints>();
                    registerEndpoints.MapEndpoints(apiV1);
                });
            });
        }
    }
}
