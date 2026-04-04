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
            services.AddHttpResponseAssertStrategy();

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
                                               IHttpCallHandler httpCallHandler,
                                               JsonSerializerOptions jsonSerializerOptions,
                                               IParameterReplacer parameterReplacementService,
                                               IHttpResponseAssertStrategy httpResponseAssertStrategy,
                                               IPrimitiveTypeConverter primitiveTypeConverter) : IAssertableHttpClient
    {

        /// <inheritdoc />
        public async Task<TResult> AssertAsync<TResult>(HttpAssertContext<TResult> context)
        {
            // Call the endpoint using context (contains resolved URL, resolved payload, etc.)
            using var httpResponseMessage = await httpCallHandler.CallAsync(context, CancellationToken.None).ConfigureAwait(false);

            // Extract all required data from HttpResponseMessage BEFORE disposal
            // Data Collection Phase: no logic, just extraction

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

            // Extract SimpleHttpResponseMessage and ContentHeaders before disposal
            var simpleHttpResponseMessage = httpResponseMessage.ToJson(jsonSerializerOptions).FromJsonStringAs<SimpleHttpResponseMessage>(jsonSerializerOptions);
            var contentHeaders = httpResponseMessage.Content.Headers.ToJson(jsonSerializerOptions).FromJsonStringAs<ImmutableList<KeyValuePair<string, ImmutableList<string>>>>(jsonSerializerOptions);

            // Deserialize current result (needed for some strategies)
            var targetType = typeof(TResult);
            var targetIsPrimitiveType = targetType.IsPrimitive || targetType.EqualsTo(typeof(string));
            var currentResult = targetIsPrimitiveType ? primitiveTypeConverter.ConvertTo<TResult>(resolvedParametersJsonString) :
                                resolvedParametersJsonString.IsNullOrWhiteSpace() ? "{}".FromJsonStringAs<TResult>(jsonSerializerOptions) :
                                resolvedParametersJsonString.FromJsonStringAs<TResult>(jsonSerializerOptions);

            // Execute filter func
            var filteredCurrentResult = context.OrderFunc(currentResult);

            // Check if status code matches expectations
            var isExpectedStatusCode = httpResponseMessage.IsSuccessStatusCode == context.IsSuccessStatusCode;

            // Build HttpResponseContext with all extracted data
            var responseContext = new HttpResponseContext<TResult>
            {
                Request = context,
                HttpStatusCode = httpResponseMessage.StatusCode,
                ContentAsString = contentAsString,
                ResolvedParametersJsonString = resolvedParametersJsonString,
                IsExpectedStatusCode = isExpectedStatusCode,
                CurrentResult = currentResult,
                FilteredCurrentResult = filteredCurrentResult,
                SimpleHttpResponseMessage = simpleHttpResponseMessage,
                ContentHeaders = contentHeaders,
                AbsoluteUrl = absoluteUrl,
                HttpCallInfo = httpCallInfo,
                Curl = curl
            };

            // Delegate to strategy for processing
            var result = await httpResponseAssertStrategy.AssertAsync(responseContext).ConfigureAwait(false);

            return result;
        }
    }
}
