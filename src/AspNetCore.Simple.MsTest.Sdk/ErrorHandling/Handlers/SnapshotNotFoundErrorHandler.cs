using System.IO;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers
{
    internal static class AddSnapshotNotFoundErrorHandlerExtension
    {
        public static void AddSnapshotNotFoundErrorHandler(this IServiceCollection services)
        {
            services.AddSourceCodeExtractor();
            services.AddSingletonIfNotExists<ITestErrorHandler, SnapshotNotFoundErrorHandler>();
        }
    }

    /// <summary>
    /// A payload or snapshot file that cannot be found is a typo in the test, not a bug in the sdk.
    /// The generic handler used to present it as "❌ UNEXPECTED TEST SDK ERROR ... this could indicate
    /// a bug in the Test SDK itself", followed by a stack trace and a dump of every embedded resource
    /// in the project. This handler names the file, shows the nearest matches and stops there.
    /// </summary>
    internal sealed class SnapshotNotFoundErrorHandler(ITextDecorator textDecorator,
                                                       ISourceCodeExtractor sourceCodeExtractor)
        : TestErrorHandler<SnapshotNotFoundException>
    {
        protected override Task<string> HandleExceptionAsync(IHttpAssertContext context,
                                                             SnapshotNotFoundException exception)
        {
            return Task.FromResult(Build(context, exception));
        }

        private string Build(IHttpAssertContext context,
                             SnapshotNotFoundException exception)
        {
            var kind = exception.IsPayload ? "PAYLOAD" : "SNAPSHOT";

            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error($"❌ {kind} FILE NOT FOUND"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("📦 Test Information"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine($"{"Project",-10} : {exception.AssemblyName}");
            sb.AppendLine($"{"Method",-10} : {context.CallerMemberName}");
            sb.AppendLine($"{"Line",-10} : {context.CallerLineNumber}");

            if (context.CallerFilePath.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"File",-10} : file:///{context.CallerFilePath.Replace('\\', '/')}:{context.CallerLineNumber}");
            }

            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("⚠️ Problem"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            // CallerArgumentExpression yields the literal itself when the reference was inlined -
            // printing it twice adds nothing.
            if (exception.ParameterName.IsNotNullOrWhiteSpace() &&
                exception.ParameterName.Trim('"').EqualsTo(exception.Reference).IsFalse())
            {
                sb.AppendLine($"{"Parameter",-12} : {exception.ParameterName}");
            }

            sb.AppendLine($"{"Reference",-12} : {textDecorator.Error(exception.Reference)}");
            sb.AppendLine();
            sb.AppendLine("It matches no embedded resource and no file on disk.");
            sb.AppendLine();

            if (exception.Candidates.Count > 0)
            {
                sb.AppendLine(textDecorator.SectionTitle("💡 Did You Mean"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();

                foreach (var candidate in exception.Candidates)
                {
                    // The short form is what belongs into the test - show it next to the full name.
                    sb.AppendLine($"  • {textDecorator.Success(ShortForm(candidate))}");
                    sb.AppendLine($"    {textDecorator.Dim(candidate)}");
                }

                sb.AppendLine();
            }
            else
            {
                sb.AppendLine(textDecorator.SectionTitle("💡 Nothing Similar Found"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine("  • Is the file included as <EmbeddedResource> in the csproj?");
                sb.AppendLine("  • Was it renamed or deleted?");
                sb.AppendLine();
            }

            var sourceCode = sourceCodeExtractor.ExtractCallCode(context.CallerFilePath, context.CallerLineNumber);

            if (sourceCode.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.SectionTitle("📝 Assert Call"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine(sourceCode);
                sb.AppendLine();
            }

            if (exception.IsPayload.IsFalse())
            {
                sb.AppendLine(textDecorator.SectionTitle("✍️ Creating A New Snapshot"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine("If this snapshot is supposed to be new, let the sdk write it:");
                sb.AppendLine();
                sb.AppendLine(textDecorator.Success("  • pass writeResponse: true on this assert, or"));
                sb.AppendLine(textDecorator.Success("  • set AspNetCoreSimpleMsTestSdk__WriteResponse=true"));
                sb.AppendLine();
            }

            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));

            return sb.ToString();
        }

        /// <summary>
        /// Turns "Project.Api.V1.Responses.Foo.json" into "Responses.Foo.json" - the form a test writes.
        /// </summary>
        private static string ShortForm(string resourceName)
        {
            var parts = resourceName.Split('.');

            return parts.Length < 3
                       ? resourceName
                       : $"{parts[^3]}.{parts[^2]}.{parts[^1]}";
        }
    }
}
