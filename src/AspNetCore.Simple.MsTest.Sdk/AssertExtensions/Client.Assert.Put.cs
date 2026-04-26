using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Argument.Check;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          bool writeResponse = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url: url,
                                              payloadAsJson: string.Empty,
                                              httpMethod: HttpMethod.Put,
                                              parameters: [],
                                              callingAssembly: callingAssembly,
                                              payloadAsJsonParameterName: string.Empty,
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url,
                                              string.Empty,
                                              HttpMethod.Put,
                                              parameters,
                                              callingAssembly,
                                              string.Empty,
                                              callerFilePath,
                                              true,
                                              writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          string payload,
                                          bool writeResponse = false,
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url,
                                              payload,
                                              HttpMethod.Put,
                                              [],
                                              callingAssembly,
                                              nameof(payload),
                                              callerFilePath,
                                              true,
                                              writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsync(this HttpClient client,
                                          string url,
                                          string payload,
                                          (string Key, object? Value)[] parameters,
                                          bool writeResponse = false,
                                          [CallerArgumentExpression(nameof(payload))]
                                          string payloadParameterName = "",
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertHttpCallAsync(url,
                                              payload,
                                              HttpMethod.Put,
                                              parameters,
                                              callingAssembly,
                                              payloadParameterName,
                                              callerFilePath,
                                              true,
                                              writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string expectedResult,
                                                            bool writeResponse = false,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync<TResult>(url,
                                                  expectedResult,
                                                  parameters,
                                                  callingAssembly,
                                                  writeResponse,
                                                  expectedResultParameterName,
                                                  callerFilePath, callerMemberName, callerLineNumber);
        }

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync(url: url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync(url: url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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

            return client.AssertPutAsync<TResult>(url: url,
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

        public static Task<TResult> AssertPutAsync<TResult>(this HttpClient client,
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
            return AssertPutAsync<TResult>(client: client,
                                           url: url,
                                           expectedResult: expectedResult,
                                           parameters: [],
                                           callingAssembly: callingAssembly,
                                           writeResponse: writeResponse,
                                           expectedResultParameterName: expectedResultParameterName,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPutAsync<TResult>(client,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPutAsync<TResult>(client,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPutAsync<TResult>(client,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return AssertPutAsync<TResult>(client,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync<TResult>(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync<TResult>(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync<TResult>(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            return client.AssertPutAsync<TResult>(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsync(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsync(url,
                                         payloadAsObject.ToJson(JsonSerializerOptions),
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsync(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsync(url,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsync(url,
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
                                              callerFilePath: callerFilePath,
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
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
                                                            [CallerFilePath] string callerFilePath = "",
                                                            [CallerMemberName] string callerMemberName = "",
                                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            return client.AssertPutAsync(url,
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
                                              isSuccessStatusCode: true,
                                              writResponse: writeResponse,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber);
        }

        public static Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                        string url,
                                                        bool writeResponse = false)
        {
            return httpClient.AssertPutAsUnauthorizedAsync(url,
                                                           null,
                                                           [],
                                                           writeResponse);
        }

        public static async Task AssertPutAsUnauthorizedAsync(this HttpClient httpClient,
                                                              string url,
                                                              object? body,
                                                              (string Key, object? Value)[] parameters,
                                                              bool writeResponse = false)
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
            var result = await httpClient.PutAsync(url, body.IsNull() ? null : stringContent).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            var currentResult = new
                                {
                                    Request = $"PUT {url}",
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
        public static Task AssertPutAsync(HttpAssertContext<string> context)
        {
            return context.Client.AssertHttpCallAsync(url: context.Url,
                                                      payloadAsJson: context.PayloadAsJson ?? string.Empty,
                                                      httpMethod: HttpMethod.Put,
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
                                                      writResponse: context.WriteResponse,
                                                      callerLineNumber: context.CallerLineNumber);
        }
    }
}
