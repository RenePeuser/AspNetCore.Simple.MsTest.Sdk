using System.Net.Mime;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Endpoints;

namespace MinimalApi.Api.Persons.V1
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: UpdatePersonEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddUpdatePersonEndpointExtension
    {
        public static void AddUpdatePersonEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, UpdatePersonEndpoint>();
        }
    }

    internal sealed class UpdatePersonEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPut("persons", (Person person) =>
                        {
                            if (person.Id == 999)
                            {
                                return Results.NotFound(new { StatusCode = 404, Message = "Person not found" });
                            }

                            return Results.Ok(person);
                        })
                        .WithName("updatePersonV1")
                        .WithSummary("Updates an existing person")
                        .WithTags("Persons")
                        .Accepts<Person>(MediaTypeNames.Application.Json)
                        .Produces<Person>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                        .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);

            routeBuilder.MapPut("persons/{id:long}", (long id, Person person) =>
                        {
                            if (id == 999)
                            {
                                return Results.NotFound(new { StatusCode = 404, Message = "Person not found" });
                            }

                            return Results.Ok(person);
                        })
                        .WithName("updatePersonByIdV1")
                        .WithSummary("Updates an existing person by ID")
                        .WithTags("Persons")
                        .Accepts<Person>(MediaTypeNames.Application.Json)
                        .Produces<Person>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                        .Produces<ProblemDetails>(StatusCodes.Status409Conflict)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}