using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, string.Empty, resultAsJson, result => result, callingAssembly);
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, resultAsJson, result => result, callingAssembly);
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Func<TResult, TResult> filterFunc) where TResult : class
        {
            return client.AssertPutAsync(url, payloadAsJson, resultAsJson, filterFunc, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string payloadAsJson,
                                                   string resultAsJson,
                                                   Func<TResult, TResult> filterFunc,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly);
        }

        public static Task AssertPutError<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertPutError<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutError<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return AssertPutError<TResult>(client, url, string.Empty, resultAsJson, callingAssembly);
        }

        public static Task AssertPutError<TResult>(this HttpClient client,
                                                       string url,
                                                       string payloadAsJson,
                                                       string resultAsJson) where TResult : class
        {
            return client.AssertPutError<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutError<TResult>(this HttpClient client,
                                                       string url,
                                                       string payloadAsJson,
                                                       string resultAsJson,
                                                       Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, item => item, HttpExtensions.PutAsErrorResultWithJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly);
        }
    }
}
