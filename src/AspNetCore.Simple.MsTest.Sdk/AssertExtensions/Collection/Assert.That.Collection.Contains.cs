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
    /// COLLECTION CONTAINS assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the collection contains the specified item.
        /// </summary>
        /// <typeparam name="T">The type of the collection elements</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="item">The item that should be in the collection</param>
        /// <param name="because">Why this item should be in the collection (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured collection variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when the collection does not contain the item</exception>
        public static void Contains<T>(this Assert _,
                                       IEnumerable<T> collection,
                                       T item,
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
                var nullOutput = BuildContainsNullCollectionOutput(methodName: "Contains",
                                                                   collectionName: collectionName,
                                                                   because: because,
                                                                   fix: fix,
                                                                   callerFilePath: callerFilePath,
                                                                   callerMemberName: callerMemberName,
                                                                   callerLineNumber: callerLineNumber,
                                                                   callingAssembly: callingAssembly);

                throw new AssertFailedException(nullOutput);
            }

            if (collection.Contains(item))
            {
                return;
            }

            var output = BuildContainsOutput(expectContains: true,
                                             collectionName: collectionName,
                                             collection: collection,
                                             item: item,
                                             because: because,
                                             fix: fix,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber,
                                             callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the collection does NOT contain the specified item.
        /// </summary>
        /// <typeparam name="T">The type of the collection elements</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="item">The item that should NOT be in the collection</param>
        /// <param name="because">Why this item should not be in the collection (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured collection variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when the collection contains the item</exception>
        public static void DoesNotContain<T>(this Assert _,
                                             IEnumerable<T> collection,
                                             T item,
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
                var nullOutput = BuildContainsNullCollectionOutput(methodName: "DoesNotContain",
                                                                   collectionName: collectionName,
                                                                   because: because,
                                                                   fix: fix,
                                                                   callerFilePath: callerFilePath,
                                                                   callerMemberName: callerMemberName,
                                                                   callerLineNumber: callerLineNumber,
                                                                   callingAssembly: callingAssembly);

                throw new AssertFailedException(nullOutput);
            }

            if (!collection.Contains(item))
            {
                return;
            }

            var output = BuildContainsOutput(expectContains: false,
                                             collectionName: collectionName,
                                             collection: collection,
                                             item: item,
                                             because: because,
                                             fix: fix,
                                             callerFilePath: callerFilePath,
                                             callerMemberName: callerMemberName,
                                             callerLineNumber: callerLineNumber,
                                             callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the collection contains all of the specified items.
        /// </summary>
        /// <typeparam name="T">The type of the collection elements</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="items">The items that should all be in the collection</param>
        /// <param name="because">Why all these items should be in the collection (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured collection variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when the collection does not contain all items</exception>
        public static void ContainsAll<T>(this Assert _,
                                          IEnumerable<T> collection,
                                          IEnumerable<T> items,
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
                var nullOutput = BuildContainsNullCollectionOutput(methodName: "ContainsAll",
                                                                   collectionName: collectionName,
                                                                   because: because,
                                                                   fix: fix,
                                                                   callerFilePath: callerFilePath,
                                                                   callerMemberName: callerMemberName,
                                                                   callerLineNumber: callerLineNumber,
                                                                   callingAssembly: callingAssembly);

                throw new AssertFailedException(nullOutput);
            }

            if (items == null)
            {
                var nullItemsOutput = BuildContainsNullItemsOutput(methodName: "ContainsAll",
                                                                   collectionName: collectionName,
                                                                   because: because,
                                                                   fix: fix,
                                                                   callerFilePath: callerFilePath,
                                                                   callerMemberName: callerMemberName,
                                                                   callerLineNumber: callerLineNumber,
                                                                   callingAssembly: callingAssembly);

                throw new AssertFailedException(nullItemsOutput);
            }

            var materializedCollection = collection.ToList();
            var materializedItems = items.ToList();
            var missingItems = materializedItems.Where(item => !materializedCollection.Contains(item)).ToList();

            if (missingItems.Count == 0)
            {
                return;
            }

            var output = BuildContainsAllOutput(collectionName: collectionName,
                                                collection: materializedCollection,
                                                expectedItems: materializedItems,
                                                missingItems: missingItems,
                                                because: because,
                                                fix: fix,
                                                callerFilePath: callerFilePath,
                                                callerMemberName: callerMemberName,
                                                callerLineNumber: callerLineNumber,
                                                callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the collection contains at least one of the specified items.
        /// </summary>
        /// <typeparam name="T">The type of the collection elements</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="collection">The collection to check</param>
        /// <param name="items">The items, at least one of which should be in the collection</param>
        /// <param name="because">Why at least one of these items should be in the collection (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="collectionName">Auto-captured collection variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when the collection contains none of the items</exception>
        public static void ContainsAny<T>(this Assert _,
                                          IEnumerable<T> collection,
                                          IEnumerable<T> items,
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
                var nullOutput = BuildContainsNullCollectionOutput(methodName: "ContainsAny",
                                                                   collectionName: collectionName,
                                                                   because: because,
                                                                   fix: fix,
                                                                   callerFilePath: callerFilePath,
                                                                   callerMemberName: callerMemberName,
                                                                   callerLineNumber: callerLineNumber,
                                                                   callingAssembly: callingAssembly);

                throw new AssertFailedException(nullOutput);
            }

            if (items == null)
            {
                var nullItemsOutput = BuildContainsNullItemsOutput(methodName: "ContainsAny",
                                                                   collectionName: collectionName,
                                                                   because: because,
                                                                   fix: fix,
                                                                   callerFilePath: callerFilePath,
                                                                   callerMemberName: callerMemberName,
                                                                   callerLineNumber: callerLineNumber,
                                                                   callingAssembly: callingAssembly);

                throw new AssertFailedException(nullItemsOutput);
            }

            var materializedCollection = collection.ToList();
            var materializedItems = items.ToList();

            if (materializedItems.Any(item => materializedCollection.Contains(item)))
            {
                return;
            }

            var output = BuildContainsAnyOutput(collectionName: collectionName,
                                                collection: materializedCollection,
                                                expectedItems: materializedItems,
                                                because: because,
                                                fix: fix,
                                                callerFilePath: callerFilePath,
                                                callerMemberName: callerMemberName,
                                                callerLineNumber: callerLineNumber,
                                                callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        // Private helper for building Contains/DoesNotContain output
        private static string BuildContainsOutput<T>(bool expectContains,
                                                     string collectionName,
                                                     IEnumerable<T> collection,
                                                     T item,
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
            var title = expectContains
                            ? "COLLECTION CONTAINS - ITEM NOT FOUND"
                            : "COLLECTION CONTAINS - UNEXPECTED ITEM FOUND";

            assertOutputHelper.BuildHeader(sb, title);

            // Test Information
            assertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber);

            // Problem
            var problem = expectContains
                              ? "Expected collection to contain the item but it was not found."
                              : "Expected collection to NOT contain the item but it was found.";

            assertOutputHelper.BuildProblemSection(sb, problem);

            // Details
            assertOutputHelper.BuildDetailsSectionHeader(sb);
            sb.AppendLine($"{"Collection",-15} : {collectionName}");
            sb.AppendLine($"{"Item Type",-15} : {typeof(T).Name}");
            sb.AppendLine($"{"Searched For",-15} : {FormatItem(item)}");
#pragma warning disable CA1851 // Possible multiple enumerations
            sb.AppendLine($"{"Collection Size",-15} : {collection.Count()}");

            var collectionItems = collection.Take(10).ToList();
            sb.AppendLine($"{"Items",-15} : [{string.Join(", ", collectionItems.Select(FormatItem))}]");

            if (collection.Count() > 10)
            {
                sb.AppendLine($"{"   ",-15}   ... ({collection.Count() - 10} more items)");
#pragma warning restore CA1851
            }

            sb.AppendLine();

            // Context (Why)
            assertOutputHelper.BuildContextSection(sb, because);

            // Fix (How)
            var additionalOptions = expectContains
                                        ? new[]
                                          {
                                              $"Verify that the item '{FormatItem(item)}' is being added to '{collectionName}'", $"Check that the collection is populated before this assertion", $"Ensure equality comparison is correctly implemented for type {typeof(T).Name}",
                                              "Review the logic that builds or filters the collection"
                                          }
                                        : new[] { $"Check why '{FormatItem(item)}' is present in '{collectionName}'", $"Review the filtering logic that should exclude this item", $"Verify that items are being removed correctly from '{collectionName}'" };

            assertOutputHelper.BuildFixSection(sb, fix,
                                               additionalOptions);

            // Footer
            assertOutputHelper.BuildFooter(sb);

            return sb.ToString();
        }
#pragma warning disable CA1859 // Use concrete types when possible

        // Private helper for building ContainsAll output
#pragma warning disable CA1859 // Use concrete types when possible - IList is more flexible than List for parameter types
        private static string BuildContainsAllOutput<T>(string collectionName,
                                                        IList<T> collection,
                                                        IList<T> expectedItems,
                                                        IList<T> missingItems,
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
            assertOutputHelper.BuildHeader(sb, "COLLECTION CONTAINS ALL - MISSING ITEMS");

            // Test Information
            assertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber);

            // Problem
            var problem = $"Expected collection to contain all {expectedItems.Count} items but {missingItems.Count} were missing.";
            assertOutputHelper.BuildProblemSection(sb, problem);

            // Details
            assertOutputHelper.BuildDetailsSectionHeader(sb);
            sb.AppendLine($"{"Collection",-15} : {collectionName}");
            sb.AppendLine($"{"Item Type",-15} : {typeof(T).Name}");
            sb.AppendLine($"{"Collection Size",-15} : {collection.Count}");
            sb.AppendLine($"{"Expected Count",-15} : {expectedItems.Count}");
            sb.AppendLine($"{"Missing Count",-15} : {missingItems.Count}");
            sb.AppendLine();
            sb.AppendLine($"{"Missing Items",-15} :");

            foreach (var item in missingItems.Take(10))
            {
                sb.AppendLine($"{"   ",-15}   - {FormatItem(item)}");
            }

            if (missingItems.Count > 10)
            {
                sb.AppendLine($"{"   ",-15}   ... ({missingItems.Count - 10} more)");
            }

            sb.AppendLine();

            // Context (Why)
            assertOutputHelper.BuildContextSection(sb, because);

            // Fix (How)
            var additionalOptions = new[]
                                    {
                                        $"Verify that all {expectedItems.Count} expected items are being added to '{collectionName}'", $"Check that the collection is fully populated before this assertion", $"Ensure equality comparison is correctly implemented for type {typeof(T).Name}",
                                        "Review the logic that builds or filters the collection", $"Check if items are being removed unexpectedly from '{collectionName}'"
                                    };

            assertOutputHelper.BuildFixSection(sb, fix,
                                               additionalOptions);

            // Footer
            assertOutputHelper.BuildFooter(sb);

            return sb.ToString();
        }
#pragma warning restore CA1859

        // Private helper for building ContainsAny output
#pragma warning disable CA1859 // Use concrete types when possible - IList is more flexible than List for parameter types
        private static string BuildContainsAnyOutput<T>(string collectionName,
                                                        IList<T> collection,
                                                        IList<T> expectedItems,
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
            assertOutputHelper.BuildHeader(sb, "COLLECTION CONTAINS ANY - NO MATCHES");

            // Test Information
            assertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber);

            // Problem
            var problem = $"Expected collection to contain at least one of {expectedItems.Count} items but none were found.";
            assertOutputHelper.BuildProblemSection(sb, problem);

            // Details
            assertOutputHelper.BuildDetailsSectionHeader(sb);
            sb.AppendLine($"{"Collection",-15} : {collectionName}");
            sb.AppendLine($"{"Item Type",-15} : {typeof(T).Name}");
            sb.AppendLine($"{"Collection Size",-15} : {collection.Count}");
            sb.AppendLine($"{"Expected Any Of",-15} : {expectedItems.Count} items");
            sb.AppendLine();
            sb.AppendLine($"{"Expected Items",-15} :");

            foreach (var item in expectedItems.Take(10))
            {
                sb.AppendLine($"{"   ",-15}   - {FormatItem(item)}");
            }

            if (expectedItems.Count > 10)
            {
                sb.AppendLine($"{"   ",-15}   ... ({expectedItems.Count - 10} more)");
            }

            sb.AppendLine();

            var collectionItems = collection.Take(5).ToList();
            sb.AppendLine($"{"Actual Items",-15} : [{string.Join(", ", collectionItems.Select(FormatItem))}]");

            if (collection.Count > 5)
            {
                sb.AppendLine($"{"   ",-15}   ... ({collection.Count - 5} more)");
            }

            sb.AppendLine();

            // Context (Why)
            assertOutputHelper.BuildContextSection(sb, because);

            // Fix (How)
            var additionalOptions = new[]
                                    {
                                        $"Verify that at least one expected item is being added to '{collectionName}'", $"Check that the collection is populated before this assertion", $"Ensure equality comparison is correctly implemented for type {typeof(T).Name}",
                                        "Review the logic that builds or filters the collection", "Verify that the expected items list is correct"
                                    };

            assertOutputHelper.BuildFixSection(sb, fix,
                                               additionalOptions);

            // Footer
            assertOutputHelper.BuildFooter(sb);

            return sb.ToString();
        }
#pragma warning restore CA1859

        // Private helper for building null collection output
        private static string BuildContainsNullCollectionOutput(string methodName,
                                                                string collectionName,
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
            assertOutputHelper.BuildHeader(sb, $"COLLECTION {methodName.ToUpperInvariant()} - NULL COLLECTION");

            // Test Information
            assertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber);

            // Problem
            assertOutputHelper.BuildProblemSection(sb, "Cannot check collection contents because the collection is null.");

            // Details
            assertOutputHelper.BuildDetailsSectionHeader(sb);
            sb.AppendLine($"{"Collection",-15} : {collectionName}");
            sb.AppendLine($"{"Value",-15} : null");
            sb.AppendLine();

            // Context (Why)
            assertOutputHelper.BuildContextSection(sb, because);

            // Fix (How)
            var additionalOptions = new[]
                                    {
                                        $"Verify that '{collectionName}' is properly initialized before this assertion", $"Check for null returns in methods that populate '{collectionName}'", "Add null checks or default values in the code under test",
                                        $"Consider using Assert.That.IsNotNull('{collectionName}') before this assertion"
                                    };

            assertOutputHelper.BuildFixSection(sb, fix,
                                               additionalOptions);

            // Footer
            assertOutputHelper.BuildFooter(sb);

            return sb.ToString();
        }

        // Private helper for building null items output
        private static string BuildContainsNullItemsOutput(string methodName,
                                                           string collectionName,
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
            assertOutputHelper.BuildHeader(sb, $"COLLECTION {methodName.ToUpperInvariant()} - NULL ITEMS");

            // Test Information
            assertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber);

            // Problem
            assertOutputHelper.BuildProblemSection(sb, "Cannot check collection contents because the items parameter is null.");

            // Details
            assertOutputHelper.BuildDetailsSectionHeader(sb);
            sb.AppendLine($"{"Collection",-15} : {collectionName}");
            sb.AppendLine($"{"Items Param",-15} : null");
            sb.AppendLine();

            // Context (Why)
            assertOutputHelper.BuildContextSection(sb, because);

            // Fix (How)
            var additionalOptions = new[] { "Verify that the items parameter is properly initialized before this assertion", "Check for null returns in methods that provide the expected items", "Add null checks or default values in the test code" };

            assertOutputHelper.BuildFixSection(sb, fix,
                                               additionalOptions);

            // Footer
            assertOutputHelper.BuildFooter(sb);

            return sb.ToString();
        }

        // Helper to format items for display
        private static string FormatItem<T>(T item)
        {
            if (item == null)
            {
                return "null";
            }

            if (item is string str)
            {
                return $"\"{str}\"";
            }

            return item.ToString() ?? "(no ToString)";
        }
    }
}