using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Endpoints;

namespace MinimalApi.Api.NativeTypes.V1
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: GetAnotherIntEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddGetAnotherIntEndpointExtension
    {
        public static void AddGetAnotherIntEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, GetAnotherIntEndpoint>();
        }
    }

    internal sealed class GetAnotherIntEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("native-types/another-int", () => Results.Ok(999))
                        .WithName("getAnotherIntV1")
                        .WithSummary("Returns a different integer (999)")
                        .WithTags("NativeTypes")
                        .Produces<int>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}