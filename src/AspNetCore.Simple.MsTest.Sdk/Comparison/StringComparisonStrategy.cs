using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Comparison
{
    public static class AddStringComparisonStrategyExtension
    {
        public static void AddStringComparisonStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ISpecificComparisonStrategy, StringComparisonStrategy>();
        }
    }

    /// <summary>
    /// Comparison strategy for string values.
    /// Performs line-by-line comparison similar to git diff.
    /// </summary>
    internal sealed class StringComparisonStrategy : ComparisonStrategyBase<string>
    {
        protected override ComparisonResult CompareTyped(ObjectAssertContext<string> context)
        {
            // Determine which value to use for expected string
            string expectedString;

            // Priority 1: If ResolvedExpectedJson is available, use it (for FromFile-based asserts)
            var resolvedJson = context.ResolvedExpectedJson;

            if (!string.IsNullOrEmpty(resolvedJson))
            {
                // If it's a JSON string (starts and ends with quotes), parse it
                if (resolvedJson.TrimStart().StartsWith('"') && resolvedJson.TrimEnd().EndsWith('"'))
                {
                    // Parse JSON string to get the actual value
                    try
                    {
                        expectedString = System.Text.Json.JsonSerializer.Deserialize<string>(resolvedJson) ?? string.Empty;
                    }
#pragma warning disable CA1031 // Do not catch general exception types
                    catch (System.Text.Json.JsonException)
#pragma warning restore CA1031
                    {
                        // If parsing fails, use the raw string
                        expectedString = resolvedJson;
                    }
                }
                else
                {
                    // Not a JSON string, use it as-is
                    expectedString = resolvedJson;
                }
            }
            else
            {
                // Priority 2: If ResolvedExpectedJson is empty, use Expected directly (for direct ObjectsAreEqual calls)
                expectedString = context.Expected ?? string.Empty;
            }

            var currentString = context.Current ?? string.Empty;

            // Normalize line endings
            expectedString = NormalizeLineEndings(expectedString);
            currentString = NormalizeLineEndings(currentString);

            // Split into lines
            var expectedLines = expectedString.Split('\n');
            var currentLines = currentString.Split('\n');

            // Find line-level differences
            var differences = FindLineDifferences(expectedLines, currentLines);

            // Filter differences using context filters (global func + per-assert func + global/per-assert predicate)
            var filteredDifferences = AssertObjectExtensions.ApplyDifferenceFiltering(differences,
                                                                                      context.DifferenceFunc,
                                                                                      context.DifferenceFilter);

            // Schema mismatch if line counts differ significantly (more than just trailing whitespace)
            var hasSchemaMismatch = filteredDifferences.Any(d =>
                                                                d.MismatchType is MismatchType.MissingInFirst or MismatchType.MissingInSecond);

            return new ComparisonResult
                   {
                       Differences = filteredDifferences,
                       FormattedExpected = expectedString,
                       FormattedCurrent = currentString,
                       HasSchemaMismatch = hasSchemaMismatch
                   };
        }

        private static string NormalizeLineEndings(string text)
        {
            return text.Replace("\r\n", "\n").Replace("\r", "\n");
        }

        private static ImmutableList<Difference> FindLineDifferences(string[] expectedLines,
                                                                     string[] currentLines)
        {
            var differences = new List<Difference>();
            var maxLines = Math.Max(expectedLines.Length, currentLines.Length);

            for (var i = 0; i < maxLines; i++)
            {
                var expectedLine = i < expectedLines.Length ? expectedLines[i] : null;
                var currentLine = i < currentLines.Length ? currentLines[i] : null;

                if (expectedLine.IsNull() && currentLine.IsNotNull())
                {
                    // Line exists in current but not in expected
                    differences.Add(new Difference
                                    {
                                        MemberPath = $"Line {i + 1}",
                                        Value1 = string.Empty,
                                        Value2 = currentLine,
                                        MismatchType = MismatchType.MissingInFirst
                                    });
                }
                else if (expectedLine.IsNotNull() && currentLine.IsNull())
                {
                    // Line exists in expected but not in current
                    differences.Add(new Difference
                                    {
                                        MemberPath = $"Line {i + 1}",
                                        Value1 = expectedLine,
                                        Value2 = string.Empty,
                                        MismatchType = MismatchType.MissingInSecond
                                    });
                }
                else if (expectedLine.IsNotNull() && currentLine.IsNotNull() && expectedLine != currentLine)
                {
                    // Line differs
                    differences.Add(new Difference
                                    {
                                        MemberPath = $"Line {i + 1}",
                                        Value1 = expectedLine,
                                        Value2 = currentLine,
                                        MismatchType = MismatchType.ValueDifference
                                    });
                }
            }

            return differences.ToImmutableList();
        }
    }
}