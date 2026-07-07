using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Endpoints;
using MinimalApi.ErrorHandling.Exceptions;

namespace MinimalApi.Api.Errors.V1.QueryNotImplemented.Endpoint
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: QueryNotImplementedEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddQueryNotImplementedEndpointExtension
    {
        public static void AddQueryNotImplementedEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, QueryNotImplementedEndpoint>();
        }
    }

    internal sealed class QueryNotImplementedEndpoint : IEndpoint
    {
        private static readonly string[] QueryMethod = ["QUERY"];

        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapMethods("errors/query-not-implemented", QueryMethod, () =>
                        {
                            throw new ProblemDetailsException("Query implementation is missing",
                                                              "Error details for QUERY method",
                                                              ("QueryPropertyA", "A"),
                                                              ("QueryPropertyB", "B"));
                        })
                        .WithName("throwQueryNotImplementedV1")
                        .WithSummary("Throws a not implemented exception for QUERY method")
                        .WithTags("Errors")
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}