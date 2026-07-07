using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// EQUALITY assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that two values are equal using default equality comparison.
        /// </summary>
        /// <typeparam name="T">The type of the values</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="expected">The expected value</param>
        /// <param name="actual">The actual value</param>
        /// <param name="because">Why these values should be equal (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="expectedName">Auto-captured variable name for expected</param>
        /// <param name="actualName">Auto-captured variable name for actual</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when values are not equal</exception>
        public static void AreEqual<T>(this Assert _,
                                       T expected,
                                       T actual,
                                       string because,
                                       string fix,
                                       [CallerArgumentExpression(nameof(expected))]
                                       string expectedName = "",
                                       [CallerArgumentExpression(nameof(actual))]
                                       string actualName = "",
                                       [CallerFilePath] string callerFilePath = "",
                                       [CallerMemberName] string callerMemberName = "",
                                       [CallerLineNumber] int callerLineNumber = 0)
        {
            // If both are collections (but not strings), use collection comparison logic
            if (expected is IEnumerable && actual is IEnumerable &&
                expected is not string && actual is not string)
            {
                // Try to compare as collections
                var expList = ((IEnumerable)expected).Cast<object?>().ToList();
                var actList = ((IEnumerable)actual).Cast<object?>().ToList();

                if (expList.Count == actList.Count && expList.SequenceEqual(actList))
                {
                    return;
                }

                // Fall through to error reporting
            }
            else if (Equals(expected, actual))
            {
                return;
            }

            var output = BuildEqualityOutput(expectEqual: true,
                                             expectedValue: expected,
                                             actualValue: actual,
                                             expectedName: expectedName,
                                             actualName: actualName,
                                             because: because,
                                             fix: fix,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that two values are NOT equal using default equality comparison.
        /// </summary>
        /// <typeparam name="T">The type of the values</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="expected">The expected value (should differ from actual)</param>
        /// <param name="actual">The actual value</param>
        /// <param name="because">Why these values should differ (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="expectedName">Auto-captured variable name for expected</param>
        /// <param name="actualName">Auto-captured variable name for actual</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when values are equal</exception>
        public static void AreNotEqual<T>(this Assert _,
                                          T expected,
                                          T actual,
                                          string because,
                                          string fix,
                                          [CallerArgumentExpression(nameof(expected))]
                                          string expectedName = "",
                                          [CallerArgumentExpression(nameof(actual))]
                                          string actualName = "",
                                          [CallerFilePath] string callerFilePath = "",
                                          [CallerMemberName] string callerMemberName = "",
                                          [CallerLineNumber] int callerLineNumber = 0)
        {
            if (!Equals(expected, actual))
            {
                return;
            }

            var output = BuildEqualityOutput(expectEqual: false,
                                             expectedValue: expected,
                                             actualValue: actual,
                                             expectedName: expectedName,
                                             actualName: actualName,
                                             because: because,
                                             fix: fix,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that two references point to the same object instance.
        /// </summary>
        /// <typeparam name="T">The type of the references</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="expected">The expected reference</param>
        /// <param name="actual">The actual reference</param>
        /// <param name="because">Why these references should be the same (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="expectedName">Auto-captured variable name for expected</param>
        /// <param name="actualName">Auto-captured variable name for actual</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when references are not the same</exception>
        public static void AreSame<T>(this Assert _,
                                      T expected,
                                      T actual,
                                      string because,
                                      string fix,
                                      [CallerArgumentExpression(nameof(expected))]
                                      string expectedName = "",
                                      [CallerArgumentExpression(nameof(actual))]
                                      string actualName = "",
                                      [CallerFilePath] string callerFilePath = "",
                                      [CallerMemberName] string callerMemberName = "",
                                      [CallerLineNumber] int callerLineNumber = 0) where T : class
        {
            if (ReferenceEquals(expected, actual))
            {
                return;
            }

            var output = BuildReferenceEqualityOutput(expectSame: true,
                                                      expectedValue: expected,
                                                      actualValue: actual,
                                                      expectedName: expectedName,
                                                      actualName: actualName,
                                                      because: because,
                                                      fix: fix,
                                                      callerFilePath: callerFilePath,
                                                      callerMemberName: callerMemberName,
                                                      callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that two references do NOT point to the same object instance.
        /// </summary>
        /// <typeparam name="T">The type of the references</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="expected">The expected reference (should differ from actual)</param>
        /// <param name="actual">The actual reference</param>
        /// <param name="because">Why these references should differ (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="expectedName">Auto-captured variable name for expected</param>
        /// <param name="actualName">Auto-captured variable name for actual</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when references are the same</exception>
        public static void AreNotSame<T>(this Assert _,
                                         T expected,
                                         T actual,
                                         string because,
                                         string fix,
                                         [CallerArgumentExpression(nameof(expected))]
                                         string expectedName = "",
                                         [CallerArgumentExpression(nameof(actual))]
                                         string actualName = "",
                                         [CallerFilePath] string callerFilePath = "",
                                         [CallerMemberName] string callerMemberName = "",
                                         [CallerLineNumber] int callerLineNumber = 0) where T : class
        {
            if (!ReferenceEquals(expected, actual))
            {
                return;
            }

            var output = BuildReferenceEqualityOutput(expectSame: false,
                                                      expectedValue: expected,
                                                      actualValue: actual,
                                                      expectedName: expectedName,
                                                      actualName: actualName,
                                                      because: because,
                                                      fix: fix,
                                                      callerFilePath: callerFilePath,
                                                      callerMemberName: callerMemberName,
                                                      callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private helper for building equality assertion output
        private static string BuildEqualityOutput<T>(bool expectEqual,
                                                     T expectedValue,
                                                     T actualValue,
                                                     string expectedName,
                                                     string actualName,
                                                     string because,
                                                     string fix,
                                                     string callerFilePath,
                                                     string callerMemberName,
                                                     int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Detect if we're comparing collections (but not strings)
            var isCollectionComparison = expectedValue is IEnumerable && actualValue is IEnumerable &&
                                         expectedValue is not string && actualValue is not string;

            // Header
            var title = isCollectionComparison
                            ? (expectEqual
                                   ? "COLLECTION EQUALITY FAILED - ORDER MATTERS"
                                   : "COLLECTION EQUALITY FAILED - VALUES ARE EQUAL")
                            : (expectEqual
                                   ? "EQUALITY FAILED - VALUES NOT EQUAL"
                                   : "EQUALITY FAILED - VALUES ARE EQUAL");

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = isCollectionComparison
                              ? (expectEqual
                                     ? "Expected collections to be equal (same elements in the same order) but they differ."
                                     : "Expected collections to be different but they are equal.")
                              : (expectEqual
                                     ? "Expected values to be equal but they differ."
                                     : "Expected values to be different but they are equal.");

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);

            if (isCollectionComparison && expectEqual)
            {
                // Collection-specific details
                var expList = ((IEnumerable)expectedValue!).Cast<object?>().ToList();
                var actList = ((IEnumerable)actualValue!).Cast<object?>().ToList();

                sb.AppendLine($"{"Expected",-15} : {expectedName}");
                sb.AppendLine($"{"Actual",-15} : {actualName}");
                sb.AppendLine($"{"Expected Count",-15} : {expList.Count}");
                sb.AppendLine($"{"Actual Count",-15} : {actList.Count}");
                sb.AppendLine();

                // Show element comparison
                var maxDisplay = Math.Min(10, Math.Max(expList.Count, actList.Count));

                if (maxDisplay > 0)
                {
                    sb.AppendLine("Element Comparison:");
                    sb.AppendLine($"{"Index",-8} {"Expected",-30} {"Actual",-30} {"Match",-10}");
                    sb.AppendLine(new string('-', 78));

                    for (var i = 0; i < maxDisplay; i++)
                    {
                        var expectedElem = i < expList.Count ? expList[i]?.ToString() ?? "null" : "<missing>";
                        var actualElem = i < actList.Count ? actList[i]?.ToString() ?? "null" : "<missing>";

                        var match = i < expList.Count && i < actList.Count &&
                                    Equals(expList[i], actList[i]);

                        // Truncate long values
                        if (expectedElem.Length > 25)
                        {
                            expectedElem = expectedElem[..25] + "...";
                        }

                        if (actualElem.Length > 25)
                        {
                            actualElem = actualElem[..25] + "...";
                        }

                        var matchSymbol = match ? "✓" : "✗";
                        sb.AppendLine($"{i,-8} {expectedElem,-30} {actualElem,-30} {matchSymbol,-10}");
                    }

                    if (Math.Max(expList.Count, actList.Count) > maxDisplay)
                    {
                        sb.AppendLine($"... ({Math.Max(expList.Count, actList.Count) - maxDisplay} more elements)");
                    }
                }

                sb.AppendLine();
            }
            else
            {
                // Standard value details
                sb.AppendLine($"{"Expected",-10} : {expectedName}");
                sb.AppendLine($"{"Actual",-10} : {actualName}");
                sb.AppendLine($"{"Type",-10} : {typeof(T).Name}");
                sb.AppendLine();
                sb.AppendLine($"{"Expected Value",-15} : {FormatValue(expectedValue)}");
                sb.AppendLine($"{"Actual Value",-15} : {FormatValue(actualValue)}");
                sb.AppendLine($"{"Are Equal",-15} : {Equals(expectedValue, actualValue)}");
                sb.AppendLine();
            }

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = isCollectionComparison
                                        ? (expectEqual
                                               ? new[]
                                                 {
                                                     $"Verify that '{actualName}' is populated with the correct elements in the correct order", $"Check if the ordering logic for '{actualName}' matches the expected sequence", "Consider using AreEquivalent() if order doesn't matter",
                                                     "Review the data source or transformation that produces the actual collection"
                                                 }
                                               : new[] { $"Ensure '{actualName}' generates unique values for this scenario", $"Check if '{expectedName}' and '{actualName}' should use different sources", "Verify the logic that differentiates these values" })
                                        : (expectEqual
                                               ? new[]
                                                 {
                                                     $"Verify that '{actualName}' is calculated correctly", $"Check the source of '{actualName}' for incorrect values", $"Ensure '{expectedName}' matches the actual business requirements",
                                                     "Consider if custom equality comparison is needed"
                                                 }
                                               : new[] { $"Ensure '{actualName}' generates unique values for this scenario", $"Check if '{expectedName}' and '{actualName}' should use different sources", "Verify the logic that differentiates these values" });

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Private helper for building reference equality assertion output
        private static string BuildReferenceEqualityOutput<T>(bool expectSame,
                                                              T expectedValue,
                                                              T actualValue,
                                                              string expectedName,
                                                              string actualName,
                                                              string because,
                                                              string fix,
                                                              string callerFilePath,
                                                              string callerMemberName,
                                                              int callerLineNumber) where T : class
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = expectSame
                            ? "REFERENCE EQUALITY FAILED - DIFFERENT INSTANCES"
                            : "REFERENCE EQUALITY FAILED - SAME INSTANCE";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = expectSame
                              ? "Expected both references to point to the same object instance, but they reference different instances."
                              : "Expected references to point to different object instances, but they reference the same instance.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Expected",-10} : {expectedName}");
            sb.AppendLine($"{"Actual",-10} : {actualName}");
            sb.AppendLine($"{"Type",-10} : {typeof(T).Name}");
            sb.AppendLine();
            sb.AppendLine($"{"Expected Ref",-15} : {GetReferenceInfo(expectedValue)}");
            sb.AppendLine($"{"Actual Ref",-15} : {GetReferenceInfo(actualValue)}");
            sb.AppendLine($"{"Are Same",-15} : {ReferenceEquals(expectedValue, actualValue)}");
            sb.AppendLine($"{"Are Equal",-15} : {Equals(expectedValue, actualValue)}");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = expectSame
                                        ? new[]
                                          {
                                              $"Ensure '{actualName}' returns the same cached/singleton instance as '{expectedName}'", $"Check if '{actualName}' is creating a new instance instead of reusing existing one", "Verify dependency injection lifetime (Singleton vs Transient vs Scoped)",
                                              "Review object creation logic to ensure proper instance sharing"
                                          }
                                        : new[]
                                          {
                                              $"Ensure '{actualName}' creates a new independent instance", $"Check if '{actualName}' is incorrectly returning a cached instance", "Verify that cloning or copying logic creates deep copies",
                                              "Review instance creation to ensure independence"
                                          };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Helper to format values for display
        private static string FormatValue<T>(T? value)
        {
            if (value is null)
            {
                return "null";
            }

            if (value is string str)
            {
                return $"\"{str}\"";
            }

            return value.ToString() ?? "(no ToString)";
        }

        // Helper to get reference information
        private static string GetReferenceInfo<T>(T? obj) where T : class
        {
            if (obj is null)
            {
                return "null";
            }

            return $"{obj.GetType().Name}@{RuntimeHelpers.GetHashCode(obj):X8}";
        }
    }
}