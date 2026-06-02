using Microsoft.AspNetCore.Mvc;
using MinimalApi.Endpoints;

namespace MinimalApi.Api.Persons.V1
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: DeletePersonEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddDeletePersonEndpointExtension
    {
        public static void AddDeletePersonEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, DeletePersonEndpoint>();
        }
    }

    internal sealed class DeletePersonEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapDelete("persons/{id:long}", (long id) => Results.NoContent())
                        .WithName("deletePersonV1")
                        .WithSummary("Deletes a person by ID")
                        .WithTags("Persons")
                        .Produces(StatusCodes.Status204NoContent)
                        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                        .MapToApiVersion(1);
        }
    }
}