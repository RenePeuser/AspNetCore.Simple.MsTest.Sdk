using System.Collections.Immutable;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddOutputFormatter();
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
    /// This step merges the logic from SchemaValidationStep and ValueComparisonStep.
    /// Flow: Diff once → Check schema first → Then check values
    /// </summary>
    internal sealed class JsonComparisonStep(IPrimitiveTypeConverter primitiveTypeConverter,
                                             IJsonDiffer jsonDiffer,
                                             IOutputFormatter outputFormatter,
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
            var expectedResultFile = context.Request.ExpectedResultFile;
            var targetType = typeof(TResult);
            var targetIsPrimitiveType = targetType.IsPrimitive || targetType.EqualsTo(typeof(string));

            // Get current result from context (already deserialized in AssertableHttpClient)
            var currentResult = context.CurrentResult;

            // Build expected result JSON
            var expectedResultAsJson = BuildExpectedResultJson(context, targetIsPrimitiveType);
            var expectedResultAsJsonParameterized = parameterReplacementService.ResolveParameters(expectedResultAsJson, context.Request.Parameters);

            // Skip comparison if IgnoreResponse marker is present
            if (expectedResultAsJsonParameterized.IsNullOrWhiteSpace() ||
                expectedResultAsJsonParameterized.Contains(IgnoreResponseComparison))
            {
                return;
            }

            // Build comparison structures
            var currentResponse = BuildCurrentResponse(context);
            var expectedResponse = BuildExpectedResponse(context, expectedResultAsJsonParameterized, targetIsPrimitiveType);

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

                return default; // Early exit - no need to check values if schema doesn't match
            }

            // Check 2: Value differences (schema matches, but values differ)
            HandleValueComparison(context, expectedResponse, currentResponse,
                                  expectedResultFile, targetIsPrimitiveType, expectedResultAsJsonParameterized);
        }

        private void HandleSchemaMismatch<TResult>(HttpResponseContext<TResult> context,
                                                   ImmutableList<Difference> schemaMismatchDifferences,
                                                   string expectedJson,
                                                   string currentResponseJson,
                                                   EmbeddedFileInfo expectedResultFile)
        {
            var differenceOutputTable = schemaMismatchDifferences.ToResultTable(context.Request.ExpectedResultParameterName, "Current");

            var schemaNotMatchingError = outputFormatter.GetOutputString(string.Empty,
                                                                         "Schema mismatch: Expected result and current result does not match",
                                                                         expectedJson,
                                                                         currentResponseJson,
                                                                         differenceOutputTable,
                                                                         string.Empty);

            // Write response file if configured
            var shouldWriteResponse = writeResponseService.ShouldWriteResponse(context.Request.WriteResponse, context.Request.CallingAssembly);

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

        private void HandleValueComparison<TResult>(HttpResponseContext<TResult> context,
                                                    SimpleHttpResponseMessage expectedResponse,
                                                    SimpleHttpResponseMessage currentResponse,
                                                    EmbeddedFileInfo expectedResultFile,
                                                    bool targetIsPrimitiveType,
                                                    string expectedResultAsJsonParameterized)
        {
            var expectedObjectAsJson = expectedResponse.ToJson(jsonSerializerOptions);

            // Build context for AssertService
            var objectAssertContext = new HttpAssertContext<SimpleHttpResponseMessage>
                                      {
                                          ExpectedResultFile = expectedResultFile,
                                          ExpectedObjectAsJson = expectedObjectAsJson,
                                          Current = currentResponse,
                                          OrderFunc = item => item, // Order func was executed already on the primitive type level
                                          DifferenceFunc = context.Request.DifferenceFunc,
                                          Parameters = context.Request.Parameters,
                                          CallingAssembly = context.Request.CallingAssembly,
                                          WriteResponse = context.Request.WriteResponse,
                                          Title = string.Empty, // TODO: Need to add HttpCallInfo to context
                                          CallerFilePath = context.Request.CallerFilePath,
                                          ExpectedResultParameterName = context.Request.ExpectedResultParameterName,
                                          CurrentResultParameterName = context.Request.CurrentResultParameterName,
                                          Client = context.Request.Client,
                                          Url = context.Request.Url,
                                          HttpMethod = context.Request.HttpMethod,
                                      };

            // Delegate to AssertService for value comparison
            assertService.ObjectsAreEqual(objectAssertContext);
        }

        private string BuildExpectedResultJson<TResult>(HttpResponseContext<TResult> context,
                                                        bool targetIsPrimitiveType)
        {
            var expectedResultFile = context.Request.ExpectedResultFile;

            // Build current simple HTTP response for comparison
            var currentSimpleHttpResponseMessage = context.SimpleHttpResponseMessage;

            var currentResolvedSimpleHttpResponse = currentSimpleHttpResponseMessage with
                                                    {
                                                        Content = new SimpleHttpContent
                                                                  {
                                                                      Headers = context.ContentHeaders,
                                                                      Value = context.IsExpectedStatusCode ? context.FilteredCurrentResult : context.ResolvedParametersJsonString.Trim('"')
                                                                  }
                                                    };

            string? expectedResultAsJson;

            var shouldWriteResponse = writeResponseService.ShouldWriteResponse(context.Request.WriteResponse, context.Request.CallingAssembly);

            if (shouldWriteResponse)
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(context.ContentAsString,
                                                                                                      context.Request.CallingAssembly,
                                                                                                      string.Empty,
                                                                                                      context.Request.ExpectedResultParameterName);

                if (expectedResultAsJson.IsNull())
                {
                    expectedResultAsJson = currentResolvedSimpleHttpResponse.ToJson(jsonSerializerOptions);
                }
            }
            else
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringFrom<TResult>(context.ContentAsString,
                                                                                             context.Request.CallingAssembly,
                                                                                             string.Empty,
                                                                                             context.Request.ExpectedResultParameterName);
            }

            return expectedResultAsJson;
        }

        private SimpleHttpResponseMessage BuildCurrentResponse<TResult>(HttpResponseContext<TResult> context)
        {
            var currentSimpleHttpResponseMessage = context.SimpleHttpResponseMessage;

            return currentSimpleHttpResponseMessage with
                   {
                       Content = currentSimpleHttpResponseMessage.Content.IsNull()
                                     ? new SimpleHttpContent()
                                       {
                                           Value = currentSimpleHttpResponseMessage,
                                           Headers = ImmutableList<KeyValuePair<string, ImmutableList<string>>>.Empty
                                       }
                                     : currentSimpleHttpResponseMessage.Content with
                                       {
                                           Value = JsonDocument.Parse(context.ContentAsString).RootElement,
                                       }
                   };
        }

        private SimpleHttpResponseMessage BuildExpectedResponse<TResult>(HttpResponseContext<TResult> context,
                                                                         string expectedResultAsJsonParameterized,
                                                                         bool targetIsPrimitiveType)
        {
            var expectedResultFile = context.Request.ExpectedResultFile;
            var currentSimpleHttpResponseMessage = context.SimpleHttpResponseMessage;

            // Build expected type
            var expectedType = targetIsPrimitiveType
                                   ? primitiveTypeConverter.ConvertTo<TResult>(context.ContentAsString)
                                   : expectedResultAsJsonParameterized.FromJsonStringOrDefault<TResult>(jsonSerializerOptions);

            var filteredExpectedType = expectedType.IsNotNull() ? context.Request.OrderFunc(expectedType) : expectedType;

            var expectedResultAsSimpleResponse = expectedResultAsJsonParameterized.FromJsonStringOrDefault<SimpleHttpResponseMessage>(jsonSerializerOptions);

            // To keep the whole code compatible with existence
            if (expectedResultAsSimpleResponse.IsNull() || expectedResultAsSimpleResponse.Content.IsNull())
            {
                expectedResultAsSimpleResponse = currentSimpleHttpResponseMessage with
                                                 {
                                                     Content = new SimpleHttpContent
                                                               {
                                                                   Headers = context.ContentHeaders,
                                                                   Value = expectedResultFile.Content.EqualsTo(IgnoreResponseComparison) ? context.FilteredCurrentResult : filteredExpectedType
                                                               }
                                                 };
            }

            // Fallback for AssertPostAsync and AssertPostAsErrorAsync
            expectedResultAsSimpleResponse = expectedResultAsSimpleResponse with { IsSuccessStatusCode = context.Request.IsSuccessStatusCode };

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
