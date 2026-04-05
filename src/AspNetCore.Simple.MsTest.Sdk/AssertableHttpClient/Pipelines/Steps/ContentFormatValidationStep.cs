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
            services.AddOutputFormatter();

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, ContentFormatValidationStep>();
        }
    }

    /// <summary>
    /// Validates that the response body content is structurally valid JSON.
    /// Checks that content starts with '{' (object) or '[' (array).
    /// This step runs after ContentTypeHeaderValidationStep and before JsonComparisonStep.
    /// </summary>
    internal sealed class ContentFormatValidationStep(IOutputFormatter outputFormatter) : IHttpAssertionStep
    {
        private const string IgnoreResponseComparison = "IgnoreResponse";

        /// <inheritdoc />
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            var expectedResultFile = context.Request.ExpectedResultFile;
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

            var errorOutput = outputFormatter.GetOutputString(string.Empty,
                                                              "Response body is not valid JSON. Expected content starting with '{' or '[' but got different format.",
                                                              expectedResultFile.Content,
                                                              contentPreview,
                                                              string.Empty,
                                                              string.Empty);

            Assert.Fail(errorOutput);
        }
    }
}
