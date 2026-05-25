using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle
{
    /// <summary>
    /// Provides ASP.NET Core Endpoint-style extensions for a more symmetrical test API.
    ///
    /// <para>
    /// Import this namespace to use .Accepts() and .Produces() methods that mirror your endpoint definitions.
    /// These are aliases for the core .WithBody() and .WithResponse() methods.
    /// </para>
    ///
    /// <example>
    /// Endpoint definition:
    /// <code>
    /// endpoints.MapPost("nodes", HandleAsync)
    ///     .Accepts&lt;CreateNodeRequest&gt;(MediaTypeNames.Application.Json)
    ///     .Produces&lt;CreateNodeResponse&gt;()
    ///     .Produces&lt;ValidationProblemDetails&gt;(StatusCodes.Status400BadRequest);
    /// </code>
    ///
    /// Test with EndpointStyle:
    /// <code>
    /// using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.EndpointStyle;
    ///
    /// await Client.AssertPost("nodes")
    ///     .Accepts&lt;CreateNodeRequest&gt;(request)
    ///     .Produces&lt;CreateNodeResponse&gt;("Expected.json")
    ///     .ExpectSuccess();
    ///
    /// await Client.AssertPost("nodes")
    ///     .Accepts&lt;CreateNodeRequest&gt;(invalidRequest)
    ///     .ProducesBadRequest&lt;ValidationProblemDetails&gt;("Error.json")
    ///     .ExpectError(HttpStatusCode.BadRequest);
    /// </code>
    /// </example>
    /// </summary>
    public static class EndpointStyleExtensions
    {
        // ============================================================
        // ACCEPTS - Aliases for WithBody (Request Body Configuration)
        // ============================================================

        /// <summary>
        /// Configures the request body (alias for WithBody).
        /// Mirrors ASP.NET Core's .Accepts&lt;T&gt;() endpoint configuration.
        /// </summary>
        /// <typeparam name="T">Type of the request body</typeparam>
        /// <param name="config">Request configuration</param>
        /// <param name="body">Request body object</param>
        /// <returns>Configuration builder for further setup</returns>
        public static IHttpRequestConfiguring Accepts<T>(
            this IHttpRequestConfiguring config,
            T body)
        {
            return config.WithBody(body);
        }

        /// <summary>
        /// Configures the request body with explicit content type.
        /// Mirrors ASP.NET Core's .Accepts&lt;T&gt;(contentType) endpoint configuration.
        /// </summary>
        /// <typeparam name="T">Type of the request body</typeparam>
        /// <param name="config">Request configuration</param>
        /// <param name="contentType">Content type (e.g., "application/json")</param>
        /// <param name="body">Request body object</param>
        /// <returns>Configuration builder for further setup</returns>
        /// <remarks>
        /// Content-Type is currently informational only. The request always sends JSON.
        /// Future versions may validate the Content-Type header.
        /// </remarks>
        public static IHttpRequestConfiguring Accepts<T>(
            this IHttpRequestConfiguring config,
            string contentType,
            T body)
        {
            // TODO: In future, could validate/set Content-Type header
            return config.WithBody(body);
        }

        /// <summary>
        /// Configures the request body as JSON string.
        /// Mirrors ASP.NET Core's .Accepts() endpoint configuration.
        /// </summary>
        /// <param name="config">Request configuration</param>
        /// <param name="bodyJson">JSON string representing the request body</param>
        /// <returns>Configuration builder for further setup</returns>
        public static IHttpRequestConfiguring Accepts(
            this IHttpRequestConfiguring config,
            string bodyJson)
        {
            return config.WithBody(bodyJson);
        }

        // ============================================================
        // PRODUCES - Aliases for WithResponse (Response Configuration)
        // ============================================================

        /// <summary>
        /// Configures the expected response (alias for WithResponse).
        /// Mirrors ASP.NET Core's .Produces&lt;T&gt;() endpoint configuration.
        /// Implicitly expects 200 OK status code.
        /// </summary>
        /// <typeparam name="TResult">Expected response type</typeparam>
        /// <param name="config">Request configuration</param>
        /// <param name="expectedJson">JSON string or embedded resource path for expected response</param>
        /// <returns>Response configuration builder</returns>
        public static IHttpResponseConfiguring<TResult> Produces<TResult>(
            this IHttpRequestConfiguring config,
            string expectedJson)
        {
            return config.WithResponse<TResult>(expectedJson);
        }

        /// <summary>
        /// Configures the expected response with explicit content type.
        /// Mirrors ASP.NET Core's .Produces&lt;T&gt;(contentType) endpoint configuration.
        /// </summary>
        /// <typeparam name="TResult">Expected response type</typeparam>
        /// <param name="config">Request configuration</param>
        /// <param name="contentType">Expected content type (e.g., "application/json")</param>
        /// <param name="expectedJson">JSON string or embedded resource path</param>
        /// <returns>Response configuration builder</returns>
        /// <remarks>
        /// Content-Type is currently informational only. Future versions may validate it.
        /// </remarks>
        public static IHttpResponseConfiguring<TResult> Produces<TResult>(
            this IHttpRequestConfiguring config,
            string contentType,
            string expectedJson)
        {
            // TODO: In future, could validate Content-Type header
            return config.WithResponse<TResult>(expectedJson);
        }

        // ============================================================
        // NOTE: We intentionally do NOT provide ProducesOk, ProducesCreated,
        // ProducesBadRequest, etc. convenience methods.
        //
        // Reason: Consistency and clarity. All tests should explicitly state
        // their expected status code using .Expect() or .ExpectSuccess().
        //
        // Example:
        //   .Produces<Person>("response.json")
        //   .ExpectSuccess()
        //
        //   .Produces<Person>("response.json")
        //   .Expect(HttpStatusCode.Created)
        //
        // This makes it crystal clear what status code is expected, and keeps
        // the API simple with fewer methods to learn.
        // ============================================================
    }
}