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
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson) where TResult : class
        {
            return client.AssertPostAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson,
                                                             Assembly callingAssembly) where TResult : class
        {
            return AssertPostAsync<TResult>(client, url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson) where TResult : class
        {
            return client.AssertPostAsync<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Assembly callingAssembly) where TResult : class
        {
            return AssertPostAsync<TResult>(client, url, payloadAsJson, resultAsJson, result => result, callingAssembly);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertPostAsync(url, payloadAsJson, resultAsJson, filterFunc, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc,
                                                             Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly);
        }

        public static Task<TResult> AssertPostError<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson) where TResult : class
        {
            return client.AssertPostError<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPostError<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson,
                                                             Assembly callingAssembly) where TResult : class
        {
            return client.AssertPostError<TResult>(url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task<TResult> AssertPostError<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson) where TResult : class
        {
            return client.AssertPostError<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPostError<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PostAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly);
        }

        public static async Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient, string url, object body)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.PostAsJsonStringAsync(url, body.ToJson()).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, $"POST with'{url}' was successful, but unauthorized was expected");
        }
    }
}
