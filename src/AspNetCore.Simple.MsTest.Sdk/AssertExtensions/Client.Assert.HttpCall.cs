using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Outputs;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        // This is only for dev who know what they are doing
        // With this method info you are able to intercept the existing assert functionality
        // to use external once
        public static MethodInfo? CustomAssertMethod { get; set; }

        // The base url of the running application. Mostly it will be https://localhost:5001/. Check your launchSettings.json
        public static string BaseUrl { get; set; } = "https://localhost:5001/";

        // Output function
        public static Action<string> LogAction { get; set; } = Console.WriteLine;

        private static readonly CurlFormatter CurlFormatter = new();
        private static readonly CurlPrinter CurlPrinter = new(CurlFormatter);
        private static readonly HttpOutputFormatter HttpOutputFormatter = new();

        // Here you can control the visibility of the token in the curl outputs.
        public static bool ShowTokenInCurl { get; set; }


        private static async Task AssertHttpCall(this HttpClient client,
                                                 string url,
                                                 string payloadAsJson,
                                                 Func<HttpClient, string, string, Task<HttpResponseMessage>> httpFunction,
                                                 HttpMethod httpMethod,
                                                 (string Key, object? Value)[] parameters,
                                                 Assembly callingAssembly,
                                                 [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "")
        {
            var jsonPayload = payloadAsJson.GetJsonString<object>(callingAssembly);
            jsonPayload = jsonPayload.ResolveParameters(parameters);

            var absoluteUrl = $"{BaseUrl}{url}";

            // Call as curl
            var curlBuilder = new CurlBuilder();
            var curl = curlBuilder.BuildFrom(httpMethod, absoluteUrl, payloadAsJson, client.DefaultRequestHeaders.Authorization, callingAssembly, ShowTokenInCurl);

            var httpResponse = await httpFunction(client, url, jsonPayload).ConfigureAwait(false);
            if (httpResponse.IsSuccessStatusCode)
            {
                CurlPrinter.PrintCurl(callingAssembly, curl);
                return;
            }

            var errorOutput = await GetOutputAsync(absoluteUrl, httpMethod, httpResponse, parameters, payloadAsJson, payloadAsJsonParameterName).ConfigureAwait(false);

            Assert.IsTrue(httpResponse.IsSuccessStatusCode, errorOutput);
            
            CurlPrinter.PrintCurl(callingAssembly, curl);


            static async Task<string> GetOutputAsync(string absoluteUrl,
                                                     HttpMethod httpMethod,
                                                     HttpResponseMessage httpResponseMessage,
                                                     (string Key, object? Value)[] parameters,
                                                     string payloadAsjson,
                                                     string payloadAsJsonParameterName)
            {
                var stringBuilder = new StringBuilder();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Error occured when calling endpoint");
                stringBuilder.AppendLine();

                // Table for call infos
                var callInfos = new
                {
                    HttpMethod = httpMethod.Method,
                    Url = absoluteUrl,
                    HttpStatusCode = httpResponseMessage.StatusCode.Cast<int>(),
                    HttpStatusName = httpResponseMessage.StatusCode
                }.ToIList();

                var table = ConsoleTable.From(callInfos).ToString();
                stringBuilder.AppendLine(table);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Payload: " + payloadAsJsonParameterName);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(payloadAsjson);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Error content:");

                var content = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                content = content.ResolveParameters(parameters);

                var formattedContent = JToken.Parse(content.IsNullOrEmpty() ? "{}" : content).ToString(Formatting.Indented);
                stringBuilder.AppendLine(formattedContent);

                return stringBuilder.ToString();
            }
        }

        private static Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<HttpClient, string, Assembly, Task<TResult>> httpFunction,
                                                             HttpMethod httpMethod,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, (client, path, _, assembly) => httpFunction(client, path, assembly), httpMethod, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        private static Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<HttpClient, string, Assembly, Task<TResult>> httpFunction,
                                                             HttpMethod httpMethod,
                                                             Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, (client, path, payloadAsJson, assembly) => httpFunction(client, path, assembly), httpMethod, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        private static Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             Func<HttpClient, string, string, Assembly, Task<TResult>> httpFunction,
                                                             HttpMethod httpMethod,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, httpFunction, httpMethod, difference => difference, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName);
        }

        private static async Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<TResult, TResult> filterFunc,
                                                                    Func<HttpClient, string, string, Assembly, Task<TResult>> httpFunction,
                                                                    HttpMethod httpMethod,
                                                                    Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    Assembly callingAssembly,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            if (CustomAssertMethod is not null)
            {
                return await AssertCustomHttpCall(client, url, payloadAsJson, expectedResult, filterFunc, httpFunction, httpMethod, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName).ConfigureAwait(false);
            }

            return await AssertHttpCallInternal(client, url, payloadAsJson, expectedResult, filterFunc, httpFunction, httpMethod, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName).ConfigureAwait(false);
        }

        private static async Task<TResult> AssertHttpCallInternal<TResult>(this HttpClient client,
                                                                            string url,
                                                                            string payloadAsJson,
                                                                            string expectedResult,
                                                                            Func<TResult, TResult> filterFunc,
                                                                            Func<HttpClient, string, string, Assembly, Task<TResult>> httpFunction,
                                                                            HttpMethod httpMethod,
                                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                            (string Key, object? Value)[] parameters,
                                                                            Assembly callingAssembly,
                                                                            [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            var jsonPayload = payloadAsJson.GetJsonString<TResult>(callingAssembly);
            jsonPayload = jsonPayload.ResolveParameters(parameters);

            var currentResult = await httpFunction(client, url, jsonPayload, callingAssembly).ConfigureAwait(false);

            if (currentResult.IsNotNull())
            {
                var currentResultAsJson = currentResult.ToJson();
                currentResultAsJson = currentResultAsJson.ResolveParameters(parameters);
                currentResult = currentResultAsJson.FromJsonStringAs<TResult>();
            }

            var absoluteUrl = $"{BaseUrl}{url}";
            // var httpCallInfo = $"{Environment.NewLine}Call: '{httpMethod} {absoluteUrl}' was not successful.";

            // Call as curl
            var curlBuilder = new CurlBuilder();
            var curl = curlBuilder.BuildFrom(httpMethod, absoluteUrl, payloadAsJson, client.DefaultRequestHeaders.Authorization, callingAssembly, ShowTokenInCurl);


            var httpCallInfo = HttpOutputFormatter.GetOutputString("Response does not match expected results.",
                                                                   httpMethod, 
                                                                   absoluteUrl);
            
            // New we print out also executed curl :) 
            Assert.That.ObjectsAreEqual(expectedResult, currentResult, filterFunc, httpCallInfo, callingAssembly, differenceFunc, curl, parameters, expectedResultParameterName, payloadAsJsonParameterName);
            
//             CurlPrinter.PrintCurl(callingAssembly, curl);
            
            return currentResult;
        }

        private static async Task<TResult> AssertCustomHttpCall<TResult>(this HttpClient client,
                                                                         string url,
                                                                         string payloadAsJson,
                                                                         string expectedResult,
                                                                         Func<TResult, TResult> filterFunc,
                                                                         Func<HttpClient, string, string, Assembly, Task<TResult>> httpFunction,
                                                                         HttpMethod httpMethod,
                                                                         Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                         (string Key, object? Value)[] parameters,
                                                                         Assembly callingAssembly,
                                                                         [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                         [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "")
        {
            if (CustomAssertMethod is null)
            {
                throw new InvalidOperationException("CustomAssertMethod have not to be null when calling AssertCustomHttpCall");
            }

            if (CustomAssertMethod.IsGenericMethod.IsFalse())
            {
                throw new InvalidOperationException("Assert delegate is not a generic method");
            }

            var genericMethod = CustomAssertMethod.MakeGenericMethod(typeof(TResult));
            var tasReturnType = genericMethod.Invoke(null, [client, url, payloadAsJson, expectedResult, filterFunc, httpFunction, httpMethod, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName]);
            if (tasReturnType is Task task)
            {
                await task.ConfigureAwait(false);

                var resultProperty = task.GetType().GetProperty("Result");
                if (resultProperty.IsNull())
                {
                    throw new InvalidOperationException("Property of Result from executing task was not found, please check that your method have a Task<T> that a result exists.");
                }

                var returnValue = resultProperty.GetValue(task);
                if (returnValue.IsNull())
                {
                    throw new InvalidOperationException("The return value of your async method was NULL which was unexpected, please check your code execution");
                }

                if (returnValue is TResult result)
                {
                    return result;
                }

                throw new InvalidOperationException($"The type of the return value from your {nameof(CustomAssertMethod)} is: {returnValue.GetType().Name} which does not expect type: {typeof(TResult).Name}");
            }

            throw new InvalidOperationException("Unknown result of invoked generic method");
        }
    }
}
