using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         parameters: [],
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         parameters: parameters,
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         differenceFunc: differenceFunc,
                                                         parameters: [],
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         differenceFunc: differenceFunc,
                                                         parameters: parameters,
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: filterFunc,
                                                         parameters: [],
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: filterFunc,
                                                         parameters: parameters,
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: filterFunc,
                                                         differenceFunc: differenceFunc,
                                                         parameters: [],
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: filterFunc,
                                                         differenceFunc: differenceFunc,
                                                         parameters: parameters,
                                                         callingAssembly: Assembly.GetCallingAssembly(),
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: item => item,
                                                         differenceFunc: difference => difference,
                                                         parameters: [],
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: item => item,
                                                         differenceFunc: difference => difference,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: item => item,
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

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: item => item,
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

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: filterFunc,
                                                         differenceFunc: difference => difference,
                                                         parameters: [],
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: filterFunc,
                                                         differenceFunc: difference => difference,
                                                         parameters: parameters,
                                                         callingAssembly: callingAssembly,
                                                         writeResponse: writeResponse,
                                                         expectedResultParameterName: expectedResultParameterName,
                                                         skipEndpointValidation: skipEndpointValidation,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertGetAsErrorAsync<TResult>(url: url,
                                                         expectedResult: expectedResult,
                                                         filterFunc: filterFunc,
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

        // MAXIMUM OVERLOAD
        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<TResult?, TResult?> filterFunc,
                                                                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   bool writeResponse = false,
                                                                   bool skipEndpointValidation = false,
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   [CallerMemberName] string callerMemberName = "",
                                                                   [CallerLineNumber] int callerLineNumber = 0)
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
                                              skipEndpointValidation: skipEndpointValidation,
                                              isSuccessStatusCode: false,
                                              writeResponse: writeResponse,
                                              callerFilePath: callerFilePath,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }
    }
}
