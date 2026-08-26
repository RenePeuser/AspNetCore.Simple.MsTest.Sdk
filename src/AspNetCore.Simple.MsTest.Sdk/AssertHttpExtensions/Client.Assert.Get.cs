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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertGetAsync(this HttpClient client,
                                          string url,
                                          bool writeResponse = false,
                                          bool skipEndpointValidation = false,
                                          HttpStatusCode? expectedHttpStatusCode = null,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url: url,
                                         parameters: [],
                                         writeResponse: writeResponse,
                                         skipEndpointValidation: skipEndpointValidation,
                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task AssertGetAsync(this HttpClient client,
                                          string url,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          bool skipEndpointValidation = false,
                                          HttpStatusCode? expectedHttpStatusCode = null,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: string.Empty,
                                              httpMethod: HttpMethod.Get,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: string.Empty,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        /// <summary>
        /// Asserts a GET request with endpoint validation only (no response comparison).
        /// Validates that the endpoint exists and returns the correct type, but doesn't compare response content.
        /// Similar to HttpClient.GetAsync() - useful for process chain tests where only success matters.
        /// </summary>
        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: string.Empty,
                                                  writeResponse: writeResponse,
                                                  ignoreResponse: true, // Automatically ignore response when no expectedResult provided
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            bool writeResponse = false,
                                                            bool ignoreResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResult,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  ignoreResponse: ignoreResponse,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                  callerFilePath: callerFilePath,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        /// <summary>
        /// Asserts a GET request with endpoint validation only (no response comparison).
        /// Validates that the endpoint exists and returns the correct type, but doesn't compare response content.
        /// Similar to HttpClient.GetAsync() - useful for process chain tests where only success matters.
        /// </summary>
        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: string.Empty,
                                                  parameters: parameters,
                                                  writeResponse: writeResponse,
                                                  ignoreResponse: true, // Automatically ignore response when no expectedResult provided
                                                  expectedResultParameterName: string.Empty,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            bool ignoreResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResult,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  ignoreResponse: ignoreResponse,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResult,
                                                  differenceFunc: differenceFunc,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  differenceFilter: differenceFilter,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResult,
                                                  differenceFunc: differenceFunc,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  differenceFilter: differenceFilter,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync(url: url,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync(url: url,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         parameters: parameters,
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Assembly callingAssembly,
                                                            bool ignoreResponse = false,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Get,
                                                       parameters: [],
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       ignoreResponse: ignoreResponse,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool ignoreResponse = false,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Get,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       ignoreResponse: ignoreResponse,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
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
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Get,
                                                       differenceFunc: differenceFunc,
                                                       parameters: [],
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       differenceFilter: differenceFilter,
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
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Get,
                                                       differenceFunc: differenceFunc,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       differenceFilter: differenceFilter,
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
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url: url,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         differenceFunc: difference => difference,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
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
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url: url,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         differenceFunc: difference => difference,
                                         parameters: parameters,
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
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
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsync(url: url,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         differenceFunc: differenceFunc,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                         differenceFilter: differenceFilter,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
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
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: string.Empty,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Get,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: string.Empty,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              differenceFilter: differenceFilter,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static async Task AssertGetAsUnauthorizedAsync(this HttpClient httpClient,
                                                              string url,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            Throw.IfNull(httpClient);
            Throw.IfNullOrWhiteSpace(url);

            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.GetAsync(url).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            Assert.That.AreEqual(HttpStatusCode.Unauthorized,
                                 result.StatusCode,
                                 because: $"GET {url} was called with an invalid bearer token, so the endpoint has to reject it with 401 Unauthorized. Any other status code means the route can be reached without valid credentials.",
                                 fix: "Check that the endpoint is covered by [Authorize] (or an equivalent policy/authentication middleware) and that no [AllowAnonymous] on the action or controller overrides it.",
                                 expectedName: "HttpStatusCode.Unauthorized",
                                 actualName: "result.StatusCode",
                                 callerFilePath: callerFilePath,
                                 callerMemberName: callerMemberName,
                                 callerLineNumber: callerLineNumber);
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
            return context.Client.AssertGetAsync(url: context.Url,
                                                 parameters: context.Parameters,
                                                 writeResponse: context.WriteResponse,
                                                 skipEndpointValidation: context.SkipEndpointValidation,
                                                 expectedHttpStatusCode: context.ExpectedHttpStatusCode,
                                                 callerFilePath: context.CallerFilePath,
                                                 callerMemberName: context.CallerMemberName,
                                                 callerLineNumber: context.CallerLineNumber);
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
            return context.Client.AssertGetAsync(url: context.Url,
                                                 expectedResult: context.ExpectedObjectAsJson,
                                                 filterFunc: context.OrderFunc,
                                                 differenceFunc: context.DifferenceFunc,
                                                 parameters: context.Parameters,
                                                 callingAssembly: context.CallingAssembly,
                                                 writeResponse: context.WriteResponse,
                                                 expectedResultParameterName: context.ExpectedResultParameterName,
                                                 skipEndpointValidation: context.SkipEndpointValidation,
                                                 expectedHttpStatusCode: context.ExpectedHttpStatusCode,
                                                 differenceFilter: context.DifferenceFilter,
                                                 callerFilePath: context.CallerFilePath,
                                                 callerMemberName: context.CallerMemberName,
                                                 callerLineNumber: context.CallerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Predicate<Difference>? differenceFilter,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResult,
                                                  differenceFunc: difference => difference,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  differenceFilter: differenceFilter,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, object? Value)[] parameters,
                                                            Predicate<Difference>? differenceFilter,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResult,
                                                  differenceFunc: difference => difference,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  differenceFilter: differenceFilter,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }
    }
}