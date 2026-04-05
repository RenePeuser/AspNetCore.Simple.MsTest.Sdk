using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    public static class AddJsonComparisonStepExtension
    {
        /// <summary>
        /// Registers the JSON comparison step and its dependencies.
        /// </summary>
        public static void AddJsonComparisonStep(this IServiceCollection services)
        {
            // 1. Register dependencies
            services.AddPrimitiveTypeConverter();
            services.AddJsonDiffer();
            services.AddSnapshotTestOutputBuilder();
            services.AddResponseWriter();
            services.AddWriteResponseService();
            services.AddAssertService();
            services.AddParameterReplacer();
            services.AddJsonSerializer();

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, JsonComparisonStep>();
        }
    }

    /// <summary>
    /// Compares the JSON response against expected JSON.
    /// Handles both schema mismatches and value differences in a single step.
    /// Flow: Build structures → Diff once → Check schema first → Then check values
    /// </summary>
    internal sealed class JsonComparisonStep(IPrimitiveTypeConverter primitiveTypeConverter,
                                             IJsonDiffer jsonDiffer,
                                             ISnapshotTestOutputBuilder snapshotTestOutputBuilder,
                                             IResponseWriter responseWriter,
                                             IWriteResponseService writeResponseService,
                                             IAssertService assertService,
                                             IParameterReplacer parameterReplacementService,
                                             JsonSerializerOptions jsonSerializerOptions) : IHttpAssertionStep
    {
        private const string IgnoreResponseComparison = "IgnoreResponse";

        /// <inheritdoc />
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            var expectedResultFile = context.ExpectedResultFile;
            var targetType = typeof(TResult);
            var targetIsPrimitiveType = targetType.IsPrimitive || targetType.EqualsTo(typeof(string));

            // Get current result from context (already deserialized in AssertableHttpClient)
            var currentResult = context.CurrentResult;

            // Apply filter function to get filtered result for comparison
            var filteredCurrentResult = context.OrderFunc(currentResult);

            // Build expected result JSON
            var expectedResultAsJson = BuildExpectedResultJson(context, targetIsPrimitiveType, filteredCurrentResult);
            var expectedResultAsJsonParameterized = parameterReplacementService.ResolveParameters(expectedResultAsJson, context.Parameters);

            // Skip comparison if IgnoreResponse marker is present
            if (expectedResultAsJsonParameterized.IsNullOrWhiteSpace() ||
                expectedResultAsJsonParameterized.Contains(IgnoreResponseComparison))
            {
                return;
            }

            // Build SimpleHttpResponseMessage from HttpResponseMessage (needed for snapshot comparison)
            var simpleHttpResponseMessage = context.HttpResponseMessage.ToJson(jsonSerializerOptions).FromJsonStringAs<SimpleHttpResponseMessage>(jsonSerializerOptions);
            var contentHeaders = context.HttpResponseMessage.Content.Headers.ToJson(jsonSerializerOptions).FromJsonStringAs<ImmutableList<KeyValuePair<string, ImmutableList<string>>>>(jsonSerializerOptions);

            // Build comparison structures
            var currentResponse = BuildCurrentResponse(context, simpleHttpResponseMessage, contentHeaders,
                                                       filteredCurrentResult);

            var expectedResponse = BuildExpectedResponse(context, expectedResultAsJsonParameterized, targetIsPrimitiveType,
                                                         simpleHttpResponseMessage, contentHeaders, filteredCurrentResult);

            var expectedJson = expectedResponse.ToJson(jsonSerializerOptions);
            var currentResponseJson = currentResponse.ToJson(jsonSerializerOptions);

            // Diff once - use for both schema and value checks
            var differences = jsonDiffer.FindDifferences(expectedJson, currentResponseJson);

            // Check 1: Schema mismatches (structure differences)
            var schemaMismatchDifferences = differences.Where(d => d.MismatchType.NotEqualsTo(MismatchType.ValueDifference) &&
                                                                   d.MemberPath.EndsWith(']').IsFalse()).ToImmutableList();

            if (schemaMismatchDifferences.Any())
            {
                HandleSchemaMismatch(context, schemaMismatchDifferences, expectedJson,
                                     currentResponseJson, expectedResultFile);

                return; // Early exit - no need to check values if schema doesn't match
            }

            // Check 2: Value differences (schema matches, but values differ)
            HandleValueComparison(context, expectedResponse, currentResponse,
                                  expectedResultFile);
        }

        private void HandleSchemaMismatch<TResult>(HttpResponseContext<TResult> context,
                                                   ImmutableList<Difference> schemaMismatchDifferences,
                                                   string expectedJson,
                                                   string currentResponseJson,
                                                   EmbeddedFileInfo expectedResultFile)
        {
            // Build complete snapshot test output with differences
            var schemaNotMatchingError = snapshotTestOutputBuilder.Build(context,
                                                                         schemaMismatchDifferences,
                                                                         expectedJson,
                                                                         currentResponseJson);

            // Write response file if configured
            var shouldWriteResponse = writeResponseService.ShouldWriteResponse(context.WriteResponse, context.CallingAssembly);

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

        private void HandleValueComparison<TResult>(HttpResponseContext<TResult> context,
                                                    SimpleHttpResponseMessage expectedResponse,
                                                    SimpleHttpResponseMessage currentResponse,
                                                    EmbeddedFileInfo expectedResultFile)
        {
            var expectedObjectAsJson = expectedResponse.ToJson(jsonSerializerOptions);

            // Build context for AssertService
            var objectAssertContext = new HttpResponseContext<SimpleHttpResponseMessage>
                                      {
                                          ExpectedResultFile = expectedResultFile,
                                          ExpectedObjectAsJson = expectedObjectAsJson,
                                          Current = currentResponse,
                                          OrderFunc = item => item, // Order func was executed already on the primitive type level
                                          DifferenceFunc = context.DifferenceFunc,
                                          Parameters = context.Parameters,
                                          CallingAssembly = context.CallingAssembly,
                                          WriteResponse = context.WriteResponse,
                                          Title = string.Empty,
                                          CallerFilePath = context.CallerFilePath,
                                          ExpectedResultParameterName = context.ExpectedResultParameterName,
                                          CurrentResultParameterName = context.CurrentResultParameterName,
                                          Client = context.Client,
                                          Url = context.Url,
                                          HttpMethod = context.HttpMethod,
                                          ResolvedExpectedJson = context.ResolvedExpectedJson,
                                          HttpResponseMessage = context.HttpResponseMessage,
                                          HttpStatusCode = context.HttpStatusCode,
                                          ContentAsString = context.ContentAsString,
                                          ContentAsStringParameterized = context.ContentAsStringParameterized,
                                          CurrentResult = currentResponse,
                                          IsExpectedStatusCode = context.IsExpectedStatusCode,
                                          AbsoluteUrl = context.AbsoluteUrl,
                                      };

            // Delegate to AssertService for value comparison
            assertService.ObjectsAreEqual(context);
        }

        private string BuildExpectedResultJson<TResult>(HttpResponseContext<TResult> context,
                                                        bool targetIsPrimitiveType,
                                                        TResult? filteredCurrentResult)
        {
            var expectedResultFile = context.ExpectedResultFile;

            // Build current simple HTTP response for comparison
            var simpleHttpResponseMessage = context.HttpResponseMessage.ToJson(jsonSerializerOptions).FromJsonStringAs<SimpleHttpResponseMessage>(jsonSerializerOptions);
            var contentHeaders = context.HttpResponseMessage.Content.Headers.ToJson(jsonSerializerOptions).FromJsonStringAs<ImmutableList<KeyValuePair<string, ImmutableList<string>>>>(jsonSerializerOptions);

            var currentResolvedSimpleHttpResponse = simpleHttpResponseMessage with
            {
                Content = new SimpleHttpContent
                {
                    Headers = contentHeaders,
                    Value = context.IsExpectedStatusCode ? filteredCurrentResult : context.ContentAsStringParameterized.Trim('"')
                }
            };

            string? expectedResultAsJson;

            var shouldWriteResponse = writeResponseService.ShouldWriteResponse(context.WriteResponse, context.CallingAssembly);

            if (shouldWriteResponse)
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(context.ContentAsString,
                                                                                                      context.CallingAssembly,
                                                                                                      string.Empty,
                                                                                                      context.ExpectedResultParameterName);

                if (expectedResultAsJson.IsNull())
                {
                    expectedResultAsJson = currentResolvedSimpleHttpResponse.ToJson(jsonSerializerOptions);
                }
            }
            else
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringFrom<TResult>(context.ContentAsString,
                                                                                             context.CallingAssembly,
                                                                                             string.Empty,
                                                                                             context.ExpectedResultParameterName);
            }

            return expectedResultAsJson;
        }

        private SimpleHttpResponseMessage BuildCurrentResponse<TResult>(HttpResponseContext<TResult> context,
                                                                        SimpleHttpResponseMessage simpleHttpResponseMessage,
                                                                        ImmutableList<KeyValuePair<string, ImmutableList<string>>> contentHeaders,
                                                                        TResult? filteredCurrentResult)
        {
            return simpleHttpResponseMessage with
            {
                Content = simpleHttpResponseMessage.Content.IsNull()
                                     ? new SimpleHttpContent()
                                     {
                                         Value = simpleHttpResponseMessage,
                                         Headers = ImmutableList<KeyValuePair<string, ImmutableList<string>>>.Empty
                                     }
                                     : simpleHttpResponseMessage.Content with
                                     {
                                         Value = JsonDocument.Parse(context.ContentAsString).RootElement,
                                     }
            };
        }

        private SimpleHttpResponseMessage BuildExpectedResponse<TResult>(HttpResponseContext<TResult> context,
                                                                         string expectedResultAsJsonParameterized,
                                                                         bool targetIsPrimitiveType,
                                                                         SimpleHttpResponseMessage simpleHttpResponseMessage,
                                                                         ImmutableList<KeyValuePair<string, ImmutableList<string>>> contentHeaders,
                                                                         TResult? filteredCurrentResult)
        {
            var expectedResultFile = context.ExpectedResultFile;

            // Build expected type
            var expectedType = targetIsPrimitiveType
                                   ? primitiveTypeConverter.ConvertTo<TResult>(context.ContentAsString)
                                   : expectedResultAsJsonParameterized.FromJsonStringOrDefault<TResult>(jsonSerializerOptions);

            var filteredExpectedType = expectedType.IsNotNull() ? context.OrderFunc(expectedType) : expectedType;

            var expectedResultAsSimpleResponse = expectedResultAsJsonParameterized.FromJsonStringOrDefault<SimpleHttpResponseMessage>(jsonSerializerOptions);

            // To keep the whole code compatible with existence
            if (expectedResultAsSimpleResponse.IsNull() || expectedResultAsSimpleResponse.Content.IsNull())
            {
                expectedResultAsSimpleResponse = simpleHttpResponseMessage with
                {
                    Content = new SimpleHttpContent
                    {
                        Headers = contentHeaders,
                        Value = expectedResultFile.Content.EqualsTo(IgnoreResponseComparison) ? filteredCurrentResult : filteredExpectedType
                    }
                };
            }

            // Fallback for AssertPostAsync and AssertPostAsErrorAsync
            expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { IsSuccessStatusCode = context.IsSuccessStatusCode };

            // Quick workaround
            if (expectedResultAsSimpleResponse.Content.Value.IsNull() &&
                expectedResultAsJsonParameterized.IsNotNullOrWhiteSpace())
            {
                expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { Content = expectedResultAsSimpleResponse.Content with { Value = expectedResultAsJsonParameterized } };
            }

            return expectedResultAsSimpleResponse;
        }
    }
}
