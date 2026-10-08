using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    internal static class AddContentTypeMismatchOutputStrategyExtension
    {
        /// <summary>
        /// Registers the content type mismatch output strategy.
        /// </summary>
        public static void AddContentTypeMismatchOutputStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IHttpFailureOutputStrategy, ContentTypeMismatchOutputStrategy>();
        }
    }

    /// <summary>
    /// Strategy for handling content type mismatch failures.
    /// Indicates that the Content-Type header doesn't match expected JSON
    /// (e.g., received text/html, image/*, etc. instead of application/json).
    /// </summary>
    internal sealed class ContentTypeMismatchOutputStrategy(ITextDecorator textDecorator,
                                                            IHttpFailureOutputHelper outputHelper) : IHttpFailureOutputStrategy
    {
        public bool CanHandle(HttpAssertionFailureType failureType)
        {
            return failureType == HttpAssertionFailureType.ContentTypeMismatch;
        }

        public void BuildHeader(StringBuilder sb,
                                IHttpResponseContext context)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("📄 CONTENT TYPE MISMATCH"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            outputHelper.BuildTestInfoSection(sb, context, textDecorator);

            sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            // Extract actual Content-Type from response
            var actualContentType = context.HttpResponseMessage.Content.Headers.ContentType?.ToString() ?? "unknown";

            sb.AppendLine("The Content-Type header indicates non-JSON content.");
            sb.AppendLine();
            sb.AppendLine($"{"Expected",-10} : application/json");
            sb.AppendLine($"{"Actual",-10} : {actualContentType}");
            sb.AppendLine();
        }
    }
}