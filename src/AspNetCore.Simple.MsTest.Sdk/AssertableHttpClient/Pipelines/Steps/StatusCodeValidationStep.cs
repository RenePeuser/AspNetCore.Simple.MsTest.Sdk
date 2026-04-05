using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    public static class AddStatusCodeValidationStepExtension
    {
        /// <summary>
        /// Registers the status code validation step and its dependencies.
        /// </summary>
        public static void AddStatusCodeValidationStep(this IServiceCollection services)
        {
            // 1. Register dependencies
            services.AddOutputFormatter();

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, StatusCodeValidationStep>();
        }
    }

    /// <summary>
    /// Validates that the HTTP status code matches expectations.
    /// Fast-fail step: if the status code is unexpected, the test fails immediately.
    /// This step replaces the UnexpectedStatusCodeStrategy.
    /// </summary>
    internal sealed class StatusCodeValidationStep(IOutputFormatter outputFormatter) : IHttpAssertionStep
    {
        /// <inheritdoc />
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            // If status code matches expectations, continue to next step
            if (context.IsExpectedStatusCode)
            {
                return;
            }

            // Status code mismatch - build error message and fail fast
            var errorInfo = context.Request.IsSuccessStatusCode
                                ? $"You expect an OK result but the response was {context.HttpStatusCode}. Please check implementation or your expected response"
                                : $"You expect an ERROR result but the response was {context.HttpStatusCode}. Please check implementation or your expected response";

            // Get expected result for comparison (simplified, no complex processing)
            var simpleExpectedResult = context.Request.ExpectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(context.ContentAsString,
                                                                                                                      context.Request.CallingAssembly,
                                                                                                                      string.Empty,
                                                                                                                      context.Request.ExpectedResultParameterName);

            // Format output for assertion failure
            var errorOutput = outputFormatter.GetOutputString(string.Empty,
                                                              errorInfo,
                                                              simpleExpectedResult,
                                                              context.ContentAsString,
                                                              string.Empty,
                                                              string.Empty);

            // Fail immediately - no response writing, no further processing
            Assert.Fail(errorOutput);
        }
    }
}
