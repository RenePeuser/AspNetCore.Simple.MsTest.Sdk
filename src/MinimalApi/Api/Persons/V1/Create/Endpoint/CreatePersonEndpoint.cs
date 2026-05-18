using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Endpoints;

namespace MinimalApi.Api.Persons.V1
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: CreatePersonEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddCreatePersonEndpointExtension
    {
        public static void AddCreatePersonEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, CreatePersonEndpoint>();
        }
    }

    internal sealed class CreatePersonEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPost("persons", (Person person) => Results.Created($"persons/{person.Id}", person))
                        .WithName("createPersonV1")
                        .WithSummary("Creates a new person")
                        .WithTags("Persons")
                        .Accepts<Person>(MediaTypeNames.Application.Json)
                        .Produces<Person>(StatusCodes.Status201Created)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}