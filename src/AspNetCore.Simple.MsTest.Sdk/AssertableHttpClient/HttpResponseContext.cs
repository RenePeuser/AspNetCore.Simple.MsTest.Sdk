using System.Net;
using System.Net.Http;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Contains all preprocessed HTTP response data for assertion processing.
    /// Built after the HTTP call completes - all data preparation happens here before strategy execution.
    /// Follows the data collection pattern: no logic, only prepared data.
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
        /// The raw HTTP response message from the server.
        /// Contains status code, headers, and content.
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
    }
}
