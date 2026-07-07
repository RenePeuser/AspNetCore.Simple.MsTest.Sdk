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
    /// COLLECTION PREDICATE assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that all items in the collection satisfy the predicate.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="predicate">The predicate that all items must satisfy</param>
        /// <param name="predicateDescription">Human-readable description of the predicate</param>
        /// <param name="because">Why all items must satisfy this predicate (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when not all items satisfy the predicate</exception>
        public static void All<T>(this Assert _,
                                  IEnumerable<T> collection,
                                  Func<T, bool> predicate,
                                  string predicateDescription,
                                  string because,
                                  string fix,
                                  [CallerArgumentExpression(nameof(collection))]
                                  string collectionName = "",
                                  [CallerFilePath] string callerFilePath = "",
                                  [CallerMemberName] string callerMemberName = "",
                                  [CallerLineNumber] int callerLineNumber = 0)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null");
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");
            }

            var items = collection.ToList();
            var failingItems = items.Where(item => !predicate(item)).ToList();

            if (failingItems.Count == 0)
            {
                return;
            }

            var output = BuildCollectionPredicateOutput(predicateType: "All",
                                                        collectionName: collectionName,
                                                        predicateDescription: predicateDescription,
                                                        totalCount: items.Count,
                                                        matchingCount: items.Count - failingItems.Count,
                                                        failingItems: failingItems,
                                                        because: because,
                                                        fix: fix,
                                                        callerFilePath: callerFilePath,
                                                        callerMemberName: callerMemberName,
                                                        callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that at least one item in the collection satisfies the predicate.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="predicate">The predicate that at least one item must satisfy</param>
        /// <param name="predicateDescription">Human-readable description of the predicate</param>
        /// <param name="because">Why at least one item must satisfy this predicate (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when no items satisfy the predicate</exception>
        public static void Any<T>(this Assert _,
                                  IEnumerable<T> collection,
                                  Func<T, bool> predicate,
                                  string predicateDescription,
                                  string because,
                                  string fix,
                                  [CallerArgumentExpression(nameof(collection))]
                                  string collectionName = "",
                                  [CallerFilePath] string callerFilePath = "",
                                  [CallerMemberName] string callerMemberName = "",
                                  [CallerLineNumber] int callerLineNumber = 0)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null");
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");
            }

            var items = collection.ToList();
            var hasMatch = items.Any(predicate);

            if (hasMatch)
            {
                return;
            }

            var output = BuildCollectionPredicateOutput(predicateType: "Any",
                                                        collectionName: collectionName,
                                                        predicateDescription: predicateDescription,
                                                        totalCount: items.Count,
                                                        matchingCount: 0,
                                                        failingItems: items,
                                                        because: because,
                                                        fix: fix,
                                                        callerFilePath: callerFilePath,
                                                        callerMemberName: callerMemberName,
                                                        callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that no items in the collection satisfy the predicate.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="predicate">The predicate that no items must satisfy</param>
        /// <param name="predicateDescription">Human-readable description of the predicate</param>
        /// <param name="because">Why no items must satisfy this predicate (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when any items satisfy the predicate</exception>
        public static void None<T>(this Assert _,
                                   IEnumerable<T> collection,
                                   Func<T, bool> predicate,
                                   string predicateDescription,
                                   string because,
                                   string fix,
                                   [CallerArgumentExpression(nameof(collection))]
                                   string collectionName = "",
                                   [CallerFilePath] string callerFilePath = "",
                                   [CallerMemberName] string callerMemberName = "",
                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null");
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");
            }

            var items = collection.ToList();
            var matchingItems = items.Where(predicate).ToList();

            if (matchingItems.Count == 0)
            {
                return;
            }

            var output = BuildCollectionPredicateOutput(predicateType: "None",
                                                        collectionName: collectionName,
                                                        predicateDescription: predicateDescription,
                                                        totalCount: items.Count,
                                                        matchingCount: matchingItems.Count,
                                                        failingItems: matchingItems,
                                                        because: because,
                                                        fix: fix,
                                                        callerFilePath: callerFilePath,
                                                        callerMemberName: callerMemberName,
                                                        callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that exactly one item in the collection satisfies the predicate.
        /// </summary>
        /// <typeparam name="T">The type of items in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="predicate">The predicate that exactly one item must satisfy</param>
        /// <param name="predicateDescription">Human-readable description of the predicate</param>
        /// <param name="because">Why exactly one item must satisfy this predicate (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when zero or multiple items satisfy the predicate</exception>
#pragma warning disable CA1720 // Identifier contains type name - Single is the method name
        public static void Single<T>(this Assert _,
                                     IEnumerable<T> collection,
                                     Func<T, bool> predicate,
                                     string predicateDescription,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(collection))]
                                     string collectionName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            if (collection is null)
            {
                throw new ArgumentNullException(nameof(collection), "Collection cannot be null");
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");
            }

            var items = collection.ToList();
            var matchingItems = items.Where(predicate).ToList();

            if (matchingItems.Count == 1)
            {
                return;
            }

            var output = BuildCollectionPredicateOutput(predicateType: "Single",
                                                        collectionName: collectionName,
                                                        predicateDescription: predicateDescription,
                                                        totalCount: items.Count,
                                                        matchingCount: matchingItems.Count,
                                                        failingItems: matchingItems,
                                                        because: because,
                                                        fix: fix,
                                                        callerFilePath: callerFilePath,
                                                        callerMemberName: callerMemberName,
                                                        callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }
#pragma warning restore CA1720

        // Private helper for building collection predicate assertion output
        private static string BuildCollectionPredicateOutput<T>(string predicateType,
                                                                string collectionName,
                                                                string predicateDescription,
                                                                int totalCount,
                                                                int matchingCount,
                                                                List<T> failingItems,
                                                                string because,
                                                                string fix,
                                                                string callerFilePath,
                                                                string callerMemberName,
                                                                int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = predicateType switch
            {
                "All" => "COLLECTION PREDICATE FAILED - NOT ALL ITEMS MATCH",
                "Any" => "COLLECTION PREDICATE FAILED - NO ITEMS MATCH",
                "None" => "COLLECTION PREDICATE FAILED - SOME ITEMS MATCH",
                "Single" => "COLLECTION PREDICATE FAILED - EXPECTED EXACTLY ONE MATCH",
                _ => "COLLECTION PREDICATE FAILED"
            };

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = predicateType switch
            {
                "All" => $"Expected all items to satisfy the predicate, but {failingItems.Count} item(s) did not.",
                "Any" => "Expected at least one item to satisfy the predicate, but no items matched.",
                "None" => $"Expected no items to satisfy the predicate, but {matchingCount} item(s) did.",
                "Single" => matchingCount == 0
                                ? "Expected exactly one item to satisfy the predicate, but no items matched."
                                : $"Expected exactly one item to satisfy the predicate, but {matchingCount} items matched.",
                _ => "Collection predicate assertion failed."
            };

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Collection",-15} : {collectionName}");
            sb.AppendLine($"{"Predicate",-15} : {predicateDescription}");
            sb.AppendLine($"{"Total Items",-15} : {totalCount}");
            sb.AppendLine($"{"Matching Items",-15} : {matchingCount}");
            sb.AppendLine($"{"Expected",-15} : {GetExpectedDescription(predicateType)}");
            sb.AppendLine();

            // Show sample items (limit to first 5)
            if (failingItems.Count > 0)
            {
                var itemLabel = predicateType switch
                {
                    "All" => "Items that failed predicate",
                    "None" => "Items that unexpectedly matched",
                    _ => "Items found"
                };

                sb.AppendLine($"{itemLabel}:");
                var itemsToShow = failingItems.Take(5).ToList();

                for (var i = 0; i < itemsToShow.Count; i++)
                {
                    sb.AppendLine($"  [{i}] {itemsToShow[i]?.ToString() ?? "(null)"}");
                }

                if (failingItems.Count > 5)
                {
                    sb.AppendLine($"  ... and {failingItems.Count - 5} more item(s)");
                }

                sb.AppendLine();
            }

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = predicateType switch
            {
                "All" => new[] { $"Review the items in '{collectionName}' that failed the predicate", $"Verify that the data source for '{collectionName}' produces items matching '{predicateDescription}'", "Check if the predicate logic is correct and matches your requirements" },
                "Any" => new[] { $"Ensure '{collectionName}' is populated with items matching '{predicateDescription}'", $"Verify the data source for '{collectionName}' is correct", "Check if the collection is empty or if the predicate logic needs adjustment" },
                "None" => new[] { $"Review the items in '{collectionName}' that unexpectedly match '{predicateDescription}'", $"Verify that '{collectionName}' is filtered correctly before this assertion", "Check if the predicate logic correctly identifies items to exclude" },
                "Single" => matchingCount == 0
                                ? new[] { $"Ensure '{collectionName}' contains at least one item matching '{predicateDescription}'", $"Verify the data source for '{collectionName}' includes the expected item", "Check if the predicate is too restrictive" }
                                : new[] { $"Ensure '{collectionName}' contains exactly one item matching '{predicateDescription}'", $"Remove duplicate items from '{collectionName}' or refine the predicate", "Check if the predicate is too broad and matches multiple items" },
                _ => new[] { $"Review the items in '{collectionName}'", "Verify the predicate logic is correct" }
            };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        private static string GetExpectedDescription(string predicateType)
        {
            return predicateType switch
            {
                "All" => "All items match",
                "Any" => "At least one item matches",
                "None" => "No items match",
                "Single" => "Exactly one item matches",
                _ => "Unknown"
            };
        }
    }
}