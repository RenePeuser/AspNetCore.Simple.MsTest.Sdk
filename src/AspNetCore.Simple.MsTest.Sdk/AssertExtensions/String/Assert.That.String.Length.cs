using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// STRING LENGTH assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the string has the expected length.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="text">The string to check</param>
        /// <param name="expectedLength">The expected length of the string</param>
        /// <param name="because">Why this length is expected (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="textName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when string length does not match expected length</exception>
        public static void HasLength(this Assert _,
                                     string? text,
                                     int expectedLength,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(text))]
                                     string textName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            var actualLength = text?.Length ?? 0;

            if (actualLength == expectedLength)
            {
                return;
            }

            var output = BuildStringLengthOutput(textName: textName,
                                                 text: text,
                                                 actualLength: actualLength,
                                                 expectedLength: expectedLength,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber,
                                                 callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the string length is within the specified range (inclusive).
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="text">The string to check</param>
        /// <param name="minLength">The minimum allowed length (inclusive)</param>
        /// <param name="maxLength">The maximum allowed length (inclusive)</param>
        /// <param name="because">Why this length range is expected (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="textName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when string length is outside the specified range</exception>
        public static void HasLengthInRange(this Assert _,
                                            string? text,
                                            int minLength,
                                            int maxLength,
                                            string because,
                                            string fix,
                                            [CallerArgumentExpression(nameof(text))]
                                            string textName = "",
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerMemberName] string callerMemberName = "",
                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            var actualLength = text?.Length ?? 0;

            if (actualLength >= minLength && actualLength <= maxLength)
            {
                return;
            }

            var output = BuildStringLengthRangeOutput(textName: textName,
                                                      text: text,
                                                      actualLength: actualLength,
                                                      minLength: minLength,
                                                      maxLength: maxLength,
                                                      because: because,
                                                      fix: fix,
                                                      callerFilePath: callerFilePath,
                                                      callerMemberName: callerMemberName,
                                                      callerLineNumber: callerLineNumber,
                                                      callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        // Private helper for building string length assertion output
        private static string BuildStringLengthOutput(string textName,
                                                      string? text,
                                                      int actualLength,
                                                      int expectedLength,
                                                      string because,
                                                      string fix,
                                                      string callerFilePath,
                                                      string callerMemberName,
                                                      int callerLineNumber,
                                                      Assembly callingAssembly)
        {
            var assertOutputHelper = HttpClientAssertExtensions.GetService<IAssertOutputHelper>(callingAssembly);
            var sb = new StringBuilder();

            // Header
            var title = "STRING LENGTH MISMATCH";

            assertOutputHelper.BuildHeader(sb, title);

            // Test Information
            assertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber);

            // Problem
            var problem = $"Expected string to have length {expectedLength} but actual length is {actualLength}.";

            assertOutputHelper.BuildProblemSection(sb, problem);

            // Details
            assertOutputHelper.BuildDetailsSectionHeader(sb);
            sb.AppendLine($"{"Variable",-15} : {textName}");
            sb.AppendLine($"{"Actual Length",-15} : {actualLength}");
            sb.AppendLine($"{"Expected Length",-15} : {expectedLength}");
            sb.AppendLine($"{"Difference",-15} : {actualLength - expectedLength} characters");

            if (text is null)
            {
                sb.AppendLine($"{"Value",-15} : null");
            }
            else if (text.Length == 0)
            {
                sb.AppendLine($"{"Value",-15} : (empty string)");
            }
            else if (text.Length <= 100)
            {
                sb.AppendLine($"{"Value",-15} : \"{text}\"");
            }
            else
            {
#pragma warning disable CA1846 // Prefer AsSpan over Substring
                sb.AppendLine($"{"Value (first 100)",-15} : \"{text.Substring(0, 100)}...\"");
#pragma warning restore CA1846
            }

            sb.AppendLine();

            // Context (Why)
            assertOutputHelper.BuildContextSection(sb, because);

            // Fix (How)
            var additionalOptions = actualLength < expectedLength
                                        ? new[] { $"Verify that '{textName}' is being populated with all expected data", $"Check if '{textName}' is being truncated or filtered before this assertion", $"Review string concatenation or formatting logic for '{textName}'" }
                                        : new[] { $"Verify that '{textName}' doesn't contain unexpected characters or padding", $"Check if '{textName}' has extra whitespace, newlines, or hidden characters", $"Review the source data for '{textName}' to ensure it matches expected format" };

            assertOutputHelper.BuildFixSection(sb, fix,
                                               additionalOptions);

            // Footer
            assertOutputHelper.BuildFooter(sb);

            return sb.ToString();
        }

        // Private helper for building string length range assertion output
        private static string BuildStringLengthRangeOutput(string textName,
                                                           string? text,
                                                           int actualLength,
                                                           int minLength,
                                                           int maxLength,
                                                           string because,
                                                           string fix,
                                                           string callerFilePath,
                                                           string callerMemberName,
                                                           int callerLineNumber,
                                                           Assembly callingAssembly)
        {
            var assertOutputHelper = HttpClientAssertExtensions.GetService<IAssertOutputHelper>(callingAssembly);
            var sb = new StringBuilder();

            // Header
            var title = "STRING LENGTH OUT OF RANGE";

            assertOutputHelper.BuildHeader(sb, title);

            // Test Information
            assertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber);

            // Problem
            var problem = $"Expected string length to be between {minLength} and {maxLength} (inclusive) but actual length is {actualLength}.";

            assertOutputHelper.BuildProblemSection(sb, problem);

            // Details
            assertOutputHelper.BuildDetailsSectionHeader(sb);
            sb.AppendLine($"{"Variable",-15} : {textName}");
            sb.AppendLine($"{"Actual Length",-15} : {actualLength}");
            sb.AppendLine($"{"Min Length",-15} : {minLength}");
            sb.AppendLine($"{"Max Length",-15} : {maxLength}");

            if (actualLength < minLength)
            {
                sb.AppendLine($"{"Difference",-15} : {minLength - actualLength} characters too short");
            }
            else if (actualLength > maxLength)
            {
                sb.AppendLine($"{"Difference",-15} : {actualLength - maxLength} characters too long");
            }

            if (text is null)
            {
                sb.AppendLine($"{"Value",-15} : null");
            }
            else if (text.Length == 0)
            {
                sb.AppendLine($"{"Value",-15} : (empty string)");
            }
            else if (text.Length <= 100)
            {
                sb.AppendLine($"{"Value",-15} : \"{text}\"");
            }
            else
            {
#pragma warning disable CA1846 // Prefer AsSpan over Substring
                sb.AppendLine($"{"Value (first 100)",-15} : \"{text.Substring(0, 100)}...\"");
#pragma warning restore CA1846
            }

            sb.AppendLine();

            // Context (Why)
            assertOutputHelper.BuildContextSection(sb, because);

            // Fix (How)
            var additionalOptions = actualLength < minLength
                                        ? new[] { $"Verify that '{textName}' contains all required content", $"Check if '{textName}' is being truncated or filtered before this assertion", $"Review the minimum length requirement ({minLength}) to ensure it is correct" }
                                        : new[] { $"Verify that '{textName}' doesn't contain excessive content or padding", $"Check if '{textName}' has duplicate data or unnecessary whitespace", $"Review the maximum length requirement ({maxLength}) to ensure it is correct" };

            assertOutputHelper.BuildFixSection(sb, fix,
                                               additionalOptions);

            // Footer
            assertOutputHelper.BuildFooter(sb);

            return sb.ToString();
        }
    }
}