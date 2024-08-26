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
                                          (string Key, string Value)[] parameters)
        {
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.PutAsync(urlParam, null), HttpMethod.Put, parameters, Assembly.GetCallingAssembly());
        }

        public static Task AssertPutAsync(this HttpClient client,
                                                string url,
                                                string payload,
                                                (string Key, string Value)[] parameters,
                                                [CallerArgumentExpression(nameof(payload))] string payloadParameterName = "")
        {
            var jsonBody = payload.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? Assembly.GetCallingAssembly().GetFileContentFrom(payload) : payload;
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.PutAsync(urlParam, new StringContent(jsonBody, Encoding.UTF8, MediaTypeNames.Application.Json)), HttpMethod.Put, parameters, Assembly.GetCallingAssembly(), payloadParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsync<TResult>(url, expectedResult, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsync(url, string.Empty, expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsync<TResult>(url, expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPutAsync<TResult>(client, url, string.Empty, expectedResult, parameters, callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPutAsync<TResult>(client, url, string.Empty, expectedResult, differenceFunc, parameters, callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPutAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, parameters, callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPutAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPutAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, differenceFunc, parameters, callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPutAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsync(url, payloadAsObject.ToJson(), expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsync(url, payloadAsJson, expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, item => item, differenceFunc, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, string Value)[] parameters,
                                                            [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPutAsync<TResult>(url, payloadAsJson, expectedResult, item => item, differenceFunc, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, difference => difference, parameters, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, difference => difference, parameters, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult, TResult> filterFunc,
                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            (string Key, string Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, differenceFunc, parameters, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PutAsJsonStringAsync<TResult>, HttpMethod.Put, callingAssembly, differenceFunc, parameters, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            return httpClient.AssertPutAsUnauthorizedAsync(url, null, []);
        }

        public static async Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                              string url,
                                                              object? body,
                                                              (string Key, string Value)[] parameters)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var newBody = "";
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
