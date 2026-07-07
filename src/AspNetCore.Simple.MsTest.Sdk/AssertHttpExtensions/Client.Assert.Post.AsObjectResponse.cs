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
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             TResult expectedResponse,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             bool writeResponse = false,
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: string.Empty,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
                                          parameters: [],
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: string.Empty,
                                          expectedResultParameterName: nameof(expectedResponse),
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: string.Empty,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
                                          parameters: parameters,
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: string.Empty,
                                          expectedResultParameterName: nameof(expectedResponse),
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             TResult expectedResponse,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             bool writeResponse = false,
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                   differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             TResult expectedResponse,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                   differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             TResult expectedResponse,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                   parameters: parameters,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             TResult expectedResponse,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                   parameters: parameters,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             TResult expectedResponse,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                   filterFunc: item => item,
                                                   differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                   filterFunc: item => item,
                                                   differenceFunc: differenceFunc,
                                                   parameters: parameters,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             TResult expectedResponse,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                   filterFunc: item => item,
                                                   differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             TResult expectedResponse,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                   filterFunc: item => item,
                                                   differenceFunc: differenceFunc,
                                                   parameters: parameters,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
                                          parameters: parameters,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
                                          parameters: parameters,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
                                          differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
                                          differenceFunc: differenceFunc,
                                          parameters: parameters,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
                                          differenceFunc: differenceFunc,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                          filterFunc: filterFunc,
                                          differenceFunc: differenceFunc,
                                          parameters: parameters,
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
    }
}