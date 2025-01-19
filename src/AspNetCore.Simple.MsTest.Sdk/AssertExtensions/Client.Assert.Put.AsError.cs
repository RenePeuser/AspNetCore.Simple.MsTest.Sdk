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
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         [],
                                                         Assembly.GetCallingAssembly(),
                                                         expectedResult.Contains(".json") ? expectedResult : nameof(expectedResult),
                                                         callerFilePath,
                                                         writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsJson,
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Put,
                                                  [],
                                                  Assembly.GetCallingAssembly(),
                                                  payloadAsJsonParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         expectedResultParameterName,
                                                         callerFilePath,
                                                         writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         expectedResult,
                                                         differenceFunc,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         expectedResultParameterName,
                                                         callerFilePath,
                                                         writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         string.Empty,
                                                         expectedResult,
                                                         parameters,
                                                         callingAssembly,
                                                         string.Empty,
                                                         expectedResultParameterName,
                                                         callerFilePath,
                                                         writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         string.Empty,
                                                         expectedResult,
                                                         differenceFunc,
                                                         parameters,
                                                         callingAssembly,
                                                         string.Empty,
                                                         expectedResultParameterName,
                                                         callerFilePath,
                                                         writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   object payloadAsObject,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsObject.ToJson(),
                                                         expectedResult,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         payloadAsObjectParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath,
                                                         writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   object payloadAsObject,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsObject.ToJson(),
                                                         expectedResult,
                                                         differenceFunc,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         payloadAsObjectParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath,
                                                         writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsJson,
                                                         expectedResult,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         payloadAsJsonParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath,
                                                         writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertPutAsErrorAsync<TResult>(url,
                                                         payloadAsJson,
                                                         expectedResult,
                                                         differenceFunc,
                                                         parameters,
                                                         Assembly.GetCallingAssembly(),
                                                         payloadAsJsonParameterName,
                                                         expectedResultParameterName,
                                                         callerFilePath,
                                                         writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   object payloadAsObject,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsObject.ToJson(),
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Put,
                                                  parameters,
                                                  callingAssembly,
                                                  payloadAsObjectParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }
        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   object payloadAsObject,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(payloadAsObject))] string payloadAsObjectParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsObject.ToJson(),
                                                  expectedResult,
                                                  item => item,
                                                  HttpMethod.Put,
                                                  differenceFunc,
                                                  parameters,
                                                  callingAssembly,
                                                  payloadAsObjectParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }
        

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url,
                                                  payloadAsJson,
                                                  expectedResult,
                                                  item => item, 
                                                  HttpMethod.Put, 
                                                  parameters, 
                                                  callingAssembly, 
                                                  payloadAsJsonParameterName, 
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  false,
                                                  writeResponse);
        }

        public static Task<TResult> AssertPutAsErrorAsync<TResult>(this HttpClient client,
                                                                   string url,
                                                                   string payloadAsJson,
                                                                   string expectedResult,
                                                                   Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                   (string Key, object? Value)[] parameters,
                                                                   Assembly callingAssembly,
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool writeResponse = false)
        {
            return client.AssertHttpCall<TResult>(url, 
                                                  payloadAsJson, 
                                                  expectedResult, 
                                                  item => item, 
                                                  HttpMethod.Put, 
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
