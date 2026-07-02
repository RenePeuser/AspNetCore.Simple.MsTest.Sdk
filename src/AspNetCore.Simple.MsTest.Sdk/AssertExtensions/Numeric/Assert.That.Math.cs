using System;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// MATHEMATICAL PROPERTY assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the integer value is even.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The integer value to check</param>
        /// <param name="because">Why this value should be even (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not even</exception>
        public static void IsEven(this Assert _,
                                  int value,
                                  string because,
                                  string fix,
                                  [CallerArgumentExpression(nameof(value))]
                                  string valueName = "",
                                  [CallerFilePath] string callerFilePath = "",
                                  [CallerMemberName] string callerMemberName = "",
                                  [CallerLineNumber] int callerLineNumber = 0)
        {
            if (value % 2 == 0)
            {
                return;
            }

            var output = BuildMathPropertyOutput(propertyName: "Even",
                                                 propertyCheck: "divisible by 2",
                                                 valueName: valueName,
                                                 actualValue: value,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber,
                                                 additionalOptions: new[]
                                                 {
                                                     $"Ensure '{valueName}' is calculated or set to an even number",
                                                     "Use modulo operation (% 2 == 0) to verify even values before this assertion",
                                                     $"If '{valueName}' comes from user input, add validation or rounding logic"
                                                 });

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the integer value is odd.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The integer value to check</param>
        /// <param name="because">Why this value should be odd (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not odd</exception>
        public static void IsOdd(this Assert _,
                                 int value,
                                 string because,
                                 string fix,
                                 [CallerArgumentExpression(nameof(value))]
                                 string valueName = "",
                                 [CallerFilePath] string callerFilePath = "",
                                 [CallerMemberName] string callerMemberName = "",
                                 [CallerLineNumber] int callerLineNumber = 0)
        {
            if (value % 2 != 0)
            {
                return;
            }

            var output = BuildMathPropertyOutput(propertyName: "Odd",
                                                 propertyCheck: "not divisible by 2",
                                                 valueName: valueName,
                                                 actualValue: value,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber,
                                                 additionalOptions: new[]
                                                 {
                                                     $"Ensure '{valueName}' is calculated or set to an odd number",
                                                     "Use modulo operation (% 2 != 0) to verify odd values before this assertion",
                                                     $"If '{valueName}' comes from user input, add validation logic"
                                                 });

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the value is positive (greater than zero).
        /// </summary>
        /// <typeparam name="T">The type of the value (must be comparable)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="because">Why this value should be positive (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not positive</exception>
        public static void IsPositive<T>(this Assert _,
                                         T value,
                                         string because,
                                         string fix,
                                         [CallerArgumentExpression(nameof(value))]
                                         string valueName = "",
                                         [CallerFilePath] string callerFilePath = "",
                                         [CallerMemberName] string callerMemberName = "",
                                         [CallerLineNumber] int callerLineNumber = 0) where T : IComparable<T>
        {
            dynamic zero = Convert.ChangeType(0, typeof(T));

            if (value.CompareTo(zero) > 0)
            {
                return;
            }

            var output = BuildMathPropertyOutput(propertyName: "Positive",
                                                 propertyCheck: "greater than zero",
                                                 valueName: valueName,
                                                 actualValue: value,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber,
                                                 additionalOptions: new[]
                                                 {
                                                     $"Verify that '{valueName}' is set to a value greater than zero",
                                                     $"Check calculations or operations that produce '{valueName}' for correctness",
                                                     "Add validation to ensure positive values before this assertion",
                                                     $"Consider using Math.Abs() if '{valueName}' should always be positive"
                                                 });

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the value is negative (less than zero).
        /// </summary>
        /// <typeparam name="T">The type of the value (must be comparable)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="because">Why this value should be negative (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not negative</exception>
        public static void IsNegative<T>(this Assert _,
                                         T value,
                                         string because,
                                         string fix,
                                         [CallerArgumentExpression(nameof(value))]
                                         string valueName = "",
                                         [CallerFilePath] string callerFilePath = "",
                                         [CallerMemberName] string callerMemberName = "",
                                         [CallerLineNumber] int callerLineNumber = 0) where T : IComparable<T>
        {
            dynamic zero = Convert.ChangeType(0, typeof(T));

            if (value.CompareTo(zero) < 0)
            {
                return;
            }

            var output = BuildMathPropertyOutput(propertyName: "Negative",
                                                 propertyCheck: "less than zero",
                                                 valueName: valueName,
                                                 actualValue: value,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber,
                                                 additionalOptions: new[]
                                                 {
                                                     $"Verify that '{valueName}' is set to a value less than zero",
                                                     $"Check calculations or operations that produce '{valueName}' for correctness",
                                                     "Add validation to ensure negative values before this assertion",
                                                     "Review the business logic that should produce negative values"
                                                 });

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the value is zero.
        /// </summary>
        /// <typeparam name="T">The type of the value (must be comparable)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="because">Why this value should be zero (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not zero</exception>
        public static void IsZero<T>(this Assert _,
                                     T value,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(value))]
                                     string valueName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0) where T : IComparable<T>
        {
            dynamic zero = Convert.ChangeType(0, typeof(T));

            if (value.CompareTo(zero) == 0)
            {
                return;
            }

            var output = BuildMathPropertyOutput(propertyName: "Zero",
                                                 propertyCheck: "equal to zero",
                                                 valueName: valueName,
                                                 actualValue: value,
                                                 because: because,
                                                 fix: fix,
                                                 callerFilePath: callerFilePath,
                                                 callerMemberName: callerMemberName,
                                                 callerLineNumber: callerLineNumber,
                                                 additionalOptions: new[]
                                                 {
                                                     $"Ensure '{valueName}' is explicitly set to zero for this test scenario",
                                                     $"Check that calculations involving '{valueName}' correctly result in zero",
                                                     "Review the initialization or reset logic for this value",
                                                     "Verify that default values are properly configured to zero"
                                                 });

            throw new AssertFailedException(output);
        }

        // Private helper for building mathematical property assertion output
        private static string BuildMathPropertyOutput<T>(string propertyName,
                                                         string propertyCheck,
                                                         string valueName,
                                                         T actualValue,
                                                         string because,
                                                         string fix,
                                                         string callerFilePath,
                                                         string callerMemberName,
                                                         int callerLineNumber,
                                                         string[] additionalOptions)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = $"MATHEMATICAL PROPERTY FAILED - EXPECTED {propertyName.ToUpperInvariant()}";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = $"Expected value to be {propertyName.ToLowerInvariant()} ({propertyCheck}) but it was not.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {valueName}");
            sb.AppendLine($"{"Type",-10} : {typeof(T).Name}");
            sb.AppendLine($"{"Value",-10} : {actualValue}");
            sb.AppendLine($"{"Expected",-10} : {propertyName} ({propertyCheck})");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}