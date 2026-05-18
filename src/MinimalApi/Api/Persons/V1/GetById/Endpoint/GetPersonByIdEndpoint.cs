using Extensions.Pack;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Endpoints;
using MinimalApi.ErrorHandling.Exceptions;

namespace MinimalApi.Api.Persons.V1
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: GetPersonByIdEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddGetPersonByIdEndpointExtension
    {
        public static void AddGetPersonByIdEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, GetPersonByIdEndpoint>();
        }
    }

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
                        .Produces<Person>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}