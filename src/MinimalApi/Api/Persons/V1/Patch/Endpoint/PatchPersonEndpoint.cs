using System.Net.Mime;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using StrategyPattern.Evolution;

namespace MinimalApi.Api.Persons.V1.Patch
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: PatchPersonEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddPatchPersonEndpointExtension
    {
        public static void AddPatchPersonEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, PatchPersonEndpoint>();
        }
    }

    internal sealed class PatchPersonEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPatch("persons", (Person person) =>
                                             {
                                                 return Results.Ok(person);
                                             })
                        .WithName("patchPersonV1")
                        .WithSummary("Partially updates a person")
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
