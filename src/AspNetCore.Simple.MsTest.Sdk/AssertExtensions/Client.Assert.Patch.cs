using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using ObjectsComparer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertPatchAsync(this HttpClient client,
                                            string url)
        {
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.PatchAsync(urlParam, null), HttpMethod.Patch, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertPatchAsync(url, string.Empty, resultAsJson, filterFunc, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly(), differenceFunc);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson,
                                                             Assembly callingAssembly) where TResult : class
        {
            return AssertPatchAsync<TResult>(client, url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson,
                                                             Assembly callingAssembly,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return AssertPatchAsync<TResult>(client, url, string.Empty, resultAsJson, callingAssembly, differenceFunc);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Assembly callingAssembly) where TResult : class
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsJson, resultAsJson, result => result, callingAssembly);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Assembly callingAssembly,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsJson, resultAsJson, result => result, differenceFunc, callingAssembly);
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, resultAsJson, item => item, differenceFunc, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc,
                                                             Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, callingAssembly, difference => difference);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, callingAssembly, differenceFunc);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson,
                                                             Assembly callingAssembly) where TResult : class
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, callingAssembly);
        }

        public static Task AssertPatchAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            return httpClient.AssertPatchAsUnauthorizedAsync(url, null);
        }

        public static async Task AssertPatchAsUnauthorizedAsync(this HttpClient httpClient, string url, object? body)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.PatchAsync(url, body.IsNull() ? null : new StringContent(body.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"PATCH {url}",
                Expected = HttpStatusCode.Unauthorized,
                Current = result.StatusCode
            }.ToIList();

            var table = ConsoleTable.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, errorOutput);
        }
    }
}
