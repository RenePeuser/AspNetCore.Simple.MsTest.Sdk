using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Context object for HTTP assertion methods that bundles common parameters
    /// to provide a cleaner API compared to methods with many individual parameters.
    /// </summary>
    /// <typeparam name="TResult">The expected result type</typeparam>
    public sealed record HttpAssertContext<TResult>
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
        /// The expected result as JSON string or file name.
        /// Can be a JSON string, a file name like "expected.json", or an embedded resource path.
        /// </summary>
        public required string? ExpectedResult { get; init; }

        /// <summary>
        /// Optional filter function to transform the result before comparison.
        /// Useful for filtering out dynamic properties like timestamps or IDs.
        /// </summary>
        public Func<TResult, TResult> FilterFunc { get; init; } = item => item;

        /// <summary>
        /// Optional function to filter differences found during comparison.
        /// Allows ignoring specific differences that are expected.
        /// </summary>
        public Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; init; } = item => item;

        /// <summary>
        /// Parameters to replace in the URL, payload, and expected result.
        /// Format: (Key, Value) tuples where Key is the placeholder and Value is the replacement.
        /// </summary>
        public required (string Key, object? Value)[] Parameters { get; init; } = [];

        /// <summary>
        /// The calling assembly. If not provided, will be automatically determined.
        /// </summary>
        public required Assembly CallingAssembly { get; init; }

        /// <summary>
        /// Whether to write the response to disk when the assertion fails.
        /// Useful for updating test snapshots.
        /// </summary>
        public required bool WriteResponse { get; init; }

        /// <summary>
        /// Whether the HTTP call is expected to succeed (2xx status code).
        /// Set to false when testing error scenarios.
        /// </summary>
        public required bool IsSuccessStatusCode { get; init; } = true;

        /// <summary>
        /// The file path of the calling test method. Usually auto-filled by CallerFilePath.
        /// </summary>
        public required string CallerFilePath { get; init; }

        /// <summary>
        /// The parameter name of the payload argument. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        public required string PayloadParameterName { get; init; }

        /// <summary>
        /// The parameter name of the expected result argument. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        public required string ExpectedResultParameterName { get; init; }

        /// <summary>
        /// Controls the visibility of the token in curl outputs.
        /// Set to true to show the token in generated curl commands.
        /// </summary>
        public bool ShowTokenInCurl { get; init; }
    }

    /// <summary>
    /// Context object for HTTP assertion methods without a result type.
    /// </summary>
    public sealed record HttpAssertContext
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
        /// Parameters to replace in the URL and payload.
        /// Format: (Key, Value) tuples where Key is the placeholder and Value is the replacement.
        /// </summary>
        public required (string Key, object? Value)[] Parameters { get; init; } = [];

        /// <summary>
        /// The calling assembly. If not provided, will be automatically determined.
        /// </summary>
        public required Assembly CallingAssembly { get; init; }

        /// <summary>
        /// Whether to write the response to disk.
        /// </summary>
        public required bool WriteResponse { get; init; }

        /// <summary>
        /// Whether the HTTP call is expected to succeed (2xx status code).
        /// Set to false when testing error scenarios.
        /// </summary>
        public required bool IsSuccessStatusCode { get; init; } = true;

        /// <summary>
        /// The file path of the calling test method. Usually auto-filled by CallerFilePath.
        /// </summary>
        public required string CallerFilePath { get; init; }

        /// <summary>
        /// The parameter name of the payload argument. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        public required string PayloadParameterName { get; init; }

        /// <summary>
        /// Controls the visibility of the token in curl outputs.
        /// Set to true to show the token in generated curl commands.
        /// </summary>
        public bool ShowTokenInCurl { get; init; }
    }
}
