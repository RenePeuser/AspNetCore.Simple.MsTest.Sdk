using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            TResult expectedResponse,
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
                                                  expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  ignoreResponse: ignoreResponse,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: nameof(expectedResponse),
                                                  callerFilePath: callerFilePath,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            TResult expectedResponse,
                                                            (string Key, object? Value)[] parameters,
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
                                                  expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  ignoreResponse: ignoreResponse,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: nameof(expectedResponse),
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            TResult expectedResponse,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                  differenceFunc: differenceFunc,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: nameof(expectedResponse),
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  differenceFilter: differenceFilter,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            TResult expectedResponse,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                  differenceFunc: differenceFunc,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: nameof(expectedResponse),
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  differenceFilter: differenceFilter,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            TResult expectedResponse,
                                                            Func<TResult?, TResult?>? filterFunc,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync(url: url,
                                         expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                         filterFunc: filterFunc,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         expectedResultParameterName: nameof(expectedResponse),
                                         skipEndpointValidation: skipEndpointValidation,
                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            TResult expectedResponse,
                                                            Func<TResult?, TResult?>? filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync(url: url,
                                         expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                         filterFunc: filterFunc,
                                         parameters: parameters,
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         expectedResultParameterName: nameof(expectedResponse),
                                         skipEndpointValidation: skipEndpointValidation,
                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            TResult expectedResponse,
                                                            Func<TResult?, TResult?>? filterFunc,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync(url: url,
                                         expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                         filterFunc: filterFunc,
                                         differenceFunc: differenceFunc,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         expectedResultParameterName: nameof(expectedResponse),
                                         skipEndpointValidation: skipEndpointValidation,
                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                         differenceFilter: differenceFilter,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            TResult expectedResponse,
                                                            Func<TResult?, TResult?>? filterFunc,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            Predicate<Difference>? differenceFilter = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync(url: url,
                                         expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                         filterFunc: filterFunc,
                                         differenceFunc: differenceFunc,
                                         parameters: parameters,
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         expectedResultParameterName: nameof(expectedResponse),
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
                                                            TResult expectedResponse,
                                                            Predicate<Difference>? differenceFilter,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                  differenceFunc: difference => difference,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: nameof(expectedResponse),
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
                                                            TResult expectedResponse,
                                                            (string Key, object? Value)[] parameters,
                                                            Predicate<Difference>? differenceFilter,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            HttpStatusCode? expectedHttpStatusCode = null,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertGetAsync<TResult>(url: url,
                                                  expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                  differenceFunc: difference => difference,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: nameof(expectedResponse),
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  expectedHttpStatusCode: expectedHttpStatusCode,
                                                  differenceFilter: differenceFilter,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }
    }
}