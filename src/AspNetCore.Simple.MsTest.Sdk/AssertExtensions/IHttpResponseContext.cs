using System.Net;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Represents an HTTP response context that extends HTTP assert context with response-specific data.
    /// This interface provides access to both request information (via IHttpAssertContext)
    /// and response information (status code, response message, etc.).
    /// Contains all non-generic response properties - generic properties remain in HttpResponseContext&lt;TResult&gt;.
    /// </summary>
    public interface IHttpResponseContext : IHttpAssertContext
    {
        /// <summary>
        /// The raw HTTP response message.
        /// IMPORTANT: This is still alive during pipeline execution (disposed after pipeline completes).
        /// Steps can access headers, status code, content, etc. directly.
        /// </summary>
        HttpResponseMessage HttpResponseMessage { get; }

        /// <summary>
        /// The HTTP status code from the response (cached for quick access).
        /// </summary>
        HttpStatusCode HttpStatusCode { get; }

        /// <summary>
        /// The raw response content as string (before parameter replacement).
        /// Pre-read from HttpResponseMessage.Content.ReadAsStringAsync() to avoid multiple reads.
        /// </summary>
        string ContentAsString { get; }

        /// <summary>
        /// The response content with parameters resolved.
        /// Ready-to-use JSON string for deserialization or comparison.
        /// </summary>
        string ContentAsStringParameterized { get; }

        /// <summary>
        /// Indicates whether the status code matches expectations.
        /// true = response.IsSuccessStatusCode == context.IsSuccessStatusCode
        /// false = unexpected status code (will trigger fast-fail in StatusCodeValidationStep)
        /// </summary>
        bool IsExpectedStatusCode { get; }

        /// <summary>
        /// The absolute URL that was called (resolved from HttpResponseMessage.RequestUri).
        /// Used for output formatting and debugging.
        /// </summary>
#pragma warning disable CA1056 // URI properties should not be strings - kept as string for compatibility with existing formatters
        string AbsoluteUrl { get; }
#pragma warning restore CA1056
    }
}
