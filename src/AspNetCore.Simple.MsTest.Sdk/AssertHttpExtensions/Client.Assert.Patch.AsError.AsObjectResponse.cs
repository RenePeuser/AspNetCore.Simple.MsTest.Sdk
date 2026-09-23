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
        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     object payloadAsObject,
                                                                     TResult expectedResponse,
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

            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                           expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                           parameters: [],
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                           expectedResultParameterName: nameof(expectedResponse),
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPatchAsErrorAsync<TResult>(this HttpClient client,
                                                                     string url,
                                                                     string payloadAsJson,
                                                                     TResult expectedResponse,
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

            return client.AssertPatchAsErrorAsync<TResult>(url: url,
                                                           payloadAsJson: payloadAsJson,
                                                           expectedResult: expectedResponse.ToJson(JsonSerializerOptionsFor(callingAssembly)),
                                                           parameters: [],
                                                           callingAssembly: callingAssembly,
                                                           writeResponse: writeResponse,
                                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                           expectedResultParameterName: nameof(expectedResponse),
                                                           skipEndpointValidation: skipEndpointValidation,
                                                           expectedHttpStatusCode: expectedHttpStatusCode,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber);
        }
    }
}