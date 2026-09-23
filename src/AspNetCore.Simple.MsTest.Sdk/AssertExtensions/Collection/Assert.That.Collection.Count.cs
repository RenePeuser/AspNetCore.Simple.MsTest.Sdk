using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// COLLECTION COUNT assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the collection has the expected count.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="expectedCount">The expected number of elements</param>
        /// <param name="because">Why this count is expected (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when collection count does not match expected</exception>
        public static void HasCount<T>(this Assert _,
                                       int expectedCount,
                                       IEnumerable<T> collection,
                                       string because,
                                       string fix,
                                       [CallerArgumentExpression(nameof(collection))]
                                       string collectionName = "",
                                       [CallerFilePath] string callerFilePath = "",
                                       [CallerMemberName] string callerMemberName = "",
                                       [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null. Use IsNullOrEmpty if you want to allow null.");
            }

            var actualCount = collection.Count();

            if (actualCount == expectedCount)
            {
                return;
            }

            var output = BuildCountAssertionOutput(collectionName: collectionName,
                                                   collection: collection,
                                                   actualCount: actualCount,
                                                   expectedCount: expectedCount,
                                                   minCount: null,
                                                   maxCount: null,
                                                   assertionType: "HasCount",
                                                   because: because,
                                                   fix: fix,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber,
                                                   callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the collection count is within the specified range (inclusive).
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="minCount">The minimum number of elements (inclusive)</param>
        /// <param name="maxCount">The maximum number of elements (inclusive)</param>
        /// <param name="because">Why this range is expected (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when collection count is outside the range</exception>
        public static void HasCountInRange<T>(this Assert _,
                                              IEnumerable<T> collection,
                                              int minCount,
                                              int maxCount,
                                              string because,
                                              string fix,
                                              [CallerArgumentExpression(nameof(collection))]
                                              string collectionName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null. Use IsNullOrEmpty if you want to allow null.");
            }

            if (minCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minCount), "Minimum count cannot be negative.");
            }

            if (maxCount < minCount)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount), "Maximum count cannot be less than minimum count.");
            }

            var actualCount = collection.Count();

            if (actualCount >= minCount && actualCount <= maxCount)
            {
                return;
            }

            var output = BuildCountAssertionOutput(collectionName: collectionName,
                                                   collection: collection,
                                                   actualCount: actualCount,
                                                   expectedCount: null,
                                                   minCount: minCount,
                                                   maxCount: maxCount,
                                                   assertionType: "HasCountInRange",
                                                   because: because,
                                                   fix: fix,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber,
                                                   callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the collection has more elements than the specified minimum count.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="minCount">The minimum number of elements (exclusive)</param>
        /// <param name="because">Why this minimum is expected (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when collection count is not greater than minimum</exception>
        public static void HasCountGreaterThan<T>(this Assert _,
                                                  IEnumerable<T> collection,
                                                  int minCount,
                                                  string because,
                                                  string fix,
                                                  [CallerArgumentExpression(nameof(collection))]
                                                  string collectionName = "",
                                                  [CallerFilePath] string callerFilePath = "",
                                                  [CallerMemberName] string callerMemberName = "",
                                                  [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null. Use IsNullOrEmpty if you want to allow null.");
            }

            if (minCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minCount), "Minimum count cannot be negative.");
            }

            var actualCount = collection.Count();

            if (actualCount > minCount)
            {
                return;
            }

            var output = BuildCountAssertionOutput(collectionName: collectionName,
                                                   collection: collection,
                                                   actualCount: actualCount,
                                                   expectedCount: null,
                                                   minCount: minCount,
                                                   maxCount: null,
                                                   assertionType: "HasCountGreaterThan",
                                                   because: because,
                                                   fix: fix,
                                                   callerFilePath: callerFilePath,
                                                   callerMemberName: callerMemberName,
                                                   callerLineNumber: callerLineNumber,
                                                   callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        // Private helper for building collection count assertion output
        private static string BuildCountAssertionOutput<T>(string collectionName,
                                                           IEnumerable<T> collection,
                                                           int actualCount,
                                                           int? expectedCount,
                                                           int? minCount,
                                                           int? maxCount,
                                                           string assertionType,
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
            var title = assertionType switch
            {
                "HasCount" => "COLLECTION COUNT - MISMATCH",
                "HasCountInRange" => "COLLECTION COUNT - OUT OF RANGE",
                "HasCountGreaterThan" => "COLLECTION COUNT - NOT GREATER THAN",
                _ => "COLLECTION COUNT - ASSERTION FAILED"
            };

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = assertionType switch
            {
                "HasCount" => $"Expected collection to have exactly {expectedCount} element(s), but found {actualCount}.",
                "HasCountInRange" => $"Expected collection count to be between {minCount} and {maxCount} (inclusive), but found {actualCount}.",
                "HasCountGreaterThan" => $"Expected collection to have more than {minCount} element(s), but found {actualCount}.",
                _ => "Collection count assertion failed."
            };

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Collection",-15} : {collectionName}");
            sb.AppendLine($"{"Type",-15} : {typeof(T).Name}");
            sb.AppendLine($"{"Actual Count",-15} : {actualCount}");

            if (expectedCount.HasValue)
            {
                sb.AppendLine($"{"Expected Count",-15} : {expectedCount.Value}");
                sb.AppendLine($"{"Difference",-15} : {actualCount - expectedCount.Value:+#;-#;0}");
            }
            else if (minCount.HasValue && maxCount.HasValue)
            {
                sb.AppendLine($"{"Min Count",-15} : {minCount.Value}");
                sb.AppendLine($"{"Max Count",-15} : {maxCount.Value}");

                if (actualCount < minCount.Value)
                {
                    sb.AppendLine($"{"Below Min By",-15} : {minCount.Value - actualCount}");
                }
                else if (actualCount > maxCount.Value)
                {
                    sb.AppendLine($"{"Above Max By",-15} : {actualCount - maxCount.Value}");
                }
            }
            else if (minCount.HasValue)
            {
                sb.AppendLine($"{"Min Count",-15} : > {minCount.Value}");
                sb.AppendLine($"{"Short By",-15} : {minCount.Value + 1 - actualCount}");
            }

            // Show first few elements for context
            var preview = collection.Take(5).ToList();

            if (preview.Count > 0)
            {
                sb.AppendLine($"{"Preview",-15} : [{string.Join(", ", preview.Select(x => x?.ToString() ?? "null"))}]{(actualCount > 5 ? "..." : "")}");
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = assertionType switch
            {
                "HasCount" => new[]
                              {
                                  $"Verify the logic that populates '{collectionName}' adds exactly {expectedCount} item(s)", $"Check if items are being filtered incorrectly before this assertion", actualCount > expectedCount!.Value
                                                                                                                                                                                                          ? $"Remove {actualCount - expectedCount.Value} extra item(s) or adjust the expected count"
                                                                                                                                                                                                          : $"Add {expectedCount.Value - actualCount} missing item(s) or adjust the expected count"
                              },
                "HasCountInRange" => new[]
                                     {
                                         $"Verify the logic that populates '{collectionName}' produces between {minCount} and {maxCount} items", actualCount < minCount!.Value
                                                                                                                                                     ? $"Add at least {minCount.Value - actualCount} more item(s)"
                                                                                                                                                     : $"Remove at least {actualCount - maxCount!.Value} item(s)",
                                         "Review filtering or query logic that builds this collection"
                                     },
                "HasCountGreaterThan" => new[]
                                         {
                                             $"Verify the logic that populates '{collectionName}' produces more than {minCount} item(s)", $"Add at least {minCount!.Value + 1 - actualCount} more item(s)", "Check if the data source has sufficient records",
                                             "Review any filtering logic that might be reducing the collection size"
                                         },
                _ => new[] { $"Review the logic that populates '{collectionName}'", "Check the test data setup" }
            };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}