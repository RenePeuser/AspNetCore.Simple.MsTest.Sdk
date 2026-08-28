using System;
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
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: GetAllPersonsEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddGetAllPersonsEndpointExtension
    {
        public static void AddGetAllPersonsEndpoint(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IEndpoint, GetAllPersonsEndpoint>();
        }
    }

    internal sealed class GetAllPersonsEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("persons", ([FromQuery] string name = "") =>
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

                            if (name.IsNotNullOrWhiteSpace())
                            {
                                persons = persons.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
                            }

                            return Results.Ok(persons);
                        })
                        .WithName("getAllPersonsV1")
                        .WithSummary("Returns all available persons")
                        .WithTags("Persons")
                        .Produces<IEnumerable<Person>>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}