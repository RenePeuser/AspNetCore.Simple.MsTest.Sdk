using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   bool writeResponse = false,
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         [],
                                                         Assembly.GetCallingAssembly(),
                                                         writeResponse,
                                                         expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         differenceFunc,
                                                         [],
                                                         Assembly.GetCallingAssembly(),
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         differenceFunc,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         filterFunc,
                                                         [],
                                                         Assembly.GetCallingAssembly(),
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         filterFunc,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         filterFunc,
                                                         differenceFunc,
                                                         [],
                                                         Assembly.GetCallingAssembly(),
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         filterFunc,
                                                         differenceFunc,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         item => item,
                                                         difference => difference,
                                                         [],
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         item => item,
                                                         difference => difference,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         item => item,
                                                         differenceFunc,
                                                         [],
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
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
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         item => item,
                                                         differenceFunc,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         filterFunc,
                                                         difference => difference,
                                                         [],
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         filterFunc,
                                                         difference => difference,
                                                         parameters,
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertGetAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         filterFunc,
                                                         differenceFunc,
                                                         [],
                                                         callingAssembly,
                                                         writeResponse,
                                                         expectedResultParameterName,
                                                         callerFilePath);
        }

        // MAXIMUM OVERLOAD
        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "")
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: string.Empty,
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Get,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: string.Empty,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: false,
                                              writResponse: writeResponse);
        }
    }
}
