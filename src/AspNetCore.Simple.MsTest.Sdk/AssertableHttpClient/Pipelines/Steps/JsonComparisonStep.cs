using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
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
            services.AddAssertService();
            services.AddParameterReplacer();
            services.AddWriteResponseService();
            services.AddJsonSerializer();

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, JsonComparisonStep>();
        }
    }

    /// <summary>
    /// Compares the JSON response against expected JSON.
    /// Prepares HTTP-specific structures (SimpleHttpResponseMessage) and delegates comparison to AssertService.
    /// Flow: Build HTTP wrapper structures → Delegate to AssertService
    /// </summary>
    internal sealed class JsonComparisonStep(IPrimitiveTypeConverter primitiveTypeConverter,
                                             IAssertService assertService,
                                             IParameterReplacer parameterReplacementService,
                                             IWriteResponseService writeResponseService,
                                             TestSdkSettings testSdkSettings,
                                             JsonSerializerOptions apiJsonSerializerOptions) : IHttpAssertionStep
    {
        private const string IgnoreResponseComparison = "IgnoreResponse";

        /// <summary>
        /// The current side of the diff is the raw response body, so nothing on this side may rename
        /// anything the api did not rename itself - see <see cref="ComparisonJsonOptions"/>.
        /// </summary>
        private readonly JsonSerializerOptions _jsonSerializerOptions = apiJsonSerializerOptions.ForComparison();

        /// <inheritdoc />
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            var expectedResultFile = context.ExpectedResultFile;
            var targetIsPrimitiveType = context.TypeIsPrimitiveType;

            // Get current result from context (already deserialized in AssertableHttpClient)
            var currentResult = context.CurrentResult;

            // Apply filter function to get filtered result for comparison
            var filteredCurrentResult = context.OrderFunc.IsNull() ? currentResult : context.OrderFunc(currentResult);

            // Build expected result JSON
            var expectedResultAsJson = BuildExpectedResultJson(context, filteredCurrentResult);
            var expectedResultAsJsonParameterized = parameterReplacementService.ResolveParameters(expectedResultAsJson, context.Parameters);

            // Skip comparison if IgnoreResponse flag is set or marker is present
            if (context.IgnoreResponse ||
                expectedResultAsJsonParameterized.IsNullOrWhiteSpace() ||
                expectedResultAsJsonParameterized.Contains(IgnoreResponseComparison))
            {
                return;
            }

            // Build SimpleHttpResponseMessage from HttpResponseMessage (needed for snapshot comparison)
            // Volatile headers are stripped here, before anything is compared or recorded - see
            // VolatileHeaderFilter.
            var simpleHttpResponseMessage = context.HttpResponseMessage.ToJson(_jsonSerializerOptions)
                                                   .FromJsonStringAs<SimpleHttpResponseMessage>(_jsonSerializerOptions)
                                                   .WithoutVolatileHeaders(testSdkSettings);

            var contentHeaders = context.HttpResponseMessage.Content.Headers.ToJson(_jsonSerializerOptions)
                                        .FromJsonStringAs<ImmutableList<KeyValuePair<string, ImmutableList<string>>>>(_jsonSerializerOptions)
                                        .WithoutVolatileHeaders(testSdkSettings);

            // Build HTTP-specific comparison structures
            var currentResponse = BuildCurrentResponse(context, simpleHttpResponseMessage, filteredCurrentResult);

            var expectedResponse = BuildExpectedResponse(context, expectedResultAsJsonParameterized, targetIsPrimitiveType,
                                                         simpleHttpResponseMessage, contentHeaders, filteredCurrentResult);

            // Prepare expected JSON for AssertService
            // For HttpResponse asserts we need both: the object AND the JSON (for WriteResponse/debugging)
            var expectedJson = expectedResponse.ToJson(_jsonSerializerOptions);

            // Build context for AssertService and delegate all comparison logic
            // Optimization: Pass both expectedResponse object AND expectedJson for flexibility
            SdkTrace.WriteLine($"[JsonComparisonStep] context.IsEmptyAnonymousObjectForCodeGeneration={context.IsEmptyAnonymousObjectForCodeGeneration}");

            var objectAssertContext = new HttpResponseContext<SimpleHttpResponseMessage>
            {
                ApiVersion = context.ApiVersion,
                AbsoluteUrl = context.AbsoluteUrl,
                CallerFilePath = context.CallerFilePath,
                CallerLineNumber = context.CallerLineNumber,
                CallerMemberName = context.CallerMemberName,
                CallingAssembly = context.CallingAssembly,
                Client = context.Client,
                ContentAsString = context.ContentAsString,
                ContentAsStringParameterized = context.ContentAsStringParameterized,
                Current = currentResponse,
                CurrentObject = currentResponse,
                CurrentResult = currentResponse,
                CurrentResultParameterName = context.CurrentResultParameterName,
                DifferenceFunc = context.DifferenceFunc,
                DifferenceFilter = context.DifferenceFilter,
                Expected = expectedResponse, // Direct object - avoids deserialize step in comparison
                ExpectedType = typeof(SimpleHttpResponseMessage),
                ExpectedObjectAsJson = expectedJson, // Keep JSON for WriteResponse
                ExpectedResultFile = expectedResultFile,
                ExpectedResultParameterName = context.ExpectedResultParameterName,
                HttpMethod = context.HttpMethod,
                IgnoreResponse = context.IgnoreResponse,
                HttpResponseMessage = context.HttpResponseMessage,
                HttpStatusCode = context.HttpStatusCode,
                IsExpectedStatusCode = context.IsExpectedStatusCode,
                IsSuccessStatusCode = context.IsExpectedStatusCode,
                // No OrderFunc: it already ran on the primitive type level, one level up.
                Parameters = context.Parameters,
                PayloadAsJson = context.PayloadAsJson,
                PayloadFile = context.PayloadFile,
                PayloadParameterName = context.PayloadParameterName,
                ResolvedExpectedJson = expectedJson,
                ResolvedPayload = context.ResolvedPayload,
                ShowTokenInCurl = context.ShowTokenInCurl,
                TypeIsPrimitiveType = targetIsPrimitiveType,
                Url = context.Url,
                WriteResponse = context.WriteResponse,
                SkipEndpointValidation = context.SkipEndpointValidation,
                ExpectedHttpStatusCode = context.ExpectedHttpStatusCode,

                // Copy failure type from original context (if already set by earlier pipeline steps)
                FailureType = context.FailureType,
                ExpectedStatusCode = context.ExpectedStatusCode,
                ActualStatusCode = context.ActualStatusCode,

                // PROTOTYPE: Copy code generation flag
                IsEmptyAnonymousObjectForCodeGeneration = context.IsEmptyAnonymousObjectForCodeGeneration
            };

            // Delegate to AssertService - it handles schema checks, value comparison, diff finding, and output building
            // Note: Exception handling (including TestSdkProblemDetailsException) is now done globally in AssertableHttpClient.AssertAsync
            assertService.ObjectsAreEqual(objectAssertContext);
        }

        private string BuildExpectedResultJson<TResult>(HttpResponseContext<TResult> context,
                                                        TResult? filteredCurrentResult)
        {
            var expectedResultFile = context.ExpectedResultFile;

            // Build current simple HTTP response for comparison
            var simpleHttpResponseMessage = context.HttpResponseMessage.ToJson(_jsonSerializerOptions).FromJsonStringAs<SimpleHttpResponseMessage>(_jsonSerializerOptions);
            var contentHeaders = context.HttpResponseMessage.Content.Headers.ToJson(_jsonSerializerOptions).FromJsonStringAs<ImmutableList<KeyValuePair<string, ImmutableList<string>>>>(_jsonSerializerOptions);

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
                                                                                                      context.ExpectedResultParameterName,
                                                                                                      context.CallerFilePath,
                                                                                                      context.CallerLineNumber,
                                                                                                      context.CallerMemberName);

                if (expectedResultAsJson.IsNull())
                {
                    expectedResultAsJson = currentResolvedSimpleHttpResponse.ToJson(_jsonSerializerOptions);
                }
            }
            else
            {
                expectedResultAsJson = expectedResultFile.Content.GetJsonStringFrom<TResult>(context.ContentAsString,
                                                                                             context.CallingAssembly,
                                                                                             context.ExpectedResultParameterName,
                                                                                             context.CallerFilePath,
                                                                                             context.CallerLineNumber,
                                                                                             context.CallerMemberName);
            }

            return expectedResultAsJson;
        }

        /// <summary>
        /// Builds the current side of the diff.
        ///
        /// Default is the raw response body: the expected side is rebuilt from the response type, so
        /// keeping this side untyped is what still surfaces a property the api returns but the type does
        /// not model.
        ///
        /// An order func flips that. It normalizes volatile data - an execution arn, a start date, a name
        /// carrying a timestamp - and <see cref="BuildExpectedResponse{TResult}"/> applies it to the
        /// expected side. Leaving the raw body here would compare normalized against volatile and the
        /// assert could never pass, no matter how often the snapshot is re-recorded. So when the caller
        /// supplied one, both sides go through the normalized object.
        /// </summary>
        private SimpleHttpResponseMessage BuildCurrentResponse<TResult>(HttpResponseContext<TResult> context,
                                                                        SimpleHttpResponseMessage simpleHttpResponseMessage,
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
                                         Value = BuildCurrentValue(context, filteredCurrentResult)
                                     }
            };
        }

        private object BuildCurrentValue<TResult>(HttpResponseContext<TResult> context,
                                                  TResult? filteredCurrentResult)
        {
            if (context.OrderFunc.IsNotNull() && filteredCurrentResult.IsNotNull())
            {
                return filteredCurrentResult;
            }

            return context.ContentAsString.IsNullOrWhiteSpace()
                       ? "{}"
                       : JsonDocument.Parse(context.ContentAsString).RootElement;
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
                                   ? primitiveTypeConverter.ConvertTo<TResult>(expectedResultAsJsonParameterized)
                                   : expectedResultAsJsonParameterized.FromJsonStringOrDefault<TResult>(_jsonSerializerOptions);

            var filteredExpectedType = expectedType.IsNotNull() && context.OrderFunc.IsNotNull() ? context.OrderFunc(expectedType) : expectedType;

            var expectedResultAsSimpleResponse = expectedResultAsJsonParameterized.FromJsonStringOrDefault<SimpleHttpResponseMessage>(_jsonSerializerOptions);

            // A snapshot recorded before volatile headers were filtered still carries them. Dropping
            // them on this side too is what keeps those snapshots green instead of failing on a
            // traceparent that can never match again.
            expectedResultAsSimpleResponse = expectedResultAsSimpleResponse?.WithoutVolatileHeaders(testSdkSettings);

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