using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddDifferencesTableBuilderExtension
    {
        public static void AddDifferencesTableBuilder(this IServiceCollection services)
        {
            // No dependencies - ITextDecorator is registered separately
            services.AddSingletonIfNotExists<IDifferencesTableBuilder, DifferencesTableBuilder>();
        }
    }

    public interface IDifferencesTableBuilder
    {
        /// <summary>
        /// Builds differences table from list of differences.
        /// Includes: MemberPath, Expected (with filename), Actual, MismatchType.
        /// Uses ConsoleTables for formatting.
        /// Note: Differences should be pre-filtered by the calling step (step-specific filtering).
        /// </summary>
        string Build(IHttpResponseContext context,
                     ImmutableList<Difference> differences);
    }

    internal sealed class DifferencesTableBuilder(ITextDecorator textDecorator) : IDifferencesTableBuilder
    {
        public string Build(IHttpResponseContext context,
                            ImmutableList<Difference> differences)
        {
            if (differences.IsEmpty)
            {
                return string.Empty;
            }

            var table = new ConsoleTable { Options = { EnableCount = false } };

            // Get response filename for Expected column header
            var responseFileName = GetResponseFileName(context);

            // Add columns with response filename
            table.AddColumn(new[]
                            {
                                "MemberPath", responseFileName, "CurrentResult",
                                "MismatchType"
                            });

            // Add data rows
            foreach (var difference in differences)
            {
                table.AddRow(difference.MemberPath,
                             difference.Value1 ?? "null",
                             difference.Value2 ?? "null",
                             difference.MismatchType.ToString());
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(textDecorator.SectionTitle("DIFFERENCES"));
            stringBuilder.Append(table.ToString().TrimEnd());

            return stringBuilder.ToString();
        }

        private static string GetResponseFileName(IHttpResponseContext context)
        {
            var expectedFileName = context.ExpectedResultFile.EmbeddedFile?.Name;

            if (expectedFileName.IsNotNullOrWhiteSpace())
            {
                return expectedFileName;
            }

            return "Expected";
        }
    }
}
