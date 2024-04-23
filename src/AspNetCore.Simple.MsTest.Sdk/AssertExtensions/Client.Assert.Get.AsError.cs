using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string resultAsJson) where TResult : class
        {
            return client.AssertGetAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string resultAsJson,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return client.AssertGetAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly(), differenceFunc);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string resultAsJson,
                                                                   Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, filterFunc, (httpClient, url, _) => HttpExtensions.GetAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Get, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string resultAsJson,
                                                                   Func<TResult, TResult> filterFunc,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, filterFunc, (httpClient, url, _) => HttpExtensions.GetAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Get, Assembly.GetCallingAssembly(), differenceFunc);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                 string url,
                                                                 string resultAsJson,
                                                                 Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, (httpClient, url, _) => HttpExtensions.GetAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string resultAsJson,
                                                                   Assembly callingAssembly,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, (httpClient, url, _) => HttpExtensions.GetAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Get, callingAssembly, differenceFunc);
        }
    }
}
