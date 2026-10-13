using System.Collections.Generic;
using System.Linq;
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
    /// The query string exactly as it reached the server (<see cref="RawQuery"/>, still encoded) and as ASP.NET Core
    /// decoded it (<see cref="Values"/>, key → values in arrival order).
    ///
    /// <para>
    /// Exists so a test can PROVE how <c>WithQueryParameter</c> encodes: the raw form shows every escape, the decoded
    /// form shows that the endpoint reads back exactly the value the test passed in.
    /// </para>
    /// </summary>
    public sealed record EchoQueryResponse(string RawQuery,
                                           IReadOnlyDictionary<string, string[]> Values);

    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: EchoQueryEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddEchoQueryEndpointExtension
    {
        public static void AddEchoQueryEndpoint(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IEndpoint, EchoQueryEndpoint>();
        }
    }

    internal sealed class EchoQueryEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            // GET for the body-less request stage, POST for the body stage (the body is ignored).
            routeBuilder.MapMethods("persons/echo-query",
                                    [HttpMethods.Get, HttpMethods.Post],
                                    (HttpRequest request) =>
                                        Results.Ok(new EchoQueryResponse(request.QueryString.Value ?? string.Empty,
                                                                         request.Query.ToDictionary(entry => entry.Key,
                                                                                                    entry => entry.Value.Select(value => value ?? string.Empty).ToArray()))))
                        .WithName("echoQueryV1")
                        .WithSummary("Echoes the request's query string back, raw and decoded")
                        .WithTags("Persons")
                        .Produces<EchoQueryResponse>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}
