using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    public static class AddDefaultHttpFailureOutputStrategyExtension
    {
        /// <summary>
        /// Registers the default HTTP failure output strategy.
        /// </summary>
        public static void AddDefaultHttpFailureOutputStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<DefaultHttpFailureOutputStrategy>();
        }
    }

    /// <summary>
    /// Fallback strategy for HTTP failures that don't have a specific handler.
    /// Provides a minimal but complete header as a safe default.
    /// This strategy is never selected via CanHandle() but is injected separately to guarantee a result.
    /// </summary>
    internal sealed class DefaultHttpFailureOutputStrategy(
        ITextDecorator textDecorator,
        IHttpFailureOutputHelper outputHelper) : IHttpFailureOutputStrategy
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

            outputHelper.BuildTestInfoSection(sb, context, textDecorator);
        }
    }
}