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
using Argument.Check;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        // ============================================================
        // Basic overloads - No response body expected (204 No Content)
        // ============================================================

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          bool writeResponse = false,
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: string.Empty,
                                              httpMethod: HttpMethod.Put,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: string.Empty,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: string.Empty,
                                              httpMethod: HttpMethod.Put,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: string.Empty,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          string payload,
                                          bool writeResponse = false,
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payload,
                                              httpMethod: HttpMethod.Put,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: nameof(payload),
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          string payload,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          [CallerArgumentExpression(nameof(payload))]
                                          string payloadParameterName = "",
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payload,
                                              httpMethod: HttpMethod.Put,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        // ============================================================
        // Object payload overloads
        // ============================================================

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          object payloadAsObject,
                                          bool writeResponse = false,
                                          [CallerArgumentExpression(nameof(payloadAsObject))]
                                          string payloadAsObjectParameterName = "",
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              httpMethod: HttpMethod.Put,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          object payloadAsObject,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          [CallerArgumentExpression(nameof(payloadAsObject))]
                                          string payloadAsObjectParameterName = "",
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              httpMethod: HttpMethod.Put,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        // ============================================================
        // Assembly overloads
        // ============================================================

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          Assembly callingAssembly,
                                          bool writeResponse = false,
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: string.Empty,
                                              httpMethod: HttpMethod.Put,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: string.Empty,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          string payload,
                                          Assembly callingAssembly,
                                          bool writeResponse = false,
                                          [CallerArgumentExpression(nameof(payload))]
                                          string payloadParameterName = "",
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payload,
                                              httpMethod: HttpMethod.Put,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          string payload,
                                          (string Key, object? Value)[] parameters,
                                          Assembly callingAssembly,
                                          bool writeResponse = false,
                                          [CallerArgumentExpression(nameof(payload))]
                                          string payloadParameterName = "",
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payload,
                                              httpMethod: HttpMethod.Put,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          object payloadAsObject,
                                          Assembly callingAssembly,
                                          bool writeResponse = false,
                                          [CallerArgumentExpression(nameof(payloadAsObject))]
                                          string payloadAsObjectParameterName = "",
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              httpMethod: HttpMethod.Put,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          object payloadAsObject,
                                          (string Key, object? Value)[] parameters,
                                          Assembly callingAssembly,
                                          bool writeResponse = false,
                                          [CallerArgumentExpression(nameof(payloadAsObject))]
                                          string payloadAsObjectParameterName = "",
                                          bool skipEndpointValidation = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              httpMethod: HttpMethod.Put,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        // ============================================================
        // Unauthorized tests
        // ============================================================

        public static Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                        string url,
                                                        bool writeResponse = false)
        {
            return httpClient.AssertPutAsUnauthorizedAsync(url: url,
                                                           body: null,
                                                           parameters: [],
                                                           writeResponse: writeResponse);
        }

        public static async Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                              string url,
                                                              object? body,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false)
        {
            Throw.IfNull(httpClient);
            Throw.IfNullOrWhiteSpace(url);
            Throw.IfNull(parameters);

            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var newBody = "";

            if (body.IsNotNull())
            {
                newBody = body.ToJson(JsonSerializerOptions).ResolveParameters(parameters);
            }

            using var stringContent = new StringContent(content: newBody, encoding: Encoding.UTF8, mediaType: MediaTypeNames.Application.Json);
            var result = await httpClient.PutAsync(requestUri: url, content: body.IsNull() ? null : stringContent).ConfigureAwait(continueOnCapturedContext: false);

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

            Assert.AreEqual(expected: HttpStatusCode.Unauthorized, actual: result.StatusCode, message: errorOutput);
        }

        // ============================================================
        // Complete Context API (Level 3) - NoContent variant
        // ============================================================

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task AssertPutAsync(HttpAssertContext<string> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                      httpMethod: HttpMethod.Put,
                                                      parameters: context.Parameters,
                                                      callingAssembly: context.CallingAssembly,
                                                      payloadAsJsonParameterName: context.PayloadParameterName,
                                                      callerFilePath: context.CallerFilePath,
                                                      isSuccessStatusCode: true,
                                                      writeResponse: context.WriteResponse,
                                                      callerLineNumber: context.CallerLineNumber);
        }
    }
}
