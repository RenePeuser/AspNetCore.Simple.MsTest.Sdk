using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Collection
{
    /// <summary>
    /// Tests for Assert.That.IsEmpty, Assert.That.IsNotEmpty, and Assert.That.IsNullOrEmpty
    /// </summary>
    [TestClass]
    [TestCategory("Collection")]
    public sealed class AssertThatCollectionNullTests
    {
        // ============================================================
        // IsEmpty Tests
        // ============================================================

        [TestMethod]
        public void IsEmpty_WhenCollectionIsEmpty_ShouldPass()
        {
            // Arrange
            var emptyList = new List<string>();

            // Act & Assert - Should NOT throw
            Assert.That.IsEmpty(emptyList,
                                because: "Testing that empty collections pass",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsEmpty_WhenCollectionHasElements_ShouldFail()
        {
            // Arrange
            var list = new List<string>
                       {
                           "Item1",
                           "Item2"
                       };

            var threw = false;

            // Act
            try
            {
                Assert.That.IsEmpty(list,
                                    because: "Testing that non-empty collections fail",
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
        public void IsEmpty_WhenCollectionIsNull_ShouldFail()
        {
            // Arrange
            List<string>? list = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsEmpty(list,
                                    because: "Testing that null collections fail IsEmpty",
                                    fix: "Initialize the collection first");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void IsEmpty_WhenCollectionHasElements_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var users = new List<TestUser>
                        {
                            new TestUser
                            {
                                Id = 1,
                                Name = "Goku"
                            },
                            new TestUser
                            {
                                Id = 2,
                                Name = "Vegeta"
                            },
                            new TestUser
                            {
                                Id = 3,
                                Name = "Gohan"
                            }
                        };

            // Act
            try
            {
                Assert.That.IsEmpty(users,
                                    because: "Users list should be empty after logout",
                                    fix: "Call ClearUsers() before this assertion");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION NOT EMPTY - EXPECTED EMPTY COLLECTION"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Users list should be empty after logout"));
                Assert.IsTrue(ex.Message.Contains("Call ClearUsers() before this assertion"));
                Assert.IsTrue(ex.Message.Contains("users"));
                Assert.IsTrue(ex.Message.Contains("Count"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsEmpty_WhenCollectionIsNull_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            List<int>? numbers = null;

            // Act
            try
            {
                Assert.That.IsEmpty(numbers,
                                    because: "Numbers collection should be initialized but empty",
                                    fix: "Initialize numbers = new List<int>() in the setup");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION NULL - EXPECTED NON-NULL COLLECTION"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Numbers collection should be initialized but empty"));
                Assert.IsTrue(ex.Message.Contains("Initialize numbers = new List<int>() in the setup"));
                Assert.IsTrue(ex.Message.Contains("numbers"));
                Assert.IsTrue(ex.Message.Contains("null"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsNotEmpty Tests
        // ============================================================

        [TestMethod]
        public void IsNotEmpty_WhenCollectionHasElements_ShouldPass()
        {
            // Arrange
            var list = new List<string>
                       {
                           "Item1",
                           "Item2"
                       };

            // Act & Assert - Should NOT throw
            Assert.That.IsNotEmpty(list,
                                   because: "Testing that non-empty collections pass",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNotEmpty_WhenCollectionIsEmpty_ShouldFail()
        {
            // Arrange
            var emptyList = new List<string>();
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNotEmpty(emptyList,
                                       because: "Testing that empty collections fail",
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
        public void IsNotEmpty_WhenCollectionIsNull_ShouldFail()
        {
            // Arrange
            List<int>? numbers = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNotEmpty(numbers,
                                       because: "Testing that null collections fail IsNotEmpty",
                                       fix: "Initialize the collection and add elements");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void IsNotEmpty_WhenCollectionIsEmpty_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var products = new List<Product>();

            // Act
            try
            {
                Assert.That.IsNotEmpty(products,
                                       because: "Products must be loaded from the database",
                                       fix: "Verify that SeedDatabase() was called in TestInitialize");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION EMPTY - EXPECTED NON-EMPTY COLLECTION"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Products must be loaded from the database"));
                Assert.IsTrue(ex.Message.Contains("Verify that SeedDatabase() was called in TestInitialize"));
                Assert.IsTrue(ex.Message.Contains("products"));
                Assert.IsTrue(ex.Message.Contains("Count"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNotEmpty_WhenCollectionIsNull_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            List<TestUser>? users = null;

            // Act
            try
            {
                Assert.That.IsNotEmpty(users,
                                       because: "Active users should exist after application startup",
                                       fix: "Check UserService.GetActiveUsers() - ensure it returns a collection, not null");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION NULL - EXPECTED NON-NULL COLLECTION"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Active users should exist after application startup"));
                Assert.IsTrue(ex.Message.Contains("Check UserService.GetActiveUsers() - ensure it returns a collection, not null"));
                Assert.IsTrue(ex.Message.Contains("users"));
                Assert.IsTrue(ex.Message.Contains("null"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNotEmpty_WithSingleElement_ShouldPass()
        {
            // Arrange
            var tags = new List<string> { "important" };

            // Act & Assert - Should NOT throw
            Assert.That.IsNotEmpty(tags,
                                   because: "At least one tag should be present",
                                   fix: "Add tags during document creation");
        }

        // ============================================================
        // IsNullOrEmpty Tests
        // ============================================================

        [TestMethod]
        public void IsNullOrEmpty_WhenCollectionIsNull_ShouldPass()
        {
            // Arrange
            List<string>? list = null;

            // Act & Assert - Should NOT throw
            Assert.That.IsNullOrEmpty(list,
                                      because: "Testing that null collections pass",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNullOrEmpty_WhenCollectionIsEmpty_ShouldPass()
        {
            // Arrange
            var emptyList = new List<string>();

            // Act & Assert - Should NOT throw
            Assert.That.IsNullOrEmpty(emptyList,
                                      because: "Testing that empty collections pass",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNullOrEmpty_WhenCollectionHasElements_ShouldFail()
        {
            // Arrange
            var list = new List<string>
                       {
                           "Item1",
                           "Item2"
                       };

            var threw = false;

            // Act
            try
            {
                Assert.That.IsNullOrEmpty(list,
                                          because: "Testing that non-empty collections fail",
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
        public void IsNullOrEmpty_WhenCollectionHasElements_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var errors = new List<string>
                         {
                             "Error 1",
                             "Error 2",
                             "Error 3",
                             "Error 4"
                         };

            // Act
            try
            {
                Assert.That.IsNullOrEmpty(errors,
                                          because: "No validation errors should occur for valid input",
                                          fix: "Check ValidateInput() method - ensure validation rules allow this input");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION HAS ELEMENTS - EXPECTED NULL OR EMPTY"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("No validation errors should occur for valid input"));
                Assert.IsTrue(ex.Message.Contains("Check ValidateInput() method - ensure validation rules allow this input"));
                Assert.IsTrue(ex.Message.Contains("errors"));
                Assert.IsTrue(ex.Message.Contains("Count"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNullOrEmpty_WithManyElements_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var numbers = new List<int>
                          {
                              1,
                              2,
                              3,
                              4,
                              5,
                              6,
                              7,
                              8,
                              9,
                              10
                          };

            // Act
            try
            {
                Assert.That.IsNullOrEmpty(numbers,
                                          because: "Numbers should not be generated for invalid requests",
                                          fix: "Add request validation before calling GenerateNumbers()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION HAS ELEMENTS - EXPECTED NULL OR EMPTY"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Numbers should not be generated for invalid requests"));
                Assert.IsTrue(ex.Message.Contains("Add request validation before calling GenerateNumbers()"));
                Assert.IsTrue(ex.Message.Contains("numbers"));
                Assert.IsTrue(ex.Message.Contains("10"));

                // Verify truncation indicator for large collections (shows first 5 + ...)
                Assert.IsTrue(ex.Message.Contains("..."));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNullOrEmpty_WithComplexObjects_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var orders = new List<Order>
                         {
                             new Order
                             {
                                 Id = 1,
                                 Total = 100.50m
                             },
                             new Order
                             {
                                 Id = 2,
                                 Total = 250.75m
                             }
                         };

            // Act
            try
            {
                Assert.That.IsNullOrEmpty(orders,
                                          because: "No orders should exist for inactive customers",
                                          fix: "Filter orders by customer.IsActive before returning");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION HAS ELEMENTS - EXPECTED NULL OR EMPTY"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("No orders should exist for inactive customers"));
                Assert.IsTrue(ex.Message.Contains("Filter orders by customer.IsActive before returning"));
                Assert.IsTrue(ex.Message.Contains("orders"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // Additional Edge Case Tests
        // ============================================================

        [TestMethod]
        public void IsEmpty_WithArrayType_ShouldPass()
        {
            // Arrange
            var emptyArray = new string[0];

            // Act & Assert - Should NOT throw
            Assert.That.IsEmpty(emptyArray,
                                because: "Testing with array type",
                                fix: "N/A");
        }

        [TestMethod]
        public void IsNotEmpty_WithArrayType_ShouldPass()
        {
            // Arrange
            var array = new[] { 1, 2, 3 };

            // Act & Assert - Should NOT throw
            Assert.That.IsNotEmpty(array,
                                   because: "Testing with array type",
                                   fix: "N/A");
        }

        [TestMethod]
        public void IsNullOrEmpty_WithArrayType_ShouldPass()
        {
            // Arrange
            int[]? array = null;

            // Act & Assert - Should NOT throw
            Assert.That.IsNullOrEmpty(array,
                                      because: "Testing with null array",
                                      fix: "N/A");
        }

        [TestMethod]
        public void IsEmpty_WithIEnumerableType_ShouldPass()
        {
            // Arrange
            IEnumerable<int> emptyEnumerable = Enumerable.Empty<int>();

            // Act & Assert - Should NOT throw
            Assert.That.IsEmpty(emptyEnumerable,
                                because: "Testing with IEnumerable type",
                                fix: "N/A");
        }

        [TestMethod]
        public void IsNotEmpty_WithIEnumerableType_ShouldPass()
        {
            // Arrange
            IEnumerable<int> enumerable = Enumerable.Range(1, 5);

            // Act & Assert - Should NOT throw
            Assert.That.IsNotEmpty(enumerable,
                                   because: "Testing with IEnumerable type",
                                   fix: "N/A");
        }

        // ============================================================
        // Test Helper Classes
        // ============================================================

        private sealed class TestUser
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string? Email { get; set; }

            public override string ToString() => $"TestUser {{ Id: {Id}, Name: {Name} }}";
        }

        private sealed class Product
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public decimal Price { get; set; }

            public override string ToString() => $"Product {{ Id: {Id}, Name: {Name}, Price: {Price:C} }}";
        }

        private sealed class Order
        {
            public int Id { get; set; }

            public decimal Total { get; set; }

            public override string ToString() => $"Order {{ Id: {Id}, Total: {Total:C} }}";
        }
    }
}