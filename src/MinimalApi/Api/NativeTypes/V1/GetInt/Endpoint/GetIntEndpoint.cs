using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Evolution;

namespace MinimalApi.Api.NativeTypes.V1.GetInt
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: GetIntEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddGetIntEndpointExtension
    {
        public static void AddGetIntEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, GetIntEndpoint>();
        }
    }

    internal sealed class GetIntEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("native-types/int", () =>
                                                    {
                                                        return Results.Ok(42);
                                                    })
                        .WithName("getIntV1")
                        .WithSummary("Returns a simple integer")
                        .WithTags("NativeTypes")
                        .Produces<int>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1)
                        .WithOpenApi();
        }
    }
}
