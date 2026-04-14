using Microsoft.AspNetCore.Mvc;
using MinimalApi.ErrorHandling.Exceptions;
using StrategyPattern.Evolution;

namespace MinimalApi.Api.Errors.V1.NotImplemented
{
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
                        .Produces<ProblemDetails>(500)
                        .WithOpenApi();
        }
    }
}
