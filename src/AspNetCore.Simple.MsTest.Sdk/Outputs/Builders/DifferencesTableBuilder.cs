using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddDifferencesTableBuilderExtension
    {
        public static void AddDifferencesTableBuilder(this IServiceCollection services)
        {
            // Register dependencies
            services.AddTableBuilder();
            services.AddCharacterDiff();

            // Register service itself
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

        /// <summary>
        /// Builds differences table for object comparison (non-HTTP context).
        /// </summary>
        string BuildObjectDifferencesTable(string expectedName,
                                           ImmutableList<Difference> differences);
    }

    internal sealed class DifferencesTableBuilder(ITableBuilder tableBuilder,
                                                  ITextDecorator textDecorator,
                                                  ICharacterDiff characterDiff) : IDifferencesTableBuilder
    {
        /// <summary>
        /// Maximum number of characters a value may occupy inside a table cell. Anything longer is
        /// shortened for the table and printed in full below it - a table cell with 40.000 characters
        /// is not a diff, it is a wall.
        /// </summary>
        private const int MaxValueLength = 160;

        /// <summary>
        /// Characters of context kept left and right of the actually differing part when a
        /// ValueDifference is too long to be shown as a whole.
        /// </summary>
        private const int DiffContextLength = 40;

        public string Build(IHttpResponseContext context,
                            ImmutableList<Difference> differences)
        {
            if (differences.IsEmpty)
            {
                return string.Empty;
            }

            // Consolidate array element differences into array-level differences
            var consolidatedDifferences = ConsolidateArrayDifferences(differences);

            // Get response filename for Expected column header
            var responseFileName = GetResponseFileName(context);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle($"🔍 Differences (Count {consolidatedDifferences.Count})"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.Append(BuildTableWithDetails(consolidatedDifferences, responseFileName, "CurrentResult").TrimEnd());

            return stringBuilder.ToString();
        }

        public string BuildObjectDifferencesTable(string expectedName,
                                                  ImmutableList<Difference> differences)
        {
            if (differences.IsEmpty)
            {
                return string.Empty;
            }

            // No consolidation here on purpose: the caller prints the raw difference count as its
            // own header, so collapsing rows would contradict that count.
            return BuildTableWithDetails(differences, expectedName, "Current");
        }

        /// <summary>
        /// Renders the difference table plus - when values had to be shortened to keep the table
        /// readable - a details block below it holding the untouched values.
        /// </summary>
        private string BuildTableWithDetails(ImmutableList<Difference> differences,
                                             string expectedColumnName,
                                             string currentColumnName)
        {
            var columns = new[]
                          {
                              "MemberPath", expectedColumnName, currentColumnName,
                              "MismatchType"
                          };

            var ordered = differences.OrderBy(difference => difference.MemberPath, StringComparer.Ordinal).ToList();

            var rows = new List<object[]>();
            var details = new List<RenderedDifference>();

            foreach (var difference in ordered)
            {
                var rendered = Render(difference);

                rows.Add([
                             difference.MemberPath ?? "N/A", rendered.ExpectedCell, rendered.CurrentCell,
                             difference.MismatchType.ToString()
                         ]);

                if (rendered.HasDetails)
                {
                    details.Add(rendered);
                }
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append(tableBuilder.BuildTable(columns, rows, enableCount: false));

            var legend = BuildLegend(ordered, expectedColumnName, currentColumnName);

            if (legend.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append(legend);
            }

            if (details.Count > 0)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.Append(BuildDetailsSection(details, expectedColumnName, currentColumnName));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// MissingInFirst / MissingInSecond do not say which side is which. Spell it out, but only
        /// for the mismatch types actually present.
        /// </summary>
        private string BuildLegend(IReadOnlyCollection<Difference> differences,
                                   string expectedColumnName,
                                   string currentColumnName)
        {
            var lines = new List<string>();

            if (differences.Any(difference => difference.MismatchType == MismatchType.MissingInFirst))
            {
                lines.Add($"MissingInFirst   → only in {currentColumnName}, not in {expectedColumnName}");
            }

            if (differences.Any(difference => difference.MismatchType == MismatchType.MissingInSecond))
            {
                lines.Add($"MissingInSecond  → only in {expectedColumnName}, not in {currentColumnName}");
            }

            if (lines.Count == 0)
            {
                return string.Empty;
            }

            return textDecorator.Dim(string.Join(Environment.NewLine, lines));
        }

        private string BuildDetailsSection(IReadOnlyCollection<RenderedDifference> details,
                                           string expectedColumnName,
                                           string currentColumnName)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(textDecorator.SectionTitle($"📋 Difference details (Count {details.Count})"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));

            foreach (var detail in details)
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(textDecorator.SectionTitle($"{detail.Difference.MemberPath}  ({detail.Difference.MismatchType})"));

                AppendDetailValue(stringBuilder, expectedColumnName, detail.DetailExpected);
                AppendDetailValue(stringBuilder, currentColumnName, detail.DetailCurrent);
            }

            return stringBuilder.ToString().TrimEnd();
        }

        private void AppendDetailValue(StringBuilder stringBuilder,
                                       string label,
                                       string? value)
        {
            if (value.IsNull())
            {
                return;
            }

            stringBuilder.AppendLine(textDecorator.Dim($"  {label}:"));

            foreach (var line in Prettify(value).Split('\n'))
            {
                stringBuilder.AppendLine($"    {line.TrimEnd('\r')}");
            }
        }

        /// <summary>
        /// Turns a value into the pair of cells shown in the table and - when it had to be
        /// shortened - remembers the untouched values for the details block.
        /// </summary>
        private RenderedDifference Render(Difference difference)
        {
            var value1 = CompressWhitespace(difference.Value1);
            var value2 = CompressWhitespace(difference.Value2);

            if (difference.MismatchType == MismatchType.ValueDifference)
            {
                var (windowedExpected, windowedCurrent, windowed) = FocusOnDifference(value1, value2);
                var (decoratedExpected, decoratedActual) = characterDiff.HighlightDifferences(windowedExpected, windowedCurrent);

                return new RenderedDifference
                {
                    Difference = difference,
                    ExpectedCell = decoratedExpected,
                    CurrentCell = decoratedActual,
                    DetailExpected = windowed ? difference.Value1 : null,
                    DetailCurrent = windowed ? difference.Value2 : null
                };
            }

            var (expectedCell, expectedTruncated) = Shorten(value1);
            var (currentCell, currentTruncated) = Shorten(value2);

            return new RenderedDifference
            {
                Difference = difference,
                ExpectedCell = expectedCell,
                CurrentCell = currentCell,
                DetailExpected = expectedTruncated ? difference.Value1 : null,
                DetailCurrent = currentTruncated ? difference.Value2 : null
            };
        }

        /// <summary>
        /// For long values, cuts a window around the part that actually differs instead of showing
        /// the first N characters - which are, by definition, the ones that are equal.
        /// </summary>
        private static (string Expected, string Current, bool Windowed) FocusOnDifference(string expected,
                                                                                          string current)
        {
            if (expected.Length <= MaxValueLength && current.Length <= MaxValueLength)
            {
                return (expected, current, false);
            }

            var commonPrefix = CommonPrefixLength(expected, current);
            var commonSuffix = CommonSuffixLength(expected, current, commonPrefix);

            var start = Math.Max(0, commonPrefix - DiffContextLength);

            var expectedEnd = Math.Min(expected.Length, Math.Max(start, expected.Length - commonSuffix + DiffContextLength));
            var currentEnd = Math.Min(current.Length, Math.Max(start, current.Length - commonSuffix + DiffContextLength));

            expectedEnd = Math.Min(expectedEnd, start + MaxValueLength);
            currentEnd = Math.Min(currentEnd, start + MaxValueLength);

            return (Window(expected, start, expectedEnd), Window(current, start, currentEnd), true);
        }

        private static string Window(string value,
                                     int start,
                                     int end)
        {
            var safeStart = Math.Min(start, value.Length);
            var safeEnd = Math.Clamp(end, safeStart, value.Length);

            var slice = value.Substring(safeStart, safeEnd - safeStart);
            var prefix = safeStart > 0 ? "…" : string.Empty;
            var suffix = safeEnd < value.Length ? "…" : string.Empty;

            return $"{prefix}{slice}{suffix}";
        }

        private static int CommonPrefixLength(string first,
                                              string second)
        {
            var maxLength = Math.Min(first.Length, second.Length);
            var length = 0;

            while (length < maxLength && first[length] == second[length])
            {
                length++;
            }

            return length;
        }

        private static int CommonSuffixLength(string first,
                                              string second,
                                              int commonPrefixLength)
        {
            var length = 0;
            var firstIndex = first.Length - 1;
            var secondIndex = second.Length - 1;

            while (firstIndex >= commonPrefixLength &&
                   secondIndex >= commonPrefixLength &&
                   first[firstIndex] == second[secondIndex])
            {
                length++;
                firstIndex--;
                secondIndex--;
            }

            return length;
        }

        private static (string Value, bool Truncated) Shorten(string value)
        {
            if (value.Length <= MaxValueLength)
            {
                return (value, false);
            }

#pragma warning disable CA1845 // Use span-based 'string.Concat' - substring is clear here
            return ($"{value.Substring(0, MaxValueLength)}… (+{value.Length - MaxValueLength} chars)", true);
#pragma warning restore CA1845
        }

        /// <summary>
        /// Pretty prints a value when it is JSON - a missing object is far easier to read as an
        /// indented tree than as one endless line.
        /// </summary>
        private static string Prettify(string value)
        {
            var trimmed = value.Trim();

            if (trimmed.StartsWith('{').IsFalse() && trimmed.StartsWith('[').IsFalse())
            {
                return value;
            }

            try
            {
                return JToken.Parse(trimmed).ToString(Newtonsoft.Json.Formatting.Indented);
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                return value;
            }
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
        /// Consolidates whole missing array elements into one array-level row.
        /// Example: emails[0], emails[1], emails[2] all MissingInFirst → emails array length mismatch.
        /// A difference *inside* an element (emails[0].domain) is never consolidated - collapsing it
        /// into an item count is exactly what hides the real diff.
        /// </summary>
        private static ImmutableList<Difference> ConsolidateArrayDifferences(ImmutableList<Difference> differences)
        {
            var consolidated = new List<Difference>();
            var arrayGroups = new Dictionary<string, List<Difference>>(StringComparer.Ordinal);

            // Group differences by array path (without index)
            foreach (var difference in differences)
            {
                var (isArrayElement, arrayPath) = ExtractArrayPath(difference.MemberPath);

                if (isArrayElement.IsFalse())
                {
                    // Not a whole array element - keep as is, including its values
                    consolidated.Add(difference);

                    continue;
                }

                if (arrayGroups.TryGetValue(arrayPath, out var group).IsFalse())
                {
                    group = new List<Difference>();
                    arrayGroups[arrayPath] = group;
                }

                group!.Add(difference);
            }

            // Process array groups
            foreach (var group in arrayGroups)
            {
                var arrayPath = group.Key;
                var arrayDifferences = group.Value;

                // A single missing element is more useful with its content than as an item count
                if (arrayDifferences.Count < 2)
                {
                    consolidated.AddRange(arrayDifferences);

                    continue;
                }

                // Check if all differences are array length mismatches (all MissingInFirst or all MissingInSecond)
                var allMissingInFirst = arrayDifferences.All(difference => difference.MismatchType == MismatchType.MissingInFirst);
                var allMissingInSecond = arrayDifferences.All(difference => difference.MismatchType == MismatchType.MissingInSecond);

                if (allMissingInFirst.IsFalse() && allMissingInSecond.IsFalse())
                {
                    // Mixed differences - keep element-level details
                    consolidated.AddRange(arrayDifferences);

                    continue;
                }

                // Consolidate to array-level difference
                // Calculate actual array lengths by analyzing indices
                var (expectedLength, currentLength) = CalculateArrayLengths(arrayDifferences, allMissingInFirst);
                var mismatchType = allMissingInFirst ? MismatchType.MissingInFirst : MismatchType.MissingInSecond;

                var indices = string.Join(", ", arrayDifferences.Select(difference => ExtractArrayIndex(difference.MemberPath))
                                                                .OrderBy(index => index));

                var value1 = expectedLength == 0 ? "[] (0 items)" : $"[{expectedLength} item(s)]";
                var value2 = currentLength == 0 ? "[] (0 items)" : $"[{currentLength} item(s)]";

                consolidated.Add(new Difference
                {
                    MemberPath = $"{arrayPath} (index {indices})",
                    Value1 = allMissingInFirst ? value1 : JoinElements(arrayDifferences, first: true),
                    Value2 = allMissingInFirst ? JoinElements(arrayDifferences, first: false) : value2,
                    MismatchType = mismatchType
                });
            }

            return consolidated.ToImmutableList();
        }

        /// <summary>
        /// Joins the values of the elements that exist on only one side, so the consolidated row
        /// still carries what is actually missing.
        /// </summary>
        private static string JoinElements(IEnumerable<Difference> differences,
                                           bool first)
        {
            var values = differences.Select(difference => first ? difference.Value1 : difference.Value2)
                                    .Where(value => value.IsNotNullOrWhiteSpace());

            return $"[{string.Join(",", values)}]";
        }

        /// <summary>
        /// Extracts the array path when - and only when - the member path points at a whole array
        /// element, meaning it ends with a numeric index.
        /// Example: "content.value.emails[0]"      → (true,  "content.value.emails")
        /// Example: "content.value.emails[0].name" → (false, "content.value.emails[0].name")
        /// Example: "content.value.name"           → (false, "content.value.name")
        /// </summary>
        private static (bool IsArrayElement, string ArrayPath) ExtractArrayPath(string memberPath)
        {
            if (memberPath.IsNullOrWhiteSpace() || memberPath.EndsWith(']').IsFalse())
            {
                return (false, memberPath);
            }

            var lastBracketIndex = memberPath.LastIndexOf('[');

            if (lastBracketIndex < 0)
            {
                return (false, memberPath);
            }

            // Key based arrays use ["someKey"] - those are not positional, so no length mismatch
            var indexString = memberPath.Substring(lastBracketIndex + 1, memberPath.Length - lastBracketIndex - 2);

            if (int.TryParse(indexString, out _).IsFalse())
            {
                return (false, memberPath);
            }

#pragma warning disable CA1845 // Use span-based 'string.Concat' - substring is clear here
            return (true, memberPath.Substring(0, lastBracketIndex));
#pragma warning restore CA1845
        }

        /// <summary>
        /// Calculates actual array lengths from differences.
        /// Uses min and max indices to determine where arrays diverge.
        /// </summary>
        private static (int ExpectedLength, int CurrentLength) CalculateArrayLengths(List<Difference> arrayDifferences,
                                                                                     bool allMissingInFirst)
        {
            if (arrayDifferences.Count == 0)
            {
                return (0, 0);
            }

            // Find min and max array indices from all differences
            var minIndex = int.MaxValue;
            var maxIndex = -1;

            foreach (var difference in arrayDifferences)
            {
                var index = ExtractArrayIndex(difference.MemberPath);

                if (index < minIndex)
                {
                    minIndex = index;
                }

                if (index > maxIndex)
                {
                    maxIndex = index;
                }
            }

            // If allMissingInFirst: elements exist in Current but not in Expected
            // If allMissingInSecond: elements exist in Expected but not in Current
            if (allMissingInFirst)
            {
                // Expected is shorter, Current has elements from minIndex to maxIndex
                // minIndex tells us where Expected ends
                return (minIndex, maxIndex + 1);
            }

            // Current is shorter, Expected has elements from minIndex to maxIndex
            // minIndex tells us where Current ends
            return (maxIndex + 1, minIndex);
        }

        /// <summary>
        /// Extracts the array index from a member path.
        /// Example: "content.value.emails[1]" → 1
        /// </summary>
        private static int ExtractArrayIndex(string memberPath)
        {
            var lastBracketStart = memberPath.LastIndexOf('[');
            var lastBracketEnd = memberPath.LastIndexOf(']');

            if (lastBracketStart < 0 || lastBracketEnd < 0 || lastBracketEnd <= lastBracketStart)
            {
                return 0;
            }

            var indexString = memberPath.Substring(lastBracketStart + 1, lastBracketEnd - lastBracketStart - 1);

            if (int.TryParse(indexString, out var index))
            {
                return index;
            }

            return 0;
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

        private sealed record RenderedDifference
        {
            public required Difference Difference { get; init; }

            public required string ExpectedCell { get; init; }

            public required string CurrentCell { get; init; }

            public required string? DetailExpected { get; init; }

            public required string? DetailCurrent { get; init; }

            public bool HasDetails => DetailExpected.IsNotNull() || DetailCurrent.IsNotNull();
        }
    }
}