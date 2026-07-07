using System;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// NUMERIC COMPARISON assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the value is greater than the threshold.
        /// </summary>
        /// <typeparam name="T">The type of the value (must implement IComparable)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="threshold">The threshold value</param>
        /// <param name="because">Why this comparison exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not greater than threshold</exception>
        public static void IsGreaterThan<T>(this Assert _,
                                            T value,
                                            T threshold,
                                            string because,
                                            string fix,
                                            [CallerArgumentExpression(nameof(value))]
                                            string valueName = "",
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerMemberName] string callerMemberName = "",
                                            [CallerLineNumber] int callerLineNumber = 0) where T : IComparable<T>
        {
            if (value.CompareTo(threshold) > 0)
            {
                return;
            }

            var output = BuildComparisonOutput(comparisonType: "GREATER THAN",
                                               comparisonSymbol: ">",
                                               value: value,
                                               threshold: threshold,
                                               valueName: valueName,
                                               because: because,
                                               fix: fix,
                                               callerFilePath: callerFilePath,
                                               callerMemberName: callerMemberName,
                                               callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the value is greater than or equal to the threshold.
        /// </summary>
        /// <typeparam name="T">The type of the value (must implement IComparable)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="threshold">The threshold value</param>
        /// <param name="because">Why this comparison exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not greater than or equal to threshold</exception>
        public static void IsGreaterThanOrEqual<T>(this Assert _,
                                                   T value,
                                                   T threshold,
                                                   string because,
                                                   string fix,
                                                   [CallerArgumentExpression(nameof(value))]
                                                   string valueName = "",
                                                   [CallerFilePath] string callerFilePath = "",
                                                   [CallerMemberName] string callerMemberName = "",
                                                   [CallerLineNumber] int callerLineNumber = 0) where T : IComparable<T>
        {
            if (value.CompareTo(threshold) >= 0)
            {
                return;
            }

            var output = BuildComparisonOutput(comparisonType: "GREATER THAN OR EQUAL",
                                               comparisonSymbol: ">=",
                                               value: value,
                                               threshold: threshold,
                                               valueName: valueName,
                                               because: because,
                                               fix: fix,
                                               callerFilePath: callerFilePath,
                                               callerMemberName: callerMemberName,
                                               callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the value is less than the threshold.
        /// </summary>
        /// <typeparam name="T">The type of the value (must implement IComparable)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="threshold">The threshold value</param>
        /// <param name="because">Why this comparison exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not less than threshold</exception>
        public static void IsLessThan<T>(this Assert _,
                                         T value,
                                         T threshold,
                                         string because,
                                         string fix,
                                         [CallerArgumentExpression(nameof(value))]
                                         string valueName = "",
                                         [CallerFilePath] string callerFilePath = "",
                                         [CallerMemberName] string callerMemberName = "",
                                         [CallerLineNumber] int callerLineNumber = 0) where T : IComparable<T>
        {
            if (value.CompareTo(threshold) < 0)
            {
                return;
            }

            var output = BuildComparisonOutput(comparisonType: "LESS THAN",
                                               comparisonSymbol: "<",
                                               value: value,
                                               threshold: threshold,
                                               valueName: valueName,
                                               because: because,
                                               fix: fix,
                                               callerFilePath: callerFilePath,
                                               callerMemberName: callerMemberName,
                                               callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the value is less than or equal to the threshold.
        /// </summary>
        /// <typeparam name="T">The type of the value (must implement IComparable)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="threshold">The threshold value</param>
        /// <param name="because">Why this comparison exists (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not less than or equal to threshold</exception>
        public static void IsLessThanOrEqual<T>(this Assert _,
                                                T value,
                                                T threshold,
                                                string because,
                                                string fix,
                                                [CallerArgumentExpression(nameof(value))]
                                                string valueName = "",
                                                [CallerFilePath] string callerFilePath = "",
                                                [CallerMemberName] string callerMemberName = "",
                                                [CallerLineNumber] int callerLineNumber = 0) where T : IComparable<T>
        {
            if (value.CompareTo(threshold) <= 0)
            {
                return;
            }

            var output = BuildComparisonOutput(comparisonType: "LESS THAN OR EQUAL",
                                               comparisonSymbol: "<=",
                                               value: value,
                                               threshold: threshold,
                                               valueName: valueName,
                                               because: because,
                                               fix: fix,
                                               callerFilePath: callerFilePath,
                                               callerMemberName: callerMemberName,
                                               callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private helper for building comparison assertion output
        private static string BuildComparisonOutput<T>(string comparisonType,
                                                       string comparisonSymbol,
                                                       T value,
                                                       T threshold,
                                                       string valueName,
                                                       string because,
                                                       string fix,
                                                       string callerFilePath,
                                                       string callerMemberName,
                                                       int callerLineNumber) where T : IComparable<T>
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = $"COMPARISON FAILED - EXPECTED {comparisonType}";
            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = $"Expected value to be {comparisonType.ToLowerInvariant()} threshold but comparison failed.";
            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {valueName}");
            sb.AppendLine($"{"Type",-10} : {typeof(T).Name}");
            sb.AppendLine($"{"Actual",-10} : {value?.ToString() ?? "null"}");
            sb.AppendLine($"{"Threshold",-10} : {threshold?.ToString() ?? "null"}");
            sb.AppendLine($"{"Expected",-10} : {valueName} {comparisonSymbol} {threshold}");
            sb.AppendLine($"{"Result",-10} : Failed ({value} is not {comparisonSymbol} {threshold})");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = new[]
                                    {
                                        $"Verify that '{valueName}' is calculated correctly to meet the condition", $"Check if the threshold value '{threshold}' is appropriate for this scenario", $"Review the logic that produces '{valueName}' to ensure it satisfies {valueName} {comparisonSymbol} {threshold}",
                                        "Consider boundary conditions and edge cases in the code under test"
                                    };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}