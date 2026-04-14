using System.Net.Mime;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.Update
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
                        .MapToApiVersion(1)
                        .WithOpenApi();
        }
    }
}
