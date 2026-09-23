using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// RANGE assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the value is within the specified range (inclusive).
        /// </summary>
        /// <typeparam name="T">The type of the value (must implement IComparable)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="min">The minimum allowed value (inclusive)</param>
        /// <param name="max">The maximum allowed value (inclusive)</param>
        /// <param name="because">Why this value should be in range (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is outside the range</exception>
        public static void IsInRange<T>(this Assert _,
                                        T value,
                                        T min,
                                        T max,
                                        string because,
                                        string fix,
                                        [CallerArgumentExpression(nameof(value))]
                                        string valueName = "",
                                        [CallerFilePath] string callerFilePath = "",
                                        [CallerMemberName] string callerMemberName = "",
                                        [CallerLineNumber] int callerLineNumber = 0) where T : IComparable<T>
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (value.CompareTo(min) >= 0 && value.CompareTo(max) <= 0)
            {
                return;
            }

            var output = BuildRangeAssertionOutput(expectInRange: true,
                                                   valueName: valueName,
                                                   actualValue: value,
                                                   minValue: min,
                                                   maxValue: max,
                                                   valueType: typeof(T),
                                                   because: because,
                                                   fix: fix,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber,
                                                   callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the value is outside the specified range (exclusive).
        /// </summary>
        /// <typeparam name="T">The type of the value (must implement IComparable)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="min">The minimum value that should be excluded</param>
        /// <param name="max">The maximum value that should be excluded</param>
        /// <param name="because">Why this value should be out of range (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is within the range</exception>
        public static void IsOutOfRange<T>(this Assert _,
                                           T value,
                                           T min,
                                           T max,
                                           string because,
                                           string fix,
                                           [CallerArgumentExpression(nameof(value))]
                                           string valueName = "",
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerMemberName] string callerMemberName = "",
                                           [CallerLineNumber] int callerLineNumber = 0) where T : IComparable<T>
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            {
                return;
            }

            var output = BuildRangeAssertionOutput(expectInRange: false,
                                                   valueName: valueName,
                                                   actualValue: value,
                                                   minValue: min,
                                                   maxValue: max,
                                                   valueType: typeof(T),
                                                   because: because,
                                                   fix: fix,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber,
                                                   callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        // Private helper for building range assertion output
        private static string BuildRangeAssertionOutput<T>(bool expectInRange,
                                                           string valueName,
                                                           T actualValue,
                                                           T minValue,
                                                           T maxValue,
                                                           Type valueType,
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
            var title = expectInRange
                            ? "RANGE CHECK - VALUE OUT OF RANGE"
                            : "RANGE CHECK - VALUE IN RANGE";

            assertOutputHelper.BuildHeader(sb, title);

            // Test Information
            assertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber);

            // Problem
            var problem = expectInRange
                              ? $"Expected value to be within range [{minValue}, {maxValue}] but it was outside."
                              : $"Expected value to be outside range [{minValue}, {maxValue}] but it was inside.";

            assertOutputHelper.BuildProblemSection(sb, problem);

            // Details
            assertOutputHelper.BuildDetailsSectionHeader(sb);
            sb.AppendLine($"{"Variable",-10} : {valueName}");
            sb.AppendLine($"{"Type",-10} : {valueType.Name}");
            sb.AppendLine($"{"Value",-10} : {actualValue}");
            sb.AppendLine($"{"Min",-10} : {minValue}");
            sb.AppendLine($"{"Max",-10} : {maxValue}");
            sb.AppendLine($"{"Expected",-10} : {(expectInRange ? "In Range" : "Out of Range")}");
            sb.AppendLine();

            // Context (Why)
            assertOutputHelper.BuildContextSection(sb, because);

            // Fix (How)
            var additionalOptions = expectInRange
                                        ? new[]
                                          {
                                              $"Verify that '{valueName}' is calculated correctly to fall within [{minValue}, {maxValue}]", $"Check boundary conditions that might push '{valueName}' outside the valid range", $"Consider adjusting the range limits if [{minValue}, {maxValue}] is too restrictive",
                                              $"Review input validation or data transformation logic for '{valueName}'"
                                          }
                                        : new[]
                                          {
                                              $"Ensure '{valueName}' is set to a value outside [{minValue}, {maxValue}]", $"Review the logic that generates '{valueName}' to avoid the excluded range", $"Check if the range boundaries [{minValue}, {maxValue}] are correctly defined",
                                              $"Verify that edge cases don't accidentally fall within the restricted range"
                                          };

            assertOutputHelper.BuildFixSection(sb, fix,
                                               additionalOptions);

            // Footer
            assertOutputHelper.BuildFooter(sb);

            return sb.ToString();
        }
    }
}