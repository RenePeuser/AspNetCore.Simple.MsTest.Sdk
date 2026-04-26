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
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   bool writeResponse = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         [],
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                   string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsJson,
                                                         expectedResult,
                                                         [],
                                                         callingAssembly,
                                                         writeResponse,
                                                         payloadAsJsonParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         differenceFunc,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         string.Empty,
                                                         expectedResult,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         string.Empty,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         string.Empty,
                                                         expectedResult,
                                                         item => item,
                                                         differenceFunc,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         string.Empty,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsObject.ToJson(JsonSerializerOptions),
                                                         expectedResult,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         payloadAsObjectParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsObject.ToJson(JsonSerializerOptions),
                                                         expectedResult,
                                                         differenceFunc,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         payloadAsObjectParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsJson,
                                                         expectedResult,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         payloadAsJsonParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsJson,
                                                         expectedResult,
                                                         differenceFunc,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         payloadAsJsonParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsObject.ToJson(JsonSerializerOptions),
                                                         expectedResult,
                                                         item => item,
                                                         difference => difference,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         payloadAsObjectParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsObject.ToJson(JsonSerializerOptions),
                                                         expectedResult,
                                                         item => item,
                                                         differenceFunc,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         payloadAsObjectParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsJson,
                                                         expectedResult,
                                                         item => item,
                                                         difference => difference,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         payloadAsJsonParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsJson,
                                                         expectedResult,
                                                         item => item,
                                                         differenceFunc,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         payloadAsJsonParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync(url,
                                                payloadAsJson,
                                                expectedResult,
                                                filterFunc,
                                                [],
                                                callingAssembly,
                                                writeResponse,
                                                payloadAsJsonParameterName,
                                                expectedResultParameterName,
                                                callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync(url,
                                                payloadAsJson,
                                                expectedResult,
                                                filterFunc,
                                                parameters,
                                                callingAssembly,
                                                writeResponse,
                                                payloadAsJsonParameterName,
                                                expectedResultParameterName,
                                                callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync(url,
                                                payloadAsJson,
                                                expectedResult,
                                                filterFunc,
                                                difference => difference,
                                                [],
                                                callingAssembly,
                                                writeResponse,
                                                payloadAsJsonParameterName,
                                                expectedResultParameterName,
                                                callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync(url,
                                                payloadAsJson,
                                                expectedResult,
                                                filterFunc,
                                                difference => difference,
                                                parameters,
                                                callingAssembly,
                                                writeResponse,
                                                payloadAsJsonParameterName,
                                                expectedResultParameterName,
                                                callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync(url,
                                                payloadAsJson,
                                                expectedResult,
                                                filterFunc,
                                                differenceFunc,
                                                [],
                                                callingAssembly,
                                                writeResponse,
                                                payloadAsJsonParameterName,
                                                expectedResultParameterName,
                                                callerFilePath, callerMemberName, callerLineNumber);
        }

        // MAXIMUM OVERLOAD
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
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
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Put,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: false,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }
    }
}
