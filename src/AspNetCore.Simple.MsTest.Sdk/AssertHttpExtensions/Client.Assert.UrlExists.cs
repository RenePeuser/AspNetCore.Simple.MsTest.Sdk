using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        /// <summary>
        /// Asserts that the url answers with a success status code.
        /// </summary>
        /// <param name="client">The client to call with</param>
        /// <param name="url">The url that has to exist</param>
        /// <param name="callerFilePath">Auto-captured file path of the calling test</param>
        /// <param name="callerMemberName">Auto-captured name of the calling test</param>
        /// <param name="callerLineNumber">Auto-captured line number in the calling test</param>
        public static void AssertUrlExists(this HttpClient client,
                                           string url,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            Assert.That.IsTrue(client.UrlExists(url),
                               because: $"The test requires '{url}' to be a reachable endpoint, but the call either matched no route or did not answer with a success status code.",
                               fix: $"Check that a route for '{url}' is mapped (controller route attribute or MapGet/MapPost/...), that the HTTP verb matches, and that the test host started without a routing error.",
                               conditionName: $"client.UrlExists(\"{url}\")",
                               callerFilePath: callerFilePath,
                               callerMemberName: callerMemberName,
                               callerLineNumber: callerLineNumber);
        }

        /// <summary>
        /// Asserts that the url does not answer with a success status code.
        /// </summary>
        /// <param name="client">The client to call with</param>
        /// <param name="url">The url that must not exist</param>
        /// <param name="callerFilePath">Auto-captured file path of the calling test</param>
        /// <param name="callerMemberName">Auto-captured name of the calling test</param>
        /// <param name="callerLineNumber">Auto-captured line number in the calling test</param>
        public static void AssertUrlNotExists(this HttpClient client,
                                              string url,
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            Assert.That.IsFalse(client.UrlExists(url),
                                because: $"The test requires '{url}' to be unreachable, but something answered it with a success status code.",
                                fix: $"Remove or restrict the route that serves '{url}' - or, if the endpoint is meant to exist now, switch this call to AssertUrlExists.",
                                conditionName: $"client.UrlExists(\"{url}\")",
                                callerFilePath: callerFilePath,
                                callerMemberName: callerMemberName,
                                callerLineNumber: callerLineNumber);
        }

        /// <summary>
        /// Asserts that the url does not answer with a success status code.
        /// </summary>
        /// <param name="client">The client to call with</param>
        /// <param name="url">The url that must not exist</param>
        /// <param name="callerFilePath">Auto-captured file path of the calling test</param>
        /// <param name="callerMemberName">Auto-captured name of the calling test</param>
        /// <param name="callerLineNumber">Auto-captured line number in the calling test</param>
        public static async Task AssertUrlNotExistsAsync(this HttpClient client,
                                                         string url,
                                                         [CallerFilePath] string callerFilePath = "",
                                                         [CallerMemberName] string callerMemberName = "",
                                                         [CallerLineNumber] int callerLineNumber = 0)
        {
            var urlExistsAsync = await client.UrlExistsAsync(url).ConfigureAwait(false);

            Assert.That.IsFalse(urlExistsAsync,
                                because: $"The test requires '{url}' to be unreachable, but something answered it with a success status code.",
                                fix: $"Remove or restrict the route that serves '{url}' - or, if the endpoint is meant to exist now, switch this call to AssertUrlExistsAsync.",
                                conditionName: $"client.UrlExistsAsync(\"{url}\")",
                                callerFilePath: callerFilePath,
                                callerMemberName: callerMemberName,
                                callerLineNumber: callerLineNumber);
        }

        /// <summary>
        /// Asserts that the url answers with a success status code.
        /// </summary>
        /// <param name="client">The client to call with</param>
        /// <param name="url">The url that has to exist</param>
        /// <param name="callerFilePath">Auto-captured file path of the calling test</param>
        /// <param name="callerMemberName">Auto-captured name of the calling test</param>
        /// <param name="callerLineNumber">Auto-captured line number in the calling test</param>
        public static async Task AssertUrlExistsAsync(this HttpClient client,
                                                      string url,
                                                      [CallerFilePath] string callerFilePath = "",
                                                      [CallerMemberName] string callerMemberName = "",
                                                      [CallerLineNumber] int callerLineNumber = 0)
        {
            var urlExistsAsync = await client.UrlExistsAsync(url).ConfigureAwait(false);

            Assert.That.IsTrue(urlExistsAsync,
                               because: $"The test requires '{url}' to be a reachable endpoint, but the call either matched no route or did not answer with a success status code.",
                               fix: $"Check that a route for '{url}' is mapped (controller route attribute or MapGet/MapPost/...), that the HTTP verb matches, and that the test host started without a routing error.",
                               conditionName: $"client.UrlExistsAsync(\"{url}\")",
                               callerFilePath: callerFilePath,
                               callerMemberName: callerMemberName,
                               callerLineNumber: callerLineNumber);
        }
    }
}
