using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Context object for HTTP assertion methods that bundles common parameters
    /// to provide a cleaner API compared to methods with many individual parameters.
    /// Inherits from ObjectAssertContext to reuse comparison logic.
    /// </summary>
    /// <typeparam name="TResult">The expected result type</typeparam>
    public record HttpAssertContext<TResult> : ObjectAssertContext<TResult>
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
        public string? PayloadAsJson { get; init; }

        /// <summary>
        /// Alias for ExpectedObjectAsJson from base class for backward compatibility.
        /// The expected result as JSON string or file name.
        /// </summary>
        public string? ExpectedResult
        {
            get => ExpectedObjectAsJson;
            init => ExpectedObjectAsJson = value ?? string.Empty;
        }

        /// <summary>
        /// Alias for OrderFunc from base class for backward compatibility.
        /// Optional filter function to transform the result before comparison.
        /// Useful for filtering out dynamic properties like timestamps or IDs.
        /// Note: Function must handle nullable inputs/outputs.
        /// </summary>
        public Func<TResult?, TResult?> FilterFunc
        {
            get => OrderFunc;
            init => OrderFunc = value;
        }

        /// <summary>
        /// Whether the HTTP call is expected to succeed (2xx status code).
        /// Set to false when testing error scenarios.
        /// </summary>
        public bool IsSuccessStatusCode { get; init; } = true;

        /// <summary>
        /// The parameter name of the payload argument. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        public string PayloadParameterName { get; init; } = string.Empty;

        /// <summary>
        /// Controls the visibility of the token in curl outputs.
        /// Set to true to show the token in generated curl commands.
        /// </summary>
        public bool ShowTokenInCurl { get; init; }

        /// <summary>
        /// Curl command for reproducing the HTTP call.
        /// </summary>
        public string? Curl { get; init; }
    }
}
