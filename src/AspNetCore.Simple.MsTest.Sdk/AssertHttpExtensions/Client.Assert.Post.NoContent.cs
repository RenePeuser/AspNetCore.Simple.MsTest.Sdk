using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        // ============================================================
        // Basic overloads - No response body expected (204 No Content)
        // ============================================================

        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           bool writeResponse = false,
                                           bool skipEndpointValidation = false,
                                           HttpStatusCode? expectedHttpStatusCode = null,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPostAsync(url: url,
                                          payloadAsJson: string.Empty,
                                          writeResponse: writeResponse,
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           string payloadAsJson,
                                           bool writeResponse = false,
                                           bool skipEndpointValidation = false,
                                           HttpStatusCode? expectedHttpStatusCode = null,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              httpMethod: HttpMethod.Post,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: nameof(payloadAsJson),
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           string payloadAsJson,
                                           (string Key, object? Value)[] parameters,
                                           bool writeResponse = false,
                                           [CallerArgumentExpression(nameof(payloadAsJson))]
                                           string payloadAsJsonParameterName = "",
                                           bool skipEndpointValidation = false,
                                           HttpStatusCode? expectedHttpStatusCode = null,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              httpMethod: HttpMethod.Post,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
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

        public static Task AssertPostAsync(this HttpClient client,
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

            _ = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          parameters: [],
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: payloadAsObjectParameterName,
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsync(this HttpClient client,
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

            _ = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          parameters: parameters,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: payloadAsObjectParameterName,
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        // ============================================================
        // Assembly overloads
        // ============================================================

        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           Assembly callingAssembly,
                                           bool writeResponse = false,
                                           bool skipEndpointValidation = false,
                                           HttpStatusCode? expectedHttpStatusCode = null,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPostAsync(url: url,
                                          payloadAsJson: string.Empty,
                                          parameters: [],
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           string payloadAsJson,
                                           Assembly callingAssembly,
                                           bool writeResponse = false,
                                           [CallerArgumentExpression(nameof(payloadAsJson))]
                                           string payloadAsJsonParameterName = "",
                                           bool skipEndpointValidation = false,
                                           HttpStatusCode? expectedHttpStatusCode = null,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsJson,
                                          parameters: [],
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: payloadAsJsonParameterName,
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           string payloadAsJson,
                                           (string Key, object? Value)[] parameters,
                                           Assembly callingAssembly,
                                           bool writeResponse = false,
                                           [CallerArgumentExpression(nameof(payloadAsJson))]
                                           string payloadAsJsonParameterName = "",
                                           bool skipEndpointValidation = false,
                                           HttpStatusCode? expectedHttpStatusCode = null,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              httpMethod: HttpMethod.Post,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              skipEndpointValidation: skipEndpointValidation,
                                              expectedHttpStatusCode: expectedHttpStatusCode,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsync(this HttpClient client,
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
            return client.AssertPostAsync(url: url,
                                          payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                          parameters: [],
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: payloadAsObjectParameterName,
                                          skipEndpointValidation: skipEndpointValidation,
                                          expectedHttpStatusCode: expectedHttpStatusCode,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsync(this HttpClient client,
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
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                              httpMethod: HttpMethod.Post,
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
        // Complete Context API (Level 3) - NoContent variant
        // ============================================================

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task AssertPostAsync(HttpAssertContext<string> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                      httpMethod: HttpMethod.Post,
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