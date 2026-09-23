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
                                                             [CallerLineNumber] int callerLineNumber = 0,
                                                             [CallerArgumentExpression(nameof(expectedResponse))]
                                                             string expectedResponseParameterName = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   parameters: parameters,
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   expectedResultParameterName: expectedResponseParameterName,
                                                   skipEndpointValidation: skipEndpointValidation,
                                                   expectedHttpStatusCode: expectedHttpStatusCode,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?>? filterFunc,
                                                             bool writeResponse = false,
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0,
                                                             [CallerArgumentExpression(nameof(expectedResponse))]
                                                             string expectedResponseParameterName = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: string.Empty,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          filterFunc: filterFunc,
                                          parameters: [],
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: string.Empty,
                                          expectedResultParameterName: expectedResponseParameterName,
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?>? filterFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0,
                                                             [CallerArgumentExpression(nameof(expectedResponse))]
                                                             string expectedResponseParameterName = "")
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: string.Empty,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          filterFunc: filterFunc,
                                          parameters: parameters,
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: string.Empty,
                                          expectedResultParameterName: expectedResponseParameterName,
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
                                                             Predicate<Difference>? differenceFilter = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             TResult expectedResponse,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))]
                                                             string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResponse))]
                                                             string expectedResponseParameterName = "",
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                              expectedResponse: expectedResponse,
                                              expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                              filterFunc: null,
                                              httpMethod: HttpMethod.Post,
                                              differenceFunc: item => item,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              writeResponse: writeResponse,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              expectedResponseParameterName: expectedResponseParameterName,
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
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
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
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
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
                                                             Predicate<Difference>? differenceFilter = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   filterFunc: null,
                                                   differenceFunc: differenceFunc,
                                                   parameters: [],
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                   expectedResultParameterName: nameof(expectedResponse),
                                                   skipEndpointValidation: skipEndpointValidation,
                                                   expectedHttpStatusCode: expectedHttpStatusCode,
                                                   differenceFilter: differenceFilter,
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
                                                             Predicate<Difference>? differenceFilter = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   filterFunc: null,
                                                   differenceFunc: differenceFunc,
                                                   parameters: parameters,
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                   expectedResultParameterName: nameof(expectedResponse),
                                                   skipEndpointValidation: skipEndpointValidation,
                                                   expectedHttpStatusCode: expectedHttpStatusCode,
                                                   differenceFilter: differenceFilter,
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
                                                             Predicate<Difference>? differenceFilter = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   filterFunc: null,
                                                   differenceFunc: differenceFunc,
                                                   parameters: [],
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                   expectedResultParameterName: nameof(expectedResponse),
                                                   skipEndpointValidation: skipEndpointValidation,
                                                   expectedHttpStatusCode: expectedHttpStatusCode,
                                                   differenceFilter: differenceFilter,
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
                                                             Predicate<Difference>? differenceFilter = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   filterFunc: null,
                                                   differenceFunc: differenceFunc,
                                                   parameters: parameters,
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                   expectedResultParameterName: nameof(expectedResponse),
                                                   skipEndpointValidation: skipEndpointValidation,
                                                   expectedHttpStatusCode: expectedHttpStatusCode,
                                                   differenceFilter: differenceFilter,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
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

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
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
                                                             Func<TResult?, TResult?>? filterFunc,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          filterFunc: filterFunc,
                                          differenceFunc: differenceFunc,
                                          parameters: [],
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: payloadAsObjectParameterName,
                                          expectedResultParameterName: nameof(expectedResponse),
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          differenceFilter: differenceFilter,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?>? filterFunc,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          filterFunc: filterFunc,
                                          differenceFunc: differenceFunc,
                                          parameters: parameters,
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: payloadAsObjectParameterName,
                                          expectedResultParameterName: nameof(expectedResponse),
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          differenceFilter: differenceFilter,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?>? filterFunc,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))]
                                                             string payloadAsJsonParameterName = "",
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             Predicate<Difference>? differenceFilter = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          filterFunc: filterFunc,
                                          differenceFunc: differenceFunc,
                                          parameters: [],
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: payloadAsJsonParameterName,
                                          expectedResultParameterName: nameof(expectedResponse),
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          differenceFilter: differenceFilter,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             TResult expectedResponse,
                                                             Func<TResult?, TResult?>? filterFunc,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))]
                                                             string payloadAsJsonParameterName = "",
                                                             bool skipEndpointValidation = false,
                                                             HttpStatusCode? expectedHttpStatusCode = null,
                                                             Predicate<Difference>? differenceFilter = null,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          filterFunc: filterFunc,
                                          differenceFunc: differenceFunc,
                                          parameters: parameters,
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: payloadAsJsonParameterName,
                                          expectedResultParameterName: nameof(expectedResponse),
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          differenceFilter: differenceFilter,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   filterFunc: null,
                                                   differenceFunc: difference => difference,
                                                   parameters: [],
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                   expectedResultParameterName: nameof(expectedResponse),
                                                   skipEndpointValidation: skipEndpointValidation,
                                                   expectedHttpStatusCode: expectedHttpStatusCode,
                                                   differenceFilter: differenceFilter,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   filterFunc: null,
                                                   differenceFunc: difference => difference,
                                                   parameters: parameters,
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                   expectedResultParameterName: nameof(expectedResponse),
                                                   skipEndpointValidation: skipEndpointValidation,
                                                   expectedHttpStatusCode: expectedHttpStatusCode,
                                                   differenceFilter: differenceFilter,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   filterFunc: null,
                                                   differenceFunc: difference => difference,
                                                   parameters: [],
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                   expectedResultParameterName: nameof(expectedResponse),
                                                   skipEndpointValidation: skipEndpointValidation,
                                                   expectedHttpStatusCode: expectedHttpStatusCode,
                                                   differenceFilter: differenceFilter,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        // differenceFilter-only twin: differenceFilter usable without an explicit differenceFunc
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                   filterFunc: null,
                                                   differenceFunc: difference => difference,
                                                   parameters: parameters,
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsJsonParameterName,
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