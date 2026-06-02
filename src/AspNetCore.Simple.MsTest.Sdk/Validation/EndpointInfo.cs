using System;
using System.Collections.Immutable;
using Asp.Versioning;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    /// <summary>
    /// Represents a parsed endpoint with resolved metadata.
    /// </summary>
    public sealed record EndpointInfo
    {
        /// <summary>
        /// HTTP method (GET, POST, PUT, DELETE, etc.)
        /// </summary>
        public required string HttpMethod { get; init; }

        /// <summary>
        /// URL pattern with resolved placeholders like {DocumentName}, {apiVersion}.
        /// Route parameters like {id} are preserved.
        /// Example: /api/v1/users/{id}
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:URI-like properties should not be strings", Justification = "This is a route pattern, not a URI")]
        public required string Url { get; init; }

        /// <summary>
        /// Parsed API version from endpoint metadata (MVC or Minimal API).
        /// </summary>
        public required ApiVersion? ApiVersion { get; init; }

        /// <summary>
        /// Expected response type from ProducesResponseTypeAttribute metadata.
        /// Used for compile-time validation against TResult in tests.
        /// DEPRECATED: Use ResponseTypesByStatusCode instead for multi-status code support.
        /// </summary>
        public required Type? ResponseType { get; init; }

        /// <summary>
        /// Response types by HTTP status code.
        /// Key: Status code (200, 400, 404, etc.)
        /// Value: Response type for that status code
        /// Example: { 200: typeof(CreateEdgeResponse), 400: typeof(ValidationProblemDetailsExtended) }
        /// </summary>
        public ImmutableDictionary<int, Type> ResponseTypesByStatusCode { get; init; } = ImmutableDictionary<int, Type>.Empty;

        /// <summary>
        /// OpenAPI tags for grouping endpoints.
        /// Example: ["Edges", "Admin"]
        /// </summary>
        public ImmutableList<string> Tags { get; init; } = ImmutableList<string>.Empty;

        /// <summary>
        /// Endpoint name (from WithName() in Minimal API or Name property in MVC).
        /// Example: "createEdgeV1"
        /// </summary>
        public string? Name { get; init; }

        /// <summary>
        /// Endpoint description (from WithDescription() or XML comments).
        /// </summary>
        public string? Description { get; init; }

        /// <summary>
        /// Endpoint summary (from WithSummary() or XML comments).
        /// </summary>
        public string? Summary { get; init; }

        /// <summary>
        /// Source location of the endpoint (Controller class or Minimal API file).
        /// Examples:
        /// - "Controllers.PersonController.CreatePerson"
        /// - "Program.cs:MapPost"
        /// - "PersonEndpoints.cs:MapPersonEndpoints"
        /// </summary>
        public string? SourceLocation { get; init; }
    }
}