using Microsoft.AspNetCore.Mvc;
using MinimalApi.Endpoints;

namespace MinimalApi.Api.NativeTypes.V1
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: GetAnotherStringEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddGetAnotherStringEndpointExtension
    {
        public static void AddGetAnotherStringEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, GetAnotherStringEndpoint>();
        }
    }

    internal sealed class GetAnotherStringEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("native-types/another-string", () => Results.Ok("Different text"))
                        .WithName("getAnotherStringV1")
                        .WithSummary("Returns a different string")
                        .WithTags("NativeTypes")
                        .Produces<string>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}