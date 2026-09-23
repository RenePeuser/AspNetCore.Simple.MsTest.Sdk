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
            services.AddTextDecoratorProvider();
            services.AddSingletonIfNotExists<ITestErrorHandler, SnapshotNotFoundErrorHandler>();
        }
    }

    /// <summary>
    /// A payload or snapshot file that cannot be found is a typo in the test, not a bug in the sdk.
    /// The generic handler used to present it as "❌ UNEXPECTED TEST SDK ERROR ... this could indicate
    /// a bug in the Test SDK itself", followed by a stack trace and a dump of every embedded resource
    /// in the project. This handler names the file, shows the nearest matches and stops there.
    /// </summary>
    internal sealed class SnapshotNotFoundErrorHandler(ITextDecoratorProvider textDecoratorProvider,
                                                       ISourceCodeExtractor sourceCodeExtractor)
        : TestErrorHandler<SnapshotNotFoundException>
    {
        protected override Task<string> HandleExceptionAsync(IObjectAssertContext context,
                                                             SnapshotNotFoundException exception)
        {
            return Task.FromResult(Build(context, exception, textDecoratorProvider.For(context.CallingAssembly)));
        }

        private string Build(IObjectAssertContext context,
                             SnapshotNotFoundException exception,
                             ITextDecorator textDecorator)
        {
            // "SNAPSHOT" on its own does not say which of the two files a call reads. An author looking
            // at a request json that is plainly there reads it as "the sdk cannot find THAT file" and
            // goes hunting in the wrong folder. A snapshot is the expected RESPONSE - the title says so.
            var kind = exception.IsPayload ? "REQUEST JSON" : "RESPONSE SNAPSHOT";

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
            sb.AppendLine($"{"Role",-12} : {(exception.IsPayload ? "request payload - the body this call SENDS" : "expected response - the body this call COMPARES against")}");

            // The reference is a bare file name far more often than not, so on its own it never reveals
            // WHERE the sdk looked. Naming the resolved target is what separates "the file is missing"
            // from "the reference resolves into a folder you did not mean" - and it is the line that
            // makes an identically named file in the sibling folder obviously not the same file.
            if (exception.ExpectedResourceName.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"Expected",-12} : {exception.ExpectedResourceName}");
            }

            if (exception.ExpectedFilePath.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine($"{"Path",-12} : file:///{exception.ExpectedFilePath.Replace('\\', '/')}");
            }

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

                // Suggestions are scoped to the role - see SnapshotNotFoundException.FindCandidates. A
                // file of the OTHER role carrying this exact name is therefore not listed above, and
                // saying nothing about it is how the author concludes the sdk is simply blind.
                if (exception.IsPayload.IsFalse())
                {
                    sb.AppendLine("  • A request json of the same name is NOT a substitute - the");
                    sb.AppendLine("    expected response has to be recorded separately.");
                }

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
                sb.AppendLine(textDecorator.Success("  • set TestSdkSettings__WriteResponse=true"));
                sb.AppendLine();

                // Recording is gated on a Debug build and the gate returns false without a word - so in
                // a Release run the author follows the advice above, nothing happens, and this very same
                // error comes back. Repeating the advice without this note is what makes that a loop.
                if (exception.CanRecord.IsFalse())
                {
                    sb.AppendLine(textDecorator.Error($"  ⚠️ '{exception.AssemblyName}' is NOT compiled in Debug."));
                    sb.AppendLine(textDecorator.Error("     Snapshot recording is a Debug-only feature: writeResponse is"));
                    sb.AppendLine(textDecorator.Error("     ignored in a Release build. Re-run the test with -c Debug."));
                    sb.AppendLine();
                }
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