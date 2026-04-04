using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddUnexpectedStatusCodeStrategyExtension
    {
        /// <summary>
        /// Registers the unexpected status code strategy and its dependencies.
        /// </summary>
        public static void AddUnexpectedStatusCodeStrategy(this IServiceCollection services)
        {
            // 1. Register dependencies
            services.AddOutputFormatter();

            // 2. Register the strategy itselfyes
            services.AddSingletonIfNotExists<IHttpStatusCodeProcessingStrategy, UnexpectedStatusCodeStrategy>();
        }
    }

    /// <summary>
    /// Strategy for handling unexpected HTTP status codes.
    /// Fast-fail strategy: immediately fails the test without further processing or response writing.
    /// </summary>
    internal sealed class UnexpectedStatusCodeStrategy(IOutputFormatter outputFormatter) : IHttpStatusCodeProcessingStrategy
    {
        /// <inheritdoc />
        public bool CanHandle<TResult>(HttpResponseContext<TResult> context)
        {
            var canHandle = (context.Request.IsSuccessStatusCode && context.IsExpectedStatusCode.IsFalse()) ||
                            (context.Request.IsSuccessStatusCode.IsFalse() && context.IsExpectedStatusCode);

            return canHandle;
        }

        /// <inheritdoc />
        public Task<TResult> ProcessAsync<TResult>(HttpResponseContext<TResult> context)
        {
            // Build error message based on expectation
            var errorInfo = context.Request.IsSuccessStatusCode
                                ? $"You expect an OK result but the response was {context.HttpStatusCode}. Please check implementation or your expected response"
                                : $"You expect an ERROR result but the response was {context.HttpStatusCode}. Please check implementation or your expected response";

            // Get expected result for comparison (simplified, no complex processing)
            var simpleExpectedResult = context.Request.ExpectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(context.ContentAsString,
                                                                                                                      context.Request.CallingAssembly,
                                                                                                                      string.Empty,
                                                                                                                      context.Request.ExpectedResultParameterName);

            // Format output for assertion failure
            var schemaNotMatchingError = outputFormatter.GetOutputString(string.Empty,
                                                                         errorInfo,
                                                                         simpleExpectedResult,
                                                                         context.ContentAsString,
                                                                         string.Empty,
                                                                         string.Empty);

            // Fail immediately - no response writing, no further processing
            Assert.Fail(schemaNotMatchingError);

            // This line is never reached but required for compilation
            return Task.FromResult<TResult>(default!);
        }
    }
}
