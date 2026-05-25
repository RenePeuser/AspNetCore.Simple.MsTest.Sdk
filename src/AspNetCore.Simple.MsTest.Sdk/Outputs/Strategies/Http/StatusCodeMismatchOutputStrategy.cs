using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    public static class AddStatusCodeMismatchOutputStrategyExtension
    {
        /// <summary>
        /// Registers the status code mismatch output strategy.
        /// </summary>
        public static void AddStatusCodeMismatchOutputStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IHttpFailureOutputStrategy, StatusCodeMismatchOutputStrategy>();
        }
    }

    /// <summary>
    /// Strategy for handling HTTP status code mismatch failures.
    /// Displays expected vs actual status codes with helpful categorization (Success, Client Error, Server Error).
    /// </summary>
    internal sealed class StatusCodeMismatchOutputStrategy(ITextDecorator textDecorator,
                                                           IHttpFailureOutputHelper outputHelper) : IHttpFailureOutputStrategy
    {
        public bool CanHandle(HttpAssertionFailureType failureType)
        {
            return failureType == HttpAssertionFailureType.StatusCodeMismatch;
        }

        public void BuildHeader(StringBuilder sb,
                                IHttpResponseContext context)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("🚫 UNEXPECTED STATUS CODE"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            outputHelper.BuildTestInfoSection(sb, context, textDecorator);

            // Failure Details section with expected vs actual
            if (context.ExpectedStatusCode.HasValue && context.ActualStatusCode.HasValue)
            {
                sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();

                var expectedRange = outputHelper.GetStatusCodeRange(context.ExpectedStatusCode.Value);
                var actualText = outputHelper.GetStatusText(context.ActualStatusCode.Value);

                sb.AppendLine($"{"Expected",-10} : {context.ExpectedStatusCode} ({expectedRange})");
                sb.AppendLine($"{"Actual",-10} : {context.ActualStatusCode} ({actualText})");
                sb.AppendLine();
            }
        }
    }
}