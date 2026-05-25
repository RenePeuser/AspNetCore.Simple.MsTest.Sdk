using System.Net;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public interface IHttpAssertContext : IObjectAssertContext
    {
        /// <summary>
        /// The HttpClient instance to use for making the request.
        /// </summary>
        HttpClient Client { get; init; }

        /// <summary>
        /// The URL to call. Can contain placeholders like {userId} that will be replaced using Parameters.
        /// </summary>
#pragma warning disable CA1056 // URI-like properties should not be strings
        string Url { get; init; }
#pragma warning restore CA1056 // URI-like properties should not be strings

        /// <summary>
        /// The HTTP method to use for the request (GET, POST, PUT, PATCH, DELETE, etc.).
        /// </summary>
        HttpMethod HttpMethod { get; init; }

        /// <summary>
        /// The payload as JSON string or file name.
        /// Can be a JSON string, a file name like "request.json", or an embedded resource path.
        /// </summary>
        string? PayloadAsJson { get; init; }

        /// <summary>
        /// The localized payload file info.
        /// Contains the resolved embedded request file information including content and physical file location.
        /// This is resolved once during context creation and reused throughout the assertion pipeline.
        /// </summary>
        EmbeddedFileInfo? PayloadFile { get; init; }

        /// <summary>
        /// The fully resolved payload JSON with all parameters replaced.
        /// This is ready-to-use and prepared once during context creation.
        /// Avoids repeated parameter resolution throughout the assertion pipeline.
        /// </summary>
        string? ResolvedPayload { get; init; }

        /// <summary>
        /// Whether the HTTP call is expected to succeed (2xx status code).
        /// Set to false when testing error scenarios.
        /// </summary>
        bool IsSuccessStatusCode { get; init; }

        /// <summary>
        /// The parameter name of the payload argument. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        string PayloadParameterName { get; init; }

        /// <summary>
        /// Controls the visibility of the token in curl outputs.
        /// Set to true to show the token in generated curl commands.
        /// </summary>
        bool ShowTokenInCurl { get; init; }

        /// <summary>
        /// The API version for the endpoint.
        /// Used to filter endpoints when using API versioning (e.g., v1, v2).
        /// Null means no specific version is required.
        /// </summary>
        string? ApiVersion { get; init; }

        /// <summary>
        /// When true, skips the response content comparison but still validates endpoint and status code.
        /// Useful for process chain tests where only the success of the call matters.
        /// The response type must still be specified correctly for endpoint validation.
        /// </summary>
        bool IgnoreResponse { get; init; }

        /// <summary>
        /// When true, skips the endpoint validation entirely.
        /// Useful when testing external APIs where endpoint metadata is not available,
        /// or when intentionally using a different response type than defined in the endpoint.
        /// </summary>
        bool SkipEndpointValidation { get; init; }

        /// <summary>
        /// The expected HTTP status code for the request.
        /// Used to validate that the endpoint returns the correct status code.
        /// When null (default), the status code is determined by:
        /// 1. The StatusCode field in the expected JSON file (if present)
        /// 2. The test type (OK/200 for success tests, BadRequest/400 for error tests)
        /// </summary>
        HttpStatusCode? ExpectedHttpStatusCode { get; init; }
    }

    /// <summary>
    /// Context object for HTTP assertion methods that bundles common parameters
    /// to provide a cleaner API compared to methods with many individual parameters.
    /// Inherits from ObjectAssertContext to reuse comparison logic.
    /// </summary>
    /// <typeparam name="TResult">The expected result type</typeparam>
    public record HttpAssertContext<TResult> : ObjectAssertContext<TResult>, IHttpAssertContext
    {
        /// <summary>
        /// The HttpClient instance to use for making the request.
        /// </summary>
        public required HttpClient Client { get; init; }

        /// <summary>
        /// The URL to call. Can contain placeholders like {userId} that will be replaced using Parameters.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1056:Uri properties should not be strings",
                                                            Justification = "URL can contain template placeholders like {userId} that need string manipulation")]
        public required string Url { get; init; }

        /// <summary>
        /// The HTTP method to use for the request (GET, POST, PUT, PATCH, DELETE, etc.).
        /// </summary>
        public required HttpMethod HttpMethod { get; init; }

        /// <summary>
        /// The payload as JSON string or file name.
        /// Can be a JSON string, a file name like "request.json", or an embedded resource path.
        /// </summary>
        public required string? PayloadAsJson { get; init; }

        /// <summary>
        /// The localized payload file info.
        /// Contains the resolved embedded request file information including content and physical file location.
        /// This is resolved once during context creation and reused throughout the assertion pipeline.
        /// </summary>
        public required EmbeddedFileInfo? PayloadFile { get; init; }

        /// <summary>
        /// The fully resolved payload JSON with all parameters replaced.
        /// This is ready-to-use and prepared once during context creation.
        /// Avoids repeated parameter resolution throughout the assertion pipeline.
        /// </summary>
        public required string? ResolvedPayload { get; init; }

        /// <summary>
        /// Whether the HTTP call is expected to succeed (2xx status code).
        /// Set to false when testing error scenarios.
        /// </summary>
        public required bool IsSuccessStatusCode { get; init; } = true;

        /// <summary>
        /// The parameter name of the payload argument. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        public required string PayloadParameterName { get; init; } = string.Empty;

        /// <summary>
        /// Controls the visibility of the token in curl outputs.
        /// Set to true to show the token in generated curl commands.
        /// </summary>
        public required bool ShowTokenInCurl { get; init; }

        /// <summary>
        /// The API version for the endpoint.
        /// Used to filter endpoints when using API versioning (e.g., "1", "2", "v1", "v2").
        /// Null means no specific version is required (matches any version or unversioned endpoints).
        /// </summary>
        public required string? ApiVersion { get; init; }

        /// <summary>
        /// When true, skips the response content comparison but still validates endpoint and status code.
        /// Useful for process chain tests where only the success of the call matters.
        /// The response type must still be specified correctly for endpoint validation.
        /// </summary>
        public required bool IgnoreResponse { get; init; }

        /// <summary>
        /// When true, skips the endpoint validation entirely.
        /// Useful when testing external APIs where endpoint metadata is not available,
        /// or when intentionally using a different response type than defined in the endpoint.
        /// </summary>
        public required bool SkipEndpointValidation { get; init; }

        /// <summary>
        /// The expected HTTP status code for the request.
        /// Used to validate that the endpoint returns the correct status code.
        /// When null (default), the status code is determined by:
        /// 1. The StatusCode field in the expected JSON file (if present)
        /// 2. The test type (OK/200 for success tests, BadRequest/400 for error tests)
        /// </summary>
        public required HttpStatusCode? ExpectedHttpStatusCode { get; init; }
    }
}