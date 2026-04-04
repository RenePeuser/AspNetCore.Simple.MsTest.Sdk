using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddExpectedStatusCodeStrategyExtension
    {
        /// <summary>
        /// Registers the expected status code strategy and its dependencies.
        /// </summary>
        public static void AddExpectedStatusCodeStrategy(this IServiceCollection services)
        {
            // 1. Register dependencies
            services.AddPrimitiveTypeConverter();
            services.AddJsonDiffer();
            services.AddOutputFormatter();
            services.AddResponseWriter();
            services.AddWriteResponseService();
            services.AddAssertService();
            services.AddParameterReplacer();
            services.AddJsonSerializer();

            // 2. Register the strategy itself
            services.AddSingletonIfNotExists<IHttpStatusCodeProcessingStrategy, ExpectedStatusCodeStrategy>();
        }
    }

    /// <summary>
    /// Strategy for handling expected HTTP status codes (both success and error).
    /// Performs full response processing: deserialization, filtering, schema validation, and assertion.
    /// </summary>
    internal sealed class ExpectedStatusCodeStrategy(IPrimitiveTypeConverter primitiveTypeConverter,
                                                     IJsonDiffer jsonDiffer,
                                                     IOutputFormatter outputFormatter,
                                                     IResponseWriter responseWriter,
                                                     IWriteResponseService writeResponseService,
                                                     IAssertService assertService,
                                                     IParameterReplacer parameterReplacementService,
                                                     JsonSerializerOptions jsonSerializerOptions) : IHttpStatusCodeProcessingStrategy
    {
        private const string IgnoreResponseComparison = "IgnoreResponse";

        /// <inheritdoc />
        public bool CanHandle<TResult>(HttpResponseContext<TResult> context) => context.IsExpectedStatusCode;

        /// <inheritdoc />
        public Task<TResult> ProcessAsync<TResult>(HttpResponseContext<TResult> context)
        {
            // Extract commonly used values from context
            var expectedResultFile = context.Request.ExpectedResultFile;
            var targetType = typeof(TResult);
            var targetIsPrimitiveType = targetType.IsPrimitive || targetType.EqualsTo(typeof(string));

            // TODO: We need these from the original AssertAsync method:
            // - httpResponseMessage (for ToJson and Content.Headers)
            // - resolvedParametersJsonString
            // - contentAsString
            // - httpCallInfo
            // - curl
            // - absoluteUrl

            // For now, this is the complete logic from lines 133-303 that needs to be ported:

            // Deserialize target type
            var currentResult = targetIsPrimitiveType ? primitiveTypeConverter.ConvertTo<TResult>(context.ResolvedParametersJsonString) :
                                context.ResolvedParametersJsonString.IsNullOrWhiteSpace() ? "{}".FromJsonStringAs<TResult>(jsonSerializerOptions) :
                                context.ResolvedParametersJsonString.FromJsonStringAs<TResult>(jsonSerializerOptions);

            // Execute filter func
            var filteredCurrentResult = context.Request.OrderFunc(currentResult);

            // Simplify the response message
            // TODO: Need httpResponseMessage here
            var currentSimpleHttResponseMessage = context.SimpleHttpResponseMessage;

            // Setup simple http response message which is the new container class for the comparison
            var currentResolvedSimpleHttpResponse = currentSimpleHttResponseMessage with
            {
                Content = new SimpleHttpContent
                {
                    Headers = context.ContentHeaders,
                    Value = context.IsExpectedStatusCode ? filteredCurrentResult : context.ResolvedParametersJsonString.Trim('"')
                }
            };

            // Normalize expected json string dependent on target type and edge cases like primitive types and so on.
            string? expectedResultAsJson;

            var shouldWriteResponse = writeResponseService.ShouldWriteResponse(context.Request.WriteResponse, context.Request.CallingAssembly);

            if (shouldWriteResponse)
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(context.ContentAsString, context.Request.CallingAssembly, context.Curl,
                                                                                                      context.Request.ExpectedResultParameterName);

                if (expectedResultAsJson.IsNull())
                {
                    expectedResultAsJson = currentResolvedSimpleHttpResponse.ToJson(jsonSerializerOptions);
                }
            }
            else
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringFrom<TResult>(context.ContentAsString, context.Request.CallingAssembly, context.Curl,
                                                                                             context.Request.ExpectedResultParameterName);
            }

            // Resolve parameters in expected result
            var expectedResultAsJsonParamterized = parameterReplacementService.ResolveParameters(expectedResultAsJson, context.Request.Parameters);

            // Edge case string as primitive type -> just string response -> no json
            var expectedType = targetIsPrimitiveType ? primitiveTypeConverter.ConvertTo<TResult>(context.ContentAsString) : expectedResultAsJsonParamterized.FromJsonStringOrDefault<TResult>(jsonSerializerOptions);

            // Execute the filter function on the expected result
            var filteredExpectedType = expectedType.IsNotNull() ? context.Request.OrderFunc(expectedType) : expectedType;

            // Create the container structure for the comparison
            var expectedResultAsSimpleResponse = expectedResultAsJsonParamterized.FromJsonStringOrDefault<SimpleHttpResponseMessage>(jsonSerializerOptions);

            // To keep the whole code compatible with existence once we have to some tricks here
            if (expectedResultAsSimpleResponse.IsNull() || expectedResultAsSimpleResponse.Content.IsNull())
            {
                expectedResultAsSimpleResponse = currentSimpleHttResponseMessage with
                {
                    Content = new SimpleHttpContent
                    {
                        Headers = context.ContentHeaders,
                        Value = expectedResultFile.Content.EqualsTo(IgnoreResponseComparison) ? filteredCurrentResult : filteredExpectedType
                    }
                };
            }

            // This is our fallback for the AssertPostAsync and AssertPostAsErrorAsync
            expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { IsSuccessStatusCode = context.Request.IsSuccessStatusCode };

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
                (context.ContentAsString.StartsWith('{') || context.ContentAsString.StartsWith('[')))
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
                                                            Value = JsonDocument.Parse(context.ContentAsString).RootElement,
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
                    var differenceOutputTable = schemaMismatchDifferences.ToResultTable(context.Request.ExpectedResultParameterName, "Current");

                    var schemaNotMatchingError = outputFormatter.GetOutputString(context.HttpCallInfo,
                                                                                 "Schema mismatch: Expected result and current result does not match", expectedJson,
                                                                                 currentResponseJson, differenceOutputTable, context.Curl);

                    if (shouldWriteResponse)
                    {
                        var writeResponseRequest = new WriteResponseRequest()
                        {
                            CallingAssembly = context.Request.CallingAssembly,
                            DifferenceFunc = context.Request.DifferenceFunc,
                            CurrentResponseAsString = currentResponseJson,
                            ExpectedResult = expectedResultFile,
                            Parameters = context.Request.Parameters,
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
                DifferenceFunc = context.Request.DifferenceFunc,
                Parameters = context.Request.Parameters,
                CallingAssembly = context.Request.CallingAssembly,
                WriteResponse = context.Request.WriteResponse,
                Title = context.HttpCallInfo,
                CallerFilePath = context.Request.CallerFilePath,
                ExpectedResultParameterName = context.Request.ExpectedResultParameterName,
                CurrentResultParameterName = context.Request.CurrentResultParameterName,
                Client = context.Request.Client,
                Url = context.Request.Url,
                HttpMethod = context.Request.HttpMethod,
            };

            assertService.ObjectsAreEqual(objectAssertContext);

            // Return the current result
            return Task.FromResult(currentResult);
        }
    }
}
