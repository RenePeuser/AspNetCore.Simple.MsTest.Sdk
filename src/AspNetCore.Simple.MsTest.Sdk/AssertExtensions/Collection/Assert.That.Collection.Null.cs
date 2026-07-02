using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// COLLECTION NULL/EMPTY CHECK assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the collection is empty (has no elements).
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="because">Why this collection should be empty (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when collection is not empty</exception>
        public static void IsEmpty<T>(this Assert _,
                                      IEnumerable<T> collection,
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
                var output = BuildCollectionNullOutput(collectionName: collectionName,
                                                       because: because,
                                                       fix: fix,
                                                       callerFilePath: callerFilePath,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);

                throw new AssertFailedException(output);
            }

            var materializedCollection = collection as ICollection<T> ?? collection.ToList();

            if (materializedCollection.Count == 0)
            {
                return;
            }

            var output2 = BuildCollectionEmptyOutput(expectEmpty: true,
                                                     collectionName: collectionName,
                                                     actualCount: materializedCollection.Count,
                                                     collection: materializedCollection,
                                                     because: because,
                                                     fix: fix,
                                                     callerFilePath: callerFilePath,
                                                     callerMemberName: callerMemberName,
                                                     callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output2);
        }

        /// <summary>
        /// Asserts that the collection is NOT empty (has at least one element).
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="because">Why this collection must not be empty (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when collection is empty</exception>
        public static void IsNotEmpty<T>(this Assert _,
                                         IEnumerable<T> collection,
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
                var output = BuildCollectionNullOutput(collectionName: collectionName,
                                                       because: because,
                                                       fix: fix,
                                                       callerFilePath: callerFilePath,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber);

                throw new AssertFailedException(output);
            }

            var materializedCollection = collection as ICollection<T> ?? collection.ToList();

            if (materializedCollection.Count > 0)
            {
                return;
            }

            var output2 = BuildCollectionEmptyOutput(expectEmpty: false,
                                                     collectionName: collectionName,
                                                     actualCount: 0,
                                                     collection: materializedCollection,
                                                     because: because,
                                                     fix: fix,
                                                     callerFilePath: callerFilePath,
                                                     callerMemberName: callerMemberName,
                                                     callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output2);
        }

        /// <summary>
        /// Asserts that the collection is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="because">Why this collection should be null or empty (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when collection is neither null nor empty</exception>
        public static void IsNullOrEmpty<T>(this Assert _,
                                            IEnumerable<T>? collection,
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
                return;
            }

            var materializedCollection = collection as ICollection<T> ?? collection.ToList();

            if (materializedCollection.Count == 0)
            {
                return;
            }

            var output = BuildCollectionNullOrEmptyOutput(expectNullOrEmpty: true,
                                                          collectionName: collectionName,
                                                          actualCount: materializedCollection.Count,
                                                          collection: materializedCollection,
                                                          because: because,
                                                          fix: fix,
                                                          callerFilePath: callerFilePath,
                                                          callerMemberName: callerMemberName,
                                                          callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private helper for building collection null output
        private static string BuildCollectionNullOutput(string collectionName,
                                                        string because,
                                                        string fix,
                                                        string callerFilePath,
                                                        string callerMemberName,
                                                        int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = "COLLECTION NULL - EXPECTED NON-NULL COLLECTION";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = "Expected collection to be non-null but received null.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {collectionName}");
            sb.AppendLine($"{"Value",-10} : null");
            sb.AppendLine($"{"Expected",-10} : Non-null collection");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = new[]
            {
                $"Verify that '{collectionName}' is properly initialized before this assertion",
                $"Check for null returns in methods that populate '{collectionName}'",
                "Add null checks or default empty collection in the code under test"
            };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Private helper for building collection empty/not empty output
        private static string BuildCollectionEmptyOutput<T>(bool expectEmpty,
                                                            string collectionName,
                                                            int actualCount,
                                                            IEnumerable<T> collection,
                                                            string because,
                                                            string fix,
                                                            string callerFilePath,
                                                            string callerMemberName,
                                                            int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = expectEmpty
                            ? "COLLECTION NOT EMPTY - EXPECTED EMPTY COLLECTION"
                            : "COLLECTION EMPTY - EXPECTED NON-EMPTY COLLECTION";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = expectEmpty
                              ? "Expected collection to be empty but it contains elements."
                              : "Expected collection to contain elements but it is empty.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {collectionName}");
            sb.AppendLine($"{"Type",-10} : {collection.GetType().Name}");
            sb.AppendLine($"{"Count",-10} : {actualCount}");
            sb.AppendLine($"{"Expected",-10} : {(expectEmpty ? "0 elements" : "At least 1 element")}");

            if (actualCount is > 0 and <= 5)
            {
                sb.AppendLine($"{"Items",-10} : [{string.Join(", ", collection.Take(5).Select(x => x?.ToString() ?? "null"))}]");
            }
            else if (actualCount > 5)
            {
                sb.AppendLine($"{"Items",-10} : [{string.Join(", ", collection.Take(5).Select(x => x?.ToString() ?? "null"))}, ...]");
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = expectEmpty
                                        ? new[]
                                        {
                                            $"Ensure the code that populates '{collectionName}' does not add elements for this scenario",
                                            $"Review the filter or query logic that produces '{collectionName}'",
                                            $"Check if elements should be removed before this assertion"
                                        }
                                        : new[]
                                        {
                                            $"Verify that the data source for '{collectionName}' contains elements",
                                            $"Check the filter or query logic that produces '{collectionName}'",
                                            $"Ensure the code that populates '{collectionName}' is executed before this assertion"
                                        };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }

        // Private helper for building collection null or empty output
        private static string BuildCollectionNullOrEmptyOutput<T>(bool expectNullOrEmpty,
                                                                  string collectionName,
                                                                  int actualCount,
                                                                  IEnumerable<T> collection,
                                                                  string because,
                                                                  string fix,
                                                                  string callerFilePath,
                                                                  string callerMemberName,
                                                                  int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = "COLLECTION HAS ELEMENTS - EXPECTED NULL OR EMPTY";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = "Expected collection to be null or empty but it contains elements.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {collectionName}");
            sb.AppendLine($"{"Type",-10} : {collection.GetType().Name}");
            sb.AppendLine($"{"Count",-10} : {actualCount}");
            sb.AppendLine($"{"Expected",-10} : null or 0 elements");

            if (actualCount is > 0 and <= 5)
            {
                sb.AppendLine($"{"Items",-10} : [{string.Join(", ", collection.Take(5).Select(x => x?.ToString() ?? "null"))}]");
            }
            else if (actualCount > 5)
            {
                sb.AppendLine($"{"Items",-10} : [{string.Join(", ", collection.Take(5).Select(x => x?.ToString() ?? "null"))}, ...]");
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = new[]
            {
                $"Ensure the code that populates '{collectionName}' returns null or an empty collection for this scenario",
                $"Review the filter or query logic that produces '{collectionName}'",
                $"Check if '{collectionName}' should be cleared before this assertion"
            };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}