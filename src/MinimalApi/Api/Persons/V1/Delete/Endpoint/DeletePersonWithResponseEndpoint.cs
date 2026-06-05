using System;
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
    // 🎯 SERVICE REGISTRATION: DeletePersonWithResponseEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddDeletePersonWithResponseEndpointExtension
    {
        public static void AddDeletePersonWithResponseEndpoint(this IServiceCollection services)
        {
            services.AddSingleton<IEndpoint, DeletePersonWithResponseEndpoint>();
        }
    }

    internal sealed class DeletePersonWithResponseEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapDelete("persons/{id:long}/with-response", (long id) =>
                        {
                            var response = new DeletePersonResponse(id,
                                                                    "Son",
                                                                    "Goku",
                                                                    true,
                                                                    DateTime.UtcNow);

                            return Results.Ok(response);
                        })
                        .WithName("deletePersonWithResponseV1")
                        .WithSummary("Deletes a person by ID and returns confirmation")
                        .WithTags("Persons")
                        .Produces<DeletePersonResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                        .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
                        .MapToApiVersion(1);
        }
    }

    public record DeletePersonResponse(long Id,
                                       string Name,
                                       string FirstName,
                                       bool Deleted,
                                       DateTime DeletedAt);
}