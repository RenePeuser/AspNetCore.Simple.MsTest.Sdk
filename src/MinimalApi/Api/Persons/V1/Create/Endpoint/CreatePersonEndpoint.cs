using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.Create
{
    internal sealed class CreatePersonEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("persons", (Person person) =>
                                            {
                                                return Results.Ok(person);
                                            })
                        .WithName("createPersonV1")
                        .WithSummary("Creates a new person")
                        .WithTags("Persons")
                        .Produces<Person>(200)
                        .Produces<ProblemDetails>(400)
                        .Produces<ProblemDetails>(500)
                        .WithOpenApi();
        }
    }
}
