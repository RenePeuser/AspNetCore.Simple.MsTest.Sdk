using System.Collections.Immutable;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    public static class AddContentFormatValidationStepExtension
    {
        /// <summary>
        /// Registers the content format validation step and its dependencies.
        /// </summary>
        public static void AddContentFormatValidationStep(this IServiceCollection services)
        {
            // 1. Register dependencies
            services.AddAssertOutputBuilder();

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, ContentFormatValidationStep>();
        }
    }

    /// <summary>
    /// Validates that the response body content is structurally valid JSON.
    /// Checks that content starts with '{' (object) or '[' (array).
    /// This step runs after ContentTypeHeaderValidationStep and before JsonComparisonStep.
    /// </summary>
    internal sealed class ContentFormatValidationStep(IAssertOutputBuilder assertOutputBuilder) : IHttpAssertionStep
    {
        private const string IgnoreResponseComparison = "IgnoreResponse";

        /// <inheritdoc />
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            var expectedResultFile = context.ExpectedResultFile;
            var contentAsString = context.ContentAsString;

            // Skip validation if IgnoreResponse marker is present
            if (expectedResultFile.Content.Contains(IgnoreResponseComparison))
            {
                return;
            }

            // Skip validation if no expected result is defined
            if (expectedResultFile.Content.IsNullOrWhiteSpace())
            {
                return;
            }

            // Skip validation if content is empty (will be handled by JsonComparisonStep)
            if (contentAsString.IsNullOrWhiteSpace())
            {
                return;
            }

            // Check if content starts with JSON structure markers
            var trimmedContent = contentAsString.Trim();
            var isValidJsonStructure = trimmedContent.StartsWith('{') || trimmedContent.StartsWith('[');

            // If content is valid JSON structure, continue to next step
            if (isValidJsonStructure)
            {
                return;
            }

            // Content body is not valid JSON structure - fail with clear error
            var contentPreview = trimmedContent.Length > 100
                                     ? string.Concat(trimmedContent.AsSpan(0, 100), "...")
                                     : trimmedContent;

            var expectedJson = expectedResultFile.Content;
            var currentJson = contentPreview;
            var differences = ImmutableList<Difference>.Empty;

            var errorOutput = assertOutputBuilder.BuildOutput(context,
                                                              differences,
                                                              expectedJson,
                                                              currentJson);

            Assert.That.Fail(errorOutput);
        }
    }
}
