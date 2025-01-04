using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Outputs;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        private static readonly CurlPrinter CurlPrinter = new CurlPrinter(CurlFormatter);
        private static readonly HttpOutputFormatter HttpOutputFormatter = new();
        private static readonly CurlBuilder CurlBuilder = new();

        private static readonly JsonSerializerOptions SerializeOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { new JsonStringEnumConverter() }
        };

        private static readonly HttpCallHandler HttpCallHandler = new HttpCallHandler(new HttpRequestMessageBuilder(new Serializer.Json.JsonSerializer(SerializeOptions)));

        // Here you can control the visibility of the token in the curl outputs.
        public static bool ShowTokenInCurl { get; set; }


        private static async Task AssertHttpCall(this HttpClient client,
                                                 string url,
                                                 string payloadAsJson,
                                                 HttpMethod httpMethod,
                                                 (string Key, object? Value)[] parameters,
                                                 Assembly callingAssembly,
                                                 [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                 bool isSuccessStatusCode = true)
        {

            await client.AssertHttpCall<object>(url, payloadAsJson, string.Empty, item => item, httpMethod, parameters, callingAssembly, payloadAsJsonParameterName, string.Empty, isSuccessStatusCode).ConfigureAwait(false);
        }

        private static Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             HttpMethod httpMethod,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                             bool isSuccessStatusCode = true)
        {
            return client.AssertHttpCall(url, payloadAsJson, expectedResult, filterFunc, httpMethod, difference => difference, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName, isSuccessStatusCode);
        }

        private static async Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                                    string url,
                                                                    string payloadAsJson,
                                                                    string expectedResult,
                                                                    Func<TResult, TResult> filterFunc,
                                                                    HttpMethod httpMethod,
                                                                    Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                    (string Key, object? Value)[] parameters,
                                                                    Assembly callingAssembly,
                                                                    [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                    [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                    bool isSuccessStatusCode = true)
        {
            if (CustomAssertMethod is not null)
            {
                return await AssertCustomHttpCall(client, url, payloadAsJson, expectedResult, filterFunc, httpMethod, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName, isSuccessStatusCode).ConfigureAwait(false);
            }

            return await AssertHttpCallInternal(client, url, payloadAsJson, expectedResult, filterFunc, httpMethod, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName, isSuccessStatusCode).ConfigureAwait(false);
        }

        private static async Task<TResult> AssertHttpCallInternal<TResult>(this HttpClient client,
                                                                            string url,
                                                                            string payloadAsJson,
                                                                            string expectedResult,
                                                                            Func<TResult, TResult> filterFunc,
                                                                            HttpMethod httpMethod,
                                                                            Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                            (string Key, object? Value)[] parameters,
                                                                            Assembly callingAssembly,
                                                                            [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                            [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                            bool isSuccessStatusCode = true)
        {
            // 1. Setup json payload
            var jsonPayload = payloadAsJson.GetJsonString<TResult>(callingAssembly);

            // 2. Resolve parameters if parameterized payload
            jsonPayload = jsonPayload.ResolveParameters(parameters);

            // 3. Call the endpoint
            using var httpResponseMessage = await HttpCallHandler.CallAsync(client, httpMethod, url, jsonPayload, CancellationToken.None, payloadAsJsonParameterName).ConfigureAwait(false);

            // 4. Get the response as json
            var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            // 5. Resolve parameters in response json
            var resolvedParametersJsonString = contentAsString.ResolveParameters(parameters);

            // 6. Deserialized target type
            var currentResult = resolvedParametersJsonString.FromJsonStringAs<TResult>();

            var filteredCurrentResult = filterFunc(currentResult);

            // 7. Build up absolute url for nice test results
            var absoluteUrl = $"{BaseUrl}{url}";

            // 8. Build curl
            var curl = CurlBuilder.BuildFrom(httpMethod, absoluteUrl, payloadAsJson, client.DefaultRequestHeaders.Authorization, callingAssembly, ShowTokenInCurl);
            var curl2 = CurlBuilder.BuildFrom(httpResponseMessage, payloadAsJson, client.DefaultRequestHeaders.Authorization, callingAssembly, ShowTokenInCurl);

            // 9. Format the output string for best readable and understandable test results
            var httpCallInfo = HttpOutputFormatter.GetOutputString("Http call infos:",
                                                                   httpMethod,
                                                                   absoluteUrl,
                                                                   httpResponseMessage.StatusCode);

            // 10. Simplify the response message
            var simpleHttpResponse = httpResponseMessage.ToJson().FromJsonStringAs<SimpleHttpResponseMessage>();
            var resolvedSimpleHttpResponse = simpleHttpResponse with
            {
                Content = new SimpleHttpContent()
                {
                    Headers = httpResponseMessage.Content.Headers.ToJson().FromJsonStringAs<IImmutableList<KeyValuePair<string, IImmutableList<string>>>>(),
                    Value = filteredCurrentResult
                }
            };


            var expectedResultAsJson = expectedResult.GetJsonString<TResult>(callingAssembly);
            var expectedResultAsJsonParamterized = expectedResultAsJson.ResolveParameters(parameters);
            var expectedType = typeof(TResult).IsTypeOf<string>() ? (TResult)(object)expectedResultAsJsonParamterized : expectedResultAsJsonParamterized.FromJsonStringAs<TResult>();
            var filteredExpectedType = filterFunc(expectedType);


            var expectedResultAsSimpleResponse = expectedResultAsJsonParamterized.FromJsonStringOrDefault<SimpleHttpResponseMessage>();

            // This means this is legacy code in which only the response json exists
            if (expectedResultAsSimpleResponse.IsNull() || expectedResultAsSimpleResponse.Content.IsNull())
            {
                expectedResultAsSimpleResponse = simpleHttpResponse with
                {
                    Content = new SimpleHttpContent()
                    {
                        Headers = httpResponseMessage.Content.Headers.ToJson().FromJsonStringAs<IImmutableList<KeyValuePair<string, IImmutableList<string>>>>(),
                        Value = filteredExpectedType
                    }
                };
            }

            // To keep legacy code compatible
            expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { IsSuccessStatusCode = isSuccessStatusCode };


            // 13. Compare the expected results and more - just response
            Assert.That.ObjectsAreEqual<SimpleHttpResponseMessage>(expectedResultAsSimpleResponse.ToJson(), resolvedSimpleHttpResponse, item => item, httpCallInfo, callingAssembly, differenceFunc, curl, parameters, expectedResultParameterName, "CurrentResult");

            return currentResult;
        }

        internal sealed record SimpleHttpResponseMessage
        {
            public string Version { get; init; } = "1.1";
            public SimpleHttpContent? Content { get; init; }
            public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
            public string ReasonPhrase { get; init; } = "OK";
            public IImmutableList<KeyValuePair<string, IImmutableList<string>>> Headers { get; init; } = ImmutableList<KeyValuePair<string, IImmutableList<string>>>.Empty;
            public IImmutableList<KeyValuePair<string, IImmutableList<string>>> TrailingHeaders { get; init; } = ImmutableList<KeyValuePair<string, IImmutableList<string>>>.Empty;
            public bool IsSuccessStatusCode { get; init; } = true;
        }

        internal sealed record SimpleHttpContent
        {
            public IImmutableList<KeyValuePair<string, IImmutableList<string>>> Headers { get; init; } = ImmutableList<KeyValuePair<string, IImmutableList<string>>>.Empty;
            public object? Value { get; init; } = string.Empty;
        }

        private static async Task<TResult> AssertCustomHttpCall<TResult>(this HttpClient client,
                                                                         string url,
                                                                         string payloadAsJson,
                                                                         string expectedResult,
                                                                         Func<TResult, TResult> filterFunc,
                                                                         HttpMethod httpMethod,
                                                                         Func<IImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                                                         (string Key, object? Value)[] parameters,
                                                                         Assembly callingAssembly,
                                                                         [CallerArgumentExpression(nameof(payloadAsJson))] string payloadAsJsonParameterName = "",
                                                                         [CallerArgumentExpression(nameof(expectedResult))] string expectedResultParameterName = "",
                                                                         bool isSuccessStatusCode = true)
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
            var tasReturnType = genericMethod.Invoke(null, [client, url, payloadAsJson, expectedResult, filterFunc, httpMethod, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName, isSuccessStatusCode]);
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
