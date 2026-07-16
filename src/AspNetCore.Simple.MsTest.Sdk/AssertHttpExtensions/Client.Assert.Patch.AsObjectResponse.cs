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
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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

            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
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

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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

            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
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

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              TResult expectedResponse,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsObject: payloadAsObject,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                    differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              TResult expectedResponse,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              Predicate<Difference>? differenceFilter = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPatchAsync<TResult>(url: url,
                                                    payloadAsObject: payloadAsObject,
                                                    expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                    differenceFunc: differenceFunc,
                                                    parameters: parameters,
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
    }
}