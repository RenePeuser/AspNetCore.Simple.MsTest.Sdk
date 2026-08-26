using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.ErrorHandling;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    public static class AddAssertableHttpClientExtension
    {
        /// <summary>
        /// Registers all assertable HTTP client services and their dependencies in the DI container.
        /// Feature-based registration following the dependency tree pattern.
        /// </summary>
        public static void AddAssertableHttpClient(this IServiceCollection services,
                                                   IConfiguration configuration)
        {
            // 1. Register all dependencies via their own extensions
            services.AddTextDecorator();
            services.AddHttpOutputFormatter();
            services.AddCurlBuilder();
            services.AddPrimitiveTypeConverter();
            services.AddJsonDiffer();
            services.AddResponseWriter();
            services.AddWriteResponseService();
            services.AddHttpCallHandler();
            services.AddAssertService();
            services.AddParameterReplacer();
            services.AddHttpAssertionPipeline();
            services.AddEmbeddedFileLocalizer(configuration);
            services.AddApiVersionResolver();
            services.AddEndpointValidator();
            services.AddEmptyAnonymousObjectDetector();

            // 2. Register error handling strategy (with all specific handlers)
            services.AddTestErrorHandlingStrategy();

            // 3. Register the service itself
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
#pragma warning disable IDE0060 // Remove unused parameter
    internal sealed class AssertableHttpClient(IHttpCallHandler httpCallHandler,
                                               IParameterReplacer parameterReplacementService,
                                               IHttpAssertionPipeline httpAssertionPipeline,
                                               IPrimitiveTypeConverter primitiveTypeConverter,
                                               JsonSerializerOptions jsonSerializerOptions,
                                               IEndpointValidator endpointValidator,
                                               IWriteResponseService writeResponseService,
                                               ITestErrorHandlingStrategy testErrorHandlingStrategy) : IAssertableHttpClient
#pragma warning restore IDE0060 // Remove unused parameter
    {
        /// <inheritdoc />
        public async Task<TResult> AssertAsync<TResult>(HttpAssertContext<TResult> context)
        {
            try
            {
                var result = await AssertInternalAsync(context).ConfigureAwait(false);

                return result;
            }
            catch (AssertFailedException) // If assert failed that is fine
            {
                throw;
            }
#pragma warning disable CA1031
            catch (Exception exception)
#pragma warning restore CA1031
            {
                // GLOBAL EXCEPTION HANDLER USING STRATEGY PATTERN
                // Delegate exception handling to the error handling strategy
                // The strategy will find the appropriate handler (ProblemDetailsErrorHandler, DefaultErrorHandler, etc.)
                // and return a formatted error message
                var errorOutput = await testErrorHandlingStrategy.HandleAsync(context, exception).ConfigureAwait(false);

                Assert.That.Fail(errorOutput);

                throw; // Never reached, but required for compiler
            }
        }

        private async Task<TResult> AssertInternalAsync<TResult>(HttpAssertContext<TResult> context)
        {
            // 0. A payload or snapshot reference that resolved to nothing must never travel further:
            //    EmbeddedFileInfo.Content then still holds the file NAME, and the downstream lookup
            //    matches manifest names by substring - "Persons.json" binds to "GetAllPersons.json"
            //    and the test goes green against a foreign snapshot.
            EnsureReferencedFilesExist(context);

            // A file that exists but is not parseable json must say so. Otherwise the shape checks look
            // at the first character only and report a structure mismatch for a plain syntax error.
            EnsureReferencedFilesAreParseable(context);

            // 1. Validate endpoint request to real world
            endpointValidator.Validate<TResult>(context);

            // 2. Call the endpoint using context (contains resolved URL, resolved payload, etc.)
            using var httpResponseMessage = await httpCallHandler.CallAsync(context, CancellationToken.None).ConfigureAwait(false);

            // 3. Minimal data preparation
            var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var resolvedParametersJsonString = parameterReplacementService.ResolveParameters(contentAsString, context.Parameters);
            var absoluteUrl = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri ?? string.Empty;

            // Check if status code matches expectations:
            // 1. If ExpectedHttpStatusCode is explicitly set (not null), check exact match
            // 2. Otherwise, fallback to category check (2xx vs 4xx/5xx) for backward compatibility
            var actualStatusCode = (int)httpResponseMessage.StatusCode;

            var isExpectedStatusCode = context.ExpectedHttpStatusCode.HasValue
                                           ? actualStatusCode == (int)context.ExpectedHttpStatusCode.Value
                                           : httpResponseMessage.IsSuccessStatusCode == context.IsSuccessStatusCode;

            // 4. Deserialize the response to TResult (this is what the user gets back - never modified!)
            var targetType = typeof(TResult);
            var targetIsPrimitiveType = targetType.IsPrimitive || targetType.EqualsTo(typeof(string));

            TResult? currentResult;
            var deserializationFailed = false;

            try
            {
                currentResult = targetIsPrimitiveType
                                    ? primitiveTypeConverter.ConvertTo<TResult>(resolvedParametersJsonString)
                                    : resolvedParametersJsonString.IsNullOrWhiteSpace()
                                        ? "{}".FromJsonStringAs<TResult>(jsonSerializerOptions)
                                        : resolvedParametersJsonString.FromJsonStringAs<TResult>(jsonSerializerOptions);
            }
            catch (JsonException)
            {
                // Deserialization failed - set flag and continue with default value
                // The pipeline (StatusCodeValidationStep) will handle the error if status code is wrong
                // Otherwise, JsonComparisonStep will catch it
                deserializationFailed = true;
                currentResult = default;
            }

            // Build context with deserialized result - HttpResponseMessage stays alive until pipeline completes
            var responseContext = new HttpResponseContext<TResult>
            {
                AbsoluteUrl = absoluteUrl,
                ApiVersion = context.ApiVersion,
                CallerFilePath = context.CallerFilePath,
                CallerLineNumber = context.CallerLineNumber,
                CallerMemberName = context.CallerMemberName,
                CallingAssembly = context.CallingAssembly,
                Client = context.Client,
                ContentAsString = contentAsString,
                ContentAsStringParameterized = resolvedParametersJsonString,
                Current = currentResult,
                CurrentObject = currentResult,
                CurrentResult = currentResult,
                CurrentResultParameterName = context.CurrentResultParameterName,
                DifferenceFunc = context.DifferenceFunc,
                DifferenceFilter = context.DifferenceFilter,
                ExpectedType = context.ExpectedType,
                ExpectedObjectAsJson = context.ExpectedObjectAsJson,
                ExpectedResultFile = context.ExpectedResultFile,
                ExpectedResultParameterName = context.ExpectedResultParameterName,
                HttpMethod = context.HttpMethod,
                IgnoreResponse = context.IgnoreResponse,
                HttpResponseMessage = httpResponseMessage,
                HttpStatusCode = httpResponseMessage.StatusCode,
                IsExpectedStatusCode = isExpectedStatusCode,
                IsSuccessStatusCode = context.IsSuccessStatusCode,
                OrderFunc = context.OrderFunc,
                Parameters = context.Parameters,
                PayloadAsJson = context.PayloadAsJson,
                PayloadFile = context.PayloadFile,
                PayloadParameterName = context.PayloadParameterName,
                ResolvedExpectedJson = context.ResolvedExpectedJson,
                ResolvedPayload = context.ResolvedPayload,
                ShowTokenInCurl = context.ShowTokenInCurl,
                TypeIsPrimitiveType = targetIsPrimitiveType,
                Url = context.Url,
                WriteResponse = context.WriteResponse,
                SkipEndpointValidation = context.SkipEndpointValidation,
                ExpectedHttpStatusCode = context.ExpectedHttpStatusCode,
                FailureType = HttpAssertionFailureType.None,
                ExpectedStatusCode = (int?)context.ExpectedHttpStatusCode,
                ActualStatusCode = null,
                Expected = context.Expected,
                IsEmptyAnonymousObjectForCodeGeneration = context.IsEmptyAnonymousObjectForCodeGeneration
            };

            // Delegate to pipeline - steps only validate, never modify the result
            // The pipeline will catch status code mismatches and other issues BEFORE we check deserialization
            // Pipeline returns context.CurrentResult (the original deserialized response)
            var result = httpAssertionPipeline.Execute(responseContext);

            // If deserialization failed but we got past the pipeline, throw now with full context
            // This should only happen if status code was correct but JSON structure was wrong
            if (deserializationFailed)
            {
                throw new JsonSerializationContextException(responseContext);
            }

            return result;
        }

        private static void EnsureReferencedFilesAreParseable<TResult>(HttpAssertContext<TResult> context)
        {
            // Validate the PARAMETER RESOLVED content. A parameterized snapshot legitimately carries
            // bare placeholders ("age": $Age$) which are not json until the parameters are applied.
            EnsureParseable(context.PayloadFile, context.ResolvedPayload, isPayload: true);
            EnsureParseable(context.ExpectedResultFile, context.ResolvedExpectedJson, isPayload: false);
        }

        private static void EnsureParseable(EmbeddedFileInfo? file,
                                            string? resolvedContent,
                                            bool isPayload)
        {
            // Only content that came from a *.json file is checked - inline json and text snapshots are
            // not this method's business, and an unresolved reference was already rejected above.
            if (file.IsNull() ||
                file.Resolved.IsFalse() ||
                resolvedContent.IsNullOrWhiteSpace() ||
                file.EmbeddedFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase).IsFalse())
            {
                return;
            }

            try
            {
                // System.Text.Json is strict per RFC 8259 and reports line and position, where
                // Newtonsoft silently accepts several malformed shapes.
                using var _ = JsonDocument.Parse(resolvedContent);
            }
            catch (JsonException exception)
            {
                throw new InvalidSnapshotJsonException(file.EmbeddedFileName,
                                                       file.EmbeddedFile?.FullName,
                                                       resolvedContent!,
                                                       isPayload,
                                                       exception);
            }
        }

        private void EnsureReferencedFilesExist<TResult>(HttpAssertContext<TResult> context)
        {
            // A payload can never be created on the fly - it is input, not a recording.
            if (context.PayloadFile.IsNotNull() && context.PayloadFile.Resolved.IsFalse())
            {
                throw new SnapshotNotFoundException(context.PayloadAsJson ?? string.Empty,
                                                    context.PayloadParameterName,
                                                    context.CallingAssembly,
                                                    isPayload: true);
            }

            // A missing snapshot is legitimate while recording - that is what write response is for.
            if (context.ExpectedResultFile.Resolved.IsFalse() &&
                writeResponseService.ShouldWriteResponse(context.WriteResponse, context.CallingAssembly).IsFalse())
            {
                throw new SnapshotNotFoundException(context.ExpectedObjectAsJson,
                                                    context.ExpectedResultParameterName,
                                                    context.CallingAssembly,
                                                    isPayload: false);
            }
        }
    }
}