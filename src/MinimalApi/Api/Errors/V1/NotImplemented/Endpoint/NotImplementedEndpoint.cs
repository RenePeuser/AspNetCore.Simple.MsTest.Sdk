using Microsoft.AspNetCore.Mvc;
using MinimalApi.Endpoints;
using MinimalApi.ErrorHandling.Exceptions;

namespace MinimalApi.Api.Errors.V1.NotImplemented.Endpoint
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: NotImplementedEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddNotImplementedEndpointExtension
    {
        public static void AddNotImplementedEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, NotImplementedEndpoint>();
        }
    }

    internal sealed class NotImplementedEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("errors/not-implemented", () =>
                        {
                            throw new ProblemDetailsException("Implementation is missing",
                                                              "Here are error details",
                                                              ("PropertyA", "A"),
                                                              ("PropertyB", "B"));
                        })
                        .WithName("throwNotImplementedV1")
                        .WithSummary("Throws a not implemented exception")
                        .WithTags("Errors")
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1)
                        .WithOpenApi();
        }
    }
}