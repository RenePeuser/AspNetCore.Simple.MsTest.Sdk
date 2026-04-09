using System;
using Asp.Versioning;

namespace AspNetCore.Simple.MsTest.Sdk
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
        /// </summary>
        public required Type? ResponseType { get; init; }
    }
}
