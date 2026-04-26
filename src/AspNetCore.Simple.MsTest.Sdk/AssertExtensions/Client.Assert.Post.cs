using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Argument.Check;
using ConsoleTables;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           bool writeResponse = false,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPostAsync(url: url,
                                          payloadAsJson: string.Empty,
                                          writeResponse: writeResponse,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           string payloadAsJson,
                                           bool writeResponse = false,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              httpMethod: HttpMethod.Post,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: nameof(payloadAsJson),
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsync(this HttpClient client,
                                           string url,
                                           string payloadAsJson,
                                           (string Key, object? Value)[] parameters,
                                           bool writeResponse = false,
                                           [CallerArgumentExpression(nameof(payloadAsJson))]
                                           string payloadAsJsonParameterName = "",
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsJson,
                                              httpMethod: HttpMethod.Post,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        /// <summary>
        /// Clean API: POST with type parameter, no expectedResult - validates endpoint and ignores response.
        /// Useful when you only care about endpoint validation (correct response type) without comparing response content.
        /// </summary>
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             bool writeResponse = false,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: string.Empty,
                                                   expectedResult: string.Empty,
                                                   parameters: [],
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   ignoreResponse: true,
                                                   payloadAsJsonParameterName: string.Empty,
                                                   expectedResultParameterName: string.Empty,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResult,
                                                   parameters: parameters,
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   expectedResultParameterName: expectedResultParameterName,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: string.Empty,
                                          expectedResult: expectedResult,
                                          filterFunc: filterFunc,
                                          parameters: [],
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: string.Empty,
                                          expectedResultParameterName: expectedResultParameterName,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<TResult?, TResult?> filterFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url: url,
                                          payloadAsJson: string.Empty,
                                          expectedResult: expectedResult,
                                          filterFunc: filterFunc,
                                          parameters: parameters,
                                          callingAssembly: callingAssembly,
                                          writeResponse: writeResponse,
                                          payloadAsJsonParameterName: string.Empty,
                                          expectedResultParameterName: expectedResultParameterName,
                                          callerFilePath: callerFilePath,
                                          callerMemberName: callerMemberName,
                                          callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url: url,
                                                   expectedResult: expectedResult,
                                                   differenceFunc: differenceFunc,
                                                   parameters: [],
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   expectedResultParameterName: expectedResultParameterName,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             bool writeResponse,
                                                             bool ignoreResponse,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))]
                                                             string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: payloadAsJson,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Post,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writResponse: writeResponse,
                                                       ignoreResponse: ignoreResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Post,
                                                       parameters: [],
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writResponse: writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Post,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writResponse: writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             string expectedResult,
                                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             Assembly callingAssembly,
                                                             bool writeResponse = false,
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPostAsync<TResult>(url,
                                                   expectedResult,
                                                   differenceFunc,
                                                   [],
                                                   callingAssembly,
                                                   writeResponse,
                                                   expectedResultParameterName,
                                                   callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertHttpCallAsync<TResult>(url: url,
                                                       payloadAsJson: string.Empty,
                                                       expectedResult: expectedResult,
                                                       filterFunc: item => item,
                                                       httpMethod: HttpMethod.Post,
                                                       differenceFunc: differenceFunc,
                                                       parameters: parameters,
                                                       callingAssembly: callingAssembly,
                                                       payloadAsJsonParameterName: string.Empty,
                                                       expectedResultParameterName: expectedResultParameterName,
                                                       callerFilePath: callerFilePath,
                                                       isSuccessStatusCode: true,
                                                       writResponse: writeResponse,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
                                                             string url,
                                                             object payloadAsObject,
                                                             string expectedResult,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult: expectedResult,
                                                   parameters: [],
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsObjectParameterName,
                                                   expectedResultParameterName: expectedResultParameterName,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
                                                   payloadAsJson: payloadAsJson,
                                                   expectedResult: expectedResult,
                                                   parameters: [],
                                                   callingAssembly: callingAssembly,
                                                   writeResponse: writeResponse,
                                                   payloadAsJsonParameterName: payloadAsJsonParameterName,
                                                   expectedResultParameterName: expectedResultParameterName,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPostAsync<TResult>(client: client,
                                            url: url,
                                            payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                            expectedResult: expectedResult,
                                            filterFunc: result => result,
                                            parameters: [],
                                            callingAssembly: callingAssembly,
                                            writeResponse: writeResponse,
                                            payloadAsJsonParameterName: payloadAsObjectParameterName,
                                            expectedResultParameterName: expectedResultParameterName,
                                            callerFilePath: callerFilePath,
                                            callerMemberName: callerMemberName,
                                            callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
                                            url: url,
                                            payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                            expectedResult: expectedResult,
                                            filterFunc: result => result,
                                            parameters: parameters,
                                            callingAssembly: callingAssembly,
                                            writeResponse: writeResponse,
                                            payloadAsJsonParameterName: payloadAsObjectParameterName,
                                            expectedResultParameterName: expectedResultParameterName,
                                            callerFilePath: callerFilePath,
                                            callerMemberName: callerMemberName,
                                            callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPostAsync<TResult>(client: client,
                                            url: url,
                                            payloadAsJson: payloadAsJson,
                                            expectedResult: expectedResult,
                                            filterFunc: result => result,
                                            parameters: [],
                                            callingAssembly: callingAssembly,
                                            writeResponse: writeResponse,
                                            payloadAsJsonParameterName: payloadAsJsonParameterName,
                                            expectedResultParameterName: expectedResultParameterName,
                                            callerFilePath: callerFilePath,
                                            callerMemberName: callerMemberName,
                                            callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client: client,
                                            url: url,
                                            payloadAsJson: payloadAsJson,
                                            expectedResult: expectedResult,
                                            filterFunc: result => result,
                                            parameters: parameters,
                                            callingAssembly: callingAssembly,
                                            writeResponse: writeResponse,
                                            payloadAsJsonParameterName: payloadAsJsonParameterName,
                                            expectedResultParameterName: expectedResultParameterName,
                                            callerFilePath: callerFilePath,
                                            callerMemberName: callerMemberName,
                                            callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPostAsync<TResult>(client,
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
                                            callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client,
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
                                            callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPostAsync<TResult>(client,
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
                                            callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return AssertPostAsync<TResult>(client,
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
                                            callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          [],
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          parameters,
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync(url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync(url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsObject.ToJson(JsonSerializerOptions),
                                                   expectedResult,
                                                   item => item,
                                                   differenceFunc,
                                                   [],
                                                   callingAssembly,
                                                   writeResponse,
                                                   payloadAsObjectParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPostAsync<TResult>(url,
                                                   payloadAsJson,
                                                   expectedResult,
                                                   item => item,
                                                   differenceFunc,
                                                   [],
                                                   callingAssembly,
                                                   writeResponse,
                                                   payloadAsJsonParameterName,
                                                   expectedResultParameterName,
                                                   callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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

            return client.AssertPostAsync<TResult>(url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          difference => difference,
                                          [],
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          difference => difference,
                                          parameters,
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertPostAsync(url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertPostAsync(url,
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

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPostAsync(url,
                                          payloadAsObject.ToJson(JsonSerializerOptions),
                                          expectedResult,
                                          filterFunc,
                                          differenceFunc,
                                          [],
                                          callingAssembly,
                                          writeResponse,
                                          payloadAsObjectParameterName,
                                          expectedResultParameterName,
                                          callerFilePath, callerMemberName, callerLineNumber);
        }

        // MAXIMUM OVERLOAD - Contains the core logic (object payload variant)
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                                             [CallerMemberName] string callerMemberName = "",
                                                             [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: payloadAsObject.ToJson(JsonSerializerOptions),
                                              expectedResult: expectedResult,
                                              filterFunc: filterFunc,
                                              httpMethod: HttpMethod.Post,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsObjectParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
            return client.AssertPostAsync(url,
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

        // MAXIMUM OVERLOAD - Contains the core logic (string payload variant)
        public static Task<TResult> AssertPostAsync<TResult>(this HttpClient client,
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
                                              httpMethod: HttpMethod.Post,
                                              differenceFunc: differenceFunc,
                                              parameters: parameters,
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: payloadAsJsonParameterName,
                                              expectedResultParameterName: expectedResultParameterName,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
                                                         string url)
        {
            return httpClient.AssertPostAsUnauthorizedAsync(url, null, []);
        }

        public static Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
                                                         string url,
                                                         object? body)
        {
            return httpClient.AssertPostAsUnauthorizedAsync(url, body, []);
        }

        public static async Task AssertPostAsUnauthorizedAsync(this HttpClient httpClient,
                                                               string url,
                                                               object? body,
                                                               (string Key, object? Value)[] parameters)
        {
            Throw.IfNull(httpClient);
            Throw.IfNullOrWhiteSpace(url);
            Throw.IfNull(parameters);

            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var newBody = "";

            if (body.IsNotNull())
            {
                newBody = body.ToJson(JsonSerializerOptions).ResolveParameters(parameters);
            }

            using var stringContent = new StringContent(newBody, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await httpClient.PostAsync(url, body.IsNull() ? null : stringContent).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
            {
                Request = $"POST {url}",
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
        public static Task AssertPostAsync(HttpAssertContext<string> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                      httpMethod: HttpMethod.Post,
                                                      parameters: context.Parameters,
                                                      callingAssembly: context.CallingAssembly,
                                                      payloadAsJsonParameterName: context.PayloadParameterName,
                                                      callerFilePath: context.CallerFilePath,
                                                      isSuccessStatusCode: true,
                                                      writResponse: context.WriteResponse,
                                                      callerLineNumber: context.CallerLineNumber);
        }

        /// <summary>
        /// Level 3: Complete Context API - All parameters in context (cleanest API).
        /// </summary>
        public static Task<TResult> AssertPostAsync<TResult>(HttpAssertContext<TResult> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                      expectedResult: context.ExpectedObjectAsJson,
                                                      filterFunc: context.OrderFunc,
                                                      httpMethod: HttpMethod.Post,
                                                      differenceFunc: context.DifferenceFunc,
                                                      parameters: context.Parameters,
                                                      callingAssembly: context.CallingAssembly,
                                                      payloadAsJsonParameterName: context.PayloadParameterName,
                                                      expectedResultParameterName: context.ExpectedResultParameterName,
                                                      callerFilePath: context.CallerFilePath,
                                                      isSuccessStatusCode: true,
                                                      writResponse: context.WriteResponse,
                                                      callerLineNumber: context.CallerLineNumber);
        }
    }
}
