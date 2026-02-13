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
                                             string url,
                                             bool writeResponse = false,
                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCallAsync(url,
                                         string.Empty,
                                         HttpMethod.Delete,
                                         [],
                                         Assembly.GetCallingAssembly(),
                                         string.Empty,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task AssertDeleteAsync(this HttpClient client,
                                             string url,
                                             (string Key, object? Value)[] parameters,
                                             bool writeResponse = false,
                                             [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCallAsync(url,
                                         string.Empty,
                                         HttpMethod.Delete,
                                         parameters,
                                         Assembly.GetCallingAssembly(),
                                         string.Empty,
                                         callerFilePath,
                                         true,
                                         writeResponse);
        }

        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                               string url,
                                                               string expectedResult,
                                                               bool writeResponse = false,
                                                               [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertDeleteAsync<TResult>(url,
                                                     expectedResult,
                                                     [],
                                                     Assembly.GetCallingAssembly(),
                                                     writeResponse,
                                                     expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                     callerFilePath);
        }

        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                               string url,
                                                               string expectedResult,
                                                               (string Key, object? Value)[] parameters,
                                                               bool writeResponse = false,
                                                               [CallerArgumentExpression(nameof(expectedResult))]
                                                               string expectedResultParameterName = "",
                                                               [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertDeleteAsync<TResult>(url,
                                                     expectedResult,
                                                     parameters,
                                                     Assembly.GetCallingAssembly(),
                                                     writeResponse,
                                                     expectedResultParameterName,
                                                     callerFilePath);
        }

        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                               string url,
                                                               string expectedResult,
                                                               Assembly callingAssembly,
                                                               bool writeResponse = false,
                                                               [CallerArgumentExpression(nameof(expectedResult))]
                                                               string expectedResultParameterName = "",
                                                               [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCallAsync<TResult>(url,
                                                  string.Empty,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Delete,
                                                  [],
                                                  callingAssembly,
                                                  string.Empty,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  true,
                                                  writeResponse);
        }

        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                               string url,
                                                               string expectedResult,
                                                               (string Key, object? Value)[] parameters,
                                                               Assembly callingAssembly,
                                                               bool writeResponse = false,
                                                               [CallerArgumentExpression(nameof(expectedResult))]
                                                               string expectedResultParameterName = "",
                                                               [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCallAsync<TResult>(url,
                                                  string.Empty,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Delete,
                                                  parameters,
                                                  callingAssembly,
                                                  string.Empty,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  true,
                                                  writeResponse);
        }

        public static async Task AssertDeleteAsUnauthorizedAsync(this HttpClient httpClient,
                                                                 string url)
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
