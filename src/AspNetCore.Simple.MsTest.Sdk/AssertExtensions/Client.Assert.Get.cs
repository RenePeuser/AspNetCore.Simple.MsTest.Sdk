using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Argument.Check;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertGetAsync(this HttpClient client,
                                          string url,
                                          bool writeResponse = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url,
                                         [],
                                         writeResponse,
                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task AssertGetAsync(this HttpClient client,
                                          string url,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url,
                                              string.Empty,
                                              HttpMethod.Get,
                                              parameters,
                                              Assembly.GetCallingAssembly(),
                                              string.Empty,
                                              callerFilePath,
                                              true,
                                              writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            bool writeResponse = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync<TResult>(url,
                                                  expectedResult,
                                                  [],
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                  callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync<TResult>(url,
                                                  expectedResult,
                                                  parameters,
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  expectedResultParameterName,
                                                  callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync<TResult>(url,
                                                  expectedResult,
                                                  differenceFunc,
                                                  [],
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  expectedResultParameterName,
                                                  callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync<TResult>(url,
                                                  expectedResult,
                                                  differenceFunc,
                                                  parameters,
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  expectedResultParameterName,
                                                  callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url,
                                         expectedResult,
                                         filterFunc,
                                         [],
                                         Assembly.GetCallingAssembly(),
                                         writeResponse,
                                         expectedResultParameterName,
                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url,
                                         expectedResult,
                                         filterFunc,
                                         parameters,
                                         Assembly.GetCallingAssembly(),
                                         writeResponse,
                                         expectedResultParameterName,
                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url,
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
                                                       writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url,
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
                                                       writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url,
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
                                                       writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url,
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
                                                       writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url,
                                         expectedResult,
                                         filterFunc,
                                         difference => difference,
                                         [],
                                         callingAssembly,
                                         writeResponse,
                                         expectedResultParameterName,
                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url,
                                         expectedResult,
                                         filterFunc,
                                         difference => difference,
                                         parameters,
                                         callingAssembly,
                                         writeResponse,
                                         expectedResultParameterName,
                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url,
                                         expectedResult,
                                         filterFunc,
                                         differenceFunc,
                                         [],
                                         callingAssembly,
                                         writeResponse,
                                         expectedResultParameterName,
                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        // MAXIMUM OVERLOAD - Contains the core logic
        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url,
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
                                              writeResponse,
                                              callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static async Task AssertGetAsUnauthorizedAsync(this HttpClient httpClient,
                                                              string url)
        {
            Throw.IfNull(httpClient);
            Throw.IfNullOrWhiteSpace(url);

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

        // ============================================================
        // Complete Context API (Level 3) - Everything in Context!
        // ============================================================

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        /// <example>
        /// <code>
        /// var context = new HttpAssertContext
        /// {
        ///     Client = httpClient,
        ///     Url = "/api/users",
        ///     Parameters = new[] { ("{userId}", 123) },
        ///     WriteResponse = true
        /// };
        ///
        /// await client.AssertGetAsync(context);
        /// </code>
        /// </example>
        public static Task AssertGetAsync(HttpAssertContext<string> context)
        {
            return context.Client.AssertGetAsync(context.Url,
                                                 context.Parameters,
                                                 context.WriteResponse,
                                                 context.CallerFilePath,
                                                 context.CallerMemberName,
                                                 context.CallerLineNumber);
        }

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        /// <example>
        /// <code>
        /// var context = new HttpAssertContext&lt;User&gt;
        /// {
        ///     Client = httpClient,
        ///     Url = "/api/users/{userId}",
        ///     ExpectedResult = "expected-user.json",
        ///     Parameters = new[] { ("{userId}", 123) },
        ///     FilterFunc = user => user with { Id = default },
        ///     WriteResponse = true
        /// };
        ///
        /// var user = await HttpClientAssertExtensions.AssertGetAsync(context);
        /// </code>
        /// </example>
        public static Task<TResult> AssertGetAsync<TResult>(HttpAssertContext<TResult> context)
        {
            return context.Client.AssertGetAsync(context.Url,
                                                 context.ExpectedObjectAsJson,
                                                 context.OrderFunc,
                                                 context.DifferenceFunc,
                                                 context.Parameters,
                                                 context.CallingAssembly,
                                                 context.WriteResponse,
                                                 context.ExpectedResultParameterName,
                                                 context.CallerFilePath,
                                                 context.CallerMemberName,
                                                 context.CallerLineNumber);
        }
    }
}
