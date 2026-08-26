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
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        // ============================================================
        // POST with Response (TResult) - All overloads
        // ============================================================

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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
        /// Clean API: POST with type parameter, no expectedResult - validates endpoint and ignores response.
        /// Useful when you only care about endpoint validation (correct response type) without comparing response content.
        /// </summary>
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             bool writeResponse = false,
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResult,
                                                   differenceFunc: differenceFunc,
                                                   differenceFilter: differenceFilter,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResult,
                                                   differenceFunc: differenceFunc,
                                                   differenceFilter: differenceFilter,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                       httpMethod: HttpMethod.Post,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                       httpMethod: HttpMethod.Post,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                       httpMethod: HttpMethod.Post,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResult,
                                                   differenceFunc: differenceFunc,
                                                   differenceFilter: differenceFilter,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Post,
                                                       differenceFunc: differenceFunc,
                                                       differenceFilter: differenceFilter,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
                                            url: url,
                                            payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                            expectedResult: expectedResult,
                                            filterFunc: result => result,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
                                            url: url,
                                            payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                            expectedResult: expectedResult,
                                            filterFunc: result => result,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
                                            url: url,
                                            payloadAsJson: payloadAsJson,
                                            expectedResult: expectedResult,
                                            filterFunc: result => result,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
                                            url: url,
                                            payloadAsJson: payloadAsJson,
                                            expectedResult: expectedResult,
                                            filterFunc: result => result,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertPostAsync(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult?, TResult?> filterFunc,
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
            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult: expectedResult,
                                          filterFunc: filterFunc,
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

        // MAXIMUM OVERLOAD - Contains the core logic (object payload variant)
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult?, TResult?> filterFunc,
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
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Post,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult?, TResult?> filterFunc,
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
            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          expectedResult: expectedResult,
                                          filterFunc: filterFunc,
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

        // MAXIMUM OVERLOAD - Contains the core logic (string payload variant)
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                              httpMethod: HttpMethod.Post,
                                              differenceFunc: differenceFunc,
                                              differenceFilter: differenceFilter,
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

        public static Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
                                                         string url,
                                                         [CallerFilePath] string callerFilePath = "",
                                                         [CallerMemberName] string callerMemberName = "",
                                                         [CallerLineNumber] int callerLineNumber = 0)
        {
            return httpClient.AssertPostAsUnauthorizedAsync(url: url, body: null, parameters: [],
                                                            callerFilePath: callerFilePath,
                                                            callerMemberName: callerMemberName,
                                                            callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
                                                         string url,
                                                         object? body,
                                                         [CallerFilePath] string callerFilePath = "",
                                                         [CallerMemberName] string callerMemberName = "",
                                                         [CallerLineNumber] int callerLineNumber = 0)
        {
            return httpClient.AssertPostAsUnauthorizedAsync(url: url, body: body, parameters: [],
                                                            callerFilePath: callerFilePath,
                                                            callerMemberName: callerMemberName,
                                                            callerLineNumber: callerLineNumber);
        }

        public static async Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
                                                               string url,
                                                               object? body,
                                                               (string Key, object? Value)[] parameters,
                                                               [CallerFilePath] string callerFilePath = "",
                                                               [CallerMemberName] string callerMemberName = "",
                                                               [CallerLineNumber] int callerLineNumber = 0)
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

            using var stringContent = new StringContent(content: newBody, encoding: Encoding.UTF8, mediaType: MediaTypeNames.Application.Json);
            var result = await httpClient.PostAsync(requestUri: url, content: body.IsNull() ? null : stringContent).ConfigureAwait(continueOnCapturedContext: false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            Assert.That.AreEqual(HttpStatusCode.Unauthorized,
                                 result.StatusCode,
                                 because: $"POST {url} was called with an invalid bearer token, so the endpoint has to reject it with 401 Unauthorized. Any other status code means the route can be reached without valid credentials.",
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
        public static Task<TResult> AssertPostAsync<TResult>(HttpAssertContext<TResult> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                      expectedResult: context.ExpectedObjectAsJson,
                                                      filterFunc: context.OrderFunc,
                                                      httpMethod: HttpMethod.Post,
                                                      differenceFunc: context.DifferenceFunc,
                                                      differenceFilter: context.DifferenceFilter,
                                                      parameters: context.Parameters,
                                                      callingAssembly: context.CallingAssembly,
                                                      payloadAsJsonParameterName: context.PayloadParameterName,
                                                      expectedResultParameterName: context.ExpectedResultParameterName,
                                                      callerFilePath: context.CallerFilePath,
                                                      isSuccessStatusCode: true,
                                                      writeResponse: context.WriteResponse,
                                                      callerLineNumber: context.CallerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResult,
                                                   differenceFunc: difference => difference,
                                                   differenceFilter: differenceFilter,
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

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResult,
                                                   differenceFunc: difference => difference,
                                                   differenceFilter: differenceFilter,
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

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult: expectedResult,
                                                   filterFunc: item => item,
                                                   differenceFunc: difference => difference,
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

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult: expectedResult,
                                                   filterFunc: item => item,
                                                   differenceFunc: difference => difference,
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

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResult,
                                                   filterFunc: item => item,
                                                   differenceFunc: difference => difference,
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

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResult,
                                                   filterFunc: item => item,
                                                   differenceFunc: difference => difference,
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
    }
}