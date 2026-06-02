using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions
{
    /// <summary>
    /// Fluent API extensions for HttpClient to enable readable, chainable HTTP assertions.
    ///
    /// <para>
    /// This is the core neutral-style API. For endpoint-symmetric extensions (Accepts/Produces),
    /// also import the EndpointStyle namespace.
    /// </para>
    ///
    /// <example>
    /// Basic usage:
    /// <code>
    /// await Client.AssertPost("api/persons")
    ///     .WithBody(person)
    ///     .WithResponse&lt;Person&gt;("Expected.json")
    ///     .ExpectSuccess();
    /// </code>
    /// </example>
    /// </summary>

    // ToDo: internal still under construction !
    internal static class HttpClientFluentExtensions
    {
        /// <summary>
        /// Initiates a fluent HTTP POST assertion chain.
        /// </summary>
        /// <param name="client">The HTTP client to use for the request</param>
        /// <param name="url">The request URL (relative or absolute)</param>
        /// <param name="callerFilePath">Automatically captured caller file path for embedded resource resolution</param>
        /// <returns>Request configuration builder</returns>
        /// <example>
        /// <code>
        /// await Client.AssertPost("api/persons")
        ///     .WithBody(person)
        ///     .WithResponse&lt;Person&gt;("Expected.json")
        ///     .ExpectSuccess();
        /// </code>
        /// </example>
        public static IHttpRequestConfiguring AssertPost(this HttpClient client,
                                                         string url,
                                                         [CallerFilePath] string callerFilePath = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return new HttpRequestBuilder(client, HttpMethod.Post, url,
                                          callingAssembly, callerFilePath);
        }

        /// <summary>
        /// Initiates a fluent HTTP GET assertion chain.
        /// </summary>
        /// <param name="client">The HTTP client to use for the request</param>
        /// <param name="url">The request URL (relative or absolute)</param>
        /// <param name="callerFilePath">Automatically captured caller file path for embedded resource resolution</param>
        /// <returns>Request configuration builder</returns>
        /// <example>
        /// <code>
        /// await Client.AssertGet("api/persons")
        ///     .WithResponse&lt;List&lt;Person&gt;&gt;("Expected.json")
        ///     .ExpectSuccess();
        /// </code>
        /// </example>
        public static IHttpRequestConfiguring AssertGet(this HttpClient client,
                                                        string url,
                                                        [CallerFilePath] string callerFilePath = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return new HttpRequestBuilder(client, HttpMethod.Get, url,
                                          callingAssembly, callerFilePath);
        }

        /// <summary>
        /// Initiates a fluent HTTP PUT assertion chain.
        /// </summary>
        /// <param name="client">The HTTP client to use for the request</param>
        /// <param name="url">The request URL (relative or absolute)</param>
        /// <param name="callerFilePath">Automatically captured caller file path for embedded resource resolution</param>
        /// <returns>Request configuration builder</returns>
        /// <example>
        /// <code>
        /// await Client.AssertPut("api/persons/1")
        ///     .WithBody(updatedPerson)
        ///     .WithResponse&lt;Person&gt;("Expected.json")
        ///     .ExpectSuccess();
        /// </code>
        /// </example>
        public static IHttpRequestConfiguring AssertPut(this HttpClient client,
                                                        string url,
                                                        [CallerFilePath] string callerFilePath = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return new HttpRequestBuilder(client, HttpMethod.Put, url,
                                          callingAssembly, callerFilePath);
        }

        /// <summary>
        /// Initiates a fluent HTTP PATCH assertion chain.
        /// </summary>
        /// <param name="client">The HTTP client to use for the request</param>
        /// <param name="url">The request URL (relative or absolute)</param>
        /// <param name="callerFilePath">Automatically captured caller file path for embedded resource resolution</param>
        /// <returns>Request configuration builder</returns>
        /// <example>
        /// <code>
        /// await Client.AssertPatch("api/persons/1")
        ///     .WithBody(partialUpdate)
        ///     .WithResponse&lt;Person&gt;("Expected.json")
        ///     .ExpectSuccess();
        /// </code>
        /// </example>
        public static IHttpRequestConfiguring AssertPatch(this HttpClient client,
                                                          string url,
                                                          [CallerFilePath] string callerFilePath = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return new HttpRequestBuilder(client, HttpMethod.Patch, url,
                                          callingAssembly, callerFilePath);
        }

        /// <summary>
        /// Initiates a fluent HTTP DELETE assertion chain.
        /// </summary>
        /// <param name="client">The HTTP client to use for the request</param>
        /// <param name="url">The request URL (relative or absolute)</param>
        /// <param name="callerFilePath">Automatically captured caller file path for embedded resource resolution</param>
        /// <returns>Request configuration builder</returns>
        /// <example>
        /// <code>
        /// await Client.AssertDelete("api/persons/1")
        ///     .ExpectNoContent();
        /// </code>
        /// </example>
        public static IHttpRequestConfiguring AssertDelete(this HttpClient client,
                                                           string url,
                                                           [CallerFilePath] string callerFilePath = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return new HttpRequestBuilder(client, HttpMethod.Delete, url,
                                          callingAssembly, callerFilePath);
        }

        // ============================================================
        // TERMINAL OVERLOADS - WithResponse with automatic status code expectation
        // ============================================================

        /// <summary>
        /// Configures the expected response and immediately expects success (2xx status).
        /// This is a terminal operation that executes the request.
        /// </summary>
        /// <typeparam name="TResult">Expected response type</typeparam>
        /// <param name="config">Request configuration</param>
        /// <param name="expectedJson">JSON string or embedded resource path for expected response</param>
        /// <param name="expectSuccess">Must be true to use this overload</param>
        /// <returns>Task that resolves to the validated response</returns>
        /// <example>
        /// <code>
        /// // Short form - no ExpectSuccess() needed
        /// await Client.AssertPost("api/persons")
        ///     .WithBody(person)
        ///     .WithResponse&lt;Person&gt;("Expected.json", expectSuccess: true);
        /// </code>
        /// </example>
        public static Task<TResult> WithResponse<TResult>(this IHttpRequestConfiguring config,
                                                          string expectedJson,
                                                          bool expectSuccess)
        {
            if (!expectSuccess)
            {
                throw new ArgumentException("expectSuccess must be true for this overload. " +
                                            "Use .WithResponse<T>(json).Expect() for explicit status code control.",
                                            nameof(expectSuccess));
            }

            return config.WithResponse<TResult>(expectedJson).ExpectSuccess();
        }

        /// <summary>
        /// Configures the expected response and immediately expects the specified status code.
        /// This is a terminal operation that executes the request.
        /// </summary>
        /// <typeparam name="TResult">Expected response type</typeparam>
        /// <param name="config">Request configuration</param>
        /// <param name="expectedJson">JSON string or embedded resource path for expected response</param>
        /// <param name="statusCode">Expected HTTP status code</param>
        /// <returns>Task that resolves to the validated response</returns>
        /// <example>
        /// <code>
        /// // With explicit status code
        /// await Client.AssertPost("api/persons")
        ///     .WithBody(person)
        ///     .WithResponse&lt;Person&gt;("Expected.json", HttpStatusCode.Created);
        /// </code>
        /// </example>
        public static Task<TResult> WithResponse<TResult>(this IHttpRequestConfiguring config,
                                                          string expectedJson,
                                                          HttpStatusCode statusCode)
        {
            return config.WithResponse<TResult>(expectedJson).ExpectStatus(statusCode);
        }

        /// <summary>
        /// Configures the expected response and immediately expects one of the specified status codes.
        /// This is a terminal operation that executes the request.
        /// </summary>
        /// <typeparam name="TResult">Expected response type</typeparam>
        /// <param name="config">Request configuration</param>
        /// <param name="expectedJson">JSON string or embedded resource path for expected response</param>
        /// <param name="statusCodes">Accepted HTTP status codes</param>
        /// <returns>Task that resolves to the validated response</returns>
        /// <example>
        /// <code>
        /// // Accept multiple status codes
        /// await Client.AssertPost("api/persons")
        ///     .WithBody(person)
        ///     .WithResponse&lt;Person&gt;("Expected.json", HttpStatusCode.OK, HttpStatusCode.Created);
        /// </code>
        /// </example>
        public static Task<TResult> WithResponse<TResult>(this IHttpRequestConfiguring config,
                                                          string expectedJson,
                                                          params HttpStatusCode[] statusCodes)
        {
            if (statusCodes.Length == 0)
            {
                throw new ArgumentException("At least one status code must be provided.", nameof(statusCodes));
            }

            return config.WithResponse<TResult>(expectedJson).Expect(statusCodes);
        }
    }
}