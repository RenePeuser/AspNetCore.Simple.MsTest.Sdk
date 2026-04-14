using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using MinimalApi.ErrorHandling;
using StrategyPattern.Evolution;

namespace MinimalApi.Startup
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: EndpointMappingStartupFilter
    // ═══════════════════════════════════════════════════════════════════════════════════
    internal static class AddEndpointMappingStartupFilterExtension
    {
        internal static void AddEndpointMappingStartupFilter(this IServiceCollection services)
        {
            services.AddSingleton<IStartupFilter, EndpointMappingStartupFilter>();
        }
    }

    /// <summary>
    /// Startup filter that ensures endpoints are mapped.
    /// Required for WebApplicationFactory compatibility.
    /// </summary>
    internal sealed class EndpointMappingStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                next(app);

                // Use error handling middleware
                app.UseErrorHandling();

                // Use routing (required for endpoint mapping)
                app.UseRouting();

                // Map endpoints using UseEndpoints
                app.UseEndpoints(endpoints =>
                {
                    var apiV1 = endpoints.MapGroup("api/v1");
                    var registerEndpoints = app.ApplicationServices.GetRequiredService<RegisterEndpoints>();
                    registerEndpoints.MapEndpoints(apiV1);
                });
            };
        }
    }
}
