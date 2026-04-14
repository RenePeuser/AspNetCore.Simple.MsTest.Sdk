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
            routeBuilder.MapPatch("persons", () =>
                                             {
                                                 var person = new Person(1, "Son", "Goku",
                                                                         99,
                                                                         [new Email("alf@gmx.de", "GMX"), new Email("abc@hotmail.de", "Microsoft")]);

                                                 return Results.Ok(person);
                                             })
                        .WithName("patchPersonV1")
                        .WithSummary("Partially updates a person")
                        .WithTags("Persons")
                        .Produces<Person>(200)
                        .Produces<ProblemDetails>(400)
                        .Produces<ProblemDetails>(500)
                        .WithOpenApi();
        }
    }
}
