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
            services.AddSnapshotTestOutputBuilder();

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, StatusCodeValidationStep>();
        }
    }

    /// <summary>
    /// Validates that the HTTP status code matches expectations.
    /// Fast-fail step: if the status code is unexpected, the test fails immediately.
    /// This step replaces the UnexpectedStatusCodeStrategy.
    /// </summary>
    internal sealed class StatusCodeValidationStep(ISnapshotTestOutputBuilder snapshotTestOutputBuilder) : IHttpAssertionStep
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
            var expectedJson = context.Request.ExpectedResultFile.Content.GetJsonStringOrDefaultFrom<TResult>(context.ContentAsString,
                                                                                                              context.Request.CallingAssembly,
                                                                                                              string.Empty,
                                                                                                              context.Request.ExpectedResultParameterName) ?? string.Empty;

            var currentJson = context.ContentAsString;

            // Build differences list (empty for now - status code mismatch is conceptual, not JSON diff)
            var differences = ImmutableList<Difference>.Empty;

            // Build complete snapshot test output
            var errorOutput = snapshotTestOutputBuilder.Build(context, differences, expectedJson,
                                                              currentJson);

            // Fail immediately - no response writing, no further processing
            Assert.Fail(errorOutput);
        }
    }
}
