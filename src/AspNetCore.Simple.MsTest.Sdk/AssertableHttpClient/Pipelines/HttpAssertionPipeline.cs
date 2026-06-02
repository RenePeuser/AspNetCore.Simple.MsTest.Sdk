using System.Collections.Generic;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    public static class AddHttpAssertionPipelineExtension
    {
        /// <summary>
        /// Registers the HTTP assertion pipeline and its steps.
        /// Steps are executed in the order they are registered.
        /// </summary>
        public static void AddHttpAssertionPipeline(this IServiceCollection services)
        {
            // Register steps in execution order
            services.AddStatusCodeValidationStep(); // 1. Status code must match expectations
            services.AddContentTypeHeaderValidationStep(); // 2. Content-Type header must be application/json
            services.AddContentFormatValidationStep(); // 3. Content body must be valid JSON structure
            services.AddJsonComparisonStep(); // 4. JSON comparison (schema + values)
            services.AddSuccessfulTestCurlPrinter(); // 5. Print curl command for successful tests

            // Register the pipeline itself
            services.AddSingletonIfNotExists<IHttpAssertionPipeline, HttpAssertionPipeline>();
        }
    }

    /// <summary>
    /// Pipeline that executes HTTP assertion steps sequentially.
    /// Each step validates a specific aspect of the HTTP response.
    /// The pipeline stops at the first failing step (Assert.That.Fail throws an exception).
    /// </summary>
    internal sealed class HttpAssertionPipeline(IEnumerable<IHttpAssertionStep> steps) : IHttpAssertionPipeline
    {
        /// <inheritdoc />
        public TResult Execute<TResult>(HttpResponseContext<TResult> context)
        {
            // Execute each step in sequence
            // If a step calls Assert.That.Fail(), execution stops immediately (exception is thrown)
            // Steps are validators only - they don't modify or return results
            foreach (var step in steps)
            {
                step.Execute(context);
            }

            // All assertions passed - return the original deserialized result
            return context.CurrentResult!;
        }
    }
}