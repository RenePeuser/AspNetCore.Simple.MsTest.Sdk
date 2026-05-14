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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         parameters: [],
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                         callerFilePath: callerFilePath,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: payloadAsJson,
                                                         expectedResult: expectedResult,
                                                         parameters: [],
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         differenceFunc: differenceFunc,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: string.Empty,
                                                         expectedResult: expectedResult,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: string.Empty,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: string.Empty,
                                                         expectedResult: expectedResult,
                                                         filterFunc: item => item,
                                                         differenceFunc: differenceFunc,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: string.Empty,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                         expectedResult: expectedResult,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                         expectedResult: expectedResult,
                                                         differenceFunc: differenceFunc,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: payloadAsJson,
                                                         expectedResult: expectedResult,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: payloadAsJson,
                                                         expectedResult: expectedResult,
                                                         differenceFunc: differenceFunc,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                         expectedResult: expectedResult,
                                                         filterFunc: item => item,
                                                         differenceFunc: difference => difference,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                         expectedResult: expectedResult,
                                                         filterFunc: item => item,
                                                         differenceFunc: differenceFunc,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: payloadAsJson,
                                                         expectedResult: expectedResult,
                                                         filterFunc: item => item,
                                                         differenceFunc: difference => difference,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync<TResult>(url: url,
                                                         payloadAsJson: payloadAsJson,
                                                         expectedResult: expectedResult,
                                                         filterFunc: item => item,
                                                         differenceFunc: differenceFunc,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync(url: url,
                                                payloadAsJson: payloadAsJson,
                                                expectedResult: expectedResult,
                                                filterFunc: filterFunc,
                                                parameters: [],
                                                callingAssembly: callingAssembly,
                                                writeResponse: writeResponse,
                                                payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                expectedResultParameterName: expectedResultParameterName,
                                                callerFilePath: callerFilePath,
                                                callerMemberName: callerMemberName,
                                                callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsErrorAsync(url: url,
                                                payloadAsJson: payloadAsJson,
                                                expectedResult: expectedResult,
                                                filterFunc: filterFunc,
                                                parameters: parameters,
                                                callingAssembly: callingAssembly,
                                                writeResponse: writeResponse,
                                                payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                expectedResultParameterName: expectedResultParameterName,
                                                callerFilePath: callerFilePath,
                                                callerMemberName: callerMemberName,
                                                callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync(url: url,
                                                payloadAsJson: payloadAsJson,
                                                expectedResult: expectedResult,
                                                filterFunc: filterFunc,
                                                differenceFunc: difference => difference,
                                                parameters: [],
                                                callingAssembly: callingAssembly,
                                                writeResponse: writeResponse,
                                                payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                expectedResultParameterName: expectedResultParameterName,
                                                callerFilePath: callerFilePath,
                                                callerMemberName: callerMemberName,
                                                callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync(url: url,
                                                payloadAsJson: payloadAsJson,
                                                expectedResult: expectedResult,
                                                filterFunc: filterFunc,
                                                differenceFunc: difference => difference,
                                                parameters: parameters,
                                                callingAssembly: callingAssembly,
                                                writeResponse: writeResponse,
                                                payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                expectedResultParameterName: expectedResultParameterName,
                                                callerFilePath: callerFilePath,
                                                callerMemberName: callerMemberName,
                                                callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsErrorAsync(url: url,
                                                payloadAsJson: payloadAsJson,
                                                expectedResult: expectedResult,
                                                filterFunc: filterFunc,
                                                differenceFunc: differenceFunc,
                                                parameters: [],
                                                callingAssembly: callingAssembly,
                                                writeResponse: writeResponse,
                                                payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                expectedResultParameterName: expectedResultParameterName,
                                                callerFilePath: callerFilePath,
                                                callerMemberName: callerMemberName,
                                                callerLineNumber: callerLineNumber);
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
                                                                   bool skipEndpointValidation = false,
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
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: false,
                                              writeResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }
    }
}
