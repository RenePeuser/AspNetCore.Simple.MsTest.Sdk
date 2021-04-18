using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertPatchAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPatchAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, string.Empty, resultAsJson, result => result, callingAssembly);
        }

        public static Task AssertPatchAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, Assembly.GetCallingAssembly());
        }

        public static Task AssertPatchAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, callingAssembly);
        }

        public static Task AssertPatchAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertPatchAsync(url, payloadAsJson, resultAsJson, filterFunc, Assembly.GetCallingAssembly());
        }

        public static Task AssertPatchAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Func<TResult, TResult> filterFunc,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, callingAssembly);
        }

        public static Task AssertPatchError<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertPatchError<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPatchError<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return AssertPatchError<TResult>(client, url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task AssertPatchError<TResult>(this HttpClient client,
                                                       string url,
                                                       string payloadAsJson,
                                                       string resultAsJson) where TResult : class
        {
            return client.AssertPatchError<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPatchError<TResult>(this HttpClient client,
                                                       string url,
                                                       string payloadAsJson,
                                                       string resultAsJson,
                                                       Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PatchAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Patch, callingAssembly);
        }
    }
}
