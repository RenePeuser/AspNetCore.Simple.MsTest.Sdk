using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectsComparer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static async Task AssertGetAsync(this HttpClient client,
                                                string url)
        {
            await client.AssertHttpCall(url, string.Empty, (client, url, _) => client.GetAsync(url), HttpMethod.Get, Assembly.GetCallingAssembly()).ConfigureAwait(false);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson) where TResult : class
        {
            return client.AssertGetAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return client.AssertGetAsync<TResult>(url, resultAsJson, differenceFunc, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertGetAsync(url, resultAsJson, Assembly.GetCallingAssembly(), filterFunc);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsAsync<TResult>, HttpMethod.Get, callingAssembly);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsAsync<TResult>, HttpMethod.Get, callingAssembly, differenceFunc);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly,
                                                            Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, filterFunc, HttpExtensions.GetAsAsync<TResult>, HttpMethod.Get, callingAssembly);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, filterFunc, HttpExtensions.GetAsAsync<TResult>, HttpMethod.Get, callingAssembly, differenceFunc);
        }


        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                 string url,
                                                                 string resultAsJson) where TResult : class
        {
            return client.AssertGetAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string resultAsJson,
                                                                   Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, filterFunc, HttpExtensions.GetAsErrorResultAsync<TResult>, HttpMethod.Get, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                 string url,
                                                                 string resultAsJson,
                                                                 Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsErrorResultAsync<TResult>, HttpMethod.Get, callingAssembly);
        }

        public static async Task AssertGetAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.GetAsync(url).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"GET {url}",
                Expected = HttpStatusCode.Unauthorized,
                Current = result.StatusCode
            }.ToIList();

            var table = ConsoleTable.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, errorOutput);
        }
    }
}
