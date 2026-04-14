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
            var expectedString = context.ResolvedExpectedJson ?? string.Empty;
            var currentString = context.Current ?? string.Empty; // Use Current instead of CurrentObject

            // Normalize line endings
            expectedString = NormalizeLineEndings(expectedString);
            currentString = NormalizeLineEndings(currentString);

            // Split into lines
            var expectedLines = expectedString.Split('\n');
            var currentLines = currentString.Split('\n');

            // Find line-level differences
            var differences = FindLineDifferences(expectedLines, currentLines);

            // Filter differences using context filters
            var commonDifferences = AssertObjectExtensions.DifferenceFunc(differences).ToImmutableList();
            var filteredDifferences = context.DifferenceFunc(commonDifferences).ToImmutableList();

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
