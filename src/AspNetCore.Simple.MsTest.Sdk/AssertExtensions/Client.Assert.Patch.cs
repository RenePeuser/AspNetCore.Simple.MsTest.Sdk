using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ConsoleTables;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertPatchAsync(this HttpClient client,
                                            string url,
                                            bool writeResponse = false,
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertHttpCallAsync(url,
                                              string.Empty,
                                              HttpMethod.Patch,
                                              [],
                                              Assembly.GetCallingAssembly(),
                                              string.Empty,
                                              callerFilePath,
                                              true,
                                              writeResponse,
                                              callerMemberName: callerMemberName);
        }

        public static Task AssertPatchAsync(this HttpClient client,
                                            string url,
                                            (string Key, object? Value)[] parameters,
                                            bool writeResponse = false,
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertHttpCallAsync(url,
                                              string.Empty,
                                              HttpMethod.Patch,
                                              parameters,
                                              Assembly.GetCallingAssembly(),
                                              string.Empty,
                                              callerFilePath,
                                              true,
                                              writeResponse,
                                              callerMemberName: callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              bool writeResponse = false,
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    expectedResult,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    expectedResult,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           expectedResult,
                                           filterFunc,
                                           difference => difference,
                                           [],
                                           Assembly.GetCallingAssembly(),
                                           writeResponse,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           expectedResult,
                                           filterFunc,
                                           difference => difference,
                                           parameters,
                                           Assembly.GetCallingAssembly(),
                                           writeResponse,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    expectedResult,
                                                    differenceFunc,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    expectedResult,
                                                    differenceFunc,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertHttpCallAsync<TResult>(url,
                                                       string.Empty,
                                                       expectedResult,
                                                       item => item,
                                                       HttpMethod.Patch,
                                                       [],
                                                       callingAssembly,
                                                       string.Empty,
                                                       expectedResultParameterName,
                                                       callerFilePath,
                                                       true,
                                                       writeResponse,
                                                       callerMemberName: callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertHttpCallAsync<TResult>(url,
                                                       string.Empty,
                                                       expectedResult,
                                                       item => item,
                                                       HttpMethod.Patch,
                                                       parameters,
                                                       callingAssembly,
                                                       string.Empty,
                                                       expectedResultParameterName,
                                                       callerFilePath,
                                                       true,
                                                       writeResponse,
                                                       callerMemberName: callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertHttpCallAsync<TResult>(url,
                                                       string.Empty,
                                                       expectedResult,
                                                       item => item,
                                                       HttpMethod.Patch,
                                                       differenceFunc,
                                                       [],
                                                       callingAssembly,
                                                       string.Empty,
                                                       expectedResultParameterName,
                                                       callerFilePath,
                                                       true,
                                                       writeResponse,
                                                       callerMemberName: callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertHttpCallAsync<TResult>(url,
                                                       string.Empty,
                                                       expectedResult,
                                                       item => item,
                                                       HttpMethod.Patch,
                                                       differenceFunc,
                                                       parameters,
                                                       callingAssembly,
                                                       string.Empty,
                                                       expectedResultParameterName,
                                                       callerFilePath,
                                                       true,
                                                       writeResponse,
                                                       callerMemberName: callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           expectedResult,
                                           filterFunc,
                                           differenceFunc,
                                           [],
                                           callingAssembly,
                                           writeResponse,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        // MAXIMUM OVERLOAD - Contains the core logic (no payload variant)
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: string.Empty,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Patch,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: string.Empty,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsObject.ToJson(JsonSerializerOptions),
                                                    expectedResult,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    payloadAsObjectParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsObject.ToJson(JsonSerializerOptions),
                                                    expectedResult,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    payloadAsObjectParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsJson,
                                                    expectedResult,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    payloadAsJsonParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsJson,
                                                    expectedResult,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    payloadAsJsonParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsObject.ToJson(JsonSerializerOptions),
                                             expectedResult,
                                             result => result,
                                             [],
                                             callingAssembly,
                                             writeResponse,
                                             payloadAsObjectParameterName,
                                             expectedResultParameterName,
                                             callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsObject.ToJson(JsonSerializerOptions),
                                             expectedResult,
                                             result => result,
                                             parameters,
                                             callingAssembly,
                                             writeResponse,
                                             payloadAsObjectParameterName,
                                             expectedResultParameterName,
                                             callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsJson,
                                             expectedResult,
                                             result => result,
                                             [],
                                             callingAssembly,
                                             writeResponse,
                                             payloadAsJsonParameterName,
                                             expectedResultParameterName,
                                             callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsJson,
                                             expectedResult,
                                             result => result,
                                             parameters,
                                             callingAssembly,
                                             writeResponse,
                                             payloadAsJsonParameterName,
                                             expectedResultParameterName,
                                             callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsObject.ToJson(JsonSerializerOptions),
                                             expectedResult,
                                             result => result,
                                             differenceFunc,
                                             [],
                                             callingAssembly,
                                             writeResponse,
                                             payloadAsObjectParameterName,
                                             expectedResultParameterName,
                                             callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsObject.ToJson(JsonSerializerOptions),
                                             expectedResult,
                                             result => result,
                                             differenceFunc,
                                             parameters,
                                             callingAssembly,
                                             writeResponse,
                                             payloadAsObjectParameterName,
                                             expectedResultParameterName,
                                             callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsJson,
                                             expectedResult,
                                             result => result,
                                             differenceFunc,
                                             [],
                                             callingAssembly,
                                             writeResponse,
                                             payloadAsJsonParameterName,
                                             expectedResultParameterName,
                                             callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return AssertPatchAsync<TResult>(client,
                                             url,
                                             payloadAsJson,
                                             expectedResult,
                                             result => result,
                                             differenceFunc,
                                             parameters,
                                             callingAssembly,
                                             writeResponse,
                                             payloadAsJsonParameterName,
                                             expectedResultParameterName,
                                             callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult,
                                           filterFunc,
                                           [],
                                           Assembly.GetCallingAssembly(),
                                           writeResponse,
                                           payloadAsObjectParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult,
                                           filterFunc,
                                           parameters,
                                           Assembly.GetCallingAssembly(),
                                           writeResponse,
                                           payloadAsObjectParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsJson,
                                           expectedResult,
                                           filterFunc,
                                           [],
                                           Assembly.GetCallingAssembly(),
                                           writeResponse,
                                           payloadAsJsonParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsJson,
                                           expectedResult,
                                           filterFunc,
                                           parameters,
                                           Assembly.GetCallingAssembly(),
                                           writeResponse,
                                           payloadAsJsonParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsObject.ToJson(JsonSerializerOptions),
                                                    expectedResult,
                                                    item => item,
                                                    differenceFunc,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    payloadAsObjectParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsObject.ToJson(JsonSerializerOptions),
                                                    expectedResult,
                                                    item => item,
                                                    differenceFunc,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    payloadAsObjectParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              string payloadAsJson,
                                                              string expectedResult,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsJson))]
                                                              string payloadAsJsonParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsJson,
                                                    expectedResult,
                                                    item => item,
                                                    differenceFunc,
                                                    [],
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    payloadAsJsonParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync<TResult>(url,
                                                    payloadAsJson,
                                                    expectedResult,
                                                    item => item,
                                                    differenceFunc,
                                                    parameters,
                                                    Assembly.GetCallingAssembly(),
                                                    writeResponse,
                                                    payloadAsJsonParameterName,
                                                    expectedResultParameterName,
                                                    callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult,
                                           filterFunc,
                                           difference => difference,
                                           [],
                                           callingAssembly,
                                           writeResponse,
                                           payloadAsObjectParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult,
                                           filterFunc,
                                           difference => difference,
                                           parameters,
                                           callingAssembly,
                                           writeResponse,
                                           payloadAsObjectParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsJson,
                                           expectedResult,
                                           filterFunc,
                                           difference => difference,
                                           [],
                                           callingAssembly,
                                           writeResponse,
                                           payloadAsJsonParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsJson,
                                           expectedResult,
                                           filterFunc,
                                           difference => difference,
                                           parameters,
                                           callingAssembly,
                                           writeResponse,
                                           payloadAsJsonParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult,
                                           filterFunc,
                                           differenceFunc,
                                           [],
                                           callingAssembly,
                                           writeResponse,
                                           payloadAsObjectParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        // MAXIMUM OVERLOAD - Contains the core logic (object payload variant)
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
                                                              string url,
                                                              object payloadAsObject,
                                                              string expectedResult,
                                                              Func<TResult?, TResult?> filterFunc,
                                                              Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                              (string Key, object? Value)[] parameters,
                                                              Assembly callingAssembly,
                                                              bool writeResponse = false,
                                                              [CallerArgumentExpression(nameof(payloadAsObject))]
                                                              string payloadAsObjectParameterName = "",
                                                              [CallerArgumentExpression(nameof(expectedResult))]
                                                              string expectedResultParameterName = "",
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Patch,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName);
        }

        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertPatchAsync(url,
                                           payloadAsJson,
                                           expectedResult,
                                           filterFunc,
                                           differenceFunc,
                                           [],
                                           callingAssembly,
                                           writeResponse,
                                           payloadAsJsonParameterName,
                                           expectedResultParameterName,
                                           callerFilePath, callerMemberName);
        }

        // MAXIMUM OVERLOAD - Contains the core logic (string payload variant)
        public static Task<TResult> AssertPatchAsync<TResult>(this HttpClient client,
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
                                                              [CallerFilePath] string callerFilePath = "",
                                                              [CallerMemberName] string callerMemberName = "")
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Patch,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName);
        }

        public static Task AssertPatchAsUnauthorizedAsync(this HttpClient httpClient,
                                                          string url)
        {
            return httpClient.AssertPatchAsUnauthorizedAsync(url, null);
        }

        public static async Task AssertPatchAsUnauthorizedAsync(this HttpClient httpClient,
                                                                string url,
                                                                object? body)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            using var stringContent = new StringContent(body.ToJson(JsonSerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

            var result = await httpClient.PatchAsync(url, body.IsNull() ? null : stringContent).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
                                {
                                    Request = $"PATCH {url}",
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
        public static Task AssertPatchAsync(HttpAssertContext<string> context)
        {
            return context.Client.AssertPatchAsync(context.Url,
                                                   context.Parameters,
                                                   context.WriteResponse,
                                                   context.CallerFilePath,
                                                   context.CallerMemberName);
        }

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task<TResult> AssertPatchAsync<TResult>(HttpAssertContext<TResult> context)
        {
            return context.Client.AssertPatchAsync(context.Url,
                                                   context.PayloadAsJson ?? string.Empty,
                                                   context.ExpectedObjectAsJson,
                                                   context.OrderFunc,
                                                   context.DifferenceFunc,
                                                   context.Parameters,
                                                   context.CallingAssembly,
                                                   context.WriteResponse,
                                                   context.PayloadParameterName,
                                                   context.ExpectedResultParameterName,
                                                   context.CallerFilePath,
                                                   context.CallerMemberName);
        }
    }
}
