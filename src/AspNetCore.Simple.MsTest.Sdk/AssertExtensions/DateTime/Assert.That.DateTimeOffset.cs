using System.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// DATETIMEOFFSET assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the actual DateTimeOffset is after the expected DateTimeOffset.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual DateTimeOffset value</param>
        /// <param name="expected">The expected DateTimeOffset to compare against</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual is not after expected</exception>
        public static void IsAfter(this Assert _,
                                   DateTimeOffset actual,
                                   DateTimeOffset expected,
                                   string because,
                                   string fix,
                                   [CallerArgumentExpression(nameof(actual))]
                                   string actualName = "",
                                   [CallerFilePath] string callerFilePath = "",
                                   [CallerMemberName] string callerMemberName = "",
                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (actual > expected)
            {
                return;
            }

            var output = BuildDateTimeOffsetComparisonOutput(comparisonType: "AFTER",
                                                             actual: actual,
                                                             expected: expected,
                                                             actualName: actualName,
                                                             because: because,
                                                             fix: fix,
                                                             callerFilePath: callerFilePath,
                                                             callerMemberName: callerMemberName,
                                                             callerLineNumber: callerLineNumber,
                                                             callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the actual DateTimeOffset is before the expected DateTimeOffset.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual DateTimeOffset value</param>
        /// <param name="expected">The expected DateTimeOffset to compare against</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual is not before expected</exception>
        public static void IsBefore(this Assert _,
                                    DateTimeOffset actual,
                                    DateTimeOffset expected,
                                    string because,
                                    string fix,
                                    [CallerArgumentExpression(nameof(actual))]
                                    string actualName = "",
                                    [CallerFilePath] string callerFilePath = "",
                                    [CallerMemberName] string callerMemberName = "",
                                    [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (actual < expected)
            {
                return;
            }

            var output = BuildDateTimeOffsetComparisonOutput(comparisonType: "BEFORE",
                                                             actual: actual,
                                                             expected: expected,
                                                             actualName: actualName,
                                                             because: because,
                                                             fix: fix,
                                                             callerFilePath: callerFilePath,
                                                             callerMemberName: callerMemberName,
                                                             callerLineNumber: callerLineNumber,
                                                             callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the actual DateTimeOffset is within the specified range (inclusive).
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual DateTimeOffset value</param>
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
                                     DateTimeOffset actual,
                                     DateTimeOffset start,
                                     DateTimeOffset end,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(actual))]
                                     string actualName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (actual >= start && actual <= end)
            {
                return;
            }

            var output = BuildDateTimeOffsetRangeOutput(actual: actual,
                                                        start: start,
                                                        end: end,
                                                        actualName: actualName,
                                                        because: because,
                                                        fix: fix,
                                                        callerFilePath: callerFilePath,
                                                        callerMemberName: callerMemberName,
                                                        callerLineNumber: callerLineNumber,
                                                        callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the actual DateTimeOffset is close to the expected DateTimeOffset within the specified tolerance.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual DateTimeOffset value</param>
        /// <param name="expected">The expected DateTimeOffset value</param>
        /// <param name="tolerance">The allowed time difference</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual is not within tolerance of expected</exception>
        public static void IsCloseTo(this Assert _,
                                     DateTimeOffset actual,
                                     DateTimeOffset expected,
                                     TimeSpan tolerance,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(actual))]
                                     string actualName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            var difference = actual > expected ? actual - expected : expected - actual;

            if (difference <= tolerance)
            {
                return;
            }

            var output = BuildDateTimeOffsetToleranceOutput(actual: actual,
                                                            expected: expected,
                                                            tolerance: tolerance,
                                                            actualDifference: difference,
                                                            actualName: actualName,
                                                            because: because,
                                                            fix: fix,
                                                            callerFilePath: callerFilePath,
                                                            callerMemberName: callerMemberName,
                                                            callerLineNumber: callerLineNumber,
                                                            callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the DateTimeOffset has the specified UTC offset.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The DateTimeOffset value to check</param>
        /// <param name="expectedOffset">The expected UTC offset</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when offset does not match</exception>
        public static void HasOffset(this Assert _,
                                     DateTimeOffset actual,
                                     TimeSpan expectedOffset,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(actual))]
                                     string actualName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (actual.Offset == expectedOffset)
            {
                return;
            }

            var output = BuildDateTimeOffsetOffsetOutput(actual: actual,
                                                         expectedOffset: expectedOffset,
                                                         actualName: actualName,
                                                         because: because,
                                                         fix: fix,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber,
                                                         callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the DateTimeOffset has UTC offset (offset = 00:00).
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The DateTimeOffset value to check</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when offset is not UTC</exception>
        public static void IsUtc(this Assert _,
                                 DateTimeOffset actual,
                                 string because,
                                 string fix,
                                 [CallerArgumentExpression(nameof(actual))]
                                 string actualName = "",
                                 [CallerFilePath] string callerFilePath = "",
                                 [CallerMemberName] string callerMemberName = "",
                                 [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (actual.Offset == TimeSpan.Zero)
            {
                return;
            }

            var output = BuildDateTimeOffsetOffsetOutput(actual: actual,
                                                         expectedOffset: TimeSpan.Zero,
                                                         actualName: actualName,
                                                         because: because,
                                                         fix: fix,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber,
                                                         callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the DateTimeOffset has local timezone offset.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The DateTimeOffset value to check</param>
        /// <param name="because">Why this assertion exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when offset is not local</exception>
        public static void IsLocal(this Assert _,
                                   DateTimeOffset actual,
                                   string because,
                                   string fix,
                                   [CallerArgumentExpression(nameof(actual))]
                                   string actualName = "",
                                   [CallerFilePath] string callerFilePath = "",
                                   [CallerMemberName] string callerMemberName = "",
                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            var localOffset = TimeZoneInfo.Local.GetUtcOffset(actual.DateTime);

            if (actual.Offset == localOffset)
            {
                return;
            }

            var output = BuildDateTimeOffsetOffsetOutput(actual: actual,
                                                         expectedOffset: localOffset,
                                                         actualName: actualName,
                                                         because: because,
                                                         fix: fix,
                                                         callerFilePath: callerFilePath,
                                                         callerMemberName: callerMemberName,
                                                         callerLineNumber: callerLineNumber,
                                                         isLocalCheck: true,
                                                         callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        // Private helper for building DateTimeOffset comparison output
        private static string BuildDateTimeOffsetComparisonOutput(string comparisonType,
                                                                  DateTimeOffset actual,
                                                                  DateTimeOffset expected,
                                                                  string actualName,
                                                                  string because,
                                                                  string fix,
                                                                  string callerFilePath,
                                                                  string callerMemberName,
                                                                  int callerLineNumber,
                                                                  Assembly callingAssembly)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator(callingAssembly);
            var sb = new StringBuilder();

            // Header
            var title = $"DATETIMEOFFSET COMPARISON - EXPECTED {comparisonType}";
            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = comparisonType == "AFTER"
                              ? "Expected DateTimeOffset to be after the comparison value but it was not."
                              : "Expected DateTimeOffset to be before the comparison value but it was not.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {actualName}");
            sb.AppendLine($"{"Actual",-10} : {actual:O}");
            sb.AppendLine($"{"Expected",-10} : {comparisonType} {expected:O}");

            var difference = actual > expected ? actual - expected : expected - actual;
            var direction = actual > expected ? "ahead" : "behind";
            sb.AppendLine($"{"Difference",-10} : {difference.TotalSeconds:F3} seconds ({direction})");

            sb.AppendLine($"{"Actual TZ",-10} : UTC{(actual.Offset >= TimeSpan.Zero ? "+" : "")}{actual.Offset:hh\\:mm}");
            sb.AppendLine($"{"Expected TZ",-10} : UTC{(expected.Offset >= TimeSpan.Zero ? "+" : "")}{expected.Offset:hh\\:mm}");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = comparisonType == "AFTER"
                                        ? new[]
                                          {
                                              $"Verify that '{actualName}' is set to a time later than the comparison value", "Check if the DateTimeOffset values are using compatible timezones", $"Review the logic that sets '{actualName}' to ensure it occurs chronologically after the expected time",
                                              "DateTimeOffset comparisons are timezone-aware and compare absolute points in time"
                                          }
                                        : new[]
                                          {
                                              $"Verify that '{actualName}' is set to a time earlier than the comparison value", "Check if the DateTimeOffset values are using compatible timezones", $"Review the logic that sets '{actualName}' to ensure it occurs chronologically before the expected time",
                                              "DateTimeOffset comparisons are timezone-aware and compare absolute points in time"
                                          };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Private helper for building DateTimeOffset range output
        private static string BuildDateTimeOffsetRangeOutput(DateTimeOffset actual,
                                                             DateTimeOffset start,
                                                             DateTimeOffset end,
                                                             string actualName,
                                                             string because,
                                                             string fix,
                                                             string callerFilePath,
                                                             string callerMemberName,
                                                             int callerLineNumber,
                                                             Assembly callingAssembly)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator(callingAssembly);
            var sb = new StringBuilder();

            // Header
            AssertOutputHelper.BuildHeader(sb, "DATETIMEOFFSET RANGE - VALUE OUT OF RANGE", textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = "Expected DateTimeOffset to be within the specified range but it was outside the bounds.";
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
                                        $"Verify that '{actualName}' falls between {start:O} and {end:O}", "DateTimeOffset range comparisons are timezone-aware and compare absolute points in time", $"Review the logic that sets '{actualName}' to ensure it produces values within the expected range",
                                        "Consider widening the acceptable range if edge cases are valid", "Verify that the range boundaries (start/end) are correctly defined"
                                    };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Private helper for building DateTimeOffset tolerance output
        private static string BuildDateTimeOffsetToleranceOutput(DateTimeOffset actual,
                                                                 DateTimeOffset expected,
                                                                 TimeSpan tolerance,
                                                                 TimeSpan actualDifference,
                                                                 string actualName,
                                                                 string because,
                                                                 string fix,
                                                                 string callerFilePath,
                                                                 string callerMemberName,
                                                                 int callerLineNumber,
                                                                 Assembly callingAssembly)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator(callingAssembly);
            var sb = new StringBuilder();

            // Header
            AssertOutputHelper.BuildHeader(sb, "DATETIMEOFFSET TOLERANCE - EXCEEDED THRESHOLD", textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = "Expected DateTimeOffset to be close to the target value within tolerance but the difference exceeded the threshold.";
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
                                        $"Verify that '{actualName}' is set close to {expected:O}", $"Consider increasing the tolerance if {tolerance.TotalSeconds:F3} seconds is too strict", "Check for timing issues or delays in the code that might cause larger differences",
                                        "DateTimeOffset comparisons are timezone-aware and compare absolute points in time", $"Review the logic that sets '{actualName}' to reduce the time difference"
                                    };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Private helper for building DateTimeOffset offset output
        private static string BuildDateTimeOffsetOffsetOutput(DateTimeOffset actual,
                                                              TimeSpan expectedOffset,
                                                              string actualName,
                                                              string because,
                                                              string fix,
                                                              string callerFilePath,
                                                              string callerMemberName,
                                                              int callerLineNumber,
                                                              Assembly callingAssembly,
                                                              bool isLocalCheck = false)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator(callingAssembly);
            var sb = new StringBuilder();

            // Header
            var title = isLocalCheck ? "DATETIMEOFFSET OFFSET - EXPECTED LOCAL TIMEZONE" : "DATETIMEOFFSET OFFSET - UNEXPECTED OFFSET";
            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = isLocalCheck
                              ? $"Expected DateTimeOffset to have local timezone offset but it did not."
                              : $"Expected DateTimeOffset to have offset UTC{(expectedOffset >= TimeSpan.Zero ? "+" : "")}{expectedOffset:hh\\:mm} but it had a different offset.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-15} : {actualName}");
            sb.AppendLine($"{"Value",-15} : {actual:O}");
            sb.AppendLine($"{"Expected Offset",-15} : UTC{(expectedOffset >= TimeSpan.Zero ? "+" : "")}{expectedOffset:hh\\:mm}");
            sb.AppendLine($"{"Actual Offset",-15} : UTC{(actual.Offset >= TimeSpan.Zero ? "+" : "")}{actual.Offset:hh\\:mm}");

            if (isLocalCheck)
            {
                sb.AppendLine($"{"Local TZ",-15} : {TimeZoneInfo.Local.DisplayName}");
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = expectedOffset == TimeSpan.Zero
                                        ? new[]
                                          {
                                              $"Use DateTimeOffset.UtcNow instead of DateTimeOffset.Now when creating '{actualName}'", $"Convert '{actualName}' to UTC using .ToUniversalTime()", "Ensure API responses or database values return UTC timestamps",
                                              "Use .ToOffset(TimeSpan.Zero) to convert to UTC offset"
                                          }
                                        : isLocalCheck
                                            ? new[]
                                              {
                                                  $"Use DateTimeOffset.Now to get current time with local offset", $"Convert '{actualName}' to local offset using .ToLocalTime()", $"Use .ToOffset(TimeZoneInfo.Local.GetUtcOffset(dateTime)) to convert to local offset",
                                                  "Be aware that local timezone depends on the system timezone settings"
                                              }
                                            : new[]
                                              {
                                                  $"Use .ToOffset(expectedOffset) to convert '{actualName}' to the correct offset", "Verify the source of the DateTimeOffset and its timezone configuration", $"Ensure '{actualName}' is created with the correct timezone offset",
                                                  "Check if timezone conversion is happening unexpectedly"
                                              };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Helper to get appropriate text decorator based on build configuration
    }
}