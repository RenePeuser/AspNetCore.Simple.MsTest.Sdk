using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Endpoints;
using MinimalApi.ErrorHandling.Exceptions;

namespace MinimalApi.Api.Errors.V1.VerbNotImplemented.Endpoint
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: VerbNotImplementedEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddVerbNotImplementedEndpointExtension
    {
        public static void AddVerbNotImplementedEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, VerbNotImplementedEndpoint>();
        }
    }

    /// <summary>
    /// Error endpoints for GET/PUT/PATCH/DELETE that always throw a <see cref="ProblemDetailsException"/>
    /// (500). Mirrors the existing POST <c>errors/not-implemented</c> so that <c>AssertXxxAsErrorAsync</c>
    /// (with differenceFunc/differenceFilter) can be exercised for every verb, not just POST/QUERY.
    /// </summary>
    internal sealed class VerbNotImplementedEndpoint : IEndpoint
    {
        private static ProblemDetailsException NotImplemented()
        {
            return new ProblemDetailsException("Implementation is missing",
                                               "Here are error details",
                                               ("PropertyA", "A"),
                                               ("PropertyB", "B"));
        }

        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("errors/get-not-implemented", IResult () => throw NotImplemented())
                        .WithName("throwGetNotImplementedV1")
                        .WithSummary("Throws a not implemented exception (GET)")
                        .WithTags("Errors")
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);

            routeBuilder.MapPut("errors/put-not-implemented", IResult ([FromBody] NotImplementedRequest _) => throw NotImplemented())
                        .WithName("throwPutNotImplementedV1")
                        .WithSummary("Throws a not implemented exception (PUT)")
                        .WithTags("Errors")
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);

            routeBuilder.MapPatch("errors/patch-not-implemented", IResult ([FromBody] NotImplementedRequest _) => throw NotImplemented())
                        .WithName("throwPatchNotImplementedV1")
                        .WithSummary("Throws a not implemented exception (PATCH)")
                        .WithTags("Errors")
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);

            routeBuilder.MapDelete("errors/delete-not-implemented", IResult () => throw NotImplemented())
                        .WithName("throwDeleteNotImplementedV1")
                        .WithSummary("Throws a not implemented exception (DELETE)")
                        .WithTags("Errors")
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }

    // Minimal body type for the PUT/PATCH error endpoints (content is irrelevant — they always throw).
    public record NotImplementedRequest(string? Value);
}
