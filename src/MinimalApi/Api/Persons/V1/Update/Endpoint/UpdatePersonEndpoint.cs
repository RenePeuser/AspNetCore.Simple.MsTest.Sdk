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
                        .Produces<Person>(200)
                        .Produces<ProblemDetails>(400)
                        .Produces<ProblemDetails>(500)
                        .WithOpenApi();
        }
    }
}
