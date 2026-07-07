using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Argument.Check;
using AspNetCore.Simple.MsTest.Sdk.Http;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        // ============================================================
        // QUERY with Response (TResult) - All overloads
        // ============================================================

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              bool writeResponse = false,
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: string.Empty,
                                                    parameters: [],
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    ignoreResponse: true,
                                                    payloadAsJsonParameterName: string.Empty,
                                                    expectedResultParameterName: string.Empty,
                                                    callerFilePath: callerFilePath,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        /// <summary>
        /// Clean API: QUERY with type parameter, no expectedResult - validates endpoint and ignores response.
        /// Useful when you only care about endpoint validation (correct response type) without comparing response content.
        /// </summary>
        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              bool writeResponse = false,
                                                              bool skipEndpointValidation = false,
                                                              HttpStatusCode? expectedHttpStatusCode = null,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: string.Empty,
                                                    expectedResult: string.Empty,
                                                    parameters: [],
                                                    callingAssembly: callingAssembly,
                                                    writeResponse: writeResponse,
                                                    ignoreResponse: true,
                                                    payloadAsJsonParameterName: string.Empty,
                                                    expectedResultParameterName: string.Empty,
                                                    skipEndpointValidation: skipEndpointValidation,
                                                    expectedHttpStatusCode: expectedHttpStatusCode,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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

            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: string.Empty,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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

            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: string.Empty,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertQueryAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
                                                    differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertQueryAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
                                                    differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse,
                                                              bool ignoreResponse,
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
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: payloadAsJson,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethodExtensions.Query,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       ignoreResponse: ignoreResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethodExtensions.Query,
                                                       parameters: [],
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethodExtensions.Query,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            return client.AssertQueryAsync<TResult>(url: url,
                                                    expectedResult: expectedResult,
                                                    differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethodExtensions.Query,
                                                       differenceFunc: differenceFunc,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       skipEndpointValidation: skipEndpointValidation,
                                                       expectedHttpStatusCode: expectedHttpStatusCode,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writeResponse: writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                    expectedResult: expectedResult,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                    expectedResult: expectedResult,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
            return AssertQueryAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
            return AssertQueryAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
            return AssertQueryAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsJson,
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
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
            return AssertQueryAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsJson,
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            return AssertQueryAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            return AssertQueryAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            return AssertQueryAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsJson,
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            return AssertQueryAsync<TResult>(client: client,
                                             url: url,
                                             payloadAsJson: payloadAsJson,
                                             expectedResult: expectedResult,
                                             filterFunc: result => result,
                                             differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
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

            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
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

            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                    expectedResult: expectedResult,
                                                    filterFunc: item => item,
                                                    differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                    expectedResult: expectedResult,
                                                    filterFunc: item => item,
                                                    differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
                                                    filterFunc: item => item,
                                                    differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertQueryAsync<TResult>(url: url,
                                                    payloadAsJson: payloadAsJson,
                                                    expectedResult: expectedResult,
                                                    filterFunc: item => item,
                                                    differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
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
            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
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
            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
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
            return client.AssertQueryAsync(url: url,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
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
            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: differenceFunc,
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

        // MAXIMUM OVERLOAD - Contains the core logic (object payload variant)
        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethodExtensions.Query,
                                              differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            return client.AssertQueryAsync(url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: filterFunc,
                                           differenceFunc: differenceFunc,
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

        // MAXIMUM OVERLOAD - Contains the core logic (string payload variant)
        public static Task<TResult> AssertQueryAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethodExtensions.Query,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              skipEndpointValidation: skipEndpointValidation,
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertQueryAsUnauthorizedAsync(this HttpClient httpClient,
                                                          string url)
        {
            return httpClient.AssertQueryAsUnauthorizedAsync(url: url, body: null, parameters: []);
        }

        public static Task AssertQueryAsUnauthorizedAsync(this HttpClient httpClient,
                                                          string url,
                                                          object? body)
        {
            return httpClient.AssertQueryAsUnauthorizedAsync(url: url, body: body, parameters: []);
        }

        public static async Task AssertQueryAsUnauthorizedAsync(this HttpClient httpClient,
                                                                string url,
                                                                object? body,
                                                                (string Key, object? Value)[] parameters)
        {
            Throw.IfNull(argument: httpClient);
            Throw.IfNullOrWhiteSpace(argument: url);
            Throw.IfNull(argument: parameters);

            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var newBody = "";

            if (body.IsNotNull())
            {
                newBody = body.ToJson(JsonSerializerOptions).ResolveParameters(parameters);
            }

            using var request = new HttpRequestMessage(new HttpMethod("QUERY"), url);

            if (body.IsNotNull())
            {
                request.Content = new StringContent(content: newBody, encoding: Encoding.UTF8, mediaType: MediaTypeNames.Application.Json);
            }

            var result = await httpClient.SendAsync(request).ConfigureAwait(continueOnCapturedContext: false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"QUERY {url}",
                Expected = HttpStatusCode.Unauthorized,
                Current = result.StatusCode
            }.ToIList();

            var table = TableFormatter.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(expected: HttpStatusCode.Unauthorized, actual: result.StatusCode, message: errorOutput);
        }

        // ============================================================
        // Complete Context API (Level 3) - Everything in Context!
        // ============================================================

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task<TResult> AssertQueryAsync<TResult>(HttpAssertContext<TResult> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                      expectedResult: context.ExpectedObjectAsJson,
                                                      filterFunc: context.OrderFunc,
                                                      httpMethod: HttpMethodExtensions.Query,
                                                      differenceFunc: context.DifferenceFunc,
                                                      parameters: context.Parameters,
                                                      callingAssembly: context.CallingAssembly,
                                                      payloadAsJsonParameterName: context.PayloadParameterName,
                                                      expectedResultParameterName: context.ExpectedResultParameterName,
                                                      callerFilePath: context.CallerFilePath,
                                                      isSuccessStatusCode: true,
                                                      writeResponse: context.WriteResponse,
                                                      callerLineNumber: context.CallerLineNumber);
        }
    }
}