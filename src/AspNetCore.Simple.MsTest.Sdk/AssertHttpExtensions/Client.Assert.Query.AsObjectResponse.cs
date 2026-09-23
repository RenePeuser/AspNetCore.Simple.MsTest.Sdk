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
        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              TResult expectedResponse,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    parameters: [],
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              TResult expectedResponse,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    parameters: [],
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              TResult expectedResponse,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync(url: url,
                                           payloadAsObject: payloadAsObject,
                                           expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                           filterFunc: filterFunc,
                                           writeResponse: writeResponse,
                                           payloadAsObjectParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: nameof(expectedResponse),
                                           skipEndpointValidation: skipEndpointValidation,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              TResult expectedResponse,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync(url: url,
                                           payloadAsObject: payloadAsObject,
                                           expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                           filterFunc: filterFunc,
                                           parameters: parameters,
                                           writeResponse: writeResponse,
                                           payloadAsObjectParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: nameof(expectedResponse),
                                           skipEndpointValidation: skipEndpointValidation,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              TResult expectedResponse,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                           filterFunc: filterFunc,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: nameof(expectedResponse),
                                           skipEndpointValidation: skipEndpointValidation,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              TResult expectedResponse,
                                                              Func<TResult?, TResult?>? filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                           filterFunc: filterFunc,
                                           parameters: parameters,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: nameof(expectedResponse),
                                           skipEndpointValidation: skipEndpointValidation,
                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              TResult expectedResponse,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsObject: payloadAsObject,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    differenceFunc: differenceFunc,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    differenceFilter: differenceFilter,
                                                    payloadAsObjectParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              TResult expectedResponse,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsObject: payloadAsObject,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    differenceFunc: differenceFunc,
                                                    parameters: parameters,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    differenceFilter: differenceFilter,
                                                    payloadAsObjectParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              TResult expectedResponse,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    differenceFunc: differenceFunc,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    differenceFilter: differenceFilter,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              TResult expectedResponse,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    differenceFunc: differenceFunc,
                                                    parameters: parameters,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    differenceFilter: differenceFilter,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              TResult expectedResponse,
                                                              Predicate<Difference>? differenceFilter,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsObject: payloadAsObject,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    differenceFunc: difference => difference,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    differenceFilter: differenceFilter,
                                                    payloadAsObjectParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              TResult expectedResponse,
                                                              (string Key, object? Value)[] parameters,
                                                              Predicate<Difference>? differenceFilter,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsObject: payloadAsObject,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    differenceFunc: difference => difference,
                                                    parameters: parameters,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    differenceFilter: differenceFilter,
                                                    payloadAsObjectParameterName: payloadAsObjectParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              TResult expectedResponse,
                                                              Predicate<Difference>? differenceFilter,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    differenceFunc: difference => difference,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    differenceFilter: differenceFilter,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              TResult expectedResponse,
                                                              (string Key, object? Value)[] parameters,
                                                              Predicate<Difference>? differenceFilter,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                    differenceFunc: difference => difference,
                                                    parameters: parameters,
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    differenceFilter: differenceFilter,
                                                    payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                    expectedResultParameterName: nameof(expectedResponse),
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }
    }
}