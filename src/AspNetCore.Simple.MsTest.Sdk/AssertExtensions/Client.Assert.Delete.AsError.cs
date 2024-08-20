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
        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string resultAsJson,
                                                                      params (string Key, string Value)[] parameters)
        {
            return client.AssertDeleteAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string resultAsJson,
                                                                      Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                      params (string Key, string Value)[] parameters)
        {
            return client.AssertDeleteAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly(), differenceFunc, parameters);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string resultAsJson,
                                                                      Assembly callingAssembly,
                                                                      params (string Key, string Value)[] parameters)
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, (httpClient, url, _) => HttpExtensions.DeleteAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Delete, callingAssembly, parameters);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string resultAsJson,
                                                                      Assembly callingAssembly,
                                                                      Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                      params (string Key, string Value)[] parameters)
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, (httpClient, url, _) => HttpExtensions.DeleteAsErrorResultAsync<TResult>(httpClient, url), HttpMethod.Delete, callingAssembly, differenceFunc, parameters);
        }
    }
}
