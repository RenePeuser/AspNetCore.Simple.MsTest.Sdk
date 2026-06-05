using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// COLLECTION EQUALITY assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that two collections are equal (same elements in the same order).
        /// </summary>
        /// <typeparam name="T">The type of elements in the collections</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="expected">The expected collection</param>
        /// <param name="actual">The actual collection</param>
        /// <param name="because">Why these collections should be equal (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="expectedName">Auto-captured expected variable name</param>
        /// <param name="actualName">Auto-captured actual variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when collections are not equal</exception>
        public static void AreEqual<T>(this Assert _,
                                       IEnumerable<T> expected,
                                       IEnumerable<T> actual,
                                       string because,
                                       string fix,
                                       [CallerArgumentExpression(nameof(expected))]
                                       string expectedName = "",
                                       [CallerArgumentExpression(nameof(actual))]
                                       string actualName = "",
                                       [CallerFilePath] string callerFilePath = "",
                                       [CallerMemberName] string callerMemberName = "",
                                       [CallerLineNumber] int callerLineNumber = 0
        )
        {
            if (expected is null && actual is null)
            {
                return;
            }

            if (expected is null || actual is null)
            {
                var output = BuildCollectionEqualityOutput(expectedCollection: expected,
                                                           actualCollection: actual,
                                                           expectedName: expectedName,
                                                           actualName: actualName,
                                                           because: because,
                                                           fix: fix,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber,
                                                           checkOrder: true);

                throw new AssertFailedException(output);
            }

            var expectedList = expected.ToList();
            var actualList = actual.ToList();

            if (expectedList.Count != actualList.Count || !expectedList.SequenceEqual(actualList))
            {
                var output = BuildCollectionEqualityOutput(expectedCollection: expected,
                                                           actualCollection: actual,
                                                           expectedName: expectedName,
                                                           actualName: actualName,
                                                           because: because,
                                                           fix: fix,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber,
                                                           checkOrder: true);

                throw new AssertFailedException(output);
            }
        }

        /// <summary>
        /// Asserts that two collections are equivalent (same elements, order doesn't matter).
        /// </summary>
        /// <typeparam name="T">The type of elements in the collections</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="expected">The expected collection</param>
        /// <param name="actual">The actual collection</param>
        /// <param name="because">Why these collections should be equivalent (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="expectedName">Auto-captured expected variable name</param>
        /// <param name="actualName">Auto-captured actual variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when collections are not equivalent</exception>
        public static void AreEquivalent<T>(this Assert _,
                                            IEnumerable<T> expected,
                                            IEnumerable<T> actual,
                                            string because,
                                            string fix,
                                            [CallerArgumentExpression(nameof(expected))]
                                            string expectedName = "",
                                            [CallerArgumentExpression(nameof(actual))]
                                            string actualName = "",
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerMemberName] string callerMemberName = "",
                                            [CallerLineNumber] int callerLineNumber = 0
        )
        {
            if (expected is null && actual is null)
            {
                return;
            }

            if (expected is null || actual is null)
            {
                var output = BuildCollectionEqualityOutput(expectedCollection: expected,
                                                           actualCollection: actual,
                                                           expectedName: expectedName,
                                                           actualName: actualName,
                                                           because: because,
                                                           fix: fix,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber,
                                                           checkOrder: false);

                throw new AssertFailedException(output);
            }

            var expectedList = expected.ToList();
            var actualList = actual.ToList();

            // Check if collections have same elements regardless of order
            if (expectedList.Count != actualList.Count ||
                !expectedList.OrderBy(x => x).SequenceEqual(actualList.OrderBy(x => x)))
            {
                var output = BuildCollectionEqualityOutput(expectedCollection: expected,
                                                           actualCollection: actual,
                                                           expectedName: expectedName,
                                                           actualName: actualName,
                                                           because: because,
                                                           fix: fix,
                                                           callerFilePath: callerFilePath,
                                                           callerMemberName: callerMemberName,
                                                           callerLineNumber: callerLineNumber,
                                                           checkOrder: false);

                throw new AssertFailedException(output);
            }
        }

        // Private helper for building collection equality output
        private static string BuildCollectionEqualityOutput<T>(
            IEnumerable<T>? expectedCollection,
            IEnumerable<T>? actualCollection,
            string expectedName,
            string actualName,
            string because,
            string fix,
            string callerFilePath,
            string callerMemberName,
            int callerLineNumber,
            bool checkOrder
        )
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = checkOrder
                            ? "COLLECTION EQUALITY FAILED - ORDER MATTERS"
                            : "COLLECTION EQUIVALENCE FAILED - ORDER IGNORED";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = checkOrder
                              ? "Expected collections to be equal (same elements in the same order) but they differ."
                              : "Expected collections to be equivalent (same elements, order doesn't matter) but they differ.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);

            if (expectedCollection is null || actualCollection is null)
            {
                sb.AppendLine($"{"Expected",-15} : {expectedName}");
                sb.AppendLine($"{"Actual",-15} : {actualName}");
                sb.AppendLine($"{"Expected Value",-15} : {(expectedCollection is null ? "null" : "[collection]")}");
                sb.AppendLine($"{"Actual Value",-15} : {(actualCollection is null ? "null" : "[collection]")}");
            }
            else
            {
                var expectedList = expectedCollection.ToList();
                var actualList = actualCollection.ToList();

                sb.AppendLine($"{"Expected",-15} : {expectedName}");
                sb.AppendLine($"{"Actual",-15} : {actualName}");
                sb.AppendLine($"{"Expected Count",-15} : {expectedList.Count}");
                sb.AppendLine($"{"Actual Count",-15} : {actualList.Count}");
                sb.AppendLine();

                // Show elements comparison
                var maxDisplay = Math.Min(10, Math.Max(expectedList.Count, actualList.Count));

                sb.AppendLine("Element Comparison:");
                sb.AppendLine($"{"Index",-8} {"Expected",-30} {"Actual",-30} {"Match",-10}");
                sb.AppendLine(new string('-', 78));

                for (var i = 0; i < maxDisplay; i++)
                {
                    var expectedValue = i < expectedList.Count ? expectedList[i]?.ToString() ?? "null" : "<missing>";
                    var actualValue = i < actualList.Count ? actualList[i]?.ToString() ?? "null" : "<missing>";

                    var match = i < expectedList.Count && i < actualList.Count &&
                                EqualityComparer<T>.Default.Equals(expectedList[i], actualList[i]);

                    // Truncate long values
                    if (expectedValue.Length > 25)
                    {
                        expectedValue = expectedValue[..25] + "...";
                    }

                    if (actualValue.Length > 25)
                    {
                        actualValue = actualValue[..25] + "...";
                    }

                    var matchSymbol = match ? "✓" : "✗";
                    sb.AppendLine($"{i,-8} {expectedValue,-30} {actualValue,-30} {matchSymbol,-10}");
                }

                if (Math.Max(expectedList.Count, actualList.Count) > maxDisplay)
                {
                    sb.AppendLine($"... ({Math.Max(expectedList.Count, actualList.Count) - maxDisplay} more elements)");
                }

                sb.AppendLine();

                // Find missing/extra elements for equivalence check
                if (!checkOrder)
                {
                    var missingInActual = expectedList.Except(actualList).ToList();
                    var extraInActual = actualList.Except(expectedList).ToList();

                    if (missingInActual.Any())
                    {
                        sb.AppendLine($"Missing in Actual ({missingInActual.Count}):");

                        foreach (var item in missingInActual.Take(5))
                        {
                            sb.AppendLine($"  - {item}");
                        }

                        if (missingInActual.Count > 5)
                        {
                            sb.AppendLine($"  ... and {missingInActual.Count - 5} more");
                        }

                        sb.AppendLine();
                    }

                    if (extraInActual.Any())
                    {
                        sb.AppendLine($"Extra in Actual ({extraInActual.Count}):");

                        foreach (var item in extraInActual.Take(5))
                        {
                            sb.AppendLine($"  - {item}");
                        }

                        if (extraInActual.Count > 5)
                        {
                            sb.AppendLine($"  ... and {extraInActual.Count - 5} more");
                        }

                        sb.AppendLine();
                    }
                }
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = checkOrder
                                        ? new[]
                                          {
                                              $"Verify that '{actualName}' is populated with the correct elements in the correct order", $"Check if the ordering logic for '{actualName}' matches the expected sequence", $"Consider using AreEquivalent() if order doesn't matter",
                                              "Review the data source or transformation that produces the actual collection"
                                          }
                                        : new[]
                                          {
                                              $"Verify that '{actualName}' contains all expected elements (order is ignored)", $"Check if '{actualName}' has extra or missing elements compared to '{expectedName}'", "Review the data source or filter logic that produces the actual collection",
                                              "Ensure no duplicate handling issues affect the comparison"
                                          };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}