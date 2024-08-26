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
        public static Task AssertPostAsync(this HttpClient client,
                                           string url)
        {
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.PostAsync(urlParam, null), HttpMethod.Post, [], Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, expectedResult, [], Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, expectedResult, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync(url, string.Empty, expectedResult, filterFunc, [], Assembly.GetCallingAssembly(), string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync(url, string.Empty, expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, expectedResult, differenceFunc, [], Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, expectedResult, differenceFunc, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, string.Empty, expectedResult, [], callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, string.Empty, expectedResult, parameters, callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, string.Empty, expectedResult, differenceFunc, [], callingAssembly, string.Empty, expectedResultParameterName);
        }
        
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, string.Empty, expectedResult, differenceFunc, parameters, callingAssembly, string.Empty, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, [], Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, payloadAsJson, expectedResult, [], Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, payloadAsJson, expectedResult, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }


        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, [], callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, parameters, callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, [], callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, differenceFunc, [], callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, payloadAsObject.ToJson(), expectedResult, result => result, differenceFunc, parameters, callingAssembly, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, differenceFunc, [], callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return AssertPostAsync<TResult>(client, url, payloadAsJson, expectedResult, result => result, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync(url, payloadAsObject.ToJson(), expectedResult, filterFunc, [], Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync(url, payloadAsObject.ToJson(), expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync(url, payloadAsJson, expectedResult, filterFunc, [], Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }
        
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync(url, payloadAsJson, expectedResult, filterFunc, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, item => item, differenceFunc, [], Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, payloadAsObject.ToJson(), expectedResult, item => item, differenceFunc, parameters, Assembly.GetCallingAssembly(), payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, payloadAsJson, expectedResult, item => item, differenceFunc, [], Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, string Value)[] parameters,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertPostAsync<TResult>(url, payloadAsJson, expectedResult, item => item, differenceFunc, parameters, Assembly.GetCallingAssembly(), payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, difference => difference, [], payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, difference => difference, parameters, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, difference => difference, [], payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             (string Key, string Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, difference => difference, parameters, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, differenceFunc, [], payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertHttpCall(url, payloadAsObject.ToJson(), expectedResult, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, differenceFunc, parameters, payloadAsObjectParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, differenceFunc, [], payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, HttpExtensions.PostAsJsonStringAsync<TResult>, HttpMethod.Post, callingAssembly, differenceFunc, parameters, payloadAsJsonParameterName, expectedResultParameterName);
        }

        public static Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            return httpClient.AssertPostAsUnauthorizedAsync(url, null, []);
        }

        public static async Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
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

            var result = await httpClient.PostAsync(url, body.IsNull() ? null : new StringContent(newBody, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"POST {url}",
                Expected = HttpStatusCode.Unauthorized,
                Current = result.StatusCode
            }.ToIList();

            var table = ConsoleTable.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, errorOutput);
        }
    }

}
