using System.Net;
using System.Net.Http;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    /// <summary>
    /// Contains minimal HTTP response data for assertion processing.
    /// The HttpResponseMessage is kept alive during pipeline execution - steps can access it directly.
    /// Follows lazy evaluation: steps extract only the data they need.
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
        /// The raw HTTP response message.
        /// IMPORTANT: This is still alive during pipeline execution (disposed after pipeline completes).
        /// Steps can access headers, status code, content, etc. directly.
        /// </summary>
        public required HttpResponseMessage HttpResponseMessage { get; init; }

        /// <summary>
        /// The HTTP status code from the response (cached for quick access).
        /// </summary>
        public required HttpStatusCode HttpStatusCode { get; init; }

        /// <summary>
        /// The raw response content as string (before parameter replacement).
        /// Pre-read from HttpResponseMessage.Content.ReadAsStringAsync() to avoid multiple reads.
        /// </summary>
        public required string ContentAsString { get; init; }

        /// <summary>
        /// The response content with parameters resolved.
        /// Ready-to-use JSON string for deserialization or comparison.
        /// </summary>
        public required string ResolvedParametersJsonString { get; init; }

        /// <summary>
        /// The deserialized current result from the API call.
        /// This is the original, unmodified response that will be returned to the user.
        /// Can be null if deserialization fails or response is empty.
        /// </summary>
        public required TResult? CurrentResult { get; init; }

        /// <summary>
        /// Indicates whether the status code matches expectations.
        /// true = response.IsSuccessStatusCode == context.IsSuccessStatusCode
        /// false = unexpected status code (will trigger fast-fail in StatusCodeValidationStep)
        /// </summary>
        public required bool IsExpectedStatusCode { get; init; }

        /// <summary>
        /// The absolute URL that was called (resolved from HttpResponseMessage.RequestUri).
        /// Used for output formatting and debugging.
        /// </summary>
#pragma warning disable CA1056 // URI properties should not be strings - kept as string for compatibility with existing formatters
        public required string AbsoluteUrl { get; init; }
#pragma warning restore CA1056
    }
}
