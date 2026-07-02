using System;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// STRING CONTENT assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that a string contains a specific substring.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="text">The text to search in</param>
        /// <param name="substring">The substring to search for</param>
        /// <param name="because">Why this substring should be present (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="comparison">String comparison rules (defaults to Ordinal)</param>
        /// <param name="textName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when text does not contain substring</exception>
        public static void Contains(this Assert _,
                                    string text,
                                    string substring,
                                    string because,
                                    string fix,
                                    StringComparison comparison = StringComparison.Ordinal,
                                    [CallerArgumentExpression(nameof(text))]
                                    string textName = "",
                                    [CallerFilePath] string callerFilePath = "",
                                    [CallerMemberName] string callerMemberName = "",
                                    [CallerLineNumber] int callerLineNumber = 0)
        {
            if (text?.Contains(substring, comparison) == true)
            {
                return;
            }

            var output = BuildStringContentOutput(expectContains: true,
                                                  textName: textName,
                                                  text: text,
                                                  substring: substring,
                                                  comparison: comparison,
                                                  because: because,
                                                  fix: fix,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that a string does NOT contain a specific substring.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="text">The text to search in</param>
        /// <param name="substring">The substring that should not be present</param>
        /// <param name="because">Why this substring should not be present (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="comparison">String comparison rules (defaults to Ordinal)</param>
        /// <param name="textName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when text contains substring</exception>
        public static void DoesNotContain(this Assert _,
                                          string text,
                                          string substring,
                                          string because,
                                          string fix,
                                          StringComparison comparison = StringComparison.Ordinal,
                                          [CallerArgumentExpression(nameof(text))]
                                          string textName = "",
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            if (text?.Contains(substring, comparison) == false || text is null)
            {
                return;
            }

            var output = BuildStringContentOutput(expectContains: false,
                                                  textName: textName,
                                                  text: text,
                                                  substring: substring,
                                                  comparison: comparison,
                                                  because: because,
                                                  fix: fix,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that a string starts with a specific prefix.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="text">The text to check</param>
        /// <param name="prefix">The expected prefix</param>
        /// <param name="because">Why this prefix should be present (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="comparison">String comparison rules (defaults to Ordinal)</param>
        /// <param name="textName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when text does not start with prefix</exception>
        public static void StartsWith(this Assert _,
                                      string text,
                                      string prefix,
                                      string because,
                                      string fix,
                                      StringComparison comparison = StringComparison.Ordinal,
                                      [CallerArgumentExpression(nameof(text))]
                                      string textName = "",
                                      [CallerFilePath] string callerFilePath = "",
                                      [CallerMemberName] string callerMemberName = "",
                                      [CallerLineNumber] int callerLineNumber = 0)
        {
            if (text?.StartsWith(prefix, comparison) == true)
            {
                return;
            }

            var output = BuildStringPrefixSuffixOutput(checkType: "StartsWith",
                                                       textName: textName,
                                                       text: text,
                                                       expected: prefix,
                                                       comparison: comparison,
                                                       because: because,
                                                       fix: fix,
                                                       callerFilePath: callerFilePath,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that a string ends with a specific suffix.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="text">The text to check</param>
        /// <param name="suffix">The expected suffix</param>
        /// <param name="because">Why this suffix should be present (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="comparison">String comparison rules (defaults to Ordinal)</param>
        /// <param name="textName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when text does not end with suffix</exception>
        public static void EndsWith(this Assert _,
                                    string text,
                                    string suffix,
                                    string because,
                                    string fix,
                                    StringComparison comparison = StringComparison.Ordinal,
                                    [CallerArgumentExpression(nameof(text))]
                                    string textName = "",
                                    [CallerFilePath] string callerFilePath = "",
                                    [CallerMemberName] string callerMemberName = "",
                                    [CallerLineNumber] int callerLineNumber = 0)
        {
            if (text?.EndsWith(suffix, comparison) == true)
            {
                return;
            }

            var output = BuildStringPrefixSuffixOutput(checkType: "EndsWith",
                                                       textName: textName,
                                                       text: text,
                                                       expected: suffix,
                                                       comparison: comparison,
                                                       because: because,
                                                       fix: fix,
                                                       callerFilePath: callerFilePath,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private helper for building Contains/DoesNotContain output
        private static string BuildStringContentOutput(bool expectContains,
                                                       string textName,
                                                       string? text,
                                                       string substring,
                                                       StringComparison comparison,
                                                       string because,
                                                       string fix,
                                                       string callerFilePath,
                                                       string callerMemberName,
                                                       int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = expectContains
                            ? "STRING CONTENT - EXPECTED TO CONTAIN SUBSTRING"
                            : "STRING CONTENT - EXPECTED NOT TO CONTAIN SUBSTRING";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = expectContains
                              ? $"Expected string to contain substring but it was not found."
                              : $"Expected string to NOT contain substring but it was found.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-15} : {textName}");
            sb.AppendLine($"{"Text",-15} : {(text is null ? "null" : $"\"{text}\"")}");
            sb.AppendLine($"{"Substring",-15} : \"{substring}\"");
            sb.AppendLine($"{"Comparison",-15} : {comparison}");

            if (text != null)
            {
                var index = text.IndexOf(substring, comparison);
                sb.AppendLine($"{"Found At",-15} : {(index >= 0 ? $"Index {index}" : "Not found")}");
                sb.AppendLine($"{"Text Length",-15} : {text.Length}");
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = expectContains
                                        ? new[]
                                        {
                                            $"Verify that the code generating '{textName}' includes the expected substring \"{substring}\"",
                                            $"Check for typos or case sensitivity in the substring (currently using {comparison})",
                                            $"Ensure the string is fully populated before this assertion",
                                            $"Consider using a different StringComparison mode if case/culture matters"
                                        }
                                        : new[]
                                        {
                                            $"Ensure the code generating '{textName}' does not include \"{substring}\"",
                                            $"Review the string building logic to prevent this substring from appearing",
                                            $"Check if the substring check is case-sensitive (currently using {comparison})"
                                        };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Private helper for building StartsWith/EndsWith output
        private static string BuildStringPrefixSuffixOutput(string checkType,
                                                            string textName,
                                                            string? text,
                                                            string expected,
                                                            StringComparison comparison,
                                                            string because,
                                                            string fix,
                                                            string callerFilePath,
                                                            string callerMemberName,
                                                            int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = checkType == "StartsWith"
                            ? "STRING PREFIX - EXPECTED TO START WITH"
                            : "STRING SUFFIX - EXPECTED TO END WITH";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var label = checkType == "StartsWith" ? "prefix" : "suffix";
            var problem = $"Expected string to {checkType.ToLowerInvariant()} the expected {label} but it did not.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-15} : {textName}");
            sb.AppendLine($"{"Text",-15} : {(text is null ? "null" : $"\"{text}\"")}");
            sb.AppendLine($"{"Expected",-15} : \"{expected}\" ({label})");
            sb.AppendLine($"{"Comparison",-15} : {comparison}");

            if (text != null)
            {
                sb.AppendLine($"{"Text Length",-15} : {text.Length}");
                sb.AppendLine($"{"Expected Length",-15} : {expected.Length}");

                if (checkType == "StartsWith" && text.Length > 0)
                {
                    var actualPrefix = text.Length >= expected.Length
                                           ? text[..expected.Length]
                                           : text;

                    sb.AppendLine($"{"Actual Prefix",-15} : \"{actualPrefix}\"");
                }
                else if (checkType == "EndsWith" && text.Length > 0)
                {
                    var actualSuffix = text.Length >= expected.Length
                                           ? text[^expected.Length..]
                                           : text;

                    sb.AppendLine($"{"Actual Suffix",-15} : \"{actualSuffix}\"");
                }
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = checkType == "StartsWith"
                                        ? new[]
                                        {
                                            $"Verify that '{textName}' is generated with the correct prefix \"{expected}\"",
                                            $"Check for leading whitespace or unexpected characters in '{textName}'",
                                            $"Ensure the string is not trimmed or modified before this assertion",
                                            $"Consider using a different StringComparison mode if case/culture matters (currently {comparison})"
                                        }
                                        : new[]
                                        {
                                            $"Verify that '{textName}' is generated with the correct suffix \"{expected}\"",
                                            $"Check for trailing whitespace or unexpected characters in '{textName}'",
                                            $"Ensure the string is not trimmed or modified before this assertion",
                                            $"Consider using a different StringComparison mode if case/culture matters (currently {comparison})"
                                        };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}