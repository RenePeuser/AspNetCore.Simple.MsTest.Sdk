using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, string.Empty, resultAsJson, result => result, callingAssembly);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string resultAsJson) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string resultAsJson,
                                                            Assembly callingAssembly) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, callingAssembly);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string resultAsJson,
                                                            Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertPutAsync(url, payloadAsJson, resultAsJson, filterFunc, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string resultAsJson,
                                                            Func<TResult, TResult> filterFunc,
                                                            Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson) where TResult : class
        {
            return client.AssertPutAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly) where TResult : class
        {
            return AssertPutAsErrorAsync<TResult>(client, url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string resultAsJson) where TResult : class
        {
            return client.AssertPutAsErrorAsync<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string resultAsJson,
                                                            Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PutAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly);
        }

        public static Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            return httpClient.AssertPutAsUnauthorizedAsync(url, null);
        }

        public static async Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient, string url, object? body)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            // ToDo: fix mutlipart form data issues
            var result = await httpClient.PutAsync(url, body.IsNull() ? null : new StringContent(body.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"PUT {url}",
                Expected = HttpStatusCode.Unauthorized,
                Current = result.StatusCode
            }.ToIList();

            var table = ConsoleTable.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, errorOutput);
        }
    }
}
