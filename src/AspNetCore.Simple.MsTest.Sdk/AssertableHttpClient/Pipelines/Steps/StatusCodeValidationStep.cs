using System.Collections.Immutable;
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
            services.AddAssertOutputBuilder();

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, StatusCodeValidationStep>();
        }
    }

    /// <summary>
    /// Validates that the HTTP status code matches expectations.
    /// Fast-fail step: if the status code is unexpected, the test fails immediately.
    /// Uses IAssertOutputBuilder to build error output (strategy resolved automatically).
    /// </summary>
    internal sealed class StatusCodeValidationStep(IAssertOutputBuilder assertOutputBuilder) : IHttpAssertionStep
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
            // Get expected result for comparison (simplified, no complex processing)
            var expectedJson = context.ExpectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(context.ContentAsString,
                                                                                                      context.CallingAssembly,
                                                                                                      string.Empty,
                                                                                                      context.ExpectedResultParameterName) ?? string.Empty;

            var currentJson = context.ContentAsString;

            // Build differences list (empty for now - status code mismatch is conceptual, not JSON diff)
            var differences = ImmutableList<Difference>.Empty;

            // Build complete output using strategy pattern (HTTP strategy will be auto-resolved)
            var errorOutput = assertOutputBuilder.BuildOutput(context, differences, expectedJson,
                                                             currentJson);

            // Fail immediately - no response writing, no further processing
            Assert.That.Fail(errorOutput);
        }
    }
}
