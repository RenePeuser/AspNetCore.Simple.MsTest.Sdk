using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddAssertableHttpClientExtension
    {
        /// <summary>
        /// Registers all assertable HTTP client services and their dependencies in the DI container.
        /// Feature-based registration following the dependency tree pattern.
        /// </summary>
        public static void AddAssertableHttpClient(this IServiceCollection services)
        {
            // 1. Register all dependencies via their own extensions
            services.AddHttpOutputFormatter();
            services.AddCurlBuilder();
            services.AddOutputFormatter();
            services.AddPrimitiveTypeConverter();
            services.AddJsonDiffer();
            services.AddResponseWriter();
            services.AddWriteResponseService();
            services.AddHttpCallHandler();
            services.AddAssertService();
            services.AddParameterReplacer();

            // 2. Register the service itself
            services.AddSingletonIfNotExists<IAssertableHttpClient, AssertableHttpClient>();
        }
    }

    /// <summary>
    /// Represents an HTTP client with assertion capabilities for API testing.
    /// Core service that performs HTTP calls with automatic response assertions.
    /// The HTTP method (GET, POST, PUT, PATCH, DELETE) is determined by the context.
    /// </summary>
    public interface IAssertableHttpClient
    {
        /// <summary>
        /// Performs an HTTP request and asserts the response against an expected result.
        /// The HTTP method is determined by the context's HttpMethod property.
        /// </summary>
        /// <typeparam name="TResult">The expected result type to deserialize the response to.</typeparam>
        /// <param name="context">The assertion context containing all request parameters including the HTTP method.</param>
        /// <returns>A task containing the deserialized response.</returns>
        Task<TResult> AssertAsync<TResult>(HttpAssertContext<TResult> context);
    }

    /// <summary>
    /// Implementation of <see cref="IAssertableHttpClient"/> that contains the core assertion logic.
    /// This class encapsulates all dependencies needed for HTTP assertions.
    /// All dependencies are injected via the primary constructor for testability and flexibility.
    /// </summary>
    internal sealed class AssertableHttpClient(IHttpOutputFormatter httpOutputFormatter,
                                               ICurlBuilder curlBuilder,
                                               IOutputFormatter outputFormatter,
                                               IPrimitiveTypeConverter primitiveTypeConverter,
                                               IJsonDiffer jsonDiffer,
                                               IResponseWriter responseWriter,
                                               IWriteResponseService writeResponseService,
                                               IHttpCallHandler httpCallHandler,
                                               JsonSerializerOptions jsonSerializerOptions,
                                               IAssertService assertService,
                                               IParameterReplacer parameterReplacementService) : IAssertableHttpClient
    {
        private const string IgnoreResponseComparison = "IgnoreResponse";

        /// <inheritdoc />
        public async Task<TResult> AssertAsync<TResult>(HttpAssertContext<TResult> context)
        {
            // Use pre-resolved files from context if available, otherwise resolve them now (fallback for backward compatibility)
            var expectedResultFile = context.ExpectedResultFile;

            // Target type is primitive type
            var targetType = typeof(TResult);
            var targetIsPrimitiveType = targetType.IsPrimitive || targetType.EqualsTo(typeof(string));

            // Call the endpoint using context (contains resolved URL, resolved payload, etc.)
            using var httpResponseMessage = await httpCallHandler.CallAsync(context, CancellationToken.None).ConfigureAwait(false);

            // Get the response as json
            var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Resolve parameters in response json
            var resolvedParametersJsonString = parameterReplacementService.ResolveParameters(contentAsString, context.Parameters);

            // Build up absolute url for nice test results
            var absoluteUrl = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri ?? string.Empty;

            // Format the output string for best readable and understandable test results
            var httpCallInfo = httpOutputFormatter.GetOutputString("Http call infos:",
                                                                   context.HttpMethod,
                                                                   absoluteUrl,
                                                                   httpResponseMessage.StatusCode);

            // Build curl using context (with runtime absolute URL)
            var curl = curlBuilder.BuildFrom(context.HttpMethod,
                                             absoluteUrl,
                                             context.ResolvedPayload ?? string.Empty,
                                             context.Client.DefaultRequestHeaders.Authorization,
                                             context.CallingAssembly,
                                             context.ShowTokenInCurl);

            // Check if status code matches expectations
            // If unexpected status code we do not write response because it would the wrong response !
            if (httpResponseMessage.IsSuccessStatusCode.NotEqualsTo(context.IsSuccessStatusCode))
            {
                var errorInfo = context.IsSuccessStatusCode
                                    ? $"You expect an OK result but the response was {httpResponseMessage.StatusCode}. Please check implementation or your expected response"
                                    : $"You expect an ERROR result but the response was {httpResponseMessage.StatusCode}. Please check implementation or your expected response";

                var simpleExpectedResult = expectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(contentAsString, context.CallingAssembly, curl,
                                                                                                          context.ExpectedResultParameterName);

                var schemaNotMatchingError = outputFormatter.GetOutputString(httpCallInfo, errorInfo, simpleExpectedResult,
                                                                             contentAsString, string.Empty, curl);

                Assert.Fail(schemaNotMatchingError);
            }

            // Deserialize target type
            var currentResult = targetIsPrimitiveType ? primitiveTypeConverter.ConvertTo<TResult>(resolvedParametersJsonString) :
                                resolvedParametersJsonString.IsNullOrWhiteSpace() ? "{}".FromJsonStringAs<TResult>(jsonSerializerOptions) :
                                resolvedParametersJsonString.FromJsonStringAs<TResult>(jsonSerializerOptions);

            // Execute filter func
            var filteredCurrentResult = context.OrderFunc(currentResult);

            // Simplify the response message
            var currentSimpleHttResponseMessage = httpResponseMessage.ToJson(jsonSerializerOptions).FromJsonStringAs<SimpleHttpResponseMessage>(jsonSerializerOptions);

            // Setup simple http response message which is the new container class for the comparison
            var currentResolvedSimpleHttpResponse = currentSimpleHttResponseMessage with
            {
                Content = new SimpleHttpContent
                {
                    Headers = httpResponseMessage.Content.Headers.ToJson(jsonSerializerOptions).FromJsonStringAs<ImmutableList<KeyValuePair<string, ImmutableList<string>>>>(jsonSerializerOptions),
                    Value = httpResponseMessage.IsSuccessStatusCode.EqualsTo(context.IsSuccessStatusCode) ? filteredCurrentResult : resolvedParametersJsonString.Trim('"')
                }
            };

            // Normalize expected json string dependent on target type and edge cases like primitive types and so on.
            string? expectedResultAsJson;

            var shouldWriteResponse = writeResponseService.ShouldWriteResponse(context.WriteResponse, context.CallingAssembly);

            if (shouldWriteResponse)
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(contentAsString, context.CallingAssembly, curl,
                                                                                                      context.ExpectedResultParameterName);

                if (expectedResultAsJson.IsNull())
                {
                    expectedResultAsJson = currentResolvedSimpleHttpResponse.ToJson(jsonSerializerOptions);
                }
            }
            else
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringFrom<TResult>(contentAsString, context.CallingAssembly, curl,
                                                                                             context.ExpectedResultParameterName);
            }

            // Resolve parameters in expected result
            var expectedResultAsJsonParamterized = parameterReplacementService.ResolveParameters(expectedResultAsJson, context.Parameters);

            // Edge case string as primitive type -> just string response -> no json
            var expectedType = targetIsPrimitiveType ? primitiveTypeConverter.ConvertTo<TResult>(contentAsString) : expectedResultAsJsonParamterized.FromJsonStringOrDefault<TResult>(jsonSerializerOptions);

            // Execute the filter function on the expected result
            var filteredExpectedType = expectedType.IsNotNull() ? context.OrderFunc(expectedType) : expectedType;

            // Create the container structure for the comparison
            var expectedResultAsSimpleResponse = expectedResultAsJsonParamterized.FromJsonStringOrDefault<SimpleHttpResponseMessage>(jsonSerializerOptions);

            // To keep the whole code compatible with existence once we have to some tricks here
            if (expectedResultAsSimpleResponse.IsNull() || expectedResultAsSimpleResponse.Content.IsNull())
            {
                expectedResultAsSimpleResponse = currentSimpleHttResponseMessage with
                {
                    Content = new SimpleHttpContent
                    {
                        Headers = httpResponseMessage.Content.Headers.ToJson(jsonSerializerOptions).FromJsonStringAs<ImmutableList<KeyValuePair<string, ImmutableList<string>>>>(jsonSerializerOptions),
                        Value = expectedResultFile.Content.EqualsTo(IgnoreResponseComparison) ? filteredCurrentResult : filteredExpectedType
                    }
                };
            }

            // This is our fallback for the AssertPostAsync and AssertPostAsErrorAsync
            expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { IsSuccessStatusCode = context.IsSuccessStatusCode };

            // Quick workaround
            if (expectedResultAsSimpleResponse.Content.Value.IsNull() &&
                expectedResultAsJsonParamterized.IsNotNullOrWhiteSpace())
            {
                expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { Content = expectedResultAsSimpleResponse.Content with { Value = expectedResultAsJsonParamterized } };
            }

            // Compare the expected results and more - just response
            var expectedObjectAsJson = expectedResultAsSimpleResponse.ToJson(jsonSerializerOptions);

            // Before we convert the json into the target type we have to do a schema check
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

                var expectedJson = expected.ToJson(jsonSerializerOptions);
                var currentResponseJson = currentResponse.ToJson(jsonSerializerOptions);

                var differences = jsonDiffer.FindDifferences(expectedJson, currentResponseJson);

                var schemaMismatchDifferences = differences.Where(d => d.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&
                                                                       d.MemberPath.EndsWith(']').IsFalse()).ToImmutableList();

                if (schemaMismatchDifferences.Any())
                {
                    var differenceOutputTable = schemaMismatchDifferences.ToResultTable(context.ExpectedResultParameterName, "Current");

                    var schemaNotMatchingError = outputFormatter.GetOutputString(httpCallInfo,
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

                        responseWriter.Write(writeResponseRequest);
                    }

                    Assert.Fail(schemaNotMatchingError);
                }
            }

            var objectAssertContext = new HttpAssertContext<SimpleHttpResponseMessage>
            {
                ExpectedResultFile = expectedResultFile,
                ExpectedObjectAsJson = expectedObjectAsJson,
                Current = currentResolvedSimpleHttpResponse,
                OrderFunc = item => item, // Order func was executed already on the primitive type level, so we can just use identity function here
                DifferenceFunc = context.DifferenceFunc,
                Parameters = context.Parameters,
                CallingAssembly = context.CallingAssembly,
                WriteResponse = context.WriteResponse,
                Title = httpCallInfo,
                CallerFilePath = context.CallerFilePath,
                ExpectedResultParameterName = context.ExpectedResultParameterName,
                CurrentResultParameterName = context.CurrentResultParameterName,
                Client = context.Client,
                Url = context.Url,
                HttpMethod = context.HttpMethod,
            };

            assertService.ObjectsAreEqual(objectAssertContext);

            // Return the current result
            return currentResult;
        }
    }
}
