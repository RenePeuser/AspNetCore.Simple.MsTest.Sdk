using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.GetAll
{
    internal sealed class GetAllPersonsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("persons", () =>
                                           {
                                               var persons = new List<Person>
                                                             {
                                                                 new(1, "Son", "Goku",
                                                                     99,
                                                                     [new Email("alf@gmx.de", "GMX"), new Email("abc@hotmail.de", "Microsoft")]),
                                                                 new(2, "Vegeta", "Unknown",
                                                                     77,
                                                                     [new Email("abc@gmx.de", "GMX"), new Email("maxmustermann@hotmail.de", "Microsoft")])
                                                             };

                                               return Results.Ok(persons);
                                           })
                        .WithName("getAllPersonsV1")
                        .WithSummary("Returns all available persons")
                        .WithTags("Persons")
                        .Produces<IEnumerable<Person>>(200)
                        .Produces<ProblemDetails>(400)
                        .Produces<ProblemDetails>(500)
                        .WithOpenApi();
        }
    }
}
