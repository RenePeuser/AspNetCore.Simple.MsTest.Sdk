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
            return client.AssertGetAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertGetAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsAsync<TResult>, HttpMethod.Get, callingAssembly);
        }


        public static Task AssertGetErrorAsync<TResult>(this HttpClient client,
                                                        string url,
                                                        string resultAsJson) where TResult : class
        {
            return client.AssertGetErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertGetErrorAsync<TResult>(this HttpClient client,
                                                        string url,
                                                        string resultAsJson,
                                                        Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsExceptionAsync<TResult>, HttpMethod.Get, callingAssembly);
        }

        public static Task AssertDeleteAsync<TResult>(this HttpClient client,
                                                      string url,
                                                      string resultAsJson) where TResult : class
        {
            return client.AssertDeleteAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertDeleteAsync<TResult>(this HttpClient client,
                                                      string url,
                                                      string resultAsJson,
                                                      Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.DeleteAsAsync<TResult>, HttpMethod.Delete, callingAssembly);
        }


        public static Task AssertDeleteErrorAsync<TResult>(this HttpClient client,
                                                           string url,
                                                           string resultAsJson) where TResult : class
        {
            return client.AssertDeleteErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertDeleteErrorAsync<TResult>(this HttpClient client,
                                                           string url,
                                                           string resultAsJson,
                                                           Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.DeleteAsExceptionAsync<TResult>, HttpMethod.Delete, callingAssembly);
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, string.Empty, resultAsJson, result => result, callingAssembly);
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, callingAssembly);
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertPutAsync(url, payloadAsJson, resultAsJson, filterFunc, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Func<TResult, TResult> filterFunc,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly);
        }

        public static Task AssertPutError<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertPutError<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutError<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return AssertPutError<TResult>(client, url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task AssertPutError<TResult>(this HttpClient client,
                                                       string url,
                                                       string payloadAsJson,
                                                       string resultAsJson) where TResult : class
        {
            return client.AssertPutError<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutError<TResult>(this HttpClient client,
                                                       string url,
                                                       string payloadAsJson,
                                                       string resultAsJson,
                                                       Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PutAsExceptionWithJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly);
        }

        public static Task AssertPostAsync<TResult>(this HttpClient client,
                                                    string url,
                                                    string resultAsJson) where TResult : class
        {
            return client.AssertPostAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPostAsync<TResult>(this HttpClient client,
                                                    string url,
                                                    string resultAsJson,
                                                    Assembly callingAssembly) where TResult : class
        {
            return AssertPostAsync<TResult>(client, url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task AssertPostAsync<TResult>(this HttpClient client,
                                                    string url,
                                                    string payloadAsJson,
                                                    string resultAsJson) where TResult : class
        {
            return client.AssertPostAsync<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPostAsync<TResult>(this HttpClient client,
                                                    string url,
                                                    string payloadAsJson,
                                                    string resultAsJson,
                                                    Assembly callingAssembly) where TResult : class
        {
            return AssertPostAsync<TResult>(client, url, payloadAsJson, resultAsJson, result => result, callingAssembly);
        }

        public static Task AssertPostAsync<TResult>(this HttpClient client,
                                                    string url,
                                                    string payloadAsJson,
                                                    string resultAsJson,
                                                    Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertPostAsync(url, payloadAsJson, resultAsJson, filterFunc, Assembly.GetCallingAssembly());
        }

        public static Task AssertPostAsync<TResult>(this HttpClient client,
                                                    string url,
                                                    string payloadAsJson,
                                                    string resultAsJson,
                                                    Func<TResult, TResult> filterFunc,
                                                    Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly);
        }

        public static Task AssertPostError<TResult>(this HttpClient client,
                                                    string url,
                                                    string resultAsJson) where TResult : class
        {
            return client.AssertPostError<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPostError<TResult>(this HttpClient client,
                                                    string url,
                                                    string resultAsJson,
                                                    Assembly callingAssembly) where TResult : class
        {
            return client.AssertPostError<TResult>(url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task AssertPostError<TResult>(this HttpClient client,
                                                        string url,
                                                        string payloadAsJson,
                                                        string resultAsJson) where TResult : class
        {
            return client.AssertPostError<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPostError<TResult>(this HttpClient client,
                                                        string url,
                                                        string payloadAsJson,
                                                        string resultAsJson,
                                                        Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PostAsExceptionWithJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly);
        }

        private static Task AssertHttpCall<TResult>(this HttpClient client,
                                                    string url,
                                                    string payloadAsJson,
                                                    string resultAsJson,
                                                    Func<TResult, TResult> filterFunc,
                                                    Func<HttpClient, string, Task<TResult>> httpFunction,
                                                    HttpMethod httpMethod,
                                                    Assembly assembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, (client, path, _) => httpFunction(client, path), httpMethod, assembly);
        }

        private static async Task AssertHttpCall<TResult>(this HttpClient client,
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
        }
    }
}
