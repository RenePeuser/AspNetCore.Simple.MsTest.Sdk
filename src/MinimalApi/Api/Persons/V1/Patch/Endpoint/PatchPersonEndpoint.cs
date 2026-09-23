using System.Net.Mime;
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
    // 🎯 SERVICE REGISTRATION: PatchPersonEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddPatchPersonEndpointExtension
    {
        public static void AddPatchPersonEndpoint(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IEndpoint, PatchPersonEndpoint>();
        }
    }

    internal sealed class PatchPersonEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapPatch("persons", (Person person) => Results.Ok(person))
                        .WithName("patchPersonV1")
                        .WithSummary("Partially updates a person")
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