using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   TResult expectedResponse,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   HttpStatusCode? expectedHttpStatusCode = null,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                         parameters: [],
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: nameof(expectedResponse),
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                         parameters: parameters,
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: nameof(expectedResponse),
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   TResult expectedResponse,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   HttpStatusCode? expectedHttpStatusCode = null,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                         differenceFunc: differenceFunc,
                                                         parameters: [],
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: nameof(expectedResponse),
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         expectedHttpStatusCode: expectedHttpStatusCode,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   TResult expectedResponse,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   HttpStatusCode? expectedHttpStatusCode = null,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync(url: url,
                                                expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                filterFunc: filterFunc,
                                                parameters: [],
                                                callingAssembly: Assembly.GetCallingAssembly(),
                                                writeResponse: writeResponse,
                                                expectedResultParameterName: nameof(expectedResponse),
                                                skipEndpointValidation: skipEndpointValidation,
                                                expectedHttpStatusCode: expectedHttpStatusCode,
                                                callerFilePath: callerFilePath,
                                                callerMemberName: callerMemberName,
                                                callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   TResult expectedResponse,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   HttpStatusCode? expectedHttpStatusCode = null,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync(url: url,
                                                expectedResult: expectedResponse.ToJson(JsonSerializerOptions),
                                                filterFunc: filterFunc,
                                                differenceFunc: differenceFunc,
                                                parameters: [],
                                                callingAssembly: Assembly.GetCallingAssembly(),
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