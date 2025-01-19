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
using System.Threading.RateLimiting;
using System.Threading.Tasks;
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
            return client.AssertHttpCall(url, 
                                         string.Empty, 
                                         HttpMethod.Post,
                                         [], 
                                         Assembly.GetCallingAssembly(),
                                         string.Empty,
                                         callerFilePath,
                                         true,
                                         writeResponse);
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
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<TResult, TResult> filterFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            string.Empty,
                                            expectedResult,
                                            [],
                                            callingAssembly,
                                            writeResponse,
                                            string.Empty,
                                            expectedResultParameterName,
                                            callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            expectedResult,
                                            item => item,
                                            parameters,
                                            callingAssembly,
                                            writeResponse,
                                            expectedResultParameterName,
                                            callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            string.Empty,
                                            expectedResult,
                                            differenceFunc,
                                            parameters,
                                            callingAssembly,
                                            writeResponse,
                                            string.Empty,
                                            expectedResultParameterName,
                                            callerFilePath);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsObject.ToJson(),
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
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsObject.ToJson(),
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
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsObject.ToJson(),
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
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsObject.ToJson(),
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
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsObject.ToJson(),
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return AssertPostAsync<TResult>(client,
                                            url,
                                            payloadAsObject.ToJson(),
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<TResult, TResult> filterFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(),
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
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(),
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
                                                             Func<TResult, TResult> filterFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsObject.ToJson(),
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsObject.ToJson(),
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
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
                                                             Func<TResult, TResult> filterFunc,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Post,
                                         difference => difference,
                                         [],
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Post,
                                         difference => difference,
                                         parameters,
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Post,
                                         difference => difference,
                                         [],
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Post,
                                         difference => difference,
                                         parameters,
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Post,
                                         differenceFunc,
                                         [],
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Post,
                                         differenceFunc,
                                         parameters,
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Post,
                                         differenceFunc,
                                         [],
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Post,
                                         differenceFunc,
                                         parameters,
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
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
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var newBody = "";

            if (body.IsNotNull())
            {
                newBody = body.ToJson().ResolveParameters(parameters);
            }

            var result = await httpClient.PostAsync(url, body.IsNull() ? null : new StringContent(newBody, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);

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
    }
}
