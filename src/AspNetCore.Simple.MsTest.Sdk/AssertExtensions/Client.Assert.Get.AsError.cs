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
        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url, expectedResult, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url, expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult, TResult> filterFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, filterFunc, (httpClient, url, _) => HttpExtensions.GetAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Get, Assembly.GetCallingAssembly(), parameters, expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult, TResult> filterFunc,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, filterFunc, (httpClient, url, _) => HttpExtensions.GetAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Get, Assembly.GetCallingAssembly(), differenceFunc, parameters, expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                 string url,
                                                                 string expectedResult,
                                                                 (string Key, string Value)[] parameters,
                                                                 Assembly callingAssembly,
                                                                 [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, item => item, (httpClient, url, _) => HttpExtensions.GetAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, parameters, expectedResultParameterName);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, string Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, item => item, (httpClient, url, _) => HttpExtensions.GetAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, differenceFunc, parameters, expectedResultParameterName);
        }
    }
}
