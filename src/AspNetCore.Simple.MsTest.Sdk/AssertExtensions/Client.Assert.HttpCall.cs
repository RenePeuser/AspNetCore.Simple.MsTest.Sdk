using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using JsonSerializer = AspNetCore.Simple.MsTest.Sdk.Serializer.Json.JsonSerializer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        private static readonly HttpOutputFormatter HttpOutputFormatter = new();

        private static readonly CurlBuilder CurlBuilder = new();

        private static readonly JsonSerializerOptions SerializeOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { new JsonStringEnumConverter() }
        };

        private static readonly HttpCallHandler HttpCallHandler = new(new HttpRequestMessageBuilder(new JsonSerializer(SerializeOptions)));

        private static readonly EmbeddedFileLocalizer embeddedFileLocalizer = new(new TestCreatorSettings());

        private static readonly PrimitiveTypeConverter PrimitiveTypeConverter = new();

        // This is only for dev who know what they are doing
        // With this method info you are able to intercept the existing assert functionality
        // to use external once
        public static MethodInfo? CustomAssertMethod { get; set; }

        // Output function
        public static Action<string> LogAction { get; set; } = Console.WriteLine;

        // Here you can control the visibility of the token in the curl outputs.
        public static bool ShowTokenInCurl { get; set; }

        // Quickfix to hold the whole api compatible
        internal static readonly string IgnoreResponseComparison = "IgnoreResponse";

        private static async Task AssertHttpCall(this HttpClient client,
                                                 string url,
                                                 string payloadAsJson,
                                                 HttpMethod httpMethod,
                                                 (string Key, object? Value)[] parameters,
                                                 Assembly callingAssembly,
                                                 [CallerArgumentExpression(nameof(payloadAsJson))]
                                                 string payloadAsJsonParameterName = "",
                                                 [CallerFilePath] string callerFilePath = "",
                                                 bool isSuccessStatusCode = true,
                                                 bool writResponse = false)
        {
            await client.AssertHttpCall<string>(url,
                                                payloadAsJson,
                                                IgnoreResponseComparison,
                                                item => item,
                                                httpMethod,
                                                parameters,
                                                callingAssembly,
                                                payloadAsJsonParameterName,
                                                string.Empty,
                                                callerFilePath,
                                                isSuccessStatusCode,
                                                writResponse).ConfigureAwait(false);
        }

        private static Task<TResult> AssertHttpCall<TResult>(this HttpClient client,
                                                             string url,
                                                             string payloadAsJson,
                                                             string expectedResult,
                                                             Func<TResult, TResult> filterFunc,
                                                             HttpMethod httpMethod,
                                                             (string Key, object? Value)[] parameters,
                                                             Assembly callingAssembly,
                                                             [CallerArgumentExpression(nameof(payloadAsJson))]
                                                             string payloadAsJsonParameterName = "",
                                                             [CallerArgumentExpression(nameof(expectedResult))]
                                                             string expectedResultParameterName = "",
                                                             [CallerFilePath] string callerFilePath = "",
                                                             bool isSuccessStatusCode = true,
                                                             bool writResponse = false)
        {
            return client.AssertHttpCall(url,
                                         payloadAsJson,
                                         expectedResult,
                                         filterFunc,
                                         httpMethod,
                                         difference => difference,
                                         parameters,
                                         callingAssembly,
                                         payloadAsJsonParameterName,
                                         expectedResultParameterName,
                                         callerFilePath,
                                         isSuccessStatusCode,
                                         writResponse);
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
                                                                   [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                   string payloadAsJsonParameterName = "",
                                                                   [CallerArgumentExpression(nameof(expectedResult))]
                                                                   string expectedResultParameterName = "",
                                                                   [CallerFilePath] string callerFilePath = "",
                                                                   bool isSuccessStatusCode = true,
                                                                   bool writResponse = false)
        {
            if (CustomAssertMethod is not null)
            {
                return await AssertCustomHttpCall(client,
                                                  url,
                                                  payloadAsJson,
                                                  expectedResult,
                                                  filterFunc,
                                                  httpMethod,
                                                  differenceFunc,
                                                  parameters,
                                                  callingAssembly,
                                                  payloadAsJsonParameterName,
                                                  expectedResultParameterName,
                                                  callerFilePath,
                                                  isSuccessStatusCode,
                                                  writResponse).ConfigureAwait(false);
            }

            return await AssertHttpCallInternal(client,
                                                url,
                                                payloadAsJson,
                                                expectedResult,
                                                filterFunc,
                                                httpMethod,
                                                differenceFunc,
                                                parameters,
                                                callingAssembly,
                                                payloadAsJsonParameterName,
                                                expectedResultParameterName,
                                                callerFilePath,
                                                isSuccessStatusCode,
                                                writResponse).ConfigureAwait(false);
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
                                                                           [CallerFilePath] string callerFilePath = "",
                                                                           bool isSuccessStatusCode = true,
                                                                           bool writResponse = false)
        {
            // localize expected response and payload
            // So the caller does not have to pass the unique file name of the embedded resource
            // - Api.V1.Users.GetAllUsersTest.Responses.GetAllUsersResponse.json
            // - GetAllUsersResponse.json
            payloadAsJson = embeddedFileLocalizer.LocalizeRequest(payloadAsJson, callerFilePath, callingAssembly);
            expectedResult = embeddedFileLocalizer.LocalizeResponse(expectedResult, callerFilePath, callingAssembly);

            // 0. Target type is primitive type
            var targetType = typeof(TResult);
            var targetIsPrimitiveType = targetType.IsPrimitive || targetType == typeof(string);

            // 1. Setup json payload
            var jsonPayload = payloadAsJson.GetJsonStringFrom(callingAssembly);

            // 2. Resolve parameters if parameterized payload
            jsonPayload = jsonPayload.ResolveParameters(parameters);

            // 3. Call the endpoint
            using var httpResponseMessage = await HttpCallHandler.CallAsync(client, httpMethod, url,
                                                                            jsonPayload, CancellationToken.None, payloadAsJsonParameterName).ConfigureAwait(false);

            // 4. Get the response as json
            var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            // 5. Resolve parameters in response json
            var resolvedParametersJsonString = contentAsString.ResolveParameters(parameters);

            // 6. Deserialized target type
            var currentResult = targetIsPrimitiveType ? PrimitiveTypeConverter.ConvertTo<TResult>(resolvedParametersJsonString) : resolvedParametersJsonString.FromJsonStringAs<TResult>();

            var filteredCurrentResult = filterFunc(currentResult);

            // 7. Build up absolute url for nice test results
            var absoluteUrl = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri ?? string.Empty;

            // 8. Build curl
            var curl = CurlBuilder.BuildFrom(httpMethod, absoluteUrl, payloadAsJson,
                                             client.DefaultRequestHeaders.Authorization, callingAssembly, ShowTokenInCurl);

            // 9. Format the output string for best readable and understandable test results
            var httpCallInfo = HttpOutputFormatter.GetOutputString("Http call infos:",
                                                                   httpMethod,
                                                                   absoluteUrl,
                                                                   httpResponseMessage.StatusCode);

            // 10. Simplify the response message
            var simpleHttpResponse = httpResponseMessage.ToJson().FromJsonStringAs<SimpleHttpResponseMessage>();

            // 11. Setup simple http response message which is the new container class for the comparison
            var resolvedSimpleHttpResponse = simpleHttpResponse with
            {
                Content = new SimpleHttpContent
                {
                    Headers = httpResponseMessage.Content.Headers.ToJson().FromJsonStringAs<IImmutableList<KeyValuePair<string, IImmutableList<string>>>>(),
                    Value = filteredCurrentResult
                }
            };

            // 12. Normalize expected json string dependent on target type and edge cases like primitive types and so on.
            string? expectedResultAsJson;

            if (writResponse && callingAssembly.IsCompiledInDebug())
            {
                expectedResultAsJson = expectedResult.GetJsonStringOrDefaultFrom<TResult>(contentAsString, callingAssembly, curl,
                                                                                          expectedResultParameterName);

                if (expectedResultAsJson.IsNull())
                {
                    expectedResultAsJson = resolvedSimpleHttpResponse.ToJson();
                }
            }
            else
            {
                expectedResultAsJson = expectedResult.GetJsonStringFrom<TResult>(contentAsString, callingAssembly, curl,
                                                                                 expectedResultParameterName);
            }

            // 13. Resolve parameters in expected result
            var expectedResultAsJsonParamterized = expectedResultAsJson.ResolveParameters(parameters);

            // 14. Edge case string as primitive type -> just string response -> no json
            var expectedType = targetIsPrimitiveType ? PrimitiveTypeConverter.ConvertTo<TResult>(expectedResultAsJsonParamterized) : expectedResultAsJsonParamterized.FromJsonStringAs<TResult>();

            // 15. Execute the filter function on the expected result
            var filteredExpectedType = filterFunc(expectedType);

            // 16. Create the container structure for the comparison
            var expectedResultAsSimpleResponse = expectedResultAsJsonParamterized.FromJsonStringOrDefault<SimpleHttpResponseMessage>();

            // 17. To keep the whole code compatible with existence once we have to some tricks here
            //     Because already existing test code just have the type response, no status code checks and more
            if (expectedResultAsSimpleResponse.IsNull() || expectedResultAsSimpleResponse.Content.IsNull())
            {
                expectedResultAsSimpleResponse = simpleHttpResponse with
                {
                    Content = new SimpleHttpContent
                    {
                        Headers = httpResponseMessage.Content.Headers.ToJson().FromJsonStringAs<IImmutableList<KeyValuePair<string, IImmutableList<string>>>>(),
                        Value = expectedResult == IgnoreResponseComparison ? filteredCurrentResult : filteredExpectedType
                    }
                };
            }

            // 18. This is our fallback for the AssertPostAsync and AssertPostAsErrorAsync
            //     Dependent on the flag we know if the response should fail or not -> so 
            //     is our code 100% compatible with the existing code
            expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { IsSuccessStatusCode = isSuccessStatusCode };

            // 19. Compare the expected results and more - just response
            Assert.That.ObjectsAreEqual(expectedResultAsSimpleResponse.ToJson(),
                                        resolvedSimpleHttpResponse,
                                        item => item,
                                        httpCallInfo,
                                        callingAssembly,
                                        differenceFunc,
                                        curl,
                                        parameters,
                                        writResponse,
                                        expectedResultParameterName,
                                        "Current response",
                                        callerFilePath);

            // 20. Return the current result
            return currentResult;
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
                                                                         [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                         string payloadAsJsonParameterName = "",
                                                                         [CallerArgumentExpression(nameof(expectedResult))]
                                                                         string expectedResultParameterName = "",
                                                                         [CallerFilePath] string callerFilePath = "",
                                                                         bool isSuccessStatusCode = true,
                                                                         bool writeResponse = false)
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
            var tasReturnType = genericMethod.Invoke(null, [client, url, payloadAsJson, expectedResult, filterFunc, httpMethod, differenceFunc, parameters, callingAssembly, payloadAsJsonParameterName, expectedResultParameterName, callerFilePath, isSuccessStatusCode, writeResponse]);

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
