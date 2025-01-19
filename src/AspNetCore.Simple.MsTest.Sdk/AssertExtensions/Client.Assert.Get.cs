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
        public static Task AssertGetAsync(this HttpClient client,
                                          string url,
                                          [CallerFilePath] string callerFilePath = "",
                                          bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         HttpMethod.Get,
                                         [],
                                         Assembly.GetCallingAssembly(),
                                         string.Empty,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task AssertGetAsync(this HttpClient client,
                                          string url,
                                          (string Key, object? Value)[] parameters,
                                          [CallerFilePath] string callerFilePath = "",
                                          bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         HttpMethod.Get,
                                         parameters,
                                         Assembly.GetCallingAssembly(),
                                         string.Empty,
                                         callerFilePath,
                                         writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertGetAsync<TResult>(url,
                                                  expectedResult,
                                                  [],
                                                  Assembly.GetCallingAssembly(),
                                                  expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                  callerFilePath,
                                                  writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, object? Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertGetAsync<TResult>(url,
                                                  expectedResult,
                                                  parameters,
                                                  Assembly.GetCallingAssembly(),
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertGetAsync<TResult>(url,
                                                  expectedResult,
                                                  differenceFunc,
                                                  [],
                                                  Assembly.GetCallingAssembly(),
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertGetAsync<TResult>(url,
                                                  expectedResult,
                                                  differenceFunc,
                                                  parameters,
                                                  Assembly.GetCallingAssembly(),
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertGetAsync(url,
                                         expectedResult,
                                         filterFunc,
                                         [],
                                         Assembly.GetCallingAssembly(),
                                         expectedResultParameterName,
                                         callerFilePath,
                                         writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertGetAsync(url,
                                         expectedResult,
                                         filterFunc,
                                         parameters,
                                         Assembly.GetCallingAssembly(),
                                         expectedResultParameterName,
                                         callerFilePath,
                                         writeResponse);
        }


        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  string.Empty,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Get,
                                                  [],
                                                  callingAssembly,
                                                  string.Empty,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  true,
                                                  writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  string.Empty,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Get,
                                                  parameters,
                                                  callingAssembly,
                                                  string.Empty,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  true,
                                                  writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  string.Empty,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Get,
                                                  differenceFunc,
                                                  [],
                                                  callingAssembly,
                                                  string.Empty,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  true,
                                                  writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  string.Empty,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Get,
                                                  differenceFunc,
                                                  parameters,
                                                  callingAssembly,
                                                  string.Empty,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  true,
                                                  writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Get,
                                         [],
                                         callingAssembly,
                                         string.Empty,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Get,
                                         parameters,
                                         callingAssembly,
                                         string.Empty,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Get,
                                         differenceFunc,
                                         [],
                                         callingAssembly,
                                         string.Empty,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Get,
                                         differenceFunc,
                                         parameters,
                                         callingAssembly,
                                         string.Empty,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
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
