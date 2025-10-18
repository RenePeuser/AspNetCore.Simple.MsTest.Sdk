using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string expectedResult,
                                                                    bool writeResponse = false,
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          expectedResult,
                                                          [],
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string expectedResult,
                                                                    (string Key, object? Value)[] parameters,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          expectedResult,
                                                          parameters,
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          expectedResult,
                                                          differenceFunc,
                                                          [],
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          expectedResult,
                                                          differenceFunc,
                                                          parameters,
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string expectedResult,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          string.Empty,
                                                          expectedResult,
                                                          [],
                                                          callingAssembly,
                                                          writeResponse,
                                                          string.Empty,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string expectedResult,
                                                                    (string Key, object? Value)[] parameters,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          string.Empty,
                                                          expectedResult,
                                                          parameters,
                                                          callingAssembly,
                                                          writeResponse,
                                                          string.Empty,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          string.Empty,
                                                          expectedResult,
                                                          differenceFunc,
                                                          [],
                                                          callingAssembly,
                                                          writeResponse,
                                                          string.Empty,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          string.Empty,
                                                          expectedResult,
                                                          differenceFunc,
                                                          parameters,
                                                          callingAssembly,
                                                          writeResponse,
                                                          string.Empty,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string expectedResult,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsObject))]
                                                                    string payloadAsObjectParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                                          expectedResult,
                                                          [],
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          payloadAsObjectParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string expectedResult,
                                                                    (string Key, object? Value)[] parameters,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsObject))]
                                                                    string payloadAsObjectParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                                          expectedResult,
                                                          parameters,
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          payloadAsObjectParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsObject))]
                                                                    string payloadAsObjectParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                                          expectedResult,
                                                          differenceFunc,
                                                          [],
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          payloadAsObjectParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsObject))]
                                                                    string payloadAsObjectParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                                          expectedResult,
                                                          differenceFunc,
                                                          parameters,
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          payloadAsObjectParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsJson,
                                                          expectedResult,
                                                          [],
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          payloadAsJsonParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    (string Key, object? Value)[] parameters,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsJson,
                                                          expectedResult,
                                                          parameters,
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          payloadAsJsonParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsJson,
                                                          expectedResult,
                                                          differenceFunc,
                                                          [],
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          payloadAsJsonParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsJson,
                                                          expectedResult,
                                                          differenceFunc,
                                                          parameters,
                                                          Assembly.GetCallingAssembly(),
                                                          writeResponse,
                                                          payloadAsJsonParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string expectedResult,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsObject))]
                                                                    string payloadAsObjectParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Post,
                                                  [],
                                                  callingAssembly,
                                                  payloadAsObjectParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string expectedResult,
                                                                    (string Key, object? Value)[] parameters,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsObject))]
                                                                    string payloadAsObjectParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Post,
                                                  parameters,
                                                  callingAssembly,
                                                  payloadAsObjectParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsObject))]
                                                                    string payloadAsObjectParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Post,
                                                  differenceFunc,
                                                  [],
                                                  callingAssembly,
                                                  payloadAsObjectParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    object payloadAsObject,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsObject))]
                                                                    string payloadAsObjectParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Post,
                                                  differenceFunc,
                                                  parameters,
                                                  callingAssembly,
                                                  payloadAsObjectParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsJson,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Post,
                                                  [],
                                                  callingAssembly,
                                                  payloadAsJsonParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    (string Key, object? Value)[] parameters,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsJson,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Post,
                                                  parameters,
                                                  callingAssembly,
                                                  payloadAsJsonParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsJson,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Post,
                                                  differenceFunc,
                                                  [],
                                                  callingAssembly,
                                                  payloadAsJsonParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsJson,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Post,
                                                  differenceFunc,
                                                  parameters,
                                                  callingAssembly,
                                                  payloadAsJsonParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }
    }
}
