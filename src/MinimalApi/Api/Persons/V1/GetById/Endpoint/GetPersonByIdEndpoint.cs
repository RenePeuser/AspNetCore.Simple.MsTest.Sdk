using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.ErrorHandling.Exceptions;
using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.GetById
{
    internal sealed class GetPersonByIdEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("persons/{id:long}", (long id) =>
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

                                                         var person = persons.FirstOrDefault(x => x.Id == id);

                                                         if (person.IsNull())
                                                         {
                                                             throw new ProblemDetailsException("Person for given Id does not exist",
                                                                                               $"The person with the Id: {id} does not exist",
                                                                                               ("Id", id));
                                                         }

                                                         return Results.Ok(person);
                                                     })
                        .WithName("getPersonByIdV1")
                        .WithSummary("Returns a person by ID")
                        .WithTags("Persons")
                        .Produces<Person>(200)
                        .Produces<ProblemDetails>(404)
                        .Produces<ProblemDetails>(500)
                        .WithOpenApi();
        }
    }
}
