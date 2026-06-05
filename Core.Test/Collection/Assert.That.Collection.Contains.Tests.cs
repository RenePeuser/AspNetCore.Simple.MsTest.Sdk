using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Collection
{
    /// <summary>
    /// Tests for Assert.That.Contains, DoesNotContain, ContainsAll, and ContainsAny
    /// </summary>
    [TestClass]
    [TestCategory("Collection")]
    public sealed class AssertThatCollectionContainsTests
    {
        // ============================================================
        // Contains Tests
        // ============================================================

        [TestMethod]
        public void Contains_WhenItemExists_ShouldPass()
        {
            // Arrange
            var collection = new List<string> { "apple", "banana", "cherry" };

            // Act & Assert - Should NOT throw
            Assert.That.Contains(collection, "banana",
                                 because: "Testing that existing item is found",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Contains_WhenItemDoesNotExist_ShouldFail()
        {
            // Arrange
            var collection = new List<string> { "apple", "banana", "cherry" };
            var threw = false;

            // Act
            try
            {
                Assert.That.Contains(collection, "orange",
                                     because: "Testing that missing item causes failure",
                                     fix: "This is expected to fail");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void Contains_WhenItemDoesNotExist_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var fruits = new List<string> { "apple", "banana", "cherry", "date", "elderberry" };

            // Act
            try
            {
                Assert.That.Contains(fruits, "mango",
                                     because: "User selected mango as their favorite fruit",
                                     fix: "Add mango to the fruit inventory or validate user selection");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION CONTAINS - ITEM NOT FOUND"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User selected mango as their favorite fruit"));
                Assert.IsTrue(ex.Message.Contains("Add mango to the fruit inventory or validate user selection"));
                Assert.IsTrue(ex.Message.Contains("fruits"));
                Assert.IsTrue(ex.Message.Contains("mango"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Contains_WithNullCollection_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            List<string>? collection = null;

            // Act
            try
            {
                Assert.That.Contains(collection!, "item",
                                     because: "Testing null collection handling",
                                     fix: "Initialize collection before checking contents");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION CONTAINS - NULL COLLECTION"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Testing null collection handling"));
                Assert.IsTrue(ex.Message.Contains("Initialize collection before checking contents"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Contains_WithComplexObject_ShouldPass()
        {
            // Arrange
            var users = new List<TestUser>
                        {
                            new() { Id = 1, Name = "Alice" },
                            new() { Id = 2, Name = "Bob" },
                            new() { Id = 3, Name = "Charlie" }
                        };
            var targetUser = users[1];

            // Act & Assert - Should NOT throw
            Assert.That.Contains(users, targetUser,
                                 because: "Bob should be in the user list",
                                 fix: "Verify user creation");
        }

        // ============================================================
        // DoesNotContain Tests
        // ============================================================

        [TestMethod]
        public void DoesNotContain_WhenItemDoesNotExist_ShouldPass()
        {
            // Arrange
            var collection = new List<string> { "apple", "banana", "cherry" };

            // Act & Assert - Should NOT throw
            Assert.That.DoesNotContain(collection, "orange",
                                       because: "Testing that missing item passes",
                                       fix: "N/A - this should pass");
        }

        [TestMethod]
        public void DoesNotContain_WhenItemExists_ShouldFail()
        {
            // Arrange
            var collection = new List<string> { "apple", "banana", "cherry" };
            var threw = false;

            // Act
            try
            {
                Assert.That.DoesNotContain(collection, "banana",
                                           because: "Testing that existing item causes failure",
                                           fix: "This is expected to fail");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void DoesNotContain_WhenItemExists_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var bannedUsers = new List<string> { "spammer1", "bot2", "abuser3" };

            // Act
            try
            {
                Assert.That.DoesNotContain(bannedUsers, "bot2",
                                           because: "This user should have been removed from the ban list after appeal",
                                           fix: "Review the unban workflow or check database update logic");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION CONTAINS - UNEXPECTED ITEM FOUND"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("This user should have been removed from the ban list after appeal"));
                Assert.IsTrue(ex.Message.Contains("Review the unban workflow or check database update logic"));
                Assert.IsTrue(ex.Message.Contains("bannedUsers"));
                Assert.IsTrue(ex.Message.Contains("bot2"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void DoesNotContain_WithNullCollection_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            List<int>? numbers = null;

            // Act
            try
            {
                Assert.That.DoesNotContain(numbers!, 42,
                                           because: "Testing null collection handling",
                                           fix: "Initialize collection before checking contents");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION DOESNOTCONTAIN - NULL COLLECTION"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Testing null collection handling"));
                Assert.IsTrue(ex.Message.Contains("Initialize collection before checking contents"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // ContainsAll Tests
        // ============================================================

        [TestMethod]
        public void ContainsAll_WhenAllItemsExist_ShouldPass()
        {
            // Arrange
            var collection = new List<string> { "apple", "banana", "cherry", "date", "elderberry" };
            var itemsToFind = new List<string> { "banana", "date", "apple" };

            // Act & Assert - Should NOT throw
            Assert.That.ContainsAll(collection, itemsToFind,
                                    because: "Testing that all expected items are found",
                                    fix: "N/A - this should pass");
        }

        [TestMethod]
        public void ContainsAll_WhenSomeItemsMissing_ShouldFail()
        {
            // Arrange
            var collection = new List<string> { "apple", "banana", "cherry" };
            var itemsToFind = new List<string> { "banana", "orange", "grape" };
            var threw = false;

            // Act
            try
            {
                Assert.That.ContainsAll(collection, itemsToFind,
                                        because: "Testing that missing items cause failure",
                                        fix: "This is expected to fail");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void ContainsAll_WhenSomeItemsMissing_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var installedFeatures = new List<string> { "feature-a", "feature-b", "feature-d" };
            var requiredFeatures = new List<string> { "feature-a", "feature-b", "feature-c", "feature-d" };

            // Act
            try
            {
                Assert.That.ContainsAll(installedFeatures, requiredFeatures,
                                        because: "Application requires all features to be installed for premium tier",
                                        fix: "Run installation script with --premium flag or check deployment configuration");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION CONTAINS ALL - MISSING ITEMS"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Application requires all features to be installed for premium tier"));
                Assert.IsTrue(ex.Message.Contains("Run installation script with --premium flag or check deployment configuration"));
                Assert.IsTrue(ex.Message.Contains("installedFeatures"));
                Assert.IsTrue(ex.Message.Contains("Missing Items"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ContainsAll_WithNullCollection_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            List<string>? collection = null;
            var items = new List<string> { "a", "b" };

            // Act
            try
            {
                Assert.That.ContainsAll(collection!, items,
                                        because: "Testing null collection handling",
                                        fix: "Initialize collection before checking contents");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION CONTAINSALL - NULL COLLECTION"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Testing null collection handling"));
                Assert.IsTrue(ex.Message.Contains("Initialize collection before checking contents"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ContainsAll_WithNullItems_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var collection = new List<string> { "a", "b", "c" };
            List<string>? items = null;

            // Act
            try
            {
                Assert.That.ContainsAll(collection, items!,
                                        because: "Testing null items parameter handling",
                                        fix: "Initialize items list before calling ContainsAll");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION CONTAINSALL - NULL ITEMS"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Testing null items parameter handling"));
                Assert.IsTrue(ex.Message.Contains("Initialize items list before calling ContainsAll"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ContainsAll_WithEmptyExpectedItems_ShouldPass()
        {
            // Arrange
            var collection = new List<int> { 1, 2, 3 };
            var emptyItems = new List<int>();

            // Act & Assert - Should NOT throw (empty set is subset of any set)
            Assert.That.ContainsAll(collection, emptyItems,
                                    because: "Empty items list should always pass",
                                    fix: "N/A - this should pass");
        }

        [TestMethod]
        public void ContainsAll_WithComplexObjects_ShouldPass()
        {
            // Arrange
            var users = new List<TestUser>
                        {
                            new() { Id = 1, Name = "Alice" },
                            new() { Id = 2, Name = "Bob" },
                            new() { Id = 3, Name = "Charlie" }
                        };
            var expectedUsers = new List<TestUser>
                                {
                                    users[0],
                                    users[2]
                                };

            // Act & Assert - Should NOT throw
            Assert.That.ContainsAll(users, expectedUsers,
                                    because: "Both Alice and Charlie should be in the list",
                                    fix: "Verify user creation");
        }

        // ============================================================
        // ContainsAny Tests
        // ============================================================

        [TestMethod]
        public void ContainsAny_WhenAtLeastOneItemExists_ShouldPass()
        {
            // Arrange
            var collection = new List<string> { "apple", "banana", "cherry" };
            var itemsToFind = new List<string> { "orange", "banana", "grape" };

            // Act & Assert - Should NOT throw
            Assert.That.ContainsAny(collection, itemsToFind,
                                    because: "Testing that at least one item is found",
                                    fix: "N/A - this should pass");
        }

        [TestMethod]
        public void ContainsAny_WhenNoItemsExist_ShouldFail()
        {
            // Arrange
            var collection = new List<string> { "apple", "banana", "cherry" };
            var itemsToFind = new List<string> { "orange", "grape", "mango" };
            var threw = false;

            // Act
            try
            {
                Assert.That.ContainsAny(collection, itemsToFind,
                                        because: "Testing that no matches cause failure",
                                        fix: "This is expected to fail");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void ContainsAny_WhenNoItemsExist_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var supportedFormats = new List<string> { "json", "xml", "csv" };
            var requestedFormats = new List<string> { "yaml", "toml", "ini" };

            // Act
            try
            {
                Assert.That.ContainsAny(supportedFormats, requestedFormats,
                                        because: "API should support at least one of the requested output formats",
                                        fix: "Add support for YAML/TOML/INI or update client documentation");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION CONTAINS ANY - NO MATCHES"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("API should support at least one of the requested output formats"));
                Assert.IsTrue(ex.Message.Contains("Add support for YAML/TOML/INI or update client documentation"));
                Assert.IsTrue(ex.Message.Contains("supportedFormats"));
                Assert.IsTrue(ex.Message.Contains("Expected Items"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ContainsAny_WithNullCollection_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            List<string>? collection = null;
            var items = new List<string> { "a", "b" };

            // Act
            try
            {
                Assert.That.ContainsAny(collection!, items,
                                        because: "Testing null collection handling",
                                        fix: "Initialize collection before checking contents");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION CONTAINSANY - NULL COLLECTION"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Testing null collection handling"));
                Assert.IsTrue(ex.Message.Contains("Initialize collection before checking contents"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ContainsAny_WithNullItems_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var collection = new List<string> { "a", "b", "c" };
            List<string>? items = null;

            // Act
            try
            {
                Assert.That.ContainsAny(collection, items!,
                                        because: "Testing null items parameter handling",
                                        fix: "Initialize items list before calling ContainsAny");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION CONTAINSANY - NULL ITEMS"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Testing null items parameter handling"));
                Assert.IsTrue(ex.Message.Contains("Initialize items list before calling ContainsAny"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ContainsAny_WithMultipleMatches_ShouldPass()
        {
            // Arrange
            var collection = new List<int> { 1, 2, 3, 4, 5 };
            var itemsToFind = new List<int> { 2, 3, 6, 7 };

            // Act & Assert - Should NOT throw (2 and 3 both exist)
            Assert.That.ContainsAny(collection, itemsToFind,
                                    because: "Multiple matches should pass",
                                    fix: "N/A - this should pass");
        }

        [TestMethod]
        public void ContainsAny_WithComplexObjects_ShouldPass()
        {
            // Arrange
            var users = new List<TestUser>
                        {
                            new() { Id = 1, Name = "Alice" },
                            new() { Id = 2, Name = "Bob" }
                        };
            var searchUsers = new List<TestUser>
                              {
                                  new() { Id = 99, Name = "Unknown" },
                                  users[1]
                              };

            // Act & Assert - Should NOT throw (Bob exists)
            Assert.That.ContainsAny(users, searchUsers,
                                    because: "Bob should be found in the list",
                                    fix: "Verify user lookup");
        }

        // ============================================================
        // Edge Case Tests
        // ============================================================

        [TestMethod]
        public void Contains_WithEmptyCollection_ShouldFail()
        {
            // Arrange
            var emptyCollection = new List<string>();
            var threw = false;

            // Act
            try
            {
                Assert.That.Contains(emptyCollection, "item",
                                     because: "Testing empty collection",
                                     fix: "Add items to collection");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void DoesNotContain_WithEmptyCollection_ShouldPass()
        {
            // Arrange
            var emptyCollection = new List<string>();

            // Act & Assert - Should NOT throw (item not in empty collection)
            Assert.That.DoesNotContain(emptyCollection, "item",
                                       because: "Empty collection contains nothing",
                                       fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Contains_WithNullItem_ShouldPassWhenNullExists()
        {
            // Arrange
            var collection = new List<string?> { "a", null, "b" };

            // Act & Assert - Should NOT throw
            Assert.That.Contains(collection, null,
                                 because: "Collection contains null",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Contains_WithLargeCollection_ShouldTruncateOutput()
        {
            // Arrange
            var largeCollection = Enumerable.Range(1, 50).ToList();

            // Act
            try
            {
                Assert.That.Contains(largeCollection, 999,
                                     because: "Testing large collection output truncation",
                                     fix: "Review collection building logic");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output is truncated (should show "more items")
                Assert.IsTrue(ex.Message.Contains("more items"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // Test Helper Classes
        // ============================================================

        private sealed class TestUser
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public override string ToString() => $"TestUser {{ Id: {Id}, Name: {Name} }}";
        }
    }
}
