using System;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// DATETIME assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the actual DateTime is after the expected DateTime.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual DateTime value</param>
        /// <param name="expected">The expected DateTime to compare against</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual is not after expected</exception>
        public static void IsAfter(this Assert _,
                                   DateTime actual,
                                   DateTime expected,
                                   string because,
                                   string fix,
                                   [CallerArgumentExpression(nameof(actual))]
                                   string actualName = "",
                                   [CallerFilePath] string callerFilePath = "",
                                   [CallerMemberName] string callerMemberName = "",
                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            if (actual > expected)
            {
                return;
            }

            var output = BuildDateTimeComparisonOutput(comparisonType: "AFTER",
                                                       actual: actual,
                                                       expected: expected,
                                                       actualName: actualName,
                                                       because: because,
                                                       fix: fix,
                                                       callerFilePath: callerFilePath,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the actual DateTime is before the expected DateTime.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual DateTime value</param>
        /// <param name="expected">The expected DateTime to compare against</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual is not before expected</exception>
        public static void IsBefore(this Assert _,
                                    DateTime actual,
                                    DateTime expected,
                                    string because,
                                    string fix,
                                    [CallerArgumentExpression(nameof(actual))]
                                    string actualName = "",
                                    [CallerFilePath] string callerFilePath = "",
                                    [CallerMemberName] string callerMemberName = "",
                                    [CallerLineNumber] int callerLineNumber = 0)
        {
            if (actual < expected)
            {
                return;
            }

            var output = BuildDateTimeComparisonOutput(comparisonType: "BEFORE",
                                                       actual: actual,
                                                       expected: expected,
                                                       actualName: actualName,
                                                       because: because,
                                                       fix: fix,
                                                       callerFilePath: callerFilePath,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the actual DateTime is within the specified range (inclusive).
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual DateTime value</param>
        /// <param name="start">The start of the range (inclusive)</param>
        /// <param name="end">The end of the range (inclusive)</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual is not within the range</exception>
        public static void IsInRange(this Assert _,
                                     DateTime actual,
                                     DateTime start,
                                     DateTime end,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(actual))]
                                     string actualName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            if (actual >= start && actual <= end)
            {
                return;
            }

            var output = BuildDateTimeRangeOutput(actual: actual,
                                                  start: start,
                                                  end: end,
                                                  actualName: actualName,
                                                  because: because,
                                                  fix: fix,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the actual DateTime is close to the expected DateTime within the specified tolerance.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual DateTime value</param>
        /// <param name="expected">The expected DateTime value</param>
        /// <param name="tolerance">The allowed time difference</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual is not within tolerance of expected</exception>
        public static void IsCloseTo(this Assert _,
                                     DateTime actual,
                                     DateTime expected,
                                     TimeSpan tolerance,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(actual))]
                                     string actualName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var difference = actual > expected ? actual - expected : expected - actual;

            if (difference <= tolerance)
            {
                return;
            }

            var output = BuildDateTimeToleranceOutput(actual: actual,
                                                      expected: expected,
                                                      tolerance: tolerance,
                                                      actualDifference: difference,
                                                      actualName: actualName,
                                                      because: because,
                                                      fix: fix,
                                                      callerFilePath: callerFilePath,
                                                      callerMemberName: callerMemberName,
                                                      callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private helper for building DateTime comparison output
        private static string BuildDateTimeComparisonOutput(string comparisonType,
                                                            DateTime actual,
                                                            DateTime expected,
                                                            string actualName,
                                                            string because,
                                                            string fix,
                                                            string callerFilePath,
                                                            string callerMemberName,
                                                            int callerLineNumber)
        {
            var textDecorator = GetTextDecorator(callerFilePath);
            var sb = new StringBuilder();

            // Header
            var title = $"DATETIME COMPARISON - EXPECTED {comparisonType}";
            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = comparisonType == "AFTER"
                              ? "Expected DateTime to be after the comparison value but it was not."
                              : "Expected DateTime to be before the comparison value but it was not.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {actualName}");
            sb.AppendLine($"{"Actual",-10} : {actual:O}");
            sb.AppendLine($"{"Expected",-10} : {comparisonType} {expected:O}");

            var difference = actual > expected ? actual - expected : expected - actual;
            var direction = actual > expected ? "ahead" : "behind";
            sb.AppendLine($"{"Difference",-10} : {difference.TotalSeconds:F3} seconds ({direction})");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = comparisonType == "AFTER"
                                        ? new[]
                                        {
                                            $"Verify that '{actualName}' is set to a time later than the comparison value",
                                            "Check if the DateTime values are using the same timezone (UTC vs Local)",
                                            $"Review the logic that sets '{actualName}' to ensure it occurs chronologically after the expected time",
                                            "Consider if you should be using UTC times for consistent comparisons"
                                        }
                                        : new[]
                                        {
                                            $"Verify that '{actualName}' is set to a time earlier than the comparison value",
                                            "Check if the DateTime values are using the same timezone (UTC vs Local)",
                                            $"Review the logic that sets '{actualName}' to ensure it occurs chronologically before the expected time",
                                            "Consider if you should be using UTC times for consistent comparisons"
                                        };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Private helper for building DateTime range output
        private static string BuildDateTimeRangeOutput(DateTime actual,
                                                       DateTime start,
                                                       DateTime end,
                                                       string actualName,
                                                       string because,
                                                       string fix,
                                                       string callerFilePath,
                                                       string callerMemberName,
                                                       int callerLineNumber)
        {
            var textDecorator = GetTextDecorator(callerFilePath);
            var sb = new StringBuilder();

            // Header
            AssertOutputHelper.BuildHeader(sb, "DATETIME RANGE - VALUE OUT OF RANGE", textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = "Expected DateTime to be within the specified range but it was outside the bounds.";
            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {actualName}");
            sb.AppendLine($"{"Actual",-10} : {actual:O}");
            sb.AppendLine($"{"Range Start",-10} : {start:O}");
            sb.AppendLine($"{"Range End",-10} : {end:O}");

            var rangeSpan = end - start;
            sb.AppendLine($"{"Range Span",-10} : {rangeSpan.TotalSeconds:F3} seconds");

            if (actual < start)
            {
                var beforeBy = start - actual;
                sb.AppendLine($"{"Status",-10} : BEFORE range by {beforeBy.TotalSeconds:F3} seconds");
            }
            else
            {
                var afterBy = actual - end;
                sb.AppendLine($"{"Status",-10} : AFTER range by {afterBy.TotalSeconds:F3} seconds");
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = new[]
            {
                $"Verify that '{actualName}' falls between {start:O} and {end:O}",
                "Check if the DateTime values are using the same timezone (UTC vs Local)",
                $"Review the logic that sets '{actualName}' to ensure it produces values within the expected range",
                "Consider widening the acceptable range if edge cases are valid",
                "Verify that the range boundaries (start/end) are correctly defined"
            };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Private helper for building DateTime tolerance output
        private static string BuildDateTimeToleranceOutput(DateTime actual,
                                                           DateTime expected,
                                                           TimeSpan tolerance,
                                                           TimeSpan actualDifference,
                                                           string actualName,
                                                           string because,
                                                           string fix,
                                                           string callerFilePath,
                                                           string callerMemberName,
                                                           int callerLineNumber)
        {
            var textDecorator = GetTextDecorator(callerFilePath);
            var sb = new StringBuilder();

            // Header
            AssertOutputHelper.BuildHeader(sb, "DATETIME TOLERANCE - EXCEEDED THRESHOLD", textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = "Expected DateTime to be close to the target value within tolerance but the difference exceeded the threshold.";
            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {actualName}");
            sb.AppendLine($"{"Actual",-10} : {actual:O}");
            sb.AppendLine($"{"Expected",-10} : {expected:O}");
            sb.AppendLine($"{"Tolerance",-10} : {tolerance.TotalSeconds:F3} seconds");
            sb.AppendLine($"{"Difference",-10} : {actualDifference.TotalSeconds:F3} seconds");

            var percentageOff = actualDifference.TotalSeconds / tolerance.TotalSeconds * 100;
            sb.AppendLine($"{"Exceeded By",-10} : {percentageOff:F1}% over tolerance");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = new[]
            {
                $"Verify that '{actualName}' is set close to {expected:O}",
                $"Consider increasing the tolerance if {tolerance.TotalSeconds:F3} seconds is too strict",
                "Check for timing issues or delays in the code that might cause larger differences",
                "Verify that both DateTime values are using the same timezone (UTC vs Local)",
                $"Review the logic that sets '{actualName}' to reduce the time difference",
                "Consider using DateTime.UtcNow instead of DateTime.Now for more predictable comparisons"
            };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        /// <summary>
        /// Asserts that the DateTime has DateTimeKind.Utc.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The DateTime value to check</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when DateTime is not UTC</exception>
        public static void IsUtc(this Assert _,
                                 DateTime actual,
                                 string because,
                                 string fix,
                                 [CallerArgumentExpression(nameof(actual))]
                                 string actualName = "",
                                 [CallerFilePath] string callerFilePath = "",
                                 [CallerMemberName] string callerMemberName = "",
                                 [CallerLineNumber] int callerLineNumber = 0)
        {
            if (actual.Kind == DateTimeKind.Utc)
            {
                return;
            }

            var output = BuildDateTimeKindOutput(expectedKind: DateTimeKind.Utc,
                                                 actual: actual,
                                                 actualName: actualName,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the DateTime has DateTimeKind.Local.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The DateTime value to check</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when DateTime is not Local</exception>
        public static void IsLocal(this Assert _,
                                   DateTime actual,
                                   string because,
                                   string fix,
                                   [CallerArgumentExpression(nameof(actual))]
                                   string actualName = "",
                                   [CallerFilePath] string callerFilePath = "",
                                   [CallerMemberName] string callerMemberName = "",
                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            if (actual.Kind == DateTimeKind.Local)
            {
                return;
            }

            var output = BuildDateTimeKindOutput(expectedKind: DateTimeKind.Local,
                                                 actual: actual,
                                                 actualName: actualName,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the DateTime has DateTimeKind.Unspecified.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The DateTime value to check</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when DateTime is not Unspecified</exception>
        public static void IsUnspecified(this Assert _,
                                         DateTime actual,
                                         string because,
                                         string fix,
                                         [CallerArgumentExpression(nameof(actual))]
                                         string actualName = "",
                                         [CallerFilePath] string callerFilePath = "",
                                         [CallerMemberName] string callerMemberName = "",
                                         [CallerLineNumber] int callerLineNumber = 0)
        {
            if (actual.Kind == DateTimeKind.Unspecified)
            {
                return;
            }

            var output = BuildDateTimeKindOutput(expectedKind: DateTimeKind.Unspecified,
                                                 actual: actual,
                                                 actualName: actualName,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private helper for building DateTime Kind output
        private static string BuildDateTimeKindOutput(DateTimeKind expectedKind,
                                                      DateTime actual,
                                                      string actualName,
                                                      string because,
                                                      string fix,
                                                      string callerFilePath,
                                                      string callerMemberName,
                                                      int callerLineNumber)
        {
            var textDecorator = GetTextDecorator(callerFilePath);
            var sb = new StringBuilder();

            // Header
            var title = $"DATETIME KIND - EXPECTED {expectedKind.ToString().ToUpperInvariant()}";
            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = $"Expected DateTime to have Kind = {expectedKind} but it was {actual.Kind}.";
            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {actualName}");
            sb.AppendLine($"{"Value",-10} : {actual:O}");
            sb.AppendLine($"{"Expected",-10} : DateTimeKind.{expectedKind}");
            sb.AppendLine($"{"Actual",-10} : DateTimeKind.{actual.Kind}");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = expectedKind switch
            {
                DateTimeKind.Utc => new[]
                {
                    $"Use DateTime.UtcNow instead of DateTime.Now when creating '{actualName}'",
                    $"Convert '{actualName}' to UTC using .ToUniversalTime()",
                    $"Use DateTime.SpecifyKind({actualName}, DateTimeKind.Utc) if the value is already in UTC",
                    "Ensure database or API responses return UTC timestamps"
                },
                DateTimeKind.Local => new[]
                {
                    $"Use DateTime.Now instead of DateTime.UtcNow when creating '{actualName}'",
                    $"Convert '{actualName}' to local time using .ToLocalTime()",
                    $"Use DateTime.SpecifyKind({actualName}, DateTimeKind.Local) if the value is already in local time",
                    "Consider if local time is appropriate or if UTC would be better for consistency"
                },
                DateTimeKind.Unspecified => new[]
                {
                    $"Use DateTime.SpecifyKind({actualName}, DateTimeKind.Unspecified) to explicitly set the kind",
                    $"Create '{actualName}' using the DateTime constructor without timezone information",
                    "Review if Unspecified is appropriate or if you should use UTC or Local instead",
                    "Be aware that Unspecified DateTimes can cause timezone-related bugs"
                },
                _ => Array.Empty<string>()
            };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Helper to get appropriate text decorator based on build configuration
#pragma warning disable CA1859 // Use concrete types when possible for improved performance - interface needed for flexibility
        private static ITextDecorator GetTextDecorator(string callerFilePath)
        {
            // Default to ANSI colors for release builds
            // Can be enhanced to detect debug mode if needed
#if DEBUG
            return new PlainTextDecorator();
#else
            return new AnsiColorTextDecorator();
#endif
        }
#pragma warning restore CA1859
    }
}