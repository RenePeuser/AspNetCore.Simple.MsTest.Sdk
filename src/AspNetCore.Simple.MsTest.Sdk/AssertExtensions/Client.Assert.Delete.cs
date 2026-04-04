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
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Delete,
                                                       differenceFunc: difference => difference,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writResponse: writeResponse);
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

        // ============================================================
        // Complete Context API (Level 3) - Everything in Context!
        // ============================================================

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task AssertDeleteAsync(HttpAssertContext<string> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: string.Empty,
                                                      httpMethod: HttpMethod.Delete,
                                                      parameters: context.Parameters,
                                                      callingAssembly: context.CallingAssembly,
                                                      payloadAsJsonParameterName: string.Empty,
                                                      callerFilePath: context.CallerFilePath,
                                                      isSuccessStatusCode: context.IsSuccessStatusCode,
                                                      writResponse: context.WriteResponse);
        }

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task<TResult> AssertDeleteAsync<TResult>(HttpAssertContext<TResult> context)
        {
            return context.Client.AssertHttpCallAsync<TResult>(url: context.Url,
                                                               payloadAsJson: string.Empty,
                                                               expectedResult: context.ExpectedObjectAsJson,
                                                               filterFunc: context.OrderFunc,
                                                               httpMethod: HttpMethod.Delete,
                                                               differenceFunc: context.DifferenceFunc,
                                                               parameters: context.Parameters,
                                                               callingAssembly: context.CallingAssembly,
                                                               payloadAsJsonParameterName: string.Empty,
                                                               expectedResultParameterName: context.ExpectedResultParameterName,
                                                               callerFilePath: context.CallerFilePath,
                                                               isSuccessStatusCode: context.IsSuccessStatusCode,
                                                               writResponse: context.WriteResponse);
        }
    }
}
