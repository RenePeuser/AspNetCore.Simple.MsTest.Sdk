using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    public static class AddSchemaMismatchOutputStrategyExtension
    {
        /// <summary>
        /// Registers the schema mismatch output strategy.
        /// </summary>
        public static void AddSchemaMismatchOutputStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IHttpFailureOutputStrategy, SchemaMismatchOutputStrategy>();
        }
    }

    /// <summary>
    /// Strategy for handling schema mismatch failures.
    /// Indicates that the response structure doesn't match the expected type schema
    /// (missing properties, extra properties, or type mismatches).
    /// </summary>
    internal sealed class SchemaMismatchOutputStrategy(
        ITextDecorator textDecorator,
        IHttpFailureOutputHelper outputHelper) : IHttpFailureOutputStrategy
    {
        public bool CanHandle(HttpAssertionFailureType failureType)
        {
            return failureType == HttpAssertionFailureType.SchemaMismatch;
        }

        public void BuildHeader(StringBuilder sb,
                                IHttpResponseContext context)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("📋 SCHEMA MISMATCH"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            outputHelper.BuildTestInfoSection(sb, context, textDecorator);

            sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("Structure doesn't match expected type schema.");
            sb.AppendLine("Properties missing, extra properties, or type mismatches detected.");
            sb.AppendLine();
        }
    }
}