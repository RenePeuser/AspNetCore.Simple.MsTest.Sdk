using ApiVersion = Asp.Versioning.ApiVersion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace MinimalApi.Api.Versioning
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 V1 AND V2 SIDE BY SIDE
    // ═══════════════════════════════════════════════════════════════════════════════════
    //
    // The typical migration phase of an API: V1 and V2 run at the same time.
    // - "versioning/number" exists once per version - the same route, two handlers
    // - "versioning/shared" is one handler mapped to V1 and V2
    //
    // The version set declares 1 and 2 for every endpoint in it. Endpoint validation has to
    // use the version each endpoint is mapped to (MapToApiVersion), otherwise both "number"
    // handlers look like V1 and the tests fail with AMBIGUOUS ENDPOINT MATCH / ENDPOINT NOT FOUND.
    //
    // The same V1/V2 pair exists three times, once per way a client sends the version:
    // - url segment:  api/v2/versioning/number
    // - query string: api/versioning-query/number?api-version=2
    // - header:       api/versioning-header/number   +   x-api-version: 2
    //
    // Own groups and named version sets, so the rest of the sample keeps reporting
    // "api-supported-versions: 1".
    internal static class MapVersioningEndpointsExtension
    {
        public static void MapVersioningEndpoints(this IEndpointRouteBuilder app)
        {
            MapUrlSegmentVersioning(app);
            MapNumberPerVersion(app, "api/versioning-query", "VersioningQuery");
            MapNumberPerVersion(app, "api/versioning-header", "VersioningHeader");
        }

        // Query string and header: the route carries no version - Asp.Versioning reads it from
        // ?api-version=... or the x-api-version header (see ApiVersionReader in Program.cs)
        private static void MapNumberPerVersion(IEndpointRouteBuilder app,
                                                string prefix,
                                                string name)
        {
            var versionSet = app.NewApiVersionSet(name)
                                .HasApiVersion(new ApiVersion(1))
                                .HasApiVersion(new ApiVersion(2))
                                .ReportApiVersions()
                                .Build();

            var group = app.MapGroup(prefix)
                           .WithApiVersionSet(versionSet);

            group.MapGet("number", () => Results.Ok(1))
                 .WithName($"get{name}NumberV1")
                 .WithTags(name)
                 .Produces<int>(StatusCodes.Status200OK)
                 .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                 .MapToApiVersion(1);

            group.MapGet("number", () => Results.Ok("one"))
                 .WithName($"get{name}NumberV2")
                 .WithTags(name)
                 .Produces<string>(StatusCodes.Status200OK)
                 .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                 .MapToApiVersion(2);
        }

        private static void MapUrlSegmentVersioning(IEndpointRouteBuilder app)
        {
            var versionSet = app.NewApiVersionSet("Versioning")
                                .HasApiVersion(new ApiVersion(1))
                                .HasApiVersion(new ApiVersion(2))
                                .ReportApiVersions()
                                .Build();

            var group = app.MapGroup("api/v{version:apiVersion}/versioning")
                           .WithApiVersionSet(versionSet);

            group.MapGet("number", () => Results.Ok(1))
                 .WithName("getNumberV1")
                 .WithSummary("V1 returns the number as integer")
                 .WithTags("Versioning")
                 .Produces<int>(StatusCodes.Status200OK)
                 .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                 .MapToApiVersion(1);

            group.MapGet("number", () => Results.Ok("one"))
                 .WithName("getNumberV2")
                 .WithSummary("V2 returns the number as text")
                 .WithTags("Versioning")
                 .Produces<string>(StatusCodes.Status200OK)
                 .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                 .MapToApiVersion(2);

            group.MapGet("shared", () => Results.Ok(7))
                 .WithName("getShared")
                 .WithSummary("One handler for V1 and V2")
                 .WithTags("Versioning")
                 .Produces<int>(StatusCodes.Status200OK)
                 .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
                 .MapToApiVersion(1)
                 .MapToApiVersion(2);

            // No MapToApiVersion: serves every version of the version set
            group.MapGet("implicit", () => Results.Ok(9))
                 .WithName("getImplicit")
                 .WithSummary("No mapping - serves V1 and V2")
                 .WithTags("Versioning")
                 .Produces<int>(StatusCodes.Status200OK)
                 .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
        }
    }
}
