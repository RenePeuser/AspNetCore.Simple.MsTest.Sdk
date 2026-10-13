using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Extensions.Pack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.Endpoints;

namespace MinimalApi.Api.Persons.V1
{
    /// <summary>
    /// A deterministic, nested person profile.
    ///
    /// <para>
    /// Exists so a test can prove that an ignore rule hits exactly ONE position: <c>name</c> occurs at the
    /// root, inside <c>address</c> and inside every <c>contacts</c> element, and <c>zipCode</c> is renamed to
    /// <c>zip_code</c> on the wire. A rule that matches by bare name, or ignores the JSON name, cannot pass
    /// those tests.
    /// </para>
    /// </summary>
    public sealed record PersonProfile(long Id,
                                       string Name,
                                       ProfileAddress Address,
                                       ImmutableList<ProfileContact> Contacts);

    public sealed record ProfileAddress(string Name,
                                        string Street,
                                        [property: JsonPropertyName("zip_code")]
                                        string ZipCode);

    public sealed record ProfileContact(string Name,
                                        string Value);

    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 SERVICE REGISTRATION: GetPersonProfileEndpoint
    // ═══════════════════════════════════════════════════════════════════════════════════
    public static class AddGetPersonProfileEndpointExtension
    {
        public static void AddGetPersonProfileEndpoint(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IEndpoint, GetPersonProfileEndpoint>();
        }
    }

    internal sealed class GetPersonProfileEndpoint : IEndpoint
    {
        public void Map(IEndpointRouteBuilder routeBuilder)
        {
            routeBuilder.MapGet("persons/{id:long}/profile",
                                (long id) => Results.Ok(new PersonProfile(id,
                                                                          "Son Goku",
                                                                          new ProfileAddress("Home", "Kame House 1", "12345"),
                                                                          ImmutableList.Create(new ProfileContact("Work", "goku@capsule.corp"),
                                                                                               new ProfileContact("Private", "goku@kame.house")))))
                        .WithName("getPersonProfileV1")
                        .WithSummary("Gets a nested, deterministic person profile")
                        .WithTags("Persons")
                        .Produces<PersonProfile>(StatusCodes.Status200OK)
                        .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                        .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError)
                        .MapToApiVersion(1);
        }
    }
}
