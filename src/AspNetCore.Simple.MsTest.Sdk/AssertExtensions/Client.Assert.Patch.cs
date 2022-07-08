using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string resultAsJson,
                                                              Assembly callingAssembly) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, string.Empty, resultAsJson, result => result, callingAssembly);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string resultAsJson,
                                                              Assembly callingAssembly) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, callingAssembly);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string resultAsJson,
                                                              Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertPatchAsync(url, payloadAsJson, resultAsJson, filterFunc, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string resultAsJson,
                                                              Func<TResult, TResult> filterFunc,
                                                              Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, callingAssembly);
        }

        public static Task<TResult> AssertPatchAsError<TResult>(this HttpClient client,
                                                              string url,
                                                              string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsError<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsError<TResult>(this HttpClient client,
                                                              string url,
                                                              string resultAsJson,
                                                              Assembly callingAssembly) where TResult : class
        {
            return AssertPatchAsError<TResult>(client, url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task<TResult> AssertPatchAsError<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsError<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsError<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string resultAsJson,
                                                              Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, callingAssembly);
        }

        public static async Task AssertPatchAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.PatchAsync(url, null).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, $"PATCH with'{url}' was successful, but unauthorized was expected");
        }
    }
}
