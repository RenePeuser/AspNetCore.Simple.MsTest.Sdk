using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers
{
    internal static class AddInvalidSnapshotJsonErrorHandlerExtension
    {
        public static void AddInvalidSnapshotJsonErrorHandler(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITestErrorHandler, InvalidSnapshotJsonErrorHandler>();
        }
    }

    /// <summary>
    /// Shows WHERE a snapshot stops being valid json, instead of the misleading structure-mismatch
    /// message the shape checks used to produce for it.
    /// </summary>
    internal sealed class InvalidSnapshotJsonErrorHandler(ITextDecorator textDecorator)
        : TestErrorHandler<InvalidSnapshotJsonException>
    {
        protected override Task<string> HandleExceptionAsync(IHttpAssertContext context,
                                                             InvalidSnapshotJsonException exception)
        {
            return Task.FromResult(Build(context, exception));
        }

        private string Build(IHttpAssertContext context,
                             InvalidSnapshotJsonException exception)
        {
            var kind = exception.IsPayload ? "PAYLOAD" : "SNAPSHOT";

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error($"❌ {kind} IS NOT VALID JSON"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Method",-10} : {context.CallerMemberName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("📄 File"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Resource",-10} : {exception.ResourceName}");

            if (exception.FilePath.IsNotNullOrWhiteSpace())
            {
                // Clickable, with the offending line when the parser told us one.
                var line = exception.LineNumber;
                var suffix = line.HasValue ? $":{line.Value}" : string.Empty;

                sb.AppendLine($"{"Path",-10} : file:///{exception.FilePath!.Replace('\\', '/')}{suffix}");
            }

            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("⚠️ Parser"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            // System.Text.Json appends its own zero based "LineNumber: x | BytePositionInLine: y".
            // Printing that next to our one based line number contradicts itself - keep the reason,
            // state the position ourselves.
            var reason = exception.ParseMessage;
            var suffixIndex = reason.IndexOf("LineNumber:", StringComparison.Ordinal);

            if (suffixIndex > 0)
            {
                reason = reason[..suffixIndex].TrimEnd(' ', '|');
            }

            sb.AppendLine(textDecorator.Error(reason));

            if (exception.LineNumber.HasValue)
            {
                var position = exception.Position.HasValue ? $", column {exception.Position.Value}" : string.Empty;

                sb.AppendLine(textDecorator.Error($"line {exception.LineNumber.Value}{position}"));
            }

            sb.AppendLine();

            AppendContent(sb, exception);

            sb.AppendLine(textDecorator.SectionTitle("💡 How To Fix"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            if (LooksLikeUnresolvedPlaceholder(exception))
            {
                sb.AppendLine(textDecorator.Error("  • The offending token looks like a placeholder that was never replaced."));
                sb.AppendLine("    Supply it through the parameters argument, e.g. parameters: [(\"$Age$\", 42)].");
                sb.AppendLine();
            }

            sb.AppendLine("  • Open the file and repair the json - a trailing comma or a missing value is typical.");
            sb.AppendLine("  • To re-record it from the current response, delete it and run with write response on.");
            sb.AppendLine();

            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));

            return sb.ToString();
        }

        /// <summary>
        /// Prints the content with line numbers and marks the failing line, capped so a large snapshot
        /// does not bury the message.
        /// </summary>
        private void AppendContent(StringBuilder sb,
                                   InvalidSnapshotJsonException exception)
        {
            if (exception.Content.IsNullOrWhiteSpace())
            {
                return;
            }

            var lines = exception.Content.Replace("\r\n", "\n").Split('\n');
            var failingLine = exception.LineNumber;

            var from = failingLine.HasValue ? Math.Max(1, failingLine.Value - 3) : 1;
            var to = failingLine.HasValue ? Math.Min(lines.Length, failingLine.Value + 3) : Math.Min(lines.Length, 15);

            sb.AppendLine(textDecorator.SectionTitle("📄 Content"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            foreach (var number in Enumerable.Range(from, to - from + 1))
            {
                var text = lines[number - 1];
                var marker = number == failingLine ? "❌" : "  ";
                var rendered = $"{marker} {number,4} | {text}";

                sb.AppendLine(number == failingLine ? textDecorator.Error(rendered) : rendered);
            }

            if (to < lines.Length)
            {
                sb.AppendLine(textDecorator.Dim($"   ... {lines.Length - to} more lines"));
            }

            sb.AppendLine();
        }

        /// <summary>
        /// A parameterized snapshot holds bare placeholders until the parameters are applied. If one
        /// survived, the json is invalid for a reason the developer fixes in one place.
        /// </summary>
        private static bool LooksLikeUnresolvedPlaceholder(InvalidSnapshotJsonException exception)
        {
            return exception.ParseMessage.StartsWith("'$'", StringComparison.Ordinal);
        }
    }
}
