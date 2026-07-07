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
        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                               string url,
                                                               TResult expectedResponse,
                                                               bool writeResponse = false,
                                                               bool skipEndpointValidation = false,
                                                               HttpStatusCode? expectedHttpStatusCode = null,
                                                               [CallerFilePath] string callerFilePath = "",
                                                               [CallerMemberName] string callerMemberName = "",
                                                               [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertDeleteAsync<TResult>(url: url,
                                                     expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                     parameters: [],
                                                     callingAssembly: callingAssembly,
                                                     writeResponse: writeResponse,
                                                     expectedResultParameterName: nameof(expectedResponse),
                                                     skipEndpointValidation: skipEndpointValidation,
                                                     expectedHttpStatusCode: expectedHttpStatusCode,
                                                     callerFilePath: callerFilePath,
                                                     callerMemberName: callerMemberName,
                                                     callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertDeleteAsync<TResult>(this HttpClient client,
                                                               string url,
                                                               TResult expectedResponse,
                                                               (string Key, object? Value)[] parameters,
                                                               bool writeResponse = false,
                                                               bool skipEndpointValidation = false,
                                                               HttpStatusCode? expectedHttpStatusCode = null,
                                                               [CallerFilePath] string callerFilePath = "",
                                                               [CallerMemberName] string callerMemberName = "",
                                                               [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertDeleteAsync<TResult>(url: url,
                                                     expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                     parameters: parameters,
                                                     callingAssembly: callingAssembly,
                                                     writeResponse: writeResponse,
                                                     expectedResultParameterName: nameof(expectedResponse),
                                                     skipEndpointValidation: skipEndpointValidation,
                                                     expectedHttpStatusCode: expectedHttpStatusCode,
                                                     callerFilePath: callerFilePath,
                                                     callerMemberName: callerMemberName,
                                                     callerLineNumber: callerLineNumber);
        }
    }
}