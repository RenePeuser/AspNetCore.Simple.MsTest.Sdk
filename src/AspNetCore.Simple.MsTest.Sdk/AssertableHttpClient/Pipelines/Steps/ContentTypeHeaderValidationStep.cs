using System.Collections.Immutable;
using System.Net.Mime;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    public static class AddContentTypeHeaderValidationStepExtension
    {
        /// <summary>
        /// Registers the content type header validation step and its dependencies.
        /// </summary>
        public static void AddContentTypeHeaderValidationStep(this IServiceCollection services)
        {
            // 1. Register dependencies
            services.AddSnapshotTestOutputBuilder();

            // 2. Register the step itself
            services.AddSingletonIfNotExists<IHttpAssertionStep, ContentTypeHeaderValidationStep>();
        }
    }

    /// <summary>
    /// Validates that the Content-Type HTTP header indicates JSON content.
    /// This prevents attempting to parse binary data (images, PDFs, etc.) as JSON.
    /// Checks the Content-Type header value for application/json.
    /// </summary>
    internal sealed class ContentTypeHeaderValidationStep(ISnapshotTestOutputBuilder snapshotTestOutputBuilder) : IHttpAssertionStep
    {
        private const string IgnoreResponseComparison = "IgnoreResponse";

        /// <inheritdoc />
        public void Execute<TResult>(HttpResponseContext<TResult> context)
        {
            var expectedResultFile = context.Request.ExpectedResultFile;

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

            // Extract Content-Type from HttpResponseMessage headers directly
            var contentTypeHeader = context.HttpResponseMessage.Content.Headers.ContentType?.ToString();

            // If no Content-Type header found, we cannot validate - continue and let ContentFormatValidationStep handle it
            if (contentTypeHeader.IsNullOrWhiteSpace())
            {
                return;
            }

            // Check if Content-Type contains application/json (may have charset, e.g., "application/json; charset=utf-8")
            var isJsonContentType = contentTypeHeader.Contains(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase) ||
                                    contentTypeHeader.Contains(MediaTypeNames.Application.ProblemJson, StringComparison.OrdinalIgnoreCase);

            // If content type is JSON, continue to next step
            if (isJsonContentType)
            {
                return;
            }

            // Content-Type header indicates non-JSON content - fail early to avoid parsing binary data
            var expectedJson = expectedResultFile.Content;
            var currentJson = $"Content-Type: {contentTypeHeader}";
            var differences = ImmutableList<Difference>.Empty;

            var errorOutput = snapshotTestOutputBuilder.Build(context,
                                                              differences,
                                                              expectedJson,
                                                              currentJson);

            Assert.Fail(errorOutput);
        }
    }
}
