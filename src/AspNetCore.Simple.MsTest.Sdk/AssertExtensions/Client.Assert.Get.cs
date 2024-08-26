using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
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
                                                (string Key, string Value)[] parameters)
        {
            await client.AssertHttpCall(url, string.Empty, (client, url, _) => client.GetAsync(url), HttpMethod.Get, parameters, Assembly.GetCallingAssembly()).ConfigureAwait(false);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertGetAsync<TResult>(url, expectedResult, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertGetAsync<TResult>(url, expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, string Value)[] parameters, [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertGetAsync(url, expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, item => item, (httpClient, url, _, _) => HttpExtensions.GetAsAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, parameters, expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, item => item, (httpClient, url, _, _) => HttpExtensions.GetAsAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, differenceFunc, parameters, expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, filterFunc, (httpClient, url, _) => HttpExtensions.GetAsAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, parameters, expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, filterFunc, (httpClient, url, _) => HttpExtensions.GetAsAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, differenceFunc, parameters, expectedResultParameterName);
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
