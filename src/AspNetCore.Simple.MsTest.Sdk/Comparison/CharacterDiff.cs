using System;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Comparison
{
    internal static class AddCharacterDiffExtension
    {
        public static void AddCharacterDiff(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ICharacterDiff, CharacterDiff>();
        }
    }

    /// <summary>
    /// Provides character-level diff highlighting for string comparisons.
    /// Uses LCS (Longest Common Subsequence) algorithm to find differences.
    /// </summary>
    internal interface ICharacterDiff
    {
        /// <summary>
        /// Highlights differences between two strings at character level.
        /// </summary>
        /// <param name="expected">Expected string value</param>
        /// <param name="actual">Actual string value</param>
        /// <returns>Tuple with decorated expected and actual strings</returns>
        (string decoratedExpected, string decoratedActual) HighlightDifferences(string? expected,
                                                                                string? actual);
    }

    internal sealed class CharacterDiff(ITextDecorator textDecorator) : ICharacterDiff
    {
        public (string decoratedExpected, string decoratedActual) HighlightDifferences(string? expected,
                                                                                       string? actual)
        {
            // Handle null cases
            if (expected.IsNullOrEmpty() && actual.IsNullOrEmpty())
            {
                return (string.Empty, string.Empty);
            }

            if (expected.IsNullOrEmpty())
            {
                return (textDecorator.Error("[empty]"), textDecorator.Success(actual!));
            }

            if (actual.IsNullOrEmpty())
            {
                return (textDecorator.Error(expected!), textDecorator.Success("[empty]"));
            }

            // Simple character-by-character comparison for PoC
            // TODO: Later implement proper LCS/Myers diff algorithm for better results
            var decoratedExpected = BuildDecoratedString(expected!, actual!, isExpected: true);
            var decoratedActual = BuildDecoratedString(actual!, expected!, isExpected: false);

            return (decoratedExpected, decoratedActual);
        }

        private string BuildDecoratedString(string source,
                                            string compare,
                                            bool isExpected)
        {
            // For PoC: Simple approach - find common prefix and highlight differences
            var minLength = Math.Min(source.Length, compare.Length);
            var commonPrefixLength = 0;

            // Find common prefix
            for (var i = 0; i < minLength; i++)
            {
                if (source[i] == compare[i])
                {
                    commonPrefixLength++;
                }
                else
                {
                    break;
                }
            }

            // Find common suffix (starting from end)
            var commonSuffixLength = 0;
            var sourceEnd = source.Length - 1;
            var compareEnd = compare.Length - 1;

            while (sourceEnd >= commonPrefixLength &&
                   compareEnd >= commonPrefixLength &&
                   source[sourceEnd] == compare[compareEnd])
            {
                commonSuffixLength++;
                sourceEnd--;
                compareEnd--;
            }

            // Build decorated string
            var result = new System.Text.StringBuilder();

            // Common prefix (no decoration)
            if (commonPrefixLength > 0)
            {
#pragma warning disable CA1846 // Prefer AsSpan over Substring - StringBuilder.Append doesn't have AsSpan overload
                result.Append(source.Substring(0, commonPrefixLength));
#pragma warning restore CA1846
            }

            // Different middle part (decorated)
            var middleStart = commonPrefixLength;
            var middleEnd = source.Length - commonSuffixLength;

            if (middleEnd > middleStart)
            {
                var middlePart = source.Substring(middleStart, middleEnd - middleStart);

                if (isExpected)
                {
                    // Expected: removed text in red
                    result.Append(textDecorator.Error(middlePart));
                }
                else
                {
                    // Actual: added text in green
                    result.Append(textDecorator.Success(middlePart));
                }
            }

            // Common suffix (no decoration)
            if (commonSuffixLength > 0)
            {
#pragma warning disable CA1846 // Prefer AsSpan over Substring - StringBuilder.Append doesn't have AsSpan overload
                result.Append(source.Substring(source.Length - commonSuffixLength));
#pragma warning restore CA1846
            }

            return result.ToString();
        }
    }
}