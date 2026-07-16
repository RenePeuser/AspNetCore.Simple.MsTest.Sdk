using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Extensions
{
    /// <summary>
    /// Fluent API entry points on <see cref="HttpClient"/>. Each starts a chain that ends in exactly
    /// one <c>ExecuteAsync()</c> terminal (Model B).
    ///
    /// <example>
    /// <code>
    /// await Client.Post("api/persons")
    ///     .WithBody(person)
    ///     .ReturnsEmbeddedJson&lt;Person&gt;("Expected.json")
    ///     .ExpectingStatus(HttpStatusCode.Created)
    ///     .ExecuteAsync();
    /// </code>
    /// </example>
    /// </summary>
    // ToDo: internal still under construction — foundation (analyzer) before going public.
    internal static class HttpClientFluentExtensions
    {
        // Note: Assembly.GetCallingAssembly() MUST be called directly in each public entry method —
        // moving it into a private helper would capture the SDK assembly, breaking embedded-resource
        // resolution. (Assembly-vs-CallerFilePath strategy is an open design question, see DESIGN_VISION.md §9.)

        /// <summary>Starts a fluent POST assertion chain.</summary>
        public static IHttpRequestConfiguring Post(this HttpClient client,
                                                   string url,
                                                   [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Post, url, Assembly.GetCallingAssembly(), callerFilePath);
        }

        /// <summary>Starts a fluent GET assertion chain.</summary>
        public static IHttpRequestConfiguring Get(this HttpClient client,
                                                  string url,
                                                  [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Get, url, Assembly.GetCallingAssembly(), callerFilePath);
        }

        /// <summary>Starts a fluent PUT assertion chain.</summary>
        public static IHttpRequestConfiguring Put(this HttpClient client,
                                                  string url,
                                                  [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Put, url, Assembly.GetCallingAssembly(), callerFilePath);
        }

        /// <summary>Starts a fluent PATCH assertion chain.</summary>
        public static IHttpRequestConfiguring Patch(this HttpClient client,
                                                    string url,
                                                    [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Patch, url, Assembly.GetCallingAssembly(), callerFilePath);
        }

        /// <summary>Starts a fluent DELETE assertion chain.</summary>
        public static IHttpRequestConfiguring Delete(this HttpClient client,
                                                     string url,
                                                     [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Delete, url, Assembly.GetCallingAssembly(), callerFilePath);
        }
    }
}
