using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// NUMERIC TOLERANCE assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the actual double value is within tolerance of the expected value.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual value</param>
        /// <param name="expected">The expected value</param>
        /// <param name="tolerance">The maximum allowed difference</param>
        /// <param name="because">Why this value should be close to expected (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual value is not within tolerance of expected</exception>
        public static void IsCloseTo(this Assert _,
                                     double actual,
                                     double expected,
                                     double tolerance,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(actual))]
                                     string actualName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (tolerance < 0)
            {
                throw new ArgumentException("Tolerance must be non-negative", nameof(tolerance));
            }

            var difference = Math.Abs(actual - expected);

            if (difference <= tolerance)
            {
                return;
            }

            var output = BuildToleranceOutput(actual: actual,
                                              expected: expected,
                                              tolerance: tolerance,
                                              difference: difference,
                                              typeName: "double",
                                              valueName: actualName,
                                              because: because,
                                              fix: fix,
                                              callerFilePath: callerFilePath,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber,
                                              callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the actual decimal value is within tolerance of the expected value.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual value</param>
        /// <param name="expected">The expected value</param>
        /// <param name="tolerance">The maximum allowed difference</param>
        /// <param name="because">Why this value should be close to expected (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual value is not within tolerance of expected</exception>
        public static void IsCloseTo(this Assert _,
                                     decimal actual,
                                     decimal expected,
                                     decimal tolerance,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(actual))]
                                     string actualName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (tolerance < 0)
            {
                throw new ArgumentException("Tolerance must be non-negative", nameof(tolerance));
            }

            var difference = Math.Abs(actual - expected);

            if (difference <= tolerance)
            {
                return;
            }

            var output = BuildToleranceOutput(actual: (double)actual,
                                              expected: (double)expected,
                                              tolerance: (double)tolerance,
                                              difference: (double)difference,
                                              typeName: "decimal",
                                              valueName: actualName,
                                              because: because,
                                              fix: fix,
                                              callerFilePath: callerFilePath,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber,
                                              callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the actual float value is within tolerance of the expected value.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="actual">The actual value</param>
        /// <param name="expected">The expected value</param>
        /// <param name="tolerance">The maximum allowed difference</param>
        /// <param name="because">Why this value should be close to expected (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actualName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when actual value is not within tolerance of expected</exception>
        public static void IsCloseTo(this Assert _,
                                     float actual,
                                     float expected,
                                     float tolerance,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(actual))]
                                     string actualName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (tolerance < 0)
            {
                throw new ArgumentException("Tolerance must be non-negative", nameof(tolerance));
            }

            var difference = Math.Abs(actual - expected);

            if (difference <= tolerance)
            {
                return;
            }

            var output = BuildToleranceOutput(actual: actual,
                                              expected: expected,
                                              tolerance: tolerance,
                                              difference: difference,
                                              typeName: "float",
                                              valueName: actualName,
                                              because: because,
                                              fix: fix,
                                              callerFilePath: callerFilePath,
                                              callerMemberName: callerMemberName,
                                              callerLineNumber: callerLineNumber,
                                              callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        // Private helper for building tolerance assertion output
        private static string BuildToleranceOutput(double actual,
                                                   double expected,
                                                   double tolerance,
                                                   double difference,
                                                   string typeName,
                                                   string valueName,
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
            var title = "TOLERANCE CHECK FAILED - VALUE OUT OF RANGE";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = $"Expected value to be within tolerance of {expected:G} but the difference exceeded the allowed threshold.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-15} : {valueName}");
            sb.AppendLine($"{"Type",-15} : {typeName}");
            sb.AppendLine($"{"Actual",-15} : {actual:G}");
            sb.AppendLine($"{"Expected",-15} : {expected:G}");
            sb.AppendLine($"{"Tolerance",-15} : ±{tolerance:G}");
            sb.AppendLine($"{"Difference",-15} : {difference:G}");
            sb.AppendLine($"{"Valid Range",-15} : [{expected - tolerance:G} to {expected + tolerance:G}]");
            sb.AppendLine($"{"Exceeded By",-15} : {difference - tolerance:G}");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = new[]
                                    {
                                        $"Review the calculation that produces '{valueName}' to ensure it matches the expected formula", $"Check if the tolerance of ±{tolerance:G} is appropriate for this scenario", $"Verify input values used in computing '{valueName}' are correct",
                                        "Consider if rounding errors or floating-point precision issues are affecting the result"
                                    };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}