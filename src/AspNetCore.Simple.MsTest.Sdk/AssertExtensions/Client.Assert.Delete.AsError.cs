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
        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string expectedResult,
                                                                      [CallerFilePath] string callerFilePath = "",
                                                                      bool writeResponse = false)
        {
            return client.AssertDeleteAsErrorAsync<TResult>(url,
                                                            expectedResult,
                                                            Assembly.GetCallingAssembly(),
                                                            expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                            callerFilePath,
                                                            writeResponse);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string expectedResult,
                                                                      (string Key, object? Value)[] parameters,
                                                                      [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                      [CallerFilePath] string callerFilePath = "",
                                                                      bool writeResponse = false)
        {
            return client.AssertDeleteAsErrorAsync<TResult>(url,
                                                            expectedResult,
                                                            parameters,
                                                            Assembly.GetCallingAssembly(),
                                                            expectedResultParameterName,
                                                            callerFilePath,
                                                            writeResponse);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string expectedResult,
                                                                      Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                      [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                      [CallerFilePath] string callerFilePath = "",
                                                                      bool writeResponse = false)
        {
            return client.AssertDeleteAsErrorAsync<TResult>(url,
                                                            expectedResult,
                                                            differenceFunc,
                                                            Assembly.GetCallingAssembly(),
                                                            expectedResultParameterName,
                                                            callerFilePath,
                                                            writeResponse);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string expectedResult,
                                                                      Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                      (string Key, object? Value)[] parameters,
                                                                      [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                      [CallerFilePath] string callerFilePath = "",
                                                                      bool writeResponse = false)
        {
            return client.AssertDeleteAsErrorAsync<TResult>(url,
                                                            expectedResult,
                                                            differenceFunc,
                                                            parameters,
                                                            Assembly.GetCallingAssembly(),
                                                            expectedResultParameterName,
                                                            callerFilePath,
                                                            writeResponse);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string expectedResult,
                                                                      Assembly callingAssembly,
                                                                      [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                      [CallerFilePath] string callerFilePath = "",
                                                                      bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  string.Empty,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Delete,
                                                  [],
                                                  callingAssembly,
                                                  string.Empty,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string expectedResult,
                                                                      (string Key, object? Value)[] parameters,
                                                                      Assembly callingAssembly,
                                                                      [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                      [CallerFilePath] string callerFilePath = "",
                                                                      bool writeResponse = false)
        {
            return client.AssertDeleteAsErrorAsync<TResult>(url,
                                                            expectedResult,
                                                            item => item,
                                                            parameters,
                                                            callingAssembly,
                                                            expectedResultParameterName,
                                                            callerFilePath,
                                                            writeResponse);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string expectedResult,
                                                                      Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                      Assembly callingAssembly,
                                                                      [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                      [CallerFilePath] string callerFilePath = "",
                                                                      bool writeResponse = false)
        {
            return client.AssertDeleteAsErrorAsync<TResult>(url,
                                                            expectedResult,
                                                            differenceFunc,
                                                            [],
                                                            callingAssembly,
                                                            expectedResultParameterName,
                                                            callerFilePath,
                                                            writeResponse);
        }

        public static Task<TResult> AssertDeleteAsErrorAsync<TResult>(this HttpClient client,
                                                                      string url,
                                                                      string expectedResult,
                                                                      Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                      (string Key, object? Value)[] parameters,
                                                                      Assembly callingAssembly,
                                                                      [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                      [CallerFilePath] string callerFilePath = "",
                                                                      bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  string.Empty,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Delete,
                                                  parameters,
                                                  callingAssembly,
                                                  string.Empty,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }
    }
}
