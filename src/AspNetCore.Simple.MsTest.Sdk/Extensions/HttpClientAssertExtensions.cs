using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class HttpClientAssertExtensions
    {
        public static Task AssertGetAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsAsync<TResult>, HttpMethod.Get);
        }


        public static Task AssertGetErrorAsync<TResult>(this HttpClient client,
                                                        string url,
                                                        string resultAsJson) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsExceptionAsync<TResult>, HttpMethod.Get);
        }

        public static Task AssertDeleteAsync<TResult>(this HttpClient client,
                                                      string url,
                                                      string resultAsJson) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.DeleteAsAsync<TResult>, HttpMethod.Delete);
        }


        public static Task AssertDeleteErrorAsync<TResult>(this HttpClient client,
                                                           string url,
                                                           string resultAsJson) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.DeleteAsExceptionAsync<TResult>, HttpMethod.Delete);
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return AssertPutAsync<TResult>(client, url, string.Empty, resultAsJson, result => result);
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson) where TResult : class
        {
            return AssertPutAsync<TResult>(client, url, payloadAsJson, resultAsJson, result => result);
        }

        public static Task AssertPutError<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return AssertPutException<TResult>(client, url, string.Empty, resultAsJson);
        }

        public static Task AssertPutException<TResult>(this HttpClient client,
                                                       string url,
                                                       string payloadAsJson,
                                                       string resultAsJson) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PutAsExceptionWithJsonStringAsync<TResult>, HttpMethod.Put);
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put);
        }

        public static Task AssertPostAsync<TResult>(this HttpClient client,
                                                    string url,
                                                    string resultAsJson) where TResult : class
        {
            return AssertPostAsync<TResult>(client, url, string.Empty, resultAsJson, result => result);
        }

        public static Task AssertPostAsync<TResult>(this HttpClient client,
                                                    string url,
                                                    string payloadAsJson,
                                                    string resultAsJson) where TResult : class
        {
            return AssertPostAsync<TResult>(client, url, payloadAsJson, resultAsJson, result => result);
        }

        public static Task AssertPostError<TResult>(this HttpClient client,
                                                    string url,
                                                    string resultAsJson) where TResult : class
        {
            return AssertPostException<TResult>(client, url, string.Empty, resultAsJson);
        }

        public static Task AssertPostException<TResult>(this HttpClient client,
                                                        string url,
                                                        string payloadAsJson,
                                                        string resultAsJson) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PostAsExceptionWithJsonStringAsync<TResult>, HttpMethod.Post);
        }

        public static Task AssertPostAsync<TResult>(this HttpClient client,
                                                    string url,
                                                    string payloadAsJson,
                                                    string resultAsJson,
                                                    Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post);
        }

        private static Task AssertHttpCall<TResult>(this HttpClient client,
                                                    string url,
                                                    string payloadAsJson,
                                                    string resultAsJson,
                                                    Func<TResult, TResult> filterFunc,
                                                    Func<HttpClient, string, Task<TResult>> httpFunction,
                                                    HttpMethod httpMethod) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, (client, path, _) => httpFunction(client, path), httpMethod);
        }

        private static async Task AssertHttpCall<TResult>(this HttpClient client,
                                                          string url,
                                                          string payloadAsJson,
                                                          string resultAsJson,
                                                          Func<TResult, TResult> filterFunc,
                                                          Func<HttpClient, string, string, Task<TResult>> httpFunction,
                                                          HttpMethod httpMethod) where TResult : class
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;
            var expectedResult = resultAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(resultAsJson) : resultAsJson;

            var currentResult = await httpFunction(client, url, jsonPayload).ConfigureAwait(false);

            var httpCallInfo = $"Call: '{httpMethod} {url}' was not successful.";

            Assert.That.ObjectsAreEqual(() => expectedResult, () => currentResult, filterFunc, httpCallInfo);
        }
    }
}
