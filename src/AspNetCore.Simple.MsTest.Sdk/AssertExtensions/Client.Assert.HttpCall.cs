using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        private static async Task AssertHttpCall(this HttpClient client,
                                                                      string url,
                                                                      string payloadAsJson,
                                                                      Func<HttpClient, string, string, Task> httpFunction,
                                                                      Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            await httpFunction(client, url, jsonPayload).ConfigureAwait(false);
        }

        private static Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                    string url,
                                                    string payloadAsJson,
                                                    string resultAsJson,
                                                    Func<TResult, TResult> filterFunc,
                                                    Func<HttpClient, string, Task<TResult>> httpFunction,
                                                    HttpMethod httpMethod,
                                                    Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, (client, path, _) => httpFunction(client, path), httpMethod, callingAssembly);
        }

        private static async Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                          string url,
                                                          string payloadAsJson,
                                                          string resultAsJson,
                                                          Func<TResult, TResult> filterFunc,
                                                          Func<HttpClient, string, string, Task<TResult>> httpFunction,
                                                          HttpMethod httpMethod,
                                                          Assembly callingAssembly) where TResult : class
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;
            var expectedResult = resultAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(resultAsJson) : resultAsJson;

            var currentResult = await httpFunction(client, url, jsonPayload).ConfigureAwait(false);

            var httpCallInfo = $"Call: '{httpMethod} {url}' was not successful.";

            Assert.That.ObjectsAreEqual(() => expectedResult, () => currentResult, filterFunc, httpCallInfo);

            return currentResult;
        }
    }
}
