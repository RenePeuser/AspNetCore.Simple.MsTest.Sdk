using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.ErrorHandling.Handlers
{
    internal static class AddSnapshotPlaceholderLostErrorHandlerExtension
    {
        public static void AddSnapshotPlaceholderLostErrorHandler(this IServiceCollection services)
        {
            services.AddTextDecoratorProvider();
            services.AddSingletonIfNotExists<ITestErrorHandler, SnapshotPlaceholderLostErrorHandler>();
        }
    }

    /// <summary>
    /// Stops a recording that would turn a parameterized snapshot into a hard coded one, and says which
    /// placeholder is about to be lost and where it could have gone.
    /// </summary>
    internal sealed class SnapshotPlaceholderLostErrorHandler(ITextDecoratorProvider textDecoratorProvider)
        : TestErrorHandler<SnapshotPlaceholderLostException>
    {
        protected override Task<string> HandleExceptionAsync(IObjectAssertContext context,
                                                             SnapshotPlaceholderLostException exception)
        {
            return Task.FromResult(Build(context, exception, textDecoratorProvider.For(context.CallingAssembly)));
        }

        private static string Build(IObjectAssertContext context,
                                    SnapshotPlaceholderLostException exception,
                                    ITextDecorator textDecorator)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ RECORDING WOULD DESTROY A PARAMETERIZED SNAPSHOT"));
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
                sb.AppendLine($"{"Path",-10} : {exception.FilePath}");
            }

            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("⚠️ What went wrong"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            var lost = string.Join(", ", exception.LostPlaceholders.Select(name => $"${name}$"));

            sb.AppendLine(exception.LostPlaceholders.Count == 1
                              ? $"  The snapshot holds the placeholder {lost}, and the recording was about to write"
                              : $"  The snapshot holds the placeholders {lost}, and the recording was about to write");

            sb.AppendLine("  this run's concrete value in its place. The file would stop being a template, and the");
            sb.AppendLine("  next run would compare a value from today against a freshly generated one.");
            sb.AppendLine();
            sb.AppendLine("  Nothing was written. The snapshot on disk is unchanged.");
            sb.AppendLine();

            sb.AppendLine(textDecorator.SectionTitle("🔧 What to do"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();

            var first = exception.LostPlaceholders.FirstOrDefault() ?? "value";

            sb.AppendLine("  Placeholders are put back per json path, taken from the file being updated. This one had");
            sb.AppendLine("  no path left to go to, which happens when:");
            sb.AppendLine();
            sb.AppendLine("    - the api stopped returning that property");
            sb.AppendLine("    - an array came back shorter, so the position no longer exists");
            sb.AppendLine("    - the sentence around the placeholder was reworded");
            sb.AppendLine();
            sb.AppendLine("  The diff above shows which one it is. If the api genuinely changed, edit the snapshot to");
            sb.AppendLine($"  match and keep \"${first}$\" where it belongs, then record again. If the value is meant to");
            sb.AppendLine("  be fixed from now on, remove the placeholder first - then there is nothing left to lose");
            sb.AppendLine("  and the recording goes through.");

            if (exception.SuppliedParameters.IsEmpty.IsFalse())
            {
                sb.AppendLine();
                sb.AppendLine($"  Supplied: {string.Join(", ", exception.SuppliedParameters.Select(name => $"${name}$"))}");
            }

            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));

            return sb.ToString();
        }
    }
}
