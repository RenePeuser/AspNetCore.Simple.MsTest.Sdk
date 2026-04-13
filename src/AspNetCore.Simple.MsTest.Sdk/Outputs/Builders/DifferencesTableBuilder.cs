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

            // Consolidate array element differences into array-level differences
            var consolidatedDifferences = ConsolidateArrayDifferences(differences);

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
            foreach (var difference in consolidatedDifferences)
            {
                var value1 = CompressWhitespace(difference.Value1);
                var value2 = CompressWhitespace(difference.Value2);

                table.AddRow(difference.MemberPath,
                             value1,
                             value2,
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

        /// <summary>
        /// Consolidates array element differences into array-level differences when appropriate.
        /// Example: emails[0], emails[1], emails[2] all MissingInFirst → emails array length mismatch
        /// </summary>
        private static ImmutableList<Difference> ConsolidateArrayDifferences(ImmutableList<Difference> differences)
        {
            var consolidated = new List<Difference>();
            var arrayGroups = new Dictionary<string, List<Difference>>();

            // Group differences by array path (without index)
            foreach (var diff in differences)
            {
                var (isArrayElement, arrayPath) = ExtractArrayPath(diff.MemberPath);

                if (isArrayElement)
                {
                    if (!arrayGroups.TryGetValue(arrayPath, out var group))
                    {
                        group = new List<Difference>();
                        arrayGroups[arrayPath] = group;
                    }
                    group.Add(diff);
                }
                else
                {
                    // Not an array element - keep as is
                    consolidated.Add(diff);
                }
            }

            // Process array groups
            foreach (var group in arrayGroups)
            {
                var arrayPath = group.Key;
                var arrayDiffs = group.Value;

                // Check if all differences are array length mismatches (all MissingInFirst or all MissingInSecond)
                var allMissingInFirst = arrayDiffs.All(d => d.MismatchType == MismatchType.MissingInFirst);
                var allMissingInSecond = arrayDiffs.All(d => d.MismatchType == MismatchType.MissingInSecond);

                if (allMissingInFirst || allMissingInSecond)
                {
                    // Consolidate to array-level difference
                    var elementCount = arrayDiffs.Count;
                    var mismatchType = allMissingInFirst ? MismatchType.MissingInFirst : MismatchType.MissingInSecond;

                    var value1 = allMissingInFirst ? "[] (0 items)" : $"[{elementCount} item(s)]";
                    var value2 = allMissingInFirst ? $"[{elementCount} item(s)]" : "[] (0 items)";

                    consolidated.Add(new Difference
                    {
                        MemberPath = arrayPath,
                        Value1 = value1,
                        Value2 = value2,
                        MismatchType = mismatchType
                    });
                }
                else
                {
                    // Mixed differences - keep element-level details
                    consolidated.AddRange(arrayDiffs);
                }
            }

            return consolidated.ToImmutableList();
        }

        /// <summary>
        /// Extracts array path from a member path.
        /// Example: "content.value.emails[0]" → (true, "content.value.emails")
        /// Example: "content.value.name" → (false, "content.value.name")
        /// </summary>
        private static (bool isArrayElement, string arrayPath) ExtractArrayPath(string memberPath)
        {
            var lastBracketIndex = memberPath.LastIndexOf('[');

            if (lastBracketIndex < 0)
            {
                return (false, memberPath);
            }

#pragma warning disable CA1845 // Use span-based 'string.Concat' - substring is clear here
            var arrayPath = memberPath.Substring(0, lastBracketIndex);
#pragma warning restore CA1845
            return (true, arrayPath);
        }

        /// <summary>
        /// Compresses whitespace in a value to make it display on a single line in the table.
        /// Removes newlines and excessive spaces, but keeps the full value (no truncation).
        /// </summary>
        private static string CompressWhitespace(string? value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return "null";
            }

            // Replace all newlines with spaces
            var compressed = value
                             .Replace("\r\n", " ")
                             .Replace("\n", " ")
                             .Replace("\r", " ")
                             .Replace("\t", " ");

            // Compress multiple spaces to single space
            while (compressed.Contains("  "))
            {
                compressed = compressed.Replace("  ", " ");
            }

            // Trim leading/trailing whitespace
            return compressed.Trim();
        }
    }
}
