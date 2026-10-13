using Extensions.Pack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Endpoints;

namespace MinimalApi.Api.Persons.V1
{
    /// <summary>The request body of <c>POST persons/echo-request</c>.</summary>
    public sealed record EchoRequestBody(string Name);

    /// <summary>
    /// Reflects a custom request header AND the request body back in the response.
    ///
    /// <para>
    /// Exists so a test can PROVE that a body verb sends both, whatever order the fluent chain configured them
    /// in (<c>WithHeader</c> before or after <c>Accepts…</c>). The GET-only <c>persons/echo-header</c> cannot
    /// carry a body.
    /// </para>
    /// </summary>
    public sealed record EchoRequestResponse(string CorrelationId,
                                             string Name);

    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: EchoRequestEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddEchoRequestEndpointExtension
    {
        public static void AddEchoRequestEndpoint(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IEndpoint, EchoRequestEndpoint>();
        }
    }

    internal sealed class EchoRequestEndpoint : IEndpoint
    {
        internal const string CorrelationIdHeaderName = "X-Correlation-Id";

        // What OPTIONS advertises for this resource - deterministic, so a test can compare it exactly. Sent twice
        // on purpose: Allow is a CONTENT header in HttpClient, Access-Control-Allow-Methods a RESPONSE header -
        // one OPTIONS assert has to see both (it used to read the response headers only).
        internal const string AllowedMethodsHeaderName = "Access-Control-Allow-Methods";

        internal const string AllowedMethods = "POST, OPTIONS";

        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("persons/echo-request",
                                 ([FromHeader(Name = CorrelationIdHeaderName)] string? correlationId,
                                  EchoRequestBody body) =>
                                     Results.Ok(new EchoRequestResponse(correlationId ?? "(absent)", body.Name)))
                        .WithName("echoRequestV1")
                        .WithSummary("Echoes the X-Correlation-Id request header and the request body back")
                        .WithTags("Persons")
                        .Produces<EchoRequestResponse>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);

            // The positive OPTIONS sample: 204 without a body, the advertised verbs in a content AND a response header.
            routeBuilder.MapMethods("persons/echo-request",
                                    [HttpMethods.Options],
                                    (HttpResponse response) =>
                                    {
                                        response.Headers.Allow = AllowedMethods;
                                        response.Headers[AllowedMethodsHeaderName] = AllowedMethods;

                                        return Results.NoContent();
                                    })
                        .WithName("echoRequestOptionsV1")
                        .WithSummary("Advertises the verbs of persons/echo-request")
                        .WithTags("Persons")
                        .Produces(StatusCodes.Status204NoContent)
                        .MapToApiVersion(1);
        }
    }
}
