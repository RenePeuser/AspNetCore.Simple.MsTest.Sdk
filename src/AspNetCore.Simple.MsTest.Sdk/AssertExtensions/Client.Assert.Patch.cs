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
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertPatchAsync(this HttpClient client,
                                            string url,
                                            [CallerFilePath] string callerFilePath = "",
                                            bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         HttpMethod.Patch,
                                         [],
                                         Assembly.GetCallingAssembly(),
                                         string.Empty,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task AssertPatchAsync(this HttpClient client,
                                            string url,
                                            (string Key, object? Value)[] parameters,
                                            [CallerFilePath] string callerFilePath = "",
                                            bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         HttpMethod.Patch,
                                         parameters,
                                         Assembly.GetCallingAssembly(),
                                         string.Empty,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    expectedResult,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    expectedResult,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync(url,
                                           string.Empty,
                                           expectedResult,
                                           filterFunc,
                                           [],
                                           Assembly.GetCallingAssembly(),
                                           string.Empty,
                                           expectedResultParameterName,
                                           callerFilePath,
                                           writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync(url,
                                           string.Empty,
                                           expectedResult,
                                           filterFunc,
                                           parameters,
                                           Assembly.GetCallingAssembly(),
                                           string.Empty,
                                           expectedResultParameterName,
                                           callerFilePath,
                                           writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    expectedResult,
                                                    differenceFunc,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    expectedResult,
                                                    differenceFunc,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             string.Empty,
                                             expectedResult,
                                             [],
                                             callingAssembly,
                                             string.Empty,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             string.Empty,
                                             expectedResult,
                                             parameters,
                                             string.Empty,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             string.Empty,
                                             expectedResult,
                                             differenceFunc,
                                             [],
                                             callingAssembly,
                                             string.Empty,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             string.Empty,
                                             expectedResult,
                                             differenceFunc,
                                             parameters,
                                             callingAssembly,
                                             string.Empty,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsObject.ToJson(),
                                                    expectedResult,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    payloadAsObjectParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsObject.ToJson(),
                                                    expectedResult,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    payloadAsObjectParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsJson,
                                                    expectedResult,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    payloadAsJsonParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsJson,
                                                    expectedResult,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    payloadAsJsonParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsObject.ToJson(),
                                             expectedResult,
                                             result => result,
                                             [],
                                             callingAssembly,
                                             payloadAsObjectParameterName,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsObject.ToJson(),
                                             expectedResult,
                                             result => result,
                                             parameters,
                                             callingAssembly,
                                             payloadAsObjectParameterName,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsJson,
                                             expectedResult,
                                             result => result,
                                             [],
                                             callingAssembly,
                                             payloadAsJsonParameterName,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsJson,
                                             expectedResult,
                                             result => result,
                                             parameters,
                                             callingAssembly,
                                             payloadAsJsonParameterName,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsObject.ToJson(),
                                             expectedResult,
                                             result => result,
                                             differenceFunc,
                                             [],
                                             callingAssembly,
                                             payloadAsObjectParameterName,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsObject.ToJson(),
                                             expectedResult,
                                             result => result,
                                             differenceFunc,
                                             parameters,
                                             callingAssembly,
                                             payloadAsObjectParameterName,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsJson,
                                             expectedResult,
                                             result => result,
                                             differenceFunc,
                                             [],
                                             callingAssembly,
                                             payloadAsJsonParameterName,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsJson,
                                             expectedResult,
                                             result => result,
                                             differenceFunc,
                                             parameters,
                                             callingAssembly,
                                             payloadAsJsonParameterName,
                                             expectedResultParameterName,
                                             callerFilePath,
                                             writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync(url,
                                           payloadAsObject.ToJson(),
                                           expectedResult,
                                           filterFunc,
                                           [],
                                           Assembly.GetCallingAssembly(),
                                           payloadAsObjectParameterName,
                                           expectedResultParameterName,
                                           callerFilePath,
                                           writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync(url,
                                           payloadAsObject.ToJson(),
                                           expectedResult,
                                           filterFunc,
                                           parameters,
                                           Assembly.GetCallingAssembly(),
                                           payloadAsObjectParameterName,
                                           expectedResultParameterName,
                                           callerFilePath,
                                           writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync(url,
                                           payloadAsJson,
                                           expectedResult,
                                           filterFunc,
                                           [],
                                           Assembly.GetCallingAssembly(),
                                           payloadAsJsonParameterName,
                                           expectedResultParameterName,
                                           callerFilePath,
                                           writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync(url,
                                           payloadAsJson,
                                           expectedResult,
                                           filterFunc,
                                           parameters,
                                           Assembly.GetCallingAssembly(),
                                           payloadAsJsonParameterName,
                                           expectedResultParameterName,
                                           callerFilePath,
                                           writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsObject.ToJson(),
                                                    expectedResult,
                                                    item => item,
                                                    differenceFunc,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    payloadAsObjectParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsObject.ToJson(),
                                                    expectedResult,
                                                    item => item,
                                                    differenceFunc,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    payloadAsObjectParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsJson,
                                                    expectedResult,
                                                    item => item,
                                                    differenceFunc,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    payloadAsJsonParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsJson,
                                                    expectedResult,
                                                    item => item,
                                                    differenceFunc,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    payloadAsJsonParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath,
                                                    writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Patch,
                                         difference => difference,
                                         [],
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Patch,
                                         difference => difference,
                                         parameters,
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Patch,
                                         difference => difference,
                                         [],
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Patch,
                                         parameters,
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Patch,
                                         [],
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Patch,
                                         parameters,
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Patch,
                                         differenceFunc,
                                         [],
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              bool writeResponse = false)
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Patch,
                                         differenceFunc,
                                         parameters,
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task AssertPatchAsUnauthorizedAsync(this HttpClient httpClient,
                                                          string url)
        {
            return httpClient.AssertPatchAsUnauthorizedAsync(url, null);
        }

        public static async Task AssertPatchAsUnauthorizedAsync(this HttpClient httpClient,
                                                                string url,
                                                                object? body)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.PatchAsync(url, body.IsNull() ? null : new StringContent(body.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"PATCH {url}",
                Expected = HttpStatusCode.Unauthorized,
                Current = result.StatusCode
            }.ToIList();

            var table = ConsoleTable.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, errorOutput);
        }
    }
}
