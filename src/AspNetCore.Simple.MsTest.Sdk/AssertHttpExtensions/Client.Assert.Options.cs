using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<HttpResponseMessage> AssertOptionsAsync(this HttpClient client,
                                                                   string url,
                                                                   IImmutableDictionary<string, string> expectedHeaders,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            IImmutableDictionary<string, ImmutableList<string>> expectedHeaderStructure = expectedHeaders.ToImmutableDictionary(item => item.Key, item => item.Value.AsImmutableList());

            return client.AssertOptionsAsync(url, expectedHeaderStructure.ToJson(JsonSerializerOptionsFor(callingAssembly)), callingAssembly,
                                             callerFilePath, callerMemberName,
                                             callerLineNumber);
        }

        public static Task<HttpResponseMessage> AssertOptionsAsync(this HttpClient client,
                                                                   string url,
                                                                   IImmutableDictionary<string, ImmutableList<string>> expectedHeaders,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertOptionsAsync(url, expectedHeaders.ToJson(JsonSerializerOptionsFor(callingAssembly)), callingAssembly,
                                             callerFilePath, callerMemberName,
                                             callerLineNumber);
        }

        public static Task<HttpResponseMessage> AssertOptionsAsync(this HttpClient client,
                                                                   string url,
                                                                   string expectedHeadersAsJson,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertOptionsAsync(url, expectedHeadersAsJson, Assembly.GetCallingAssembly(),
                                             callerFilePath, callerMemberName,
                                             callerLineNumber);
        }

        private static async Task<HttpResponseMessage> AssertOptionsAsync(this HttpClient client,
                                                                          string url,
                                                                          string expectedHeadersAsJson,
                                                                          Assembly callingAssembly,
                                                                          string callerFilePath,
                                                                          string callerMemberName,
                                                                          int callerLineNumber)
        {
            var expectedHeaders = expectedHeadersAsJson.GetJsonStringFrom<object>(string.Empty, callingAssembly, string.Empty);

            using var request = new HttpRequestMessage(HttpMethod.Options, url);
            var result = await client.SendAsync(request).ConfigureAwait(false);
            var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.That.AreEqual(HttpStatusCode.NoContent,
                                 result.StatusCode,
                                 because: $"An OPTIONS call to {request.RequestUri?.AbsoluteUri} has to answer 204 NoContent - that is how the endpoint advertises which verbs and headers it supports.",
                                 fix: $"Check that the OPTIONS middleware is registered and that the action serving '{request.RequestUri?.AbsoluteUri}' carries the [HttpOptions] attribute.",
                                 expectedName: "HttpStatusCode.NoContent",
                                 actualName: "result.StatusCode",
                                 callerFilePath: callerFilePath,
                                 callerMemberName: callerMemberName,
                                 callerLineNumber: callerLineNumber);

            Assert.That.IsNullOrWhiteSpace(content,
                                           because: "A 204 NoContent answer to OPTIONS must not carry a body - the headers alone are the payload.",
                                           fix: "Make the OPTIONS action return NoContent() / Results.NoContent() instead of writing a body.",
                                           valueName: "response content",
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);

            var headers = result.Headers.ToDictionary(item => item.Key, item => item.Value);

            Assert.That.ObjectsAreEqual(expectedHeaders,
                                        headers,
                                        callingAssembly,
                                        callerFilePath: callerFilePath,
                                        callerMemberName: callerMemberName,
                                        callerLineNumber: callerLineNumber);

            return result;
        }
    }
}