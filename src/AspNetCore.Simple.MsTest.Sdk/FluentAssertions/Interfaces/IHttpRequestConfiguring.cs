using System.Net;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Fluent interface for configuring HTTP request parameters.
    /// This is the initial state after calling AssertPost/Get/Put/etc.
    /// </summary>
    public interface IHttpRequestConfiguring
    {
        /// <summary>
        /// Configures the request body from a JSON string.
        /// </summary>
        /// <param name="bodyJson">JSON string representing the request body</param>
        /// <returns>Configuration builder for further request setup</returns>
        IHttpRequestConfiguring WithBody(string bodyJson);

        /// <summary>
        /// Configures the request body from an object (will be serialized to JSON).
        /// </summary>
        /// <typeparam name="T">Type of the request body</typeparam>
        /// <param name="body">Request body object</param>
        /// <returns>Configuration builder for further request setup</returns>
        IHttpRequestConfiguring WithBody<T>(T body);

        /// <summary>
        /// Configures parameters that will be replaced in request/response JSON placeholders.
        /// </summary>
        /// <param name="parameters">Array of key-value pairs for parameter substitution</param>
        /// <returns>Configuration builder for further request setup</returns>
        IHttpRequestConfiguring WithParameters(params (string Key, object? Value)[] parameters);

        /// <summary>
        /// Adds a custom HTTP header to the request.
        /// </summary>
        /// <param name="key">Header name</param>
        /// <param name="value">Header value</param>
        /// <returns>Configuration builder for further request setup</returns>
        IHttpRequestConfiguring WithHeader(string key,
                                           string value);

        /// <summary>
        /// Configures the expected response body and transitions to response configuration state.
        /// </summary>
        /// <typeparam name="TResult">Expected response type</typeparam>
        /// <param name="expectedJson">JSON string or embedded resource path for expected response</param>
        /// <returns>Response configuration builder</returns>
        IHttpResponseConfiguring<TResult> WithResponse<TResult>(string expectedJson);

        /// <summary>
        /// Expects any successful HTTP status code (2xx range) without validating response body.
        /// This is a terminal operation.
        /// </summary>
        /// <returns>Assertable that can be awaited to execute the request</returns>
        IHttpStatusAssertable ExpectSuccess();

        /// <summary>
        /// Expects one of the specified HTTP status codes without validating response body.
        /// This is a terminal operation.
        /// </summary>
        /// <param name="codes">Accepted HTTP status codes</param>
        /// <returns>Assertable that can be awaited to execute the request</returns>
        IHttpStatusAssertable Expect(params HttpStatusCode[] codes);

        /// <summary>
        /// Expects HTTP 204 No Content status code.
        /// This is a terminal operation.
        /// </summary>
        /// <returns>Assertable that can be awaited to execute the request</returns>
        IHttpStatusAssertable ExpectNoContent();

        /// <summary>
        /// Expects a specific error status code (4xx or 5xx) without validating response body.
        /// This is a terminal operation.
        /// </summary>
        /// <param name="code">Expected error status code</param>
        /// <returns>Assertable that can be awaited to execute the request</returns>
        IHttpStatusAssertable ExpectError(HttpStatusCode code);
    }
}