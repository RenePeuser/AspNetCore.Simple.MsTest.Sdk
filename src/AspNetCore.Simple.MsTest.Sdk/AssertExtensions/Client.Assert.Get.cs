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

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static async Task AssertGetAsync(this HttpClient client,
                                                string url,
                                                params (string Key, string Value)[] parameters)
        {
            await client.AssertHttpCall(url, string.Empty, (client, url, _) => client.GetAsync(url), HttpMethod.Get, Assembly.GetCallingAssembly(), parameters).ConfigureAwait(false);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertGetAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertGetAsync<TResult>(url, resultAsJson, differenceFunc, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Func<TResult, TResult> filterFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertGetAsync(url, resultAsJson, Assembly.GetCallingAssembly(), filterFunc, parameters);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, (httpClient, url, _, _) => HttpExtensions.GetAsAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, parameters);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, (httpClient, url, _, _) => HttpExtensions.GetAsAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, differenceFunc, parameters);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly,
                                                            Func<TResult, TResult> filterFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, filterFunc, (httpClient, url, _) => HttpExtensions.GetAsAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, parameters);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, filterFunc, (httpClient, url, _) => HttpExtensions.GetAsAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, differenceFunc, parameters);
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
