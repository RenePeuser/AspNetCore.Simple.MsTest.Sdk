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
        // ============================================================
        // PATCH with Response (TResult) - All overloads
        // ============================================================

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              bool writeResponse = false,
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
                                                    parameters: [],
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    expectedResultParameterName: expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
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
            return client.AssertPatchAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
                                                    parameters: parameters,
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: difference => difference,
                                           parameters: [],
                                           callingAssembly: Assembly.GetCallingAssembly(),
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
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
            return client.AssertPatchAsync(url: url,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: difference => difference,
                                           parameters: parameters,
                                           callingAssembly: Assembly.GetCallingAssembly(),
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
                                                    differenceFunc: differenceFunc,
                                                    differenceFilter: differenceFilter,
                                                    parameters: [],
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
                                                    differenceFunc: differenceFunc,
                                                    differenceFilter: differenceFilter,
                                                    parameters: parameters,
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
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
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: null,
                                                       httpMethod: HttpMethod.Patch,
                                                       parameters: [],
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
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
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: null,
                                                       httpMethod: HttpMethod.Patch,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
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
                                                       filterFunc: null,
                                                       httpMethod: HttpMethod.Patch,
                                                       differenceFunc: differenceFunc,
                                                       differenceFilter: differenceFilter,
                                                       parameters: [],
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
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
                                                       filterFunc: null,
                                                       httpMethod: HttpMethod.Patch,
                                                       differenceFunc: differenceFunc,
                                                       differenceFilter: differenceFilter,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: differenceFunc,
                                           differenceFilter: differenceFilter,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        // MAXIMUM OVERLOAD - Contains the core logic (no payload variant)
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: string.Empty,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Patch,
                                              differenceFunc: differenceFunc,
                                              differenceFilter: differenceFilter,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: string.Empty,
                                              expectedResultParameterName: expectedResultParameterName,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber,
                                              expectedHttpStatusCode: expectedHttpStatusCode);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    expectedResult: expectedResult,
                                                    parameters: [],
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    expectedResult: expectedResult,
                                                    parameters: parameters,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
                                                    parameters: [],
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
                                                    parameters: parameters,
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPatchAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             parameters: [],
                                             callingAssembly: callingAssembly,
                                             writeResponse: writeResponse,
                                             skipEndpointValidation: skipEndpointValidation,
                                             payloadAsJsonParameterName: payloadAsObjectParameterName,
                                             expectedResultParameterName: expectedResultParameterName,
                                             expectedHttpStatusCode: expectedHttpStatusCode,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPatchAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             parameters: parameters,
                                             callingAssembly: callingAssembly,
                                             writeResponse: writeResponse,
                                             skipEndpointValidation: skipEndpointValidation,
                                             payloadAsJsonParameterName: payloadAsObjectParameterName,
                                             expectedResultParameterName: expectedResultParameterName,
                                             expectedHttpStatusCode: expectedHttpStatusCode,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPatchAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsJson,
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             parameters: [],
                                             callingAssembly: callingAssembly,
                                             writeResponse: writeResponse,
                                             skipEndpointValidation: skipEndpointValidation,
                                             payloadAsJsonParameterName: payloadAsJsonParameterName,
                                             expectedResultParameterName: expectedResultParameterName,
                                             expectedHttpStatusCode: expectedHttpStatusCode,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPatchAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsJson,
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             parameters: parameters,
                                             callingAssembly: callingAssembly,
                                             writeResponse: writeResponse,
                                             skipEndpointValidation: skipEndpointValidation,
                                             payloadAsJsonParameterName: payloadAsJsonParameterName,
                                             expectedResultParameterName: expectedResultParameterName,
                                             expectedHttpStatusCode: expectedHttpStatusCode,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPatchAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             differenceFunc: differenceFunc,
                                             differenceFilter: differenceFilter,
                                             parameters: [],
                                             callingAssembly: callingAssembly,
                                             writeResponse: writeResponse,
                                             skipEndpointValidation: skipEndpointValidation,
                                             payloadAsJsonParameterName: payloadAsObjectParameterName,
                                             expectedResultParameterName: expectedResultParameterName,
                                             expectedHttpStatusCode: expectedHttpStatusCode,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPatchAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             differenceFunc: differenceFunc,
                                             differenceFilter: differenceFilter,
                                             parameters: parameters,
                                             callingAssembly: callingAssembly,
                                             writeResponse: writeResponse,
                                             skipEndpointValidation: skipEndpointValidation,
                                             payloadAsJsonParameterName: payloadAsObjectParameterName,
                                             expectedResultParameterName: expectedResultParameterName,
                                             expectedHttpStatusCode: expectedHttpStatusCode,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPatchAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsJson,
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             differenceFunc: differenceFunc,
                                             differenceFilter: differenceFilter,
                                             parameters: [],
                                             callingAssembly: callingAssembly,
                                             writeResponse: writeResponse,
                                             skipEndpointValidation: skipEndpointValidation,
                                             payloadAsJsonParameterName: payloadAsJsonParameterName,
                                             expectedResultParameterName: expectedResultParameterName,
                                             expectedHttpStatusCode: expectedHttpStatusCode,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPatchAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsJson,
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             differenceFunc: differenceFunc,
                                             differenceFilter: differenceFilter,
                                             parameters: parameters,
                                             callingAssembly: callingAssembly,
                                             writeResponse: writeResponse,
                                             skipEndpointValidation: skipEndpointValidation,
                                             payloadAsJsonParameterName: payloadAsJsonParameterName,
                                             expectedResultParameterName: expectedResultParameterName,
                                             expectedHttpStatusCode: expectedHttpStatusCode,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           parameters: parameters,
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           parameters: [],
                                           callingAssembly: Assembly.GetCallingAssembly(),
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           parameters: parameters,
                                           callingAssembly: Assembly.GetCallingAssembly(),
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    expectedResult: expectedResult,
                                                    filterFunc: null,
                                                    differenceFunc: differenceFunc,
                                                    differenceFilter: differenceFilter,
                                                    parameters: [],
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    expectedResult: expectedResult,
                                                    filterFunc: null,
                                                    differenceFunc: differenceFunc,
                                                    differenceFilter: differenceFilter,
                                                    parameters: parameters,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
                                                    filterFunc: null,
                                                    differenceFunc: differenceFunc,
                                                    differenceFilter: differenceFilter,
                                                    parameters: [],
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
                                                    filterFunc: null,
                                                    differenceFunc: differenceFunc,
                                                    differenceFilter: differenceFilter,
                                                    parameters: parameters,
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: difference => difference,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: difference => difference,
                                           parameters: parameters,
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: difference => difference,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: difference => difference,
                                           parameters: parameters,
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: differenceFunc,
                                           differenceFilter: differenceFilter,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        // MAXIMUM OVERLOAD - Contains the core logic (object payload variant)
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Patch,
                                              differenceFunc: differenceFunc,
                                              differenceFilter: differenceFilter,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: differenceFunc,
                                           differenceFilter: differenceFilter,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           skipEndpointValidation: skipEndpointValidation,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        // MAXIMUM OVERLOAD - Contains the core logic (string payload variant)
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Patch,
                                              differenceFunc: differenceFunc,
                                              differenceFilter: differenceFilter,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        // ============================================================
        // Complete Context API (Level 3) - Everything in Context!
        // ============================================================

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task<TResult> AssertPatchAsync<TResult>(HttpAssertContext<TResult> context)
        {
            return context.Client.AssertPatchAsync(url: context.Url,
                                                   payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                   expectedResult: context.ExpectedObjectAsJson,
                                                   filterFunc: context.OrderFunc,
                                                   differenceFunc: context.DifferenceFunc,
                                                   differenceFilter: context.DifferenceFilter,
                                                   parameters: context.Parameters,
                                                   callingAssembly: context.CallingAssembly,
                                                   writeResponse: context.WriteResponse,
                                                   payloadAsJsonParameterName: context.PayloadParameterName,
                                                   expectedResultParameterName: context.ExpectedResultParameterName,
                                                   callerFilePath: context.CallerFilePath,
                                                   callerMemberName: context.CallerMemberName,
                                                   callerLineNumber: context.CallerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
                                                    differenceFunc: difference => difference,
                                                    differenceFilter: differenceFilter,
                                                    parameters: [],
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
                                                    differenceFunc: difference => difference,
                                                    differenceFilter: differenceFilter,
                                                    parameters: parameters,
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Predicate<Difference>? differenceFilter,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    expectedResult: expectedResult,
                                                    filterFunc: null,
                                                    differenceFunc: difference => difference,
                                                    differenceFilter: differenceFilter,
                                                    parameters: [],
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              Predicate<Difference>? differenceFilter,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    expectedResult: expectedResult,
                                                    filterFunc: null,
                                                    differenceFunc: difference => difference,
                                                    differenceFilter: differenceFilter,
                                                    parameters: parameters,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Predicate<Difference>? differenceFilter,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
                                                    filterFunc: null,
                                                    differenceFunc: difference => difference,
                                                    differenceFilter: differenceFilter,
                                                    parameters: [],
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              Predicate<Difference>? differenceFilter,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
                                                    filterFunc: null,
                                                    differenceFunc: difference => difference,
                                                    differenceFilter: differenceFilter,
                                                    parameters: parameters,
                                                    callingAssembly: Assembly.GetCallingAssembly(),
                                                    writeResponse: writeResponse,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: expectedResultParameterName,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }
    }
}