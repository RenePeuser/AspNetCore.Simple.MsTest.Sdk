using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Contains all preprocessed HTTP response data for assertion processing.
    /// Built after the HTTP call completes - all data preparation happens here before strategy execution.
    /// Follows the data collection pattern: no logic, only prepared data.
    /// NOTE: Does not hold HttpResponseMessage reference as it needs to be disposed.
    /// All required data is extracted before disposal.
    /// </summary>
    /// <typeparam name="TResult">The type of the deserialized response</typeparam>
    public record HttpResponseContext<TResult>
    {
        /// <summary>
        /// The original request context containing all request parameters.
        /// Immutable reference to the request configuration.
        /// </summary>
        public required HttpAssertContext<TResult> Request { get; init; }

        /// <summary>
        /// The HTTP status code from the response.
        /// Extracted from HttpResponseMessage before disposal.
        /// </summary>
        public required HttpStatusCode HttpStatusCode { get; init; }

        /// <summary>
        /// The raw response content as string (before parameter replacement).
        /// Direct output from HttpResponseMessage.Content.ReadAsStringAsync().
        /// </summary>
        public required string ContentAsString { get; init; }

        /// <summary>
        /// The response content with parameters resolved.
        /// Ready-to-use JSON string for deserialization or comparison.
        /// </summary>
        public required string ResolvedParametersJsonString { get; init; }

        /// <summary>
        /// Indicates whether the status code matches expectations.
        /// true = response.IsSuccessStatusCode == context.IsSuccessStatusCode
        /// false = unexpected status code (will trigger fast-fail strategy)
        /// </summary>
        public required bool IsExpectedStatusCode { get; init; }

        /// <summary>
        /// The deserialized current result (primitive or complex type).
        /// Result of deserializing ResolvedParametersJsonString to TResult.
        /// </summary>
        public required TResult? CurrentResult { get; init; }

        /// <summary>
        /// The current result after applying the OrderFunc filter.
        /// This is what will be compared against the expected result.
        /// </summary>
        public required TResult? FilteredCurrentResult { get; init; }

        /// <summary>
        /// The simplified HTTP response message extracted from HttpResponseMessage before disposal.
        /// Contains status code, headers, and metadata but not the raw HttpResponseMessage.
        /// </summary>
        public required SimpleHttpResponseMessage SimpleHttpResponseMessage { get; init; }

        /// <summary>
        /// The response content headers extracted and preprocessed before HttpResponseMessage disposal.
        /// Used for building comparison structures.
        /// </summary>
        public required ImmutableList<KeyValuePair<string, ImmutableList<string>>> ContentHeaders { get; init; }

        /// <summary>
        /// The absolute URL that was called (resolved from HttpResponseMessage).
        /// Used for output formatting and debugging.
        /// </summary>
#pragma warning disable CA1056 // URI properties should not be strings - kept as string for compatibility with existing formatters
        public required string AbsoluteUrl { get; init; }
#pragma warning restore CA1056

        /// <summary>
        /// Formatted HTTP call information for output/error messages.
        /// Contains method, URL, and status code in readable format.
        /// </summary>
        public required string HttpCallInfo { get; init; }

        /// <summary>
        /// Generated cURL command representing this HTTP call.
        /// Used for debugging and reproducibility.
        /// </summary>
        public required string Curl { get; init; }
    }
}
