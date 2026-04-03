using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspNetCore.Simple.MsTest.Sdk.Outputs;
using Extensions.Pack;
using JsonSerializer = AspNetCore.Simple.MsTest.Sdk.Serializer.Json.JsonSerializer;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        private static readonly HttpOutputFormatter HttpOutputFormatter = new();

        private static readonly CurlBuilder CurlBuilder = new();

        private static readonly OutputFormatter OutputFormatter = new(new CurlFormatter());

        private static readonly PrimitiveTypeConverter PrimitiveTypeConverter = new();

        private static readonly JsonDiffer JsonDiffer = new JsonDiffer();

        private static readonly ParameterReplacer ParameterReplacer = new();

        private static readonly ResponseWriter ResponseWriter = new ResponseWriter([
                                                                                       new DifferenceResponseWriter(JsonDiffer, new JsonPathWriter(), ParameterReplacer),
                                                                                       new OverwriteAllResponseWriter(ParameterReplacer)
                                                                                   ]);

        private static readonly WriteResponseService WriteResponseService = new();

        // You have the possible to set and pass the api settings specific json options
        public static JsonSerializerOptions JsonSerializerOptions
        {
            get => _jsonSerializerOptions;

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
        private const string IgnoreResponseComparison = "IgnoreResponse";

        private static JsonSerializerOptions _jsonSerializerOptions = new()
                                                                      {
                                                                          PropertyNameCaseInsensitive = true,
                                                                          PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                                          DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                                                                          NumberHandling = JsonNumberHandling.AllowReadingFromString,
                                                                          Converters = { new JsonStringEnumConverter() }
                                                                      };

        private static readonly EmbeddedFileLocalizer EmbeddedFileLocalizer = new(new TestCreatorSettings(), JsonSerializerOptions);

#pragma warning disable CA1859
        private static Task AssertHttpCallAsync(this HttpClient client,
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
#pragma warning restore CA1859
        {
            return client.AssertHttpCallAsync<string>(url,
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
                                                      writResponse);
        }

        // Context-based non-generic overload
#pragma warning disable CA1859
        internal static Task AssertHttpCallAsync(HttpAssertContextInternal context)
#pragma warning restore CA1859
        {
            var genericContext = HttpAssertContextInternalFactory.ToGeneric<string>(context,
                                                                                    IgnoreResponseComparison);

            return AssertHttpCallAsync(genericContext);
        }

        private static Task<TResult> AssertHttpCallAsync<TResult>(this HttpClient client,
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
            return client.AssertHttpCallAsync(url,
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

        private static Task<TResult> AssertHttpCallAsync<TResult>(this HttpClient client,
                                                                  string url,
                                                                  string payloadAsJson,
                                                                  string expectedResult,
                                                                  Func<TResult, TResult> filterFunc,
                                                                  HttpMethod httpMethod,
                                                                  Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
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
            // Convert to internal context and call the context-based method
            var context = HttpAssertContextInternalFactory.FromParameters(client,
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
                                                                          writResponse);

            return AssertHttpCallAsync(context);
        }

        internal static async Task<TResult> AssertHttpCallAsync<TResult>(HttpAssertContextInternal<TResult> context)
        {
            // Special case if expected and current jsons are parameters passed by we need to set the
            // correct parameter names
            var payloadAsJson = EmbeddedFileLocalizer.LocalizeRequest(context.PayloadAsJson, context.CallerFilePath, context.CallingAssembly);
            var expectedResult = EmbeddedFileLocalizer.LocalizeResponse(context.ExpectedResult, context.CallerFilePath, context.CallingAssembly);

            var payloadAsJsonParameterName = context.PayloadParameterName;
            var expectedResultParameterName = context.ExpectedResultParameterName;

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

            // Important to replace the parameters after ordering, because the order can change the
            // position of the parameters in the json and if we replace before, we can end up with
            // wrong replacements
            var sortedParameters = context.Parameters.OrderByDescending(p => p.Key.Length);

            var url = context.Url;

            foreach (var valueTuple in sortedParameters)
            {
                url = url.Replace(valueTuple.Key, valueTuple.Value?.ToString());
            }

            // Update context with resolved values
            var updatedContext = new HttpAssertContextInternal<TResult>
                                 {
                                     Client = context.Client,
                                     Url = url,
                                     PayloadAsJson = payloadAsJson,
                                     ExpectedResult = expectedResult,
                                     FilterFunc = context.FilterFunc,
                                     HttpMethod = context.HttpMethod,
                                     DifferenceFunc = context.DifferenceFunc,
                                     Parameters = context.Parameters,
                                     CallingAssembly = context.CallingAssembly,
                                     PayloadParameterName = payloadAsJsonParameterName,
                                     ExpectedResultParameterName = expectedResultParameterName,
                                     CallerFilePath = context.CallerFilePath,
                                     IsSuccessStatusCode = context.IsSuccessStatusCode,
                                     WriteResponse = context.WriteResponse
                                 };

            if (CustomAssertMethod is not null)
            {
                return await AssertCustomHttpCallAsync(updatedContext).ConfigureAwait(false);
            }

            return await AssertHttpCallInternalAsync(updatedContext).ConfigureAwait(false);
        }

        //private static Task<TResult> AssertHttpCallInternalAsync<TResult>(this HttpClient client,
        //                                                                  string url,
        //                                                                  string payloadAsJson,
        //                                                                  string expectedResult,
        //                                                                  Func<TResult, TResult> filterFunc,
        //                                                                  HttpMethod httpMethod,
        //                                                                  Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
        //                                                                  (string Key, object? Value)[] parameters,
        //                                                                  Assembly callingAssembly,
        //                                                                  [CallerArgumentExpression(nameof(payloadAsJson))]
        //                                                                  string payloadAsJsonParameterName = "",
        //                                                                  [CallerArgumentExpression(nameof(expectedResult))]
        //                                                                  string expectedResultParameterName = "",
        //                                                                  [CallerFilePath] string callerFilePath = "",
        //                                                                  bool isSuccessStatusCode = true,
        //                                                                  bool writResponse = false)
        //{
        //    // Convert to internal context and call the context-based method
        //    var context = HttpAssertContextInternalFactory.FromParameters<TResult>(client,
        //                                                                    url,
        //                                                                    payloadAsJson,
        //                                                                    expectedResult,
        //                                                                    filterFunc,
        //                                                                    httpMethod,
        //                                                                    differenceFunc,
        //                                                                    parameters,
        //                                                                    callingAssembly,
        //                                                                    payloadAsJsonParameterName,
        //                                                                    expectedResultParameterName,
        //                                                                    callerFilePath,
        //                                                                    isSuccessStatusCode,
        //                                                                    writResponse);

        //    return AssertHttpCallInternalAsync(context);
        //}

        private static async Task<TResult> AssertHttpCallInternalAsync<TResult>(HttpAssertContextInternal<TResult> context)
        {
            // localize expected response and payload
            // So the caller does not have to pass the unique file name of the embedded resource
            // - Api.V1.Users.GetAllUsersTest.Responses.GetAllUsersResponse.json
            // - GetAllUsersResponse.json
            var payloadAsJsonFile = EmbeddedFileLocalizer.LocalizeRequestFile(context.PayloadAsJson, context.CallerFilePath, context.CallingAssembly);
            var expectedResultFile = EmbeddedFileLocalizer.LocalizeResponseFile(context.ExpectedResult, context.CallerFilePath, context.CallingAssembly);

            // 0. Target type is primitive type
            var targetType = typeof(TResult);
            var targetIsPrimitiveType = targetType.IsPrimitive || targetType.EqualsTo(typeof(string));

            // 1. Setup json payload
            var jsonPayload = payloadAsJsonFile.Content;

            // 2. Resolve parameters if parameterized payload
            jsonPayload = jsonPayload.ResolveParameters(context.Parameters);

            // 3. Call the endpoint
            using var httpResponseMessage = await _httpCallHandler.CallAsync(context.Client,
                                                                             context.HttpMethod,
                                                                             context.Url,
                                                                             jsonPayload,
                                                                             CancellationToken.None,
                                                                             context.PayloadParameterName).ConfigureAwait(false);

            // 4. Get the response as json
            var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            // 5. Resolve parameters in response json
            var resolvedParametersJsonString = contentAsString.ResolveParameters(context.Parameters);

            // 6. Build up absolute url for nice test results
            var absoluteUrl = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri ?? string.Empty;

            // 7. Format the output string for best readable and understandable test results
            var httpCallInfo = HttpOutputFormatter.GetOutputString("Http call infos:",
                                                                   context.HttpMethod,
                                                                   absoluteUrl,
                                                                   httpResponseMessage.StatusCode);

            // 8. Build curl
            var curl = CurlBuilder.BuildFrom(context.HttpMethod,
                                             absoluteUrl,
                                             jsonPayload,
                                             context.Client.DefaultRequestHeaders.Authorization,
                                             context.CallingAssembly,
                                             ShowTokenInCurl);

            // 9. We have a use cases:
            // - User wants OK response, but we get an error response
            // - User wants NOT OK response, but we get an OK response
            // In this use case writeResponse:true must be ignored
            // otherwise we would write the wrong response
            if (httpResponseMessage.IsSuccessStatusCode.NotEqualsTo(context.IsSuccessStatusCode))
            {
                var errorInfo = context.IsSuccessStatusCode
                                    ? $"You expect an OK result but the response was {httpResponseMessage.StatusCode}. Please check implementation or your expected response"
                                    : $"You expect an ERROR result but the response was {httpResponseMessage.StatusCode}. Please check implementation or your expected response";

                var simpleExpectedResult = expectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(contentAsString, context.CallingAssembly, curl,
                                                                                                          context.ExpectedResultParameterName);

                var schemaNotMatchingError = OutputFormatter.GetOutputString(httpCallInfo, errorInfo, simpleExpectedResult,
                                                                             contentAsString, string.Empty, curl);

                Assert.Fail(schemaNotMatchingError);
            }

            // 10. Deserialized target type
            var currentResult = targetIsPrimitiveType ? PrimitiveTypeConverter.ConvertTo<TResult>(resolvedParametersJsonString) :
                                resolvedParametersJsonString.IsNullOrWhiteSpace() ? "{}".FromJsonStringAs<TResult>(JsonSerializerOptions) : resolvedParametersJsonString.FromJsonStringAs<TResult>();

            // 11. Execute filter func
            var filteredCurrentResult = context.FilterFunc(currentResult);

            // 12. Simplify the response message
            var currentSimpleHttResponseMessage = httpResponseMessage.ToJson(JsonSerializerOptions).FromJsonStringAs<SimpleHttpResponseMessage>(JsonSerializerOptions);

            // 13. Setup simple http response message which is the new container class for the comparison
            var currentResolvedSimpleHttpResponse = currentSimpleHttResponseMessage with
                                                    {
                                                        Content = new SimpleHttpContent
                                                                  {
                                                                      Headers = httpResponseMessage.Content.Headers.ToJson(JsonSerializerOptions).FromJsonStringAs<ImmutableList<KeyValuePair<string, ImmutableList<string>>>>(JsonSerializerOptions),
                                                                      Value = httpResponseMessage.IsSuccessStatusCode.EqualsTo(context.IsSuccessStatusCode) ? filteredCurrentResult : resolvedParametersJsonString.Trim('"')
                                                                  }
                                                    };

            // 14. Normalize expected json string dependent on target type and edge cases like primitive types and so on.
            string? expectedResultAsJson;

            var shouldWriteResponse = WriteResponseService.ShouldWriteResponse(context.WriteResponse, context.CallingAssembly);

            if (shouldWriteResponse)
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(contentAsString, context.CallingAssembly, curl,
                                                                                                      context.ExpectedResultParameterName);

                if (expectedResultAsJson.IsNull())
                {
                    expectedResultAsJson = currentResolvedSimpleHttpResponse.ToJson(JsonSerializerOptions);
                }
            }
            else
            {
                // try first new vrsion
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringFrom<TResult>(contentAsString, context.CallingAssembly, curl,
                                                                                             context.ExpectedResultParameterName);
            }

            // 15. Resolve parameters in expected result
            var expectedResultAsJsonParamterized = expectedResultAsJson.ResolveParameters(context.Parameters);

            // 16. Edge case string as primitive type -> just string response -> no json
            var expectedType = targetIsPrimitiveType ? PrimitiveTypeConverter.ConvertTo<TResult>(contentAsString) : expectedResultAsJsonParamterized.FromJsonStringOrDefault<TResult>(JsonSerializerOptions);

            // 17. Execute the filter function on the expected result
            var filteredExpectedType = expectedType.IsNotNull() ? context.FilterFunc(expectedType) : expectedType;

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
                                                                   Headers = httpResponseMessage.Content.Headers.ToJson(JsonSerializerOptions).FromJsonStringAs<ImmutableList<KeyValuePair<string, ImmutableList<string>>>>(JsonSerializerOptions),
                                                                   Value = expectedResultFile.Content.EqualsTo(IgnoreResponseComparison) ? filteredCurrentResult : filteredExpectedType
                                                               }
                                                 };
            }

            // 20. This is our fallback for the AssertPostAsync and AssertPostAsErrorAsync
            //     Dependent on the flag we know if the response should fail or not -> so
            //     is our code 100% compatible with the existing code
            expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { IsSuccessStatusCode = context.IsSuccessStatusCode };

            // NEW quick workaround
            if (expectedResultAsSimpleResponse.Content.Value.IsNull() &&
                expectedResultAsJsonParamterized.IsNotNullOrWhiteSpace())
            {
                expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { Content = expectedResultAsSimpleResponse.Content with { Value = expectedResultAsJsonParamterized } };
            }

            // 21. Compare the expected results and more - just response
            var expectedObjectAsJson = expectedResultAsSimpleResponse.ToJson(JsonSerializerOptions);

            // 22. Before we convert the json into the target type we have to do a schema check !!
            //     Otherwise, we lost unexpected properties
            if (expectedResultAsJsonParamterized.IsNotNullOrWhiteSpace() &&
                expectedResultAsJsonParamterized.DoesNotContain(IgnoreResponseComparison) &&
                (contentAsString.StartsWith('{') || contentAsString.StartsWith('[')))
            {
                var currentResponse = currentSimpleHttResponseMessage with
                                      {
                                          Content = currentSimpleHttResponseMessage.Content.IsNull()
                                                        ? new SimpleHttpContent()
                                                          {
                                                              Value = currentSimpleHttResponseMessage,
                                                              Headers = ImmutableList<KeyValuePair<string, ImmutableList<string>>>.Empty
                                                          }
                                                        : currentSimpleHttResponseMessage.Content with
                                                          {
                                                              Value = JsonDocument.Parse(contentAsString).RootElement,
                                                          }
                                      };

                var expected = currentSimpleHttResponseMessage with
                               {
                                   Content = currentSimpleHttResponseMessage.Content.IsNull()
                                                 ? new SimpleHttpContent()
                                                   {
                                                       Value = currentSimpleHttResponseMessage,
                                                       Headers = ImmutableList<KeyValuePair<string, ImmutableList<string>>>.Empty
                                                   }
                                                 : currentSimpleHttResponseMessage.Content with
                                                   {
                                                       Value = JsonDocument.Parse(expectedResultAsJsonParamterized).RootElement,
                                                   }
                               };

                var expectedJson = expected.ToJson(JsonSerializerOptions);
                var currentResponseJson = currentResponse.ToJson(JsonSerializerOptions);

                var differences = JsonDiffer.FindDifferences(expectedJson, currentResponseJson);

                var schemaMismatchDifferences = differences.Where(d => d.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&

                                                                       // Very important an collection with more or less entries is not a schema mismatch
                                                                       d.MemberPath.EndsWith(']').IsFalse()).ToImmutableList();


                var filtered = differenceFunc(differences).ToImmutableList();
                var globalFiltered = AssertObjectExtensions.DifferenceFunc(filtered).ToImmutableList();

                var schemaMismatchDifferences = globalFiltered.Where(d => d.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&
                                                                          // Very important an collection with more or less entries is not a schema mismatch
                                                                          d.MemberPath.EndsWith(']').IsFalse()).ToImmutableList();

                if (schemaMismatchDifferences.Any())
                {
                    var differenceOutputTable = schemaMismatchDifferences.ToResultTable(context.ExpectedResultParameterName, "Current");

                    var schemaNotMatchingError = OutputFormatter.GetOutputString(httpCallInfo,
                                                                                 "Schema mismatch: Expected result and current result does not match", expectedJson,
                                                                                 currentResponseJson, differenceOutputTable, curl);

                    if (shouldWriteResponse)
                    {
                        var writeResponseRequest = new WriteResponseRequest()
                                                   {
                                                       CallingAssembly = context.CallingAssembly,
                                                       DifferenceFunc = context.DifferenceFunc,
                                                       CurrentResponseAsString = currentResponseJson,
                                                       ExpectedResult = expectedResultFile,
                                                       Parameters = context.Parameters,
                                                       Mode = ResponseWriteMode.DifferencesOnly
                                                   };

                        ResponseWriter.Write(writeResponseRequest);
                    }

                    Assert.Fail(schemaNotMatchingError);
                }
            }

            Assert.That.ObjectsAreEqual(expectedObjectAsJson,
                                        currentResolvedSimpleHttpResponse,
                                        item => item,
                                        httpCallInfo,
                                        context.CallingAssembly,
                                        context.DifferenceFunc,
                                        curl,
                                        context.Parameters,
                                        context.WriteResponse,
                                        context.ExpectedResultParameterName,
                                        "Current response",
                                        context.CallerFilePath);

            // 22. Return the current result
            return currentResult;
        }

        //private static Task<TResult> AssertCustomHttpCallAsync<TResult>(this HttpClient client,
        //                                                                string url,
        //                                                                string payloadAsJson,
        //                                                                string expectedResult,
        //                                                                Func<TResult, TResult> filterFunc,
        //                                                                HttpMethod httpMethod,
        //                                                                Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
        //                                                                (string Key, object? Value)[] parameters,
        //                                                                Assembly callingAssembly,
        //                                                                [CallerArgumentExpression(nameof(payloadAsJson))]
        //                                                                string payloadAsJsonParameterName = "",
        //                                                                [CallerArgumentExpression(nameof(expectedResult))]
        //                                                                string expectedResultParameterName = "",
        //                                                                [CallerFilePath] string callerFilePath = "",
        //                                                                bool isSuccessStatusCode = true,
        //                                                                bool writeResponse = false)
        //{
        //    // Convert to internal context and call the context-based method
        //    var context = HttpAssertContextInternalFactory.FromParameters<TResult>(client,
        //                                                                    url,
        //                                                                    payloadAsJson,
        //                                                                    expectedResult,
        //                                                                    filterFunc,
        //                                                                    httpMethod,
        //                                                                    differenceFunc,
        //                                                                    parameters,
        //                                                                    callingAssembly,
        //                                                                    payloadAsJsonParameterName,
        //                                                                    expectedResultParameterName,
        //                                                                    callerFilePath,
        //                                                                    isSuccessStatusCode,
        //                                                                    writeResponse);

        //    return AssertCustomHttpCallAsync(context);
        //}

        private static async Task<TResult> AssertCustomHttpCallAsync<TResult>(HttpAssertContextInternal<TResult> context)
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

            var tasReturnType = genericMethod.Invoke(null, [
                                                               context.Client,
                                                               context.Url,
                                                               context.PayloadAsJson,
                                                               context.ExpectedResult,
                                                               context.FilterFunc,
                                                               context.HttpMethod,
                                                               context.DifferenceFunc,
                                                               context.Parameters,
                                                               context.CallingAssembly,
                                                               context.PayloadParameterName,
                                                               context.ExpectedResultParameterName,
                                                               context.CallerFilePath,
                                                               context.IsSuccessStatusCode,
                                                               context.WriteResponse
                                                           ]);

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
