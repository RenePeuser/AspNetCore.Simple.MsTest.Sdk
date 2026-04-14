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
                        .Produces<string>(200)
                        .Produces<ProblemDetails>(500)
                        .WithOpenApi();
        }
    }
}
