using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
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
                                          params (string Key, string Value)[] parameters)
        {
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.PutAsync(urlParam, null), HttpMethod.Put, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                                string url,
                                                string payload,
                                                params (string Key, string Value)[] parameters)
        {
            var jsonBody = payload.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? Assembly.GetCallingAssembly().GetFileContentFrom(payload) : payload;
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.PutAsync(urlParam, new StringContent(jsonBody, Encoding.UTF8, MediaTypeNames.Application.Json)), HttpMethod.Put, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Func<TResult, TResult> filterFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPutAsync(url, string.Empty, resultAsJson, filterFunc, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly(), differenceFunc, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return AssertPutAsync<TResult>(client, url, string.Empty, resultAsJson, callingAssembly, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string resultAsJson,
                                                             Assembly callingAssembly,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             params (string Key, string Value)[] parameters) where TResult : class
        {
            return AssertPutAsync<TResult>(client, url, string.Empty, resultAsJson, callingAssembly, differenceFunc, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string resultAsJson,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsObject.ToJson(), resultAsJson, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string resultAsJson,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, resultAsJson, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string resultAsJson,
                                                            Assembly callingAssembly,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return AssertPutAsync<TResult>(client, url, payloadAsObject.ToJson(), resultAsJson, result => result, callingAssembly, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string resultAsJson,
                                                            Assembly callingAssembly,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return AssertPutAsync<TResult>(client, url, payloadAsJson, resultAsJson, result => result, callingAssembly, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string resultAsJson,
                                                            Assembly callingAssembly,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return AssertPutAsync<TResult>(client, url, payloadAsObject.ToJson(), resultAsJson, result => result, differenceFunc, callingAssembly, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Assembly callingAssembly,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             params (string Key, string Value)[] parameters) where TResult : class
        {
            return AssertPutAsync<TResult>(client, url, payloadAsJson, resultAsJson, result => result, differenceFunc, callingAssembly, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string resultAsJson,
                                                            Func<TResult, TResult> filterFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPutAsync(url, payloadAsObject.ToJson(), resultAsJson, filterFunc, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc,
                                                             params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPutAsync(url, payloadAsJson, resultAsJson, filterFunc, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string resultAsJson,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsObject.ToJson(), resultAsJson, item => item, differenceFunc, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string resultAsJson,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, resultAsJson, item => item, differenceFunc, Assembly.GetCallingAssembly(), parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string resultAsJson,
                                                            Func<TResult, TResult> filterFunc,
                                                            Assembly callingAssembly,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), resultAsJson, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, difference => difference, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc,
                                                             Assembly callingAssembly,
                                                             params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, difference => difference, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string resultAsJson,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), resultAsJson, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, differenceFunc, parameters);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string resultAsJson,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             params (string Key, string Value)[] parameters) where TResult : class
        {
            return client.AssertHttpCall(url, payloadAsJson, resultAsJson, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, differenceFunc, parameters);
        }

        public static Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            return httpClient.AssertPutAsUnauthorizedAsync(url, null);
        }

        public static async Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                              string url,
                                                              object? body,
                                                              params (string Key, string Value)[] parameters)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            string newBody = "";
            if (body.IsNotNull())
            {
                newBody = body.ToJson();
                parameters.ForEach(p => newBody = newBody.Replace(p.Key, p.Value));
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
