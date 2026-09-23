using Extensions.Pack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Endpoints;

namespace MinimalApi.Api.Persons.V1
{
    /// <summary>
    /// Reflects a custom request header back in the response body.
    ///
    /// <para>
    /// Exists so a test can PROVE that a request header actually left the client. Custom headers used to
    /// be dropped silently on the typed assertion path, and no test could catch it because no endpoint
    /// ever looked at one.
    /// </para>
    /// </summary>
    public sealed record EchoHeaderResponse(string CorrelationId);

    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: EchoHeaderEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddEchoHeaderEndpointExtension
    {
        public static void AddEchoHeaderEndpoint(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IEndpoint, EchoHeaderEndpoint>();
        }
    }

    internal sealed class EchoHeaderEndpoint : IEndpoint
    {
        internal const string CorrelationIdHeaderName = "X-Correlation-Id";

        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("persons/echo-header",
                                ([FromHeader(Name = CorrelationIdHeaderName)] string? correlationId) =>
                                    Results.Ok(new EchoHeaderResponse(correlationId ?? "(absent)")))
                        .WithName("echoHeaderV1")
                        .WithSummary("Echoes the X-Correlation-Id request header back")
                        .WithTags("Persons")
                        .Produces<EchoHeaderResponse>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}