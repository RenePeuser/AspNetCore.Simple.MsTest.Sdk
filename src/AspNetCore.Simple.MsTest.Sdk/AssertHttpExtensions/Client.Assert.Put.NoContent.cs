using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Argument.Check;
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
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          bool skipEndpointValidation = false,
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          string payload,
                                          bool writeResponse = false,
                                          bool skipEndpointValidation = false,
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
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
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
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
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
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
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
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
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
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
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
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
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
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
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
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
                                          HttpStatusCode? expectedHttpStatusCode = null,
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
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        // ============================================================
        // Unauthorized tests
        // ============================================================

        public static Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                        string url,
                                                        bool writeResponse = false,
                                                        [CallerFilePath] string callerFilePath = "",
                                                        [CallerMemberName] string callerMemberName = "",
                                                        [CallerLineNumber] int callerLineNumber = 0)
        {
            return httpClient.AssertPutAsUnauthorizedAsync(url: url,
                                                           body: null,
                                                           parameters: [],
                                                           writeResponse: writeResponse,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static async Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                              string url,
                                                              object? body,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "",
                                                              [CallerLineNumber] int callerLineNumber = 0)
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

            Assert.That.AreEqual(HttpStatusCode.Unauthorized,
                                 result.StatusCode,
                                 because: $"PUT {url} was called with an invalid bearer token, so the endpoint has to reject it with 401 Unauthorized. Any other status code means the route can be reached without valid credentials.",
                                 fix: "Check that the endpoint is covered by [Authorize] (or an equivalent policy/authentication middleware) and that no [AllowAnonymous] on the action or controller overrides it.",
                                 expectedName: "HttpStatusCode.Unauthorized",
                                 actualName: "result.StatusCode",
                                 callerFilePath: callerFilePath,
                                 callerMemberName: callerMemberName,
                                 callerLineNumber: callerLineNumber);
        }
    }
}