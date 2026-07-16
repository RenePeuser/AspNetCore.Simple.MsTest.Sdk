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
        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string expectedResult,
                                                                     bool writeResponse = false,
                                                                     bool skipEndpointValidation = false,
                                                                     HttpStatusCode? expectedHttpStatusCode = null,
                                                                     [CallerFilePath] string callerFilePath = "",
                                                                     [CallerMemberName] string callerMemberName = "",
                                                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           expectedResult: expectedResult,
                                                           parameters: [],
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           expectedResultParameterName: expectedResult.Contains(value: ".json") ? expectedResult : nameof(expectedResult),
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           expectedResult: expectedResult,
                                                           parameters: parameters,
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           expectedResult: expectedResult,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: [],
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           expectedResult: expectedResult,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: parameters,
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: string.Empty,
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: difference => difference,
                                                           parameters: [],
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: string.Empty,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: string.Empty,
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: difference => difference,
                                                           parameters: parameters,
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: string.Empty,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: string.Empty,
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: [],
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: string.Empty,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: string.Empty,
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: parameters,
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: string.Empty,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsObject: payloadAsObject,
                                                           expectedResult: expectedResult,
                                                           parameters: [],
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           payloadAsObjectParameterName: payloadAsObjectParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsObject: payloadAsObject,
                                                           expectedResult: expectedResult,
                                                           parameters: parameters,
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           payloadAsObjectParameterName: payloadAsObjectParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsObject: payloadAsObject,
                                                           expectedResult: expectedResult,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: [],
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           payloadAsObjectParameterName: payloadAsObjectParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsObject: payloadAsObject,
                                                           expectedResult: expectedResult,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: parameters,
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           payloadAsObjectParameterName: payloadAsObjectParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsJson,
                                                           expectedResult: expectedResult,
                                                           parameters: [],
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsJson,
                                                           expectedResult: expectedResult,
                                                           parameters: parameters,
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsJson,
                                                           expectedResult: expectedResult,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: [],
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsJson,
                                                           expectedResult: expectedResult,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: parameters,
                                                           callingAssembly: Assembly.GetCallingAssembly(),
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: difference => difference,
                                                           parameters: [],
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: difference => difference,
                                                           parameters: parameters,
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: [],
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: parameters,
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                           expectedResultParameterName: expectedResultParameterName,
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsJson,
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
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

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsJson,
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: difference => difference,
                                                           parameters: parameters,
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

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsJson,
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
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

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsJson,
                                                           expectedResult: expectedResult,
                                                           filterFunc: item => item,
                                                           differenceFunc: differenceFunc,
                                                           differenceFilter: differenceFilter,
                                                           parameters: parameters,
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

        // MAXIMUM OVERLOAD
        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     string expectedResult,
                                                                     Func<TResult?, TResult?> filterFunc,
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
                                              isSuccessStatusCode: false,
                                              writeResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber,
                                              expectedHttpStatusCode: expectedHttpStatusCode);
        }
    }
}