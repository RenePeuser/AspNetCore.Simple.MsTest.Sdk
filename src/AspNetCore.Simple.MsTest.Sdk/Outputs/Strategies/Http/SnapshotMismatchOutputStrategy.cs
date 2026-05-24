using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    /// <summary>
    /// Strategy for handling snapshot mismatch failures.
    /// Indicates that JSON values differ from the expected snapshot
    /// (all properties exist but have different values).
    /// </summary>
    internal sealed class SnapshotMismatchOutputStrategy(ITextDecorator textDecorator) : IHttpFailureOutputStrategy
    {
        public bool CanHandle(HttpAssertionFailureType failureType)
        {
            return failureType == HttpAssertionFailureType.SnapshotMismatch;
        }

        public void BuildHeader(StringBuilder sb,
                                IHttpResponseContext context)
        {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("📸 SNAPSHOT MISMATCH"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            HttpFailureOutputHelper.BuildTestInfoSection(sb, context, textDecorator);

            sb.AppendLine(textDecorator.SectionTitle("⚠️ Failure Details"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("JSON values differ from the expected snapshot.");
            sb.AppendLine("All properties exist but have different values.");
            sb.AppendLine();
        }
    }
}