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
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           bool writeResponse = false,
                                           [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          string.Empty,
                                          writeResponse,
                                          callerFilePath);
        }

        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           string payloadAsJson,
                                           bool writeResponse = false,
                                           [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              httpMethod: HttpMethod.Post,
                                              parameters: [],
                                              callingAssembly: Assembly.GetCallingAssembly(),
                                              payloadAsJsonParameterName: nameof(payloadAsJson),
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             bool writeResponse = false,
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   expectedResult, [],
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                   callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   expectedResult,
                                                   parameters,
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   expectedResultParameterName,
                                                   callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          string.Empty,
                                          expectedResult,
                                          filterFunc,
                                          [],
                                          Assembly.GetCallingAssembly(),
                                          writeResponse,
                                          string.Empty,
                                          expectedResultParameterName,
                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          string.Empty,
                                          expectedResult,
                                          filterFunc,
                                          parameters,
                                          Assembly.GetCallingAssembly(),
                                          writeResponse,
                                          string.Empty,
                                          expectedResultParameterName,
                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   expectedResult,
                                                   differenceFunc,
                                                   [],
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   expectedResultParameterName,
                                                   callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   expectedResult,
                                                   differenceFunc,
                                                   parameters,
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   expectedResultParameterName,
                                                   callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
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
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writResponse: writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
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
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writResponse: writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   expectedResult,
                                                   differenceFunc,
                                                   [],
                                                   callingAssembly,
                                                   writeResponse,
                                                   expectedResultParameterName,
                                                   callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Post,
                                                       differenceFunc: differenceFunc,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writResponse: writeResponse);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult,
                                                   [],
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   payloadAsObjectParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult,
                                                   parameters,
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   payloadAsObjectParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsJson,
                                                   expectedResult,
                                                   [],
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   payloadAsJsonParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsJson,
                                                   expectedResult,
                                                   parameters,
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   payloadAsJsonParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsObject.ToJson(JsonSerializerOptions),
                                            expectedResult,
                                            result => result,
                                            [],
                                            callingAssembly,
                                            writeResponse,
                                            payloadAsObjectParameterName,
                                            expectedResultParameterName,
                                            callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsObject.ToJson(JsonSerializerOptions),
                                            expectedResult,
                                            result => result,
                                            parameters,
                                            callingAssembly,
                                            writeResponse,
                                            payloadAsObjectParameterName,
                                            expectedResultParameterName,
                                            callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsJson,
                                            expectedResult,
                                            result => result,
                                            [],
                                            callingAssembly,
                                            writeResponse,
                                            payloadAsJsonParameterName,
                                            expectedResultParameterName,
                                            callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsJson,
                                            expectedResult,
                                            result => result,
                                            parameters,
                                            callingAssembly,
                                            writeResponse,
                                            payloadAsJsonParameterName,
                                            expectedResultParameterName,
                                            callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsObject.ToJson(JsonSerializerOptions),
                                            expectedResult,
                                            result => result,
                                            differenceFunc,
                                            [],
                                            callingAssembly,
                                            writeResponse,
                                            payloadAsObjectParameterName,
                                            expectedResultParameterName,
                                            callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsObject.ToJson(JsonSerializerOptions),
                                            expectedResult,
                                            result => result,
                                            differenceFunc,
                                            parameters,
                                            callingAssembly,
                                            writeResponse,
                                            payloadAsObjectParameterName,
                                            expectedResultParameterName,
                                            callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsJson,
                                            expectedResult,
                                            result => result,
                                            differenceFunc,
                                            [],
                                            callingAssembly,
                                            writeResponse,
                                            payloadAsJsonParameterName,
                                            expectedResultParameterName,
                                            callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsJson,
                                            expectedResult,
                                            result => result,
                                            differenceFunc,
                                            parameters,
                                            callingAssembly,
                                            writeResponse,
                                            payloadAsJsonParameterName,
                                            expectedResultParameterName,
                                            callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          [],
                                          Assembly.GetCallingAssembly(),
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          parameters,
                                          Assembly.GetCallingAssembly(),
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsJson,
                                          expectedResult,
                                          filterFunc,
                                          [],
                                          Assembly.GetCallingAssembly(),
                                          writeResponse,
                                          payloadAsJsonParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsJson,
                                          expectedResult,
                                          filterFunc,
                                          parameters,
                                          Assembly.GetCallingAssembly(),
                                          writeResponse,
                                          payloadAsJsonParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))]
                                                             string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult,
                                                   item => item,
                                                   differenceFunc,
                                                   [],
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   payloadAsObjectParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult,
                                                   item => item,
                                                   differenceFunc,
                                                   parameters,
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   payloadAsObjectParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))]
                                                             string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsJson,
                                                   expectedResult,
                                                   item => item,
                                                   differenceFunc,
                                                   [],
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   payloadAsJsonParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsJson,
                                                   expectedResult,
                                                   item => item,
                                                   differenceFunc,
                                                   parameters,
                                                   Assembly.GetCallingAssembly(),
                                                   writeResponse,
                                                   payloadAsJsonParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          difference => difference,
                                          [],
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          difference => difference,
                                          parameters,
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsJson,
                                          expectedResult,
                                          filterFunc,
                                          difference => difference,
                                          [],
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsJsonParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsJson,
                                          expectedResult,
                                          filterFunc,
                                          difference => difference,
                                          parameters,
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsJsonParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          differenceFunc,
                                          [],
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
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
                                                             [CallerArgumentExpression(nameof(payloadAsObject))]
                                                             string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Post,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsJson,
                                          expectedResult,
                                          filterFunc,
                                          differenceFunc,
                                          [],
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsJsonParameterName,
                                          expectedResultParameterName,
                                          callerFilePath);
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
                                                             [CallerArgumentExpression(nameof(payloadAsJson))]
                                                             string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Post,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse);
        }

        public static Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
                                                         string url)
        {
            return httpClient.AssertPostAsUnauthorizedAsync(url, null, []);
        }

        public static Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
                                                         string url,
                                                         object? body)
        {
            return httpClient.AssertPostAsUnauthorizedAsync(url, body, []);
        }

        public static async Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
                                                               string url,
                                                               object? body,
                                                               (string Key, object? Value)[] parameters)
        {
            Throw.IfNull(httpClient);
            Throw.IfNullOrWhiteSpace(url);
            Throw.IfNull(parameters);

            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var newBody = "";

            if (body.IsNotNull())
            {
                newBody = body.ToJson(JsonSerializerOptions).ResolveParameters(parameters);
            }

            using var stringContent = new StringContent(newBody, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await httpClient.PostAsync(url, body.IsNull() ? null : stringContent).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
                                {
                                    Request = $"POST {url}",
                                    Expected = HttpStatusCode.Unauthorized,
                                    Current = result.StatusCode
                                }.ToIList();

            var table = ConsoleTable.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, errorOutput);
        }

        // ============================================================
        // Complete Context API (Level 3) - Everything in Context!
        // ============================================================

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task AssertPostAsync(HttpAssertContext<string> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                      httpMethod: HttpMethod.Post,
                                                      parameters: context.Parameters,
                                                      callingAssembly: context.CallingAssembly,
                                                      payloadAsJsonParameterName: context.PayloadParameterName,
                                                      callerFilePath: context.CallerFilePath,
                                                      isSuccessStatusCode: true,
                                                      writResponse: context.WriteResponse);
        }

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
                                                      parameters: context.Parameters,
                                                      callingAssembly: context.CallingAssembly,
                                                      payloadAsJsonParameterName: context.PayloadParameterName,
                                                      expectedResultParameterName: context.ExpectedResultParameterName,
                                                      callerFilePath: context.CallerFilePath,
                                                      isSuccessStatusCode: true,
                                                      writResponse: context.WriteResponse);
        }
    }
}
