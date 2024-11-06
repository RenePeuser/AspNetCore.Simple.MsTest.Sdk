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
        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string expectedResult)
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, expectedResult, [], Assembly.GetCallingAssembly(), expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult));
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string expectedResult,
                                                                     (string Key, object? Value)[] parameters,
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, expectedResult, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, expectedResult, differenceFunc, [], Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     (string Key, object? Value)[] parameters,
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string expectedResult,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, string.Empty, expectedResult, [], callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string expectedResult,
                                                                     (string Key, object? Value)[] parameters,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, string.Empty, expectedResult, parameters, callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, string.Empty, expectedResult, differenceFunc, [], callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     (string Key, object? Value)[] parameters,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, string.Empty, expectedResult, differenceFunc, parameters, callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     object payloadAsObject,
                                                                     string expectedResult,
                                                                     [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, [], Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     object payloadAsObject,
                                                                     string expectedResult,
                                                                     (string Key, object? Value)[] parameters,
                                                                     [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     object payloadAsObject,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, differenceFunc, [], Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     object payloadAsObject,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     (string Key, object? Value)[] parameters,
                                                                     [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     string expectedResult,
                                                                     [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, payloadAsJson, expectedResult, [], Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     string expectedResult,
                                                                     (string Key, object? Value)[] parameters,
                                                                     [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, payloadAsJson, expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, payloadAsJson, expectedResult, differenceFunc, [], Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     (string Key, object? Value)[] parameters,
                                                                     [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsErrorAsync<TResult>(url, payloadAsJson, expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     object payloadAsObject,
                                                                     string expectedResult,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, [],  callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     object payloadAsObject,
                                                                     string expectedResult,
                                                                     (string Key, object? Value)[] parameters,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, parameters, callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     object payloadAsObject,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, differenceFunc, [], callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     object payloadAsObject,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     (string Key, object? Value)[] parameters,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, differenceFunc, parameters, callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     string expectedResult,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, [],  callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     string expectedResult,
                                                                     (string Key, object? Value)[] parameters,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, differenceFunc, [], callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     string expectedResult,
                                                                     Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                     (string Key, object? Value)[] parameters,
                                                                     Assembly callingAssembly,
                                                                     [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                     [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }
    }
}
