using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Evolution;

namespace MinimalApi.Api.NativeTypes.V1.GetString
{
    internal sealed class GetStringEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("native-types/string", () =>
                                                       {
                                                           return Results.Ok("String only");
                                                       })
                        .WithName("getStringV1")
                        .WithSummary("Returns a simple string")
                        .WithTags("NativeTypes")
                        .Produces<string>(200)
                        .Produces<ProblemDetails>(500)
                        .WithOpenApi();
        }
    }
}
