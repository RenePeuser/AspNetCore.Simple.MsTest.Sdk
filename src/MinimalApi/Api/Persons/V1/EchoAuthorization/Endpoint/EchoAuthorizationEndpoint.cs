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
    /// The <c>Authorization</c> header as it reached the server: <c>"(absent)"</c> when the request carried none,
    /// otherwise its value - an empty string included.
    ///
    /// <para>
    /// Exists so a test can PROVE which credentials left the client: the per-request token of <c>WithBearerToken</c>,
    /// the client's default, or none at all after <c>WithoutAuthorization</c>.
    /// </para>
    /// </summary>
    public sealed record EchoAuthorizationResponse(string Authorization);

    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: EchoAuthorizationEndpoint / RequiresTokenEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddEchoAuthorizationEndpointExtension
    {
        public static void AddEchoAuthorizationEndpoint(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IEndpoint, EchoAuthorizationEndpoint>();
            services.AddSingletonIfNotExists<IEndpoint, RequiresTokenEndpoint>();
        }
    }

    internal sealed class EchoAuthorizationEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            // GET for the body-less request stage, POST for the body stage (the body is ignored).
            routeBuilder.MapMethods("persons/echo-authorization",
                                    [HttpMethods.Get, HttpMethods.Post],
                                    (HttpRequest request) =>
                                        Results.Ok(new EchoAuthorizationResponse(request.Headers.Authorization.Count == 0
                                                                                     ? "(absent)"
                                                                                     : request.Headers.Authorization.ToString())))
                        .WithName("echoAuthorizationV1")
                        .WithSummary("Echoes the Authorization request header back")
                        .WithTags("Persons")
                        .Produces<EchoAuthorizationResponse>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }

    /// <summary>
    /// 401 unless the request carries exactly <c>Authorization: Bearer valid-token</c> - a stand-in for a protected
    /// endpoint without wiring a real authentication scheme into the sample. Every verb the SDK's
    /// <c>Assert…AsUnauthorizedAsync</c> covers is mapped.
    /// </summary>
    internal sealed class RequiresTokenEndpoint : IEndpoint
    {
        internal const string ValidAuthorization = "Bearer valid-token";

        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapMethods("persons/requires-token",
                                    [HttpMethods.Get, HttpMethods.Post, HttpMethods.Put, HttpMethods.Patch, HttpMethods.Delete, "QUERY"],
                                    (HttpRequest request) => request.Headers.Authorization.ToString() == ValidAuthorization
                                                                 ? Results.NoContent()
                                                                 : Results.Unauthorized())
                        .WithName("requiresTokenV1")
                        .WithSummary("Returns 204 for 'Bearer valid-token', 401 otherwise")
                        .WithTags("Persons")
                        .Produces(StatusCodes.Status204NoContent)
                        .Produces(StatusCodes.Status401Unauthorized)
                        .MapToApiVersion(1);
        }
    }
}
