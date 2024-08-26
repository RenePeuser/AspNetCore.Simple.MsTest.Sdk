using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsErrorAsync<TResult>(url, expectedResult, [], Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsErrorAsync<TResult>(url, expectedResult, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsErrorAsync<TResult>(url, expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, string Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsErrorAsync<TResult>(url, string.Empty, expectedResult, parameters, callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsErrorAsync<TResult>(url, string.Empty, expectedResult, differenceFunc, parameters, callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   object payloadAsObject,
                                                                   string expectedResult,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsErrorAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   object payloadAsObject,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsErrorAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsErrorAsync<TResult>(url, payloadAsJson, expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsErrorAsync<TResult>(url, payloadAsJson, expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(),payloadAsJsonParameterName,  expectedResultParameterName);
        }



        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   object payloadAsObject,
                                                                   string expectedResult,
                                                                   (string Key, string Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, item => item, HttpExtensions.PutAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, parameters, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   object payloadAsObject,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, item => item, HttpExtensions.PutAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, differenceFunc, parameters, payloadAsObjectParameterName, expectedResultParameterName);
        }


        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   (string Key, string Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, item => item, HttpExtensions.PutAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, parameters, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, item => item, HttpExtensions.PutAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, differenceFunc, parameters, payloadAsJsonParameterName, expectedResultParameterName);
        }
    }
}
