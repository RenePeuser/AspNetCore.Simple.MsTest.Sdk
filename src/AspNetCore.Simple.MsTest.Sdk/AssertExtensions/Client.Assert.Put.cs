using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        // ============================================================
        // PUT with Response (TResult) - All overloads
        // ============================================================

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            bool writeResponse = false,
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync(url: url,
                                         payloadAsJson: string.Empty,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: string.Empty,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
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

            return client.AssertPutAsync(url: url,
                                         payloadAsJson: string.Empty,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         parameters: parameters,
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: string.Empty,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync<TResult>(url: url,
                                                  expectedResult: expectedResult,
                                                  differenceFunc: differenceFunc,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync<TResult>(url: url,
                                                  expectedResult: expectedResult,
                                                  differenceFunc: differenceFunc,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           expectedResult: expectedResult,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           expectedResult: expectedResult,
                                           differenceFunc: item => item,
                                           parameters: parameters,
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsJson: string.Empty,
                                           expectedResult: expectedResult,
                                           differenceFunc: differenceFunc,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: string.Empty,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsJson: string.Empty,
                                           expectedResult: expectedResult,
                                           differenceFunc: differenceFunc,
                                           parameters: parameters,
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: string.Empty,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
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

            return client.AssertPutAsync<TResult>(url: url,
                                                  payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult: expectedResult,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync<TResult>(url: url,
                                                  payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult: expectedResult,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync<TResult>(url: url,
                                                  payloadAsJson: payloadAsJson,
                                                  expectedResult: expectedResult,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync<TResult>(url: url,
                                                  payloadAsJson: payloadAsJson,
                                                  expectedResult: expectedResult,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
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
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsObject: payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult: expectedResult,
                                           filterFunc: result => result,
                                           differenceFunc: difference => difference,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsObjectParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsObject: payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult: expectedResult,
                                           filterFunc: result => result,
                                           differenceFunc: difference => difference,
                                           parameters: parameters,
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsObjectParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
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
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: result => result,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: result => result,
                                           parameters: parameters,
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult: expectedResult,
                                           filterFunc: result => result,
                                           differenceFunc: differenceFunc,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                           expectedResult: expectedResult,
                                           filterFunc: result => result,
                                           differenceFunc: differenceFunc,
                                           parameters: parameters,
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: payloadAsObjectParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: result => result,
                                           differenceFunc: differenceFunc,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           payloadAsJson: payloadAsJson,
                                           expectedResult: expectedResult,
                                           filterFunc: result => result,
                                           differenceFunc: differenceFunc,
                                           parameters: parameters,
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           payloadAsJsonParameterName: payloadAsJsonParameterName,
                                           expectedResultParameterName: expectedResultParameterName,
                                           skipEndpointValidation: skipEndpointValidation,
                                           callerFilePath: callerFilePath,
                                           callerMemberName: callerMemberName,
                                           callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
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

            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsObjectParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         parameters: parameters,
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsObjectParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsJson,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsJson,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         parameters: parameters,
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertPutAsync<TResult>(url: url,
                                                  payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult: expectedResult,
                                                  filterFunc: item => item,
                                                  differenceFunc: differenceFunc,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync<TResult>(url: url,
                                                  payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                  expectedResult: expectedResult,
                                                  filterFunc: item => item,
                                                  differenceFunc: differenceFunc,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string payloadAsJson,
                                                            string expectedResult,
                                                            Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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

            return client.AssertPutAsync<TResult>(url: url,
                                                  payloadAsJson: payloadAsJson,
                                                  expectedResult: expectedResult,
                                                  filterFunc: item => item,
                                                  differenceFunc: differenceFunc,
                                                  parameters: [],
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync<TResult>(url: url,
                                                  payloadAsJson: payloadAsJson,
                                                  expectedResult: expectedResult,
                                                  filterFunc: item => item,
                                                  differenceFunc: differenceFunc,
                                                  parameters: parameters,
                                                  callingAssembly: callingAssembly,
                                                  writeResponse: writeResponse,
                                                  payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                  expectedResultParameterName: expectedResultParameterName,
                                                  skipEndpointValidation: skipEndpointValidation,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         differenceFunc: difference => difference,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsObjectParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            object payloadAsObject,
                                                            string expectedResult,
                                                            Func<TResult?, TResult?> filterFunc,
                                                            (string Key, object? Value)[] parameters,
                                                            Assembly callingAssembly,
                                                            bool writeResponse = false,
                                                            [CallerArgumentExpression(nameof(payloadAsObject))]
                                                            string payloadAsJsonParameterName = "",
                                                            [CallerArgumentExpression(nameof(expectedResult))]
                                                            string expectedResultParameterName = "",
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         differenceFunc: difference => difference,
                                         parameters: parameters,
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsJson,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         differenceFunc: difference => difference,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsJson,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         differenceFunc: difference => difference,
                                         parameters: parameters,
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         differenceFunc: differenceFunc,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsObjectParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        // MAXIMUM OVERLOAD - Contains the core logic (object payload variant)
        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
                                                            bool skipEndpointValidation = false,
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Put,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              skipEndpointValidation: skipEndpointValidation,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return client.AssertPutAsync(url: url,
                                         payloadAsJson: payloadAsJson,
                                         expectedResult: expectedResult,
                                         filterFunc: filterFunc,
                                         differenceFunc: differenceFunc,
                                         parameters: [],
                                         callingAssembly: callingAssembly,
                                         writeResponse: writeResponse,
                                         payloadAsJsonParameterName: payloadAsJsonParameterName,
                                         expectedResultParameterName: expectedResultParameterName,
                                         skipEndpointValidation: skipEndpointValidation,
                                         callerFilePath: callerFilePath,
                                         callerMemberName: callerMemberName,
                                         callerLineNumber: callerLineNumber);
        }

        // MAXIMUM OVERLOAD - Contains the core logic (string payload variant)
        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
                                              isSuccessStatusCode: true,
                                              writeResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        // ============================================================
        // Complete Context API (Level 3) - Everything in Context!
        // ============================================================

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task<TResult> AssertPutAsync<TResult>(HttpAssertContext<TResult> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                      expectedResult: context.ExpectedObjectAsJson,
                                                      filterFunc: context.OrderFunc,
                                                      httpMethod: HttpMethod.Put,
                                                      differenceFunc: context.DifferenceFunc,
                                                      parameters: context.Parameters,
                                                      callingAssembly: context.CallingAssembly,
                                                      payloadAsJsonParameterName: context.PayloadParameterName,
                                                      expectedResultParameterName: context.ExpectedResultParameterName,
                                                      callerFilePath: context.CallerFilePath,
                                                      isSuccessStatusCode: true,
                                                      writeResponse: context.WriteResponse,
                                                      callerLineNumber: context.CallerLineNumber);
        }
    }
}