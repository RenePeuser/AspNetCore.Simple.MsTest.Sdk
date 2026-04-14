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
    /// Required for WebApplicationFactory compatibility where code after app.Build() may not execute.
    /// </summary>
    internal sealed class EndpointMappingStartupFilter(RegisterEndpoints registerEndpoints,
                                                       ILogger<EndpointMappingStartupFilter> logger) : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return app =>
            {
                logger.LogInformation("EndpointMappingStartupFilter: Starting configuration");

                // Use error handling middleware
                app.UseErrorHandling();

                // Map endpoints - WebApplication implements both IApplicationBuilder and IEndpointRouteBuilder
                if (app is IEndpointRouteBuilder routeBuilder)
                {
                    logger.LogInformation("EndpointMappingStartupFilter: Mapping endpoints");
                    var apiV1 = routeBuilder.MapGroup("api/v1");
                    registerEndpoints.MapEndpoints(apiV1);
                    logger.LogInformation("EndpointMappingStartupFilter: Endpoints mapped successfully");
                }
                else
                {
                    logger.LogWarning("EndpointMappingStartupFilter: IApplicationBuilder is not IEndpointRouteBuilder");
                }

                // Call the next middleware
                next(app);
            };
        }
    }
}
