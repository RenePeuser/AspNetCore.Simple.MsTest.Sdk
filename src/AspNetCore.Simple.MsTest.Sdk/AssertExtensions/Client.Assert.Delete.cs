using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertDeleteAsync(this HttpClient client,
                                             string url)
        {
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.DeleteAsync(urlParam), HttpMethod.Delete, [], Assembly.GetCallingAssembly());
        }
        
        public static Task AssertDeleteAsync(this HttpClient client,
                                             string url,
                                             (string Key, string Value)[] parameters)
        {
            return client.AssertHttpCall(url, string.Empty, (clientParam, urlParam, _) => clientParam.DeleteAsync(urlParam), HttpMethod.Delete, parameters, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                               string url,
                                                               string expectedResult,
                                                               [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertDeleteAsync<TResult>(url, expectedResult, [], Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                               string url,
                                                               string expectedResult,
                                                               (string Key, string Value)[] parameters,
                                                               [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertDeleteAsync<TResult>(url, expectedResult, parameters, Assembly.GetCallingAssembly(), expectedResultParameterName);
        }

        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                               string url,
                                                               string expectedResult,
                                                               Assembly callingAssembly,
                                                               [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, item => item, (httpClient, url, _) => HttpExtensions.DeleteAsAsync<TResult>(httpClient, url), HttpMethod.Delete, [],  callingAssembly, expectedResultParameterName);
        }

        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                      string url,
                                                      string expectedResult,
                                                      (string Key, string Value)[] parameters,
                                                      Assembly callingAssembly,
                                                      [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, string.Empty, expectedResult, item => item, (httpClient, url, _) => HttpExtensions.DeleteAsAsync<TResult>(httpClient, url), HttpMethod.Delete, parameters, callingAssembly, expectedResultParameterName);
        }

        public static async Task AssertDeleteAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"DELETE {url}",
                Expected = HttpStatusCode.Unauthorized,
                Current = result.StatusCode
            }.ToIList();

            var table = ConsoleTable.From(currentResult);
            var errorOutput = $"{Environment.NewLine}{Environment.NewLine}{table}";

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, errorOutput);
        }
    }
}
