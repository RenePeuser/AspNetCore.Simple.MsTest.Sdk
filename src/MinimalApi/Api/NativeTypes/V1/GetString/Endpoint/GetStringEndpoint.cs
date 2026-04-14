using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Evolution;

namespace MinimalApi.Api.NativeTypes.V1.GetString
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: GetStringEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddGetStringEndpointExtension
    {
        public static void AddGetStringEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, GetStringEndpoint>();
        }
    }

    internal sealed class GetStringEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("native-types/string", () =>
                                                       {
                                                           return Results.Ok("String only");
                                                       })
                        .WithName("getStringV1")
                        .WithSummary("Returns a simple string")
                        .WithTags("NativeTypes")
                        .Produces<string>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1)
                        .WithOpenApi();
        }
    }
}
