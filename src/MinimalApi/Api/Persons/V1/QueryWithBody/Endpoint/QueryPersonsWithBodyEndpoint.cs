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
    // 🎯 SERVICE REGISTRATION: QueryPersonsWithBodyEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddQueryPersonsWithBodyEndpointExtension
    {
        public static void AddQueryPersonsWithBodyEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, QueryPersonsWithBodyEndpoint>();
        }
    }

    internal sealed class QueryPersonsWithBodyEndpoint : IEndpoint
    {
        private static readonly string[] QueryMethod = ["QUERY"];

        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapMethods("persons/search", QueryMethod, ([FromBody] PersonSearchRequest searchRequest) =>
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

                            var results = persons.AsEnumerable();

                            if (searchRequest.Name.IsNotNullOrWhiteSpace())
                            {
                                results = results.Where(p => p.Name.Contains(searchRequest.Name, StringComparison.OrdinalIgnoreCase));
                            }

                            if (searchRequest.MinAge.HasValue)
                            {
                                results = results.Where(p => p.Age >= searchRequest.MinAge.Value);
                            }

                            if (searchRequest.MaxAge.HasValue)
                            {
                                results = results.Where(p => p.Age <= searchRequest.MaxAge.Value);
                            }

                            return Results.Ok(results.ToList());
                        })
                        .WithName("queryPersonsWithBodyV1")
                        .WithSummary("Query persons using HTTP QUERY method with request body")
                        .WithTags("Persons")
                        .Produces<IEnumerable<Person>>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }

    public record PersonSearchRequest(string? Name, int? MinAge, int? MaxAge);
}
