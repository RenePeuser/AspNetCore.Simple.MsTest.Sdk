using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
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
            routeBuilder.MapDelete("persons/{id:long}", (long id) =>
                        {
                            if (id == 999)
                            {
                                return Results.NotFound(new
                                                        {
                                                            StatusCode = 404,
                                                            Message = "Person not found"
                                                        });
                            }

                            return Results.NoContent();
                        })
                        .WithName("deletePersonV1")
                        .WithSummary("Deletes a person by ID")
                        .WithTags("Persons")
                        .Produces(StatusCodes.Status204NoContent)
                        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                        .MapToApiVersion(1);
        }
    }
}