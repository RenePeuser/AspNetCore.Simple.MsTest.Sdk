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
        public static Task AssertPatchAsync(this HttpClient client,
                                            string url)
        {
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.PatchAsync(urlParam, null), HttpMethod.Patch, [], Assembly.GetCallingAssembly());
        }

        public static Task AssertPatchAsync(this HttpClient client,
                                            string url,
                                            (string Key, string Value)[] parameters)
        {
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.PatchAsync(urlParam, null), HttpMethod.Patch, parameters, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult)
        {
            return client.AssertPatchAsync<TResult>(url, expectedResult, [], Assembly.GetCallingAssembly(), expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult));
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, expectedResult, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync(url, string.Empty, expectedResult, filterFunc, [], Assembly.GetCallingAssembly(), string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync(url, string.Empty, expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, expectedResult, differenceFunc, [], Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, string.Empty, expectedResult, [], expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, string.Empty, expectedResult, parameters, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, string.Empty, expectedResult, differenceFunc, [], callingAssembly, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, string.Empty, expectedResult, differenceFunc, parameters, callingAssembly, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, [], Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              (string Key, string Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, expectedResult, [], Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              (string Key, string Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, [], callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              (string Key, string Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, parameters, callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, [], callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, differenceFunc, [], callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, string Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, differenceFunc, parameters, callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, differenceFunc, [], callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPatchAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync(url, payloadAsObject.ToJson(), expectedResult, filterFunc, [], Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              (string Key, string Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync(url, payloadAsObject.ToJson(), expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync(url, payloadAsJson, expectedResult, filterFunc, [], Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              (string Key, string Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync(url, payloadAsJson, expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, item => item, differenceFunc, [], Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, string Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, item => item, differenceFunc, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, expectedResult, item => item, differenceFunc, [], Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, string Value)[] parameters,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPatchAsync<TResult>(url, payloadAsJson, expectedResult, item => item, differenceFunc, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, difference => difference, [],callingAssembly, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              (string Key, string Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, difference => difference, parameters,callingAssembly, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, difference => difference, [],callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, difference => difference, parameters,callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, differenceFunc, [], callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, differenceFunc, parameters, callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult, TResult> filterFunc,
                                                              Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, differenceFunc, [], callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PatchAsJsonStringAsync<TResult>, HttpMethod.Patch, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }


        public static Task AssertPatchAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            return httpClient.AssertPatchAsUnauthorizedAsync(url, null);
        }

        public static async Task AssertPatchAsUnauthorizedAsync(this HttpClient httpClient, string url, object? body)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.PatchAsync(url, body.IsNull() ? null : new StringContent(body.ToJson(), Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"PATCH {url}",
                Expected = HttpStatusCode.Unauthorized,
                Current = result.StatusCode
            }.ToIList();

            var table = ConsoleTable.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, errorOutput);
        }
    }
}
