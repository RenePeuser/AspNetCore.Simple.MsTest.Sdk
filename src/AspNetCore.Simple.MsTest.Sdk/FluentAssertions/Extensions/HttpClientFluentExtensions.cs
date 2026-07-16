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
    /// <para>
    /// The <c>Assert</c> prefix is deliberate: it makes the chain read as an assertion, not a plain
    /// request (<c>client.AssertPost(...)</c> vs. the misleading <c>client.Post(...)</c>), and it keeps
    /// the name aligned with the existing overload API (<c>AssertPostAsync</c>) so migration stays
    /// mechanical. See DESIGN_VISION.md §3.
    /// </para>
    ///
    /// <example>
    /// <code>
    /// await Client.AssertPost("api/persons")
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
        public static IHttpRequestConfiguring AssertPost(this HttpClient client,
                                                         string url,
                                                         [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Post, url, Assembly.GetCallingAssembly(), callerFilePath);
        }

        /// <summary>Starts a fluent GET assertion chain.</summary>
        public static IHttpRequestConfiguring AssertGet(this HttpClient client,
                                                        string url,
                                                        [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Get, url, Assembly.GetCallingAssembly(), callerFilePath);
        }

        /// <summary>Starts a fluent PUT assertion chain.</summary>
        public static IHttpRequestConfiguring AssertPut(this HttpClient client,
                                                        string url,
                                                        [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Put, url, Assembly.GetCallingAssembly(), callerFilePath);
        }

        /// <summary>Starts a fluent PATCH assertion chain.</summary>
        public static IHttpRequestConfiguring AssertPatch(this HttpClient client,
                                                          string url,
                                                          [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Patch, url, Assembly.GetCallingAssembly(), callerFilePath);
        }

        /// <summary>Starts a fluent DELETE assertion chain.</summary>
        public static IHttpRequestConfiguring AssertDelete(this HttpClient client,
                                                           string url,
                                                           [CallerFilePath] string callerFilePath = "")
        {
            return new HttpRequestBuilder(client, HttpMethod.Delete, url, Assembly.GetCallingAssembly(), callerFilePath);
        }
    }
}
