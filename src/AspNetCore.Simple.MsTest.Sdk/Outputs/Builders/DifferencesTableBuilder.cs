using System.Collections.Immutable;
using System.Linq;
using System.Text;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddDifferencesTableBuilderExtension
    {
        public static void AddDifferencesTableBuilder(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IDifferencesTableBuilder, DifferencesTableBuilder>();
        }
    }

    public interface IDifferencesTableBuilder
    {
        /// <summary>
        /// Builds differences table from list of differences.
        /// Includes: MemberPath, Expected, Actual, MismatchType.
        /// Uses ConsoleTables for formatting.
        /// Note: Differences should be pre-filtered by the calling step (step-specific filtering).
        /// </summary>
        string Build(ImmutableList<Difference> differences);
    }

    internal sealed class DifferencesTableBuilder : IDifferencesTableBuilder
    {
        public string Build(ImmutableList<Difference> differences)
        {
            if (differences.IsEmpty)
            {
                return string.Empty;
            }

            var table = new ConsoleTable { Options = { EnableCount = false } };

            // Add columns
            table.AddColumn(new[]
                            {
                                "MemberPath", "Expected", "Actual",
                                "MismatchType"
                            });

            // Add data rows
            foreach (var difference in differences)
            {
                table.AddRow(difference.MemberPath ?? string.Empty,
                             difference.Value1 ?? "null",
                             difference.Value2 ?? "null",
                             difference.MismatchType.ToString());
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("DIFFERENCES");
            stringBuilder.AppendLine(table.ToString());

            return stringBuilder.ToString();
        }
    }
}
