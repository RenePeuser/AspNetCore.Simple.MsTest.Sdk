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

            // Use pre-deserialized and pre-filtered data from context
            var currentResult = context.CurrentResult;
            var filteredCurrentResult = context.FilteredCurrentResult;

            // TODO: We need HttpResponseMessage in context for:
            // 1. httpResponseMessage.ToJson() -> SimpleHttpResponseMessage
            // 2. httpResponseMessage.Content.Headers
            // For now, throw NotImplementedException to discuss with user

            throw new NotImplementedException("ExpectedStatusCodeStrategy needs HttpResponseMessage in context. " +
                                              "Current context only has HttpStatusCode. " +
                                              "We need to add: " +
                                              "1. HttpResponseMessage (or SimpleHttpResponseMessage) " +
                                              "2. Content.Headers");
        }
    }
}
