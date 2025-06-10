using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Outputs;
using Extensions.Pack;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using JsonSerializer = AspNetCore.Simple.MsTest.Sdk.Serializer.Json.JsonSerializer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        private static readonly HttpOutputFormatter HttpOutputFormatter = new();

        private static readonly CurlBuilder CurlBuilder = new();

        private static readonly OutputFormatter OutputFormatter = new(new CurlFormatter());

        private static readonly EmbeddedFileLocalizer embeddedFileLocalizer = new(new TestCreatorSettings());

        private static readonly PrimitiveTypeConverter PrimitiveTypeConverter = new();

        private static readonly JsonDiffer JsonDiffer = new JsonDiffer();

        // You have the possible to set and pass the api settings specific json options
        public static JsonSerializerOptions JsonSerializerOptions
        {
            get
            {
                return _jsonSerializerOptions;
            }

            set
            {
                _jsonSerializerOptions = value;
                _httpCallHandler = new HttpCallHandler(new HttpRequestMessageBuilder(new JsonSerializer(_jsonSerializerOptions)));
            }
        }

        private static HttpCallHandler _httpCallHandler = new(new HttpRequestMessageBuilder(new JsonSerializer(JsonSerializerOptions)));

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

        private static JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { new JsonStringEnumConverter() }
        };

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
            // Special case if expected and current jsons are parameters passed by we need to set the
            // correct parameter names
            payloadAsJson = embeddedFileLocalizer.LocalizeRequest(payloadAsJson, callerFilePath, callingAssembly);
            expectedResult = embeddedFileLocalizer.LocalizeResponse(expectedResult, callerFilePath, callingAssembly);

            if (payloadAsJson.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                payloadAsJsonParameterName.EndsWith(".json", StringComparison.OrdinalIgnoreCase).IsFalse())
            {
                payloadAsJsonParameterName = payloadAsJson;
            }

            if (expectedResult.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                expectedResultParameterName.EndsWith(".json", StringComparison.OrdinalIgnoreCase).IsFalse())
            {
                expectedResultParameterName = expectedResult;
            }

            // NEW query params and more :)
            foreach (var valueTuple in parameters)
            {
                url = url.Replace(valueTuple.Key, valueTuple.Value?.ToString());
            }

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
                                                                           [CallerArgumentExpression(nameof(payloadAsJson))]
                                                                           string payloadAsJsonParameterName = "",
                                                                           [CallerArgumentExpression(nameof(expectedResult))]
                                                                           string expectedResultParameterName = "",
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
            using var httpResponseMessage = await _httpCallHandler.CallAsync(client,
                                                                            httpMethod,
                                                                            url,
                                                                            jsonPayload,
                                                                            CancellationToken.None,
                                                                            payloadAsJsonParameterName).ConfigureAwait(false);

            // 4. Get the response as json
            var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            // 5. Resolve parameters in response json
            var resolvedParametersJsonString = contentAsString.ResolveParameters(parameters);

            // 6. Build up absolute url for nice test results
            var absoluteUrl = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri ?? string.Empty;

            // 7. Format the output string for best readable and understandable test results
            var httpCallInfo = HttpOutputFormatter.GetOutputString("Http call infos:",
                                                                   httpMethod,
                                                                   absoluteUrl,
                                                                   httpResponseMessage.StatusCode);

            // 8. Build curl
            var curl = CurlBuilder.BuildFrom(httpMethod,
                                             absoluteUrl,
                                             jsonPayload,
                                             client.DefaultRequestHeaders.Authorization,
                                             callingAssembly,
                                             ShowTokenInCurl);

            // 9. We have a use cases:
            // - User wants OK response, but we get an error response
            // - User wants NOT OK response, but we get an OK response
            // In this use case writeResponse:true must be ignored
            // otherwise we would write the wrong response
            if (httpResponseMessage.IsSuccessStatusCode != isSuccessStatusCode)
            {
                var errorInfo = isSuccessStatusCode ?
                                    $"You expect an OK result but the response was {httpResponseMessage.StatusCode}. Please check implementation or your expected response" :
                                    $"You expect an ERROR result but the response was {httpResponseMessage.StatusCode}. Please check implementation or your expected response";

                var simpleExpectedResult = expectedResult.GetJsonStringOrDefaultFrom<TResult>(contentAsString, callingAssembly, curl,
                                                                                     expectedResultParameterName);

                var schemaNotMatchingError = OutputFormatter.GetOutputString(httpCallInfo, errorInfo, simpleExpectedResult,
                                                                             contentAsString, string.Empty, curl);

                Assert.Fail(schemaNotMatchingError);
            }

            // 10. Deserialized target type
            var currentResult = targetIsPrimitiveType ? PrimitiveTypeConverter.ConvertTo<TResult>(resolvedParametersJsonString) : resolvedParametersJsonString.IsNullOrWhiteSpace() ? "{}".FromJsonStringAs<TResult>(JsonSerializerOptions) : resolvedParametersJsonString.FromJsonStringAs<TResult>();

            // 11. Execute filter func
            var filteredCurrentResult = filterFunc(currentResult);

            // 12. Simplify the response message
            var currentSimpleHttResponseMessage = httpResponseMessage.ToJson(JsonSerializerOptions).FromJsonStringAs<SimpleHttpResponseMessage>(JsonSerializerOptions);

            // 13. Setup simple http response message which is the new container class for the comparison
            var currentResolvedSimpleHttpResponse = currentSimpleHttResponseMessage with
            {
                Content = new SimpleHttpContent
                {
                    Headers = httpResponseMessage.Content.Headers.ToJson(JsonSerializerOptions).FromJsonStringAs<IImmutableList<KeyValuePair<string, IImmutableList<string>>>>(JsonSerializerOptions),
                    Value = httpResponseMessage.IsSuccessStatusCode == isSuccessStatusCode ? filteredCurrentResult : resolvedParametersJsonString.Trim('"')
                }
            };

            // 14. Normalize expected json string dependent on target type and edge cases like primitive types and so on.
            string? expectedResultAsJson;

            if ((writResponse || AssertObjectExtensions.WriteResponse) && callingAssembly.IsCompiledInDebug())
            {
                expectedResultAsJson = expectedResult.GetJsonStringOrDefaultFrom<TResult>(contentAsString, callingAssembly, curl,
                                                                                          expectedResultParameterName);

                if (expectedResultAsJson.IsNull())
                {
                    expectedResultAsJson = currentResolvedSimpleHttpResponse.ToJson(JsonSerializerOptions);
                }
            }
            else
            {

                // try first new vrsion
                expectedResultAsJson = expectedResult.GetJsonStringFrom<TResult>(contentAsString, callingAssembly, curl,
                                                                                 expectedResultParameterName);
            }

            // 15. Resolve parameters in expected result
            var expectedResultAsJsonParamterized = expectedResultAsJson.ResolveParameters(parameters);

            
            // 16. Before we convert the json into the target type we have to do a schema check !!
            //     Otherwise, we lost unexpected properties
            if (resolvedParametersJsonString.IsNotNullOrWhiteSpace())
            {
                var differences = JsonDiffer.FindDifferences(expectedResultAsJsonParamterized, resolvedParametersJsonString);
                if (differences.Any(d => d.MismatchType.NotEqualsTo(MismatchType.ValueDifference)))
                {
                    var differenceOutputTable = differences.ToResultTable(expectedResultParameterName, "Current");

                    var doc = JsonDocument.Parse(expectedResultAsJsonParamterized);
                    string minified = System.Text.Json.JsonSerializer.Serialize(doc.RootElement);
                    
                    var schemaNotMatchingError = OutputFormatter.GetOutputString(httpCallInfo, "Schema mismatch: Expected result and current result does not match", resolvedParametersJsonString,
                                                                                 minified, differenceOutputTable, curl);

                    Assert.Fail(schemaNotMatchingError);
                }
            }
            
            // 16. Edge case string as primitive type -> just string response -> no json
            var expectedType = targetIsPrimitiveType ? PrimitiveTypeConverter.ConvertTo<TResult>(expectedResultAsJsonParamterized) : expectedResultAsJsonParamterized.FromJsonStringOrDefault<TResult>(JsonSerializerOptions);

            // 17. Execute the filter function on the expected result
            var filteredExpectedType = expectedType.IsNotNull() ? filterFunc(expectedType) : expectedType;

            // 18. Create the container structure for the comparison
            var expectedResultAsSimpleResponse = expectedResultAsJsonParamterized.FromJsonStringOrDefault<SimpleHttpResponseMessage>(JsonSerializerOptions);

            // 19. To keep the whole code compatible with existence once we have to some tricks here
            //     Because already existing test code just have the type response, no status code checks and more
            if (expectedResultAsSimpleResponse.IsNull() || expectedResultAsSimpleResponse.Content.IsNull())
            {
                expectedResultAsSimpleResponse = currentSimpleHttResponseMessage with
                {
                    Content = new SimpleHttpContent
                    {
                        Headers = httpResponseMessage.Content.Headers.ToJson(JsonSerializerOptions).FromJsonStringAs<IImmutableList<KeyValuePair<string, IImmutableList<string>>>>(JsonSerializerOptions),
                        Value = expectedResult == IgnoreResponseComparison ? filteredCurrentResult : filteredExpectedType
                    }
                };
            }

            // 20. This is our fallback for the AssertPostAsync and AssertPostAsErrorAsync
            //     Dependent on the flag we know if the response should fail or not -> so 
            //     is our code 100% compatible with the existing code
            expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { IsSuccessStatusCode = isSuccessStatusCode };

            // NEW quick workaround
            if (expectedResultAsSimpleResponse.Content.Value.IsNull() &&
                expectedResultAsJsonParamterized.IsNotNullOrWhiteSpace())
            {
                expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { Content = expectedResultAsSimpleResponse.Content with { Value = expectedResultAsJsonParamterized } };
            }

            // 21. Compare the expected results and more - just response
            var expectedObjectAsJson = expectedResultAsSimpleResponse.ToJson(JsonSerializerOptions);

            Assert.That.ObjectsAreEqual(expectedObjectAsJson,
                                        currentResolvedSimpleHttpResponse,
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

            // 22. Return the current result
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
