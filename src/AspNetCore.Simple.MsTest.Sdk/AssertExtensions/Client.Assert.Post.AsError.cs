using System.Collections.Generic;
using System.Collections.Immutable;
using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string resultAsJson,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPostAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string resultAsJson,
                                                                    Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPostAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly(), differenceFunc, parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string resultAsJson,
                                                                    Assembly callingAssembly,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPostAsErrorAsync<TResult>(url, string.Empty, resultAsJson, callingAssembly, parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string resultAsJson,
                                                                    Assembly callingAssembly,
                                                                    Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPostAsErrorAsync<TResult>(url, string.Empty, resultAsJson, callingAssembly, differenceFunc, parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string resultAsJson,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPostAsErrorAsync<TResult>(url, payloadAsObject.ToJson(), resultAsJson, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string resultAsJson,
                                                                    Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPostAsErrorAsync<TResult>(url, payloadAsObject.ToJson(), resultAsJson, Assembly.GetCallingAssembly(), differenceFunc, parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string resultAsJson,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPostAsErrorAsync<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string resultAsJson,
                                                                    Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPostAsErrorAsync<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly(), differenceFunc, parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string resultAsJson,
                                                                    Assembly callingAssembly,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), resultAsJson, item => item, HttpExtensions.PostAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string resultAsJson,
                                                                    Assembly callingAssembly,
                                                                    Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), resultAsJson, item => item, HttpExtensions.PostAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, differenceFunc, parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string resultAsJson,
                                                                    Assembly callingAssembly,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PostAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, parameters);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string resultAsJson,
                                                                    Assembly callingAssembly,
                                                                    Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PostAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, differenceFunc, parameters);
        }
    }
}
