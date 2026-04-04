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
                                                          item => item,
                                                          difference => difference,
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
                                                          item => item,
                                                          difference => difference,
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
                                                          item => item,
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
                                                          item => item,
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
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsJson,
                                                          expectedResult,
                                                          parameters,
                                                          callingAssembly,
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
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                                          expectedResult,
                                                          item => item,
                                                          difference => difference,
                                                          [],
                                                          callingAssembly,
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
                                                                    Assembly callingAssembly,
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
                                                          item => item,
                                                          difference => difference,
                                                          parameters,
                                                          callingAssembly,
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
                                                                    Assembly callingAssembly,
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
                                                          item => item,
                                                          differenceFunc,
                                                          [],
                                                          callingAssembly,
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
                                                                    Assembly callingAssembly,
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
                                                          item => item,
                                                          differenceFunc,
                                                          parameters,
                                                          callingAssembly,
                                                          writeResponse,
                                                          payloadAsObjectParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
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
            return client.AssertPostAsErrorAsync<TResult>(url,
                                                          payloadAsJson,
                                                          expectedResult,
                                                          item => item,
                                                          difference => difference,
                                                          [],
                                                          callingAssembly,
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
                                                                    Assembly callingAssembly,
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
                                                          item => item,
                                                          difference => difference,
                                                          parameters,
                                                          callingAssembly,
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
                                                                    Assembly callingAssembly,
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
                                                          item => item,
                                                          differenceFunc,
                                                          [],
                                                          callingAssembly,
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
                                                                    Assembly callingAssembly,
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
                                                          item => item,
                                                          differenceFunc,
                                                          parameters,
                                                          callingAssembly,
                                                          writeResponse,
                                                          payloadAsJsonParameterName,
                                                          expectedResultParameterName,
                                                          callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<TResult?, TResult?> filterFunc,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync(url,
                                                 payloadAsJson,
                                                 expectedResult,
                                                 filterFunc,
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
                                                                    Func<TResult?, TResult?> filterFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync(url,
                                                 payloadAsJson,
                                                 expectedResult,
                                                 filterFunc,
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
                                                                    Func<TResult?, TResult?> filterFunc,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync(url,
                                                 payloadAsJson,
                                                 expectedResult,
                                                 filterFunc,
                                                 difference => difference,
                                                 [],
                                                 callingAssembly,
                                                 writeResponse,
                                                 payloadAsJsonParameterName,
                                                 expectedResultParameterName,
                                                 callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<TResult?, TResult?> filterFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync(url,
                                                 payloadAsJson,
                                                 expectedResult,
                                                 filterFunc,
                                                 difference => difference,
                                                 parameters,
                                                 callingAssembly,
                                                 writeResponse,
                                                 payloadAsJsonParameterName,
                                                 expectedResultParameterName,
                                                 callerFilePath);
        }

        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<TResult?, TResult?> filterFunc,
                                                                    Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    Assembly callingAssembly,
                                                                    bool writeResponse = false,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                    string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))]
                                                                    string expectedResultParameterName = "",
                                                                    [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertPostAsErrorAsync(url,
                                                 payloadAsJson,
                                                 expectedResult,
                                                 filterFunc,
                                                 differenceFunc,
                                                 [],
                                                 callingAssembly,
                                                 writeResponse,
                                                 payloadAsJsonParameterName,
                                                 expectedResultParameterName,
                                                 callerFilePath);
        }

        // MAXIMUM OVERLOAD
        public static Task<TResult> AssertPostAsErrorAsync<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<TResult?, TResult?> filterFunc,
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
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Post,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: false,
                                              writResponse: writeResponse);
        }
    }
}
