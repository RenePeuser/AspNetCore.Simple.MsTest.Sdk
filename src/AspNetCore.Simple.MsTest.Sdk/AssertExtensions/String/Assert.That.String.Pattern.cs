using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// STRING PATTERN MATCHING assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the text matches the specified regular expression pattern.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="text">The text to check</param>
        /// <param name="pattern">The regular expression pattern to match</param>
        /// <param name="because">Why this text should match the pattern (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="options">Regular expression options (default: None)</param>
        /// <param name="textName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when text does not match the pattern</exception>
        public static void Matches(this Assert _,
                                   string? text,
                                   string pattern,
                                   string because,
                                   string fix,
                                   RegexOptions options = RegexOptions.None,
                                   [CallerArgumentExpression(nameof(text))]
                                   string textName = "",
                                   [CallerFilePath] string callerFilePath = "",
                                   [CallerMemberName] string callerMemberName = "",
                                   [CallerLineNumber] int callerLineNumber = 0
        )
        {
            if (text is not null && Regex.IsMatch(text, pattern, options))
            {
                return;
            }

            var output = BuildPatternMatchOutput(expectMatch: true,
                                                 text: text,
                                                 pattern: pattern,
                                                 options: options,
                                                 textName: textName,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the text does NOT match the specified regular expression pattern.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="text">The text to check</param>
        /// <param name="pattern">The regular expression pattern that should not match</param>
        /// <param name="because">Why this text should not match the pattern (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="options">Regular expression options (default: None)</param>
        /// <param name="textName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when text matches the pattern</exception>
        public static void DoesNotMatch(this Assert _,
                                        string? text,
                                        string pattern,
                                        string because,
                                        string fix,
                                        RegexOptions options = RegexOptions.None,
                                        [CallerArgumentExpression(nameof(text))]
                                        string textName = "",
                                        [CallerFilePath] string callerFilePath = "",
                                        [CallerMemberName] string callerMemberName = "",
                                        [CallerLineNumber] int callerLineNumber = 0
        )
        {
            if (text is null || !Regex.IsMatch(text, pattern, options))
            {
                return;
            }

            var output = BuildPatternMatchOutput(expectMatch: false,
                                                 text: text,
                                                 pattern: pattern,
                                                 options: options,
                                                 textName: textName,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private helper for building pattern match assertion output
        private static string BuildPatternMatchOutput(
            bool expectMatch,
            string? text,
            string pattern,
            RegexOptions options,
            string textName,
            string because,
            string fix,
            string callerFilePath,
            string callerMemberName,
            int callerLineNumber
        )
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = expectMatch
                            ? "PATTERN MISMATCH - EXPECTED MATCH"
                            : "PATTERN MATCH - EXPECTED NO MATCH";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = expectMatch
                              ? "Expected text to match the regular expression pattern but it did not match."
                              : "Expected text to NOT match the regular expression pattern but it matched.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-15} : {textName}");
            sb.AppendLine($"{"Text",-15} : {text ?? "(null)"}");
            sb.AppendLine($"{"Pattern",-15} : {pattern}");
            sb.AppendLine($"{"Options",-15} : {options}");

            if (text is not null)
            {
                var match = Regex.Match(text, pattern, options);
                sb.AppendLine($"{"Matches",-15} : {match.Success}");

                if (match.Success && !expectMatch)
                {
                    sb.AppendLine($"{"Match Value",-15} : {match.Value}");
                    sb.AppendLine($"{"Match Index",-15} : {match.Index}");
                }
            }

            sb.AppendLine($"{"Expected",-15} : {(expectMatch ? "Match" : "No Match")}");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = expectMatch
                                        ? new[]
                                          {
                                              $"Verify that '{textName}' contains the expected format or structure", $"Review the regex pattern '{pattern}' to ensure it matches the expected format", "Check if the text needs preprocessing (trimming, normalization, etc.)",
                                              "Test the pattern at regex101.com to validate it works as expected"
                                          }
                                        : new[] { $"Ensure '{textName}' does not contain the pattern '{pattern}'", "Review the regex pattern to ensure it correctly identifies invalid input", $"Verify that validation logic properly sanitizes '{textName}'" };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}