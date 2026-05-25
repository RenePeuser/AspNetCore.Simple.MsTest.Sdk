using System.Reflection;
using System.Runtime.CompilerServices;
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
    public static class HttpClientFluentExtensions
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
        public static IHttpRequestConfiguring AssertPost(
            this HttpClient client,
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
        public static IHttpRequestConfiguring AssertGet(
            this HttpClient client,
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
        public static IHttpRequestConfiguring AssertPut(
            this HttpClient client,
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
        public static IHttpRequestConfiguring AssertPatch(
            this HttpClient client,
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
        public static IHttpRequestConfiguring AssertDelete(
            this HttpClient client,
            string url,
            [CallerFilePath] string callerFilePath = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return new HttpRequestBuilder(client, HttpMethod.Delete, url,
                                          callingAssembly, callerFilePath);
        }
    }
}