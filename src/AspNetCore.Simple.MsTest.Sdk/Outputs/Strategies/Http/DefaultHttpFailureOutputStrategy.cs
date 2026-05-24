using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    /// <summary>
    /// Fallback strategy for HTTP failures that don't have a specific handler.
    /// Provides a minimal but complete header as a safe default.
    /// This strategy is never selected via CanHandle() but is injected separately to guarantee a result.
    /// </summary>
    internal sealed class DefaultHttpFailureOutputStrategy(ITextDecorator textDecorator) : IHttpFailureOutputStrategy
    {
        /// <summary>
        /// Always returns false - this strategy is used as explicit fallback, not via CanHandle().
        /// </summary>
        public bool CanHandle(HttpAssertionFailureType failureType)
        {
            return false;
        }

        public void BuildHeader(StringBuilder sb,
                                IHttpResponseContext context)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ HTTP ASSERTION FAILED"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            HttpFailureOutputHelper.BuildTestInfoSection(sb, context, textDecorator);
        }
    }
}