using System.Collections.Immutable;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
#pragma warning disable IDE0060 // Remove unused parameter
    internal sealed class AssertableHttpClient(IHttpCallHandler httpCallHandler,
                                               IParameterReplacer parameterReplacementService,
                                               IHttpAssertionPipeline httpAssertionPipeline,
                                               IPrimitiveTypeConverter primitiveTypeConverter,
                                               JsonSerializerOptions jsonSerializerOptions,
                                               IEndpointValidator endpointValidator) : IAssertableHttpClient
#pragma warning restore IDE0060 // Remove unused parameter
    {
        /// <inheritdoc />
        public async Task<TResult> AssertAsync<TResult>(HttpAssertContext<TResult> context)
        {
            // 1. Validate endpoint request to real world
            endpointValidator.Validate<TResult>(context);

            // 2. Call the endpoint using context (contains resolved URL, resolved payload, etc.)
            using var httpResponseMessage = await httpCallHandler.CallAsync(context, CancellationToken.None).ConfigureAwait(false);

            // 3. Minimal data preparation
            var contentAsString = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var resolvedParametersJsonString = parameterReplacementService.ResolveParameters(contentAsString, context.Parameters);
            var absoluteUrl = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri ?? string.Empty;
            var isExpectedStatusCode = httpResponseMessage.IsSuccessStatusCode == context.IsSuccessStatusCode;

            // 4. Deserialize the response to TResult (this is what the user gets back - never modified!)
            var targetType = typeof(TResult);
            var targetIsPrimitiveType = targetType.IsPrimitive || targetType.EqualsTo(typeof(string));

            var currentResult = targetIsPrimitiveType
                                    ? primitiveTypeConverter.ConvertTo<TResult>(resolvedParametersJsonString)
                                    : resolvedParametersJsonString.IsNullOrWhiteSpace()
                                        ? "{}".FromJsonStringAs<TResult>(jsonSerializerOptions)
                                        : resolvedParametersJsonString.FromJsonStringAs<TResult>(jsonSerializerOptions);

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
                                      ExpectedObjectAsJson = context.ExpectedObjectAsJson,
                                      ExpectedResultFile = context.ExpectedResultFile,
                                      ExpectedResultParameterName = context.ExpectedResultParameterName,
                                      HttpMethod = context.HttpMethod,
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
                                  };

            // Delegate to pipeline - steps only validate, never modify the result
            // Pipeline returns context.CurrentResult (the original deserialized response)
            var result = httpAssertionPipeline.Execute(responseContext);

            return result;
        }
    }
}
