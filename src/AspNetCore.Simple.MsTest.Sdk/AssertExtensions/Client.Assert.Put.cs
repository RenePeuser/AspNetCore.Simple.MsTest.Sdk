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
        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          bool writeResponse = false,
                                          [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         HttpMethod.Put,
                                         [],
                                         Assembly.GetCallingAssembly(),
                                         string.Empty,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         string.Empty,
                                         HttpMethod.Put,
                                         parameters,
                                         Assembly.GetCallingAssembly(),
                                         string.Empty,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          string payload,
                                          bool writeResponse = false,
                                          [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payload,
                                         HttpMethod.Put,
                                         [],
                                         Assembly.GetCallingAssembly(),
                                         nameof(payload),
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          string payload,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          [CallerArgumentExpression(nameof(payload))]
                                          string payloadParameterName = "",
                                          [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payload,
                                         HttpMethod.Put,
                                         parameters,
                                         Assembly.GetCallingAssembly(),
                                         payloadParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            bool writeResponse = false,
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync<TResult>(url,
                                                  expectedResult,
                                                  [],
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                  callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync<TResult>(url,
                                                  expectedResult,
                                                  parameters,
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  expectedResultParameterName,
                                                  callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync<TResult>(url,
                                                  expectedResult,
                                                  differenceFunc,
                                                  [],
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  expectedResultParameterName,
                                                  callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync<TResult>(url,
                                                  expectedResult,
                                                  differenceFunc,
                                                  parameters,
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  expectedResultParameterName,
                                                  callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return AssertPutAsync<TResult>(client,
                                           url,
                                           expectedResult,
                                           [],
                                           callingAssembly,
                                           writeResponse,
                                           expectedResultParameterName,
                                           callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return AssertPutAsync<TResult>(client,
                                           url,
                                           expectedResult,
                                           item => item,
                                           parameters,
                                           callingAssembly,
                                           writeResponse,
                                           expectedResultParameterName,
                                           callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return AssertPutAsync<TResult>(client,
                                           url,
                                           string.Empty,
                                           expectedResult,
                                           differenceFunc,
                                           [],
                                           callingAssembly,
                                           writeResponse,
                                           string.Empty,
                                           expectedResultParameterName,
                                           callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return AssertPutAsync<TResult>(client,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return client.AssertPutAsync<TResult>(url,
                                                  payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult,
                                                  [],
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  payloadAsObjectParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return client.AssertPutAsync<TResult>(url,
                                                  payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult,
                                                  parameters,
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  payloadAsObjectParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return client.AssertPutAsync<TResult>(url,
                                                  payloadAsJson,
                                                  expectedResult,
                                                  [],
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  payloadAsJsonParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return client.AssertPutAsync<TResult>(url,
                                                  payloadAsJson,
                                                  expectedResult,
                                                  parameters,
                                                  Assembly.GetCallingAssembly(),
                                                  writeResponse,
                                                  payloadAsJsonParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return AssertPutAsync<TResult>(client,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return AssertPutAsync<TResult>(client,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return AssertPutAsync<TResult>(client,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return AssertPutAsync<TResult>(client,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync<TResult>(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync<TResult>(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync<TResult>(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPutAsync<TResult>(url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(JsonSerializerOptions),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Put,
                                         difference => difference,
                                         [],
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(JsonSerializerOptions),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Put,
                                         difference => difference,
                                         parameters,
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Put,
                                         difference => difference,
                                         [],
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Put,
                                         difference => difference,
                                         parameters,
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(JsonSerializerOptions),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Put,
                                         differenceFunc,
                                         [],
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsObject.ToJson(JsonSerializerOptions),
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Put,
                                         differenceFunc,
                                         parameters,
                                         callingAssembly,
                                         payloadAsObjectParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Put,
                                         differenceFunc,
                                         [],
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         HttpMethod.Put,
                                         differenceFunc,
                                         parameters,
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                        string url,
                                                        bool writeResponse = false)
        {
            return httpClient.AssertPutAsUnauthorizedAsync(url,
                                                           null,
                                                           [],
                                                           writeResponse);
        }

        public static async Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                              string url,
                                                              object? body,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var newBody = "";

            if (body.IsNotNull())
            {
                newBody = body.ToJson(JsonSerializerOptions).ResolveParameters(parameters);
            }

            var result = await httpClient.PutAsync(url, body.IsNull() ? null : new StringContent(newBody, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"PUT {url}",
                Expected = HttpStatusCode.Unauthorized,
                Current = result.StatusCode
            }.ToIList();

            var table = ConsoleTable.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, errorOutput);
        }
    }
}
