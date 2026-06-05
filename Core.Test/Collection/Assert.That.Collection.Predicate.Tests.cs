using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Collection
{
    /// <summary>
    /// Tests for Assert.That collection predicate methods (All, Any, None, Single)
    /// </summary>
    [TestClass]
    [TestCategory("Collection")]
    public sealed class AssertThatCollectionPredicateTests
    {
        // ============================================================
        // All Tests
        // ============================================================

        [TestMethod]
        public void All_WhenAllItemsMatchPredicate_ShouldPass()
        {
            // Arrange
            var numbers = new[]
                          {
                              2, 4, 6,
                              8, 10
                          };

            // Act & Assert - Should NOT throw
            Assert.That.All(numbers,
                            predicate: x => x % 2 == 0,
                            predicateDescription: "is even",
                            because: "All numbers in this collection should be even",
                            fix: "N/A - this should pass");
        }

        [TestMethod]
        public void All_WhenSomeItemsDoNotMatchPredicate_ShouldFail()
        {
            // Arrange
            var numbers = new[]
                          {
                              2, 4, 5,
                              8, 9
                          };

            var threw = false;

            // Act
            try
            {
                Assert.That.All(numbers,
                                predicate: x => x % 2 == 0,
                                predicateDescription: "is even",
                                because: "All numbers should be even for this test",
                                fix: "Filter the collection to include only even numbers");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void All_WhenFailure_ShouldProduceBeautifulOutput()
        {
            // Arrange
            var users = new[]
                        {
                            new TestUser
                            {
                                Id = 1,
                                Name = "Goku",
                                Age = 30
                            },
                            new TestUser
                            {
                                Id = 2,
                                Name = "Vegeta",
                                Age = 35
                            },
                            new TestUser
                            {
                                Id = 3,
                                Name = "Piccolo",
                                Age = 15
                            },
                            new TestUser
                            {
                                Id = 4,
                                Name = "Gohan",
                                Age = 17
                            }
                        };

            // Act
            try
            {
                Assert.That.All(users,
                                predicate: u => u.Age >= 18,
                                predicateDescription: "age >= 18 (adult)",
                                because: "All users must be adults to access this feature",
                                fix: "Add age validation at registration or filter users before this assertion");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION PREDICATE FAILED - NOT ALL ITEMS MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("All users must be adults to access this feature"));
                Assert.IsTrue(ex.Message.Contains("Add age validation at registration or filter users before this assertion"));
                Assert.IsTrue(ex.Message.Contains("age >= 18 (adult)"));
                Assert.IsTrue(ex.Message.Contains("Items that failed predicate"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void All_WhenEmptyCollection_ShouldPass()
        {
            // Arrange
            var numbers = Array.Empty<int>();

            // Act & Assert - Should NOT throw (vacuous truth)
            Assert.That.All(numbers,
                            predicate: x => x < 0,
                            predicateDescription: "is negative",
                            because: "Empty collections vacuously satisfy all predicates",
                            fix: "N/A - this should pass");
        }

        [TestMethod]
        public void All_WhenNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            int[]? numbers = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                Assert.That.All(numbers,
                                predicate: x => x > 0,
                                predicateDescription: "is positive",
                                because: "Testing null collection",
                                fix: "N/A");
            });
        }

        // ============================================================
        // Any Tests
        // ============================================================

        [TestMethod]
        public void Any_WhenAtLeastOneItemMatchesPredicate_ShouldPass()
        {
            // Arrange
            var numbers = new[]
                          {
                              1, 3, 5,
                              6, 7
                          };

            // Act & Assert - Should NOT throw
            Assert.That.Any(numbers,
                            predicate: x => x % 2 == 0,
                            predicateDescription: "is even",
                            because: "There should be at least one even number",
                            fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Any_WhenNoItemsMatchPredicate_ShouldFail()
        {
            // Arrange
            var numbers = new[]
                          {
                              1, 3, 5,
                              7, 9
                          };

            var threw = false;

            // Act
            try
            {
                Assert.That.Any(numbers,
                                predicate: x => x % 2 == 0,
                                predicateDescription: "is even",
                                because: "Expected at least one even number",
                                fix: "Add even numbers to the collection");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void Any_WhenFailure_ShouldProduceBeautifulOutput()
        {
            // Arrange
            var orders = new[]
                         {
                             new TestOrder
                             {
                                 Id = 1,
                                 Status = "Pending",
                                 Amount = 100
                             },
                             new TestOrder
                             {
                                 Id = 2,
                                 Status = "Pending",
                                 Amount = 200
                             },
                             new TestOrder
                             {
                                 Id = 3,
                                 Status = "Cancelled",
                                 Amount = 150
                             }
                         };

            // Act
            try
            {
                Assert.That.Any(orders,
                                predicate: o => o.Status == "Completed",
                                predicateDescription: "status is 'Completed'",
                                because: "At least one order should be completed for this report",
                                fix: "Ensure the order processing workflow completes orders correctly");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION PREDICATE FAILED - NO ITEMS MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("At least one order should be completed for this report"));
                Assert.IsTrue(ex.Message.Contains("Ensure the order processing workflow completes orders correctly"));
                Assert.IsTrue(ex.Message.Contains("status is 'Completed'"));
                Assert.IsTrue(ex.Message.Contains("At least one item matches"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Any_WhenEmptyCollection_ShouldFail()
        {
            // Arrange
            var numbers = Array.Empty<int>();
            var threw = false;

            // Act
            try
            {
                Assert.That.Any(numbers,
                                predicate: x => x > 0,
                                predicateDescription: "is positive",
                                because: "Empty collections have no items to match",
                                fix: "Populate the collection with items");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException for empty collection");
        }

        [TestMethod]
        public void Any_WhenNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            int[]? numbers = null;

            // Act & Assert
            Action testAction = () => Assert.That.Any(numbers,
                                                      predicate: (int x) => x > 0,
                                                      predicateDescription: "is positive",
                                                      because: "Testing null collection",
                                                      fix: "N/A");

            Assert.That.Throws<ArgumentNullException>(testAction,
                                                      because: "Null collection should throw",
                                                      fix: "N/A");
        }

        // ============================================================
        // None Tests
        // ============================================================

        [TestMethod]
        public void None_WhenNoItemsMatchPredicate_ShouldPass()
        {
            // Arrange
            var numbers = new[]
                          {
                              1, 3, 5,
                              7, 9
                          };

            // Act & Assert - Should NOT throw
            Assert.That.None(numbers,
                             predicate: x => x % 2 == 0,
                             predicateDescription: "is even",
                             because: "All numbers should be odd",
                             fix: "N/A - this should pass");
        }

        [TestMethod]
        public void None_WhenSomeItemsMatchPredicate_ShouldFail()
        {
            // Arrange
            var numbers = new[]
                          {
                              1, 3, 4,
                              7, 9
                          };

            var threw = false;

            // Act
            try
            {
                Assert.That.None(numbers,
                                 predicate: x => x % 2 == 0,
                                 predicateDescription: "is even",
                                 because: "No even numbers should be present",
                                 fix: "Filter out even numbers from the collection");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void None_WhenFailure_ShouldProduceBeautifulOutput()
        {
            // Arrange
            var products = new[]
                           {
                               new TestProduct
                               {
                                   Id = 1,
                                   Name = "Widget",
                                   Price = 10.99m,
                                   IsDiscontinued = false
                               },
                               new TestProduct
                               {
                                   Id = 2,
                                   Name = "Gadget",
                                   Price = 20.99m,
                                   IsDiscontinued = false
                               },
                               new TestProduct
                               {
                                   Id = 3,
                                   Name = "OldGizmo",
                                   Price = 5.99m,
                                   IsDiscontinued = true
                               },
                               new TestProduct
                               {
                                   Id = 4,
                                   Name = "OldThing",
                                   Price = 3.99m,
                                   IsDiscontinued = true
                               }
                           };

            // Act
            try
            {
                Assert.That.None(products,
                                 predicate: p => p.IsDiscontinued,
                                 predicateDescription: "is discontinued",
                                 because: "Discontinued products should be filtered from the active catalog",
                                 fix: "Apply IsDiscontinued = false filter before this assertion");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION PREDICATE FAILED - SOME ITEMS MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Discontinued products should be filtered from the active catalog"));
                Assert.IsTrue(ex.Message.Contains("Apply IsDiscontinued = false filter before this assertion"));
                Assert.IsTrue(ex.Message.Contains("is discontinued"));
                Assert.IsTrue(ex.Message.Contains("Items that unexpectedly matched"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void None_WhenEmptyCollection_ShouldPass()
        {
            // Arrange
            var numbers = Array.Empty<int>();

            // Act & Assert - Should NOT throw (vacuous truth)
            Assert.That.None(numbers,
                             predicate: x => x > 0,
                             predicateDescription: "is positive",
                             because: "Empty collections have no items to match",
                             fix: "N/A - this should pass");
        }

        [TestMethod]
        public void None_WhenNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            int[]? numbers = null;

            // Act & Assert
            Action testAction = () => Assert.That.None(numbers,
                                                       predicate: (int x) => x < 0,
                                                       predicateDescription: "is negative",
                                                       because: "Testing null collection",
                                                       fix: "N/A");

            Assert.That.Throws<ArgumentNullException>(testAction,
                                                      because: "Null collection should throw",
                                                      fix: "N/A");
        }

        // ============================================================
        // Single Tests
        // ============================================================

        [TestMethod]
        public void Single_WhenExactlyOneItemMatchesPredicate_ShouldPass()
        {
            // Arrange
            var numbers = new[]
                          {
                              1, 2, 3,
                              5, 7
                          };

            // Act & Assert - Should NOT throw
            Assert.That.Single(numbers,
                               predicate: x => x % 2 == 0,
                               predicateDescription: "is even",
                               because: "Exactly one even number should exist",
                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Single_WhenNoItemsMatchPredicate_ShouldFail()
        {
            // Arrange
            var numbers = new[]
                          {
                              1, 3, 5,
                              7, 9
                          };

            var threw = false;

            // Act
            try
            {
                Assert.That.Single(numbers,
                                   predicate: x => x % 2 == 0,
                                   predicateDescription: "is even",
                                   because: "Expected exactly one even number",
                                   fix: "Add exactly one even number to the collection");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void Single_WhenMultipleItemsMatchPredicate_ShouldFail()
        {
            // Arrange
            var numbers = new[]
                          {
                              2, 4, 5,
                              7, 9
                          };

            var threw = false;

            // Act
            try
            {
                Assert.That.Single(numbers,
                                   predicate: x => x % 2 == 0,
                                   predicateDescription: "is even",
                                   because: "Expected exactly one even number",
                                   fix: "Ensure only one even number exists in the collection");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void Single_WhenFailureWithZeroMatches_ShouldProduceBeautifulOutput()
        {
            // Arrange
            var users = new[]
                        {
                            new TestUser
                            {
                                Id = 1,
                                Name = "Goku",
                                Age = 30,
                                Role = "User"
                            },
                            new TestUser
                            {
                                Id = 2,
                                Name = "Vegeta",
                                Age = 35,
                                Role = "User"
                            },
                            new TestUser
                            {
                                Id = 3,
                                Name = "Piccolo",
                                Age = 40,
                                Role = "User"
                            }
                        };

            // Act
            try
            {
                Assert.That.Single(users,
                                   predicate: u => u.Role == "Admin",
                                   predicateDescription: "role is 'Admin'",
                                   because: "Exactly one admin user should exist in the system",
                                   fix: "Seed the database with exactly one admin user");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION PREDICATE FAILED - EXPECTED EXACTLY ONE MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Exactly one admin user should exist in the system"));
                Assert.IsTrue(ex.Message.Contains("Seed the database with exactly one admin user"));
                Assert.IsTrue(ex.Message.Contains("role is 'Admin'"));
                Assert.IsTrue(ex.Message.Contains("no items matched"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Single_WhenFailureWithMultipleMatches_ShouldProduceBeautifulOutput()
        {
            // Arrange
            var accounts = new[]
                           {
                               new TestAccount
                               {
                                   Id = 1,
                                   Email = "user1@test.com",
                                   IsPrimary = true
                               },
                               new TestAccount
                               {
                                   Id = 2,
                                   Email = "user2@test.com",
                                   IsPrimary = false
                               },
                               new TestAccount
                               {
                                   Id = 3,
                                   Email = "user3@test.com",
                                   IsPrimary = true
                               },
                               new TestAccount
                               {
                                   Id = 4,
                                   Email = "user4@test.com",
                                   IsPrimary = true
                               }
                           };

            // Act
            try
            {
                Assert.That.Single(accounts,
                                   predicate: a => a.IsPrimary,
                                   predicateDescription: "IsPrimary is true",
                                   because: "Each user can have only one primary account",
                                   fix: "Ensure business logic sets only one account as primary");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION PREDICATE FAILED - EXPECTED EXACTLY ONE MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Each user can have only one primary account"));
                Assert.IsTrue(ex.Message.Contains("Ensure business logic sets only one account as primary"));
                Assert.IsTrue(ex.Message.Contains("IsPrimary is true"));
                Assert.IsTrue(ex.Message.Contains("3 items matched"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Single_WhenEmptyCollection_ShouldFail()
        {
            // Arrange
            var numbers = Array.Empty<int>();
            var threw = false;

            // Act
            try
            {
                Assert.That.Single(numbers,
                                   predicate: x => x > 0,
                                   predicateDescription: "is positive",
                                   because: "Empty collections have no items",
                                   fix: "Populate the collection with exactly one matching item");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException for empty collection");
        }

        [TestMethod]
        public void Single_WhenNullCollection_ShouldThrowArgumentNullException()
        {
            // Arrange
            int[]? numbers = null;

            // Act & Assert
            Action testAction = () => Assert.That.Single(numbers,
                                                         predicate: (int x) => x == 5,
                                                         predicateDescription: "equals 5",
                                                         because: "Testing null collection",
                                                         fix: "N/A");

            Assert.That.Throws<ArgumentNullException>(testAction,
                                                      because: "Null collection should throw",
                                                      fix: "N/A");
        }

        // ============================================================
        // Test Helper Classes
        // ============================================================

        private sealed class TestUser
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public int Age { get; set; }

            public string Role { get; set; } = "User";

            public override string ToString() => $"TestUser {{ Id: {Id}, Name: {Name}, Age: {Age}, Role: {Role} }}";
        }

        private sealed class TestOrder
        {
            public int Id { get; set; }

            public string Status { get; set; } = string.Empty;

            public decimal Amount { get; set; }

            public override string ToString() => $"TestOrder {{ Id: {Id}, Status: {Status}, Amount: {Amount:C} }}";
        }

        private sealed class TestProduct
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public decimal Price { get; set; }

            public bool IsDiscontinued { get; set; }

            public override string ToString() => $"TestProduct {{ Id: {Id}, Name: {Name}, Price: {Price:C}, IsDiscontinued: {IsDiscontinued} }}";
        }

        private sealed class TestAccount
        {
            public int Id { get; set; }

            public string Email { get; set; } = string.Empty;

            public bool IsPrimary { get; set; }

            public override string ToString() => $"TestAccount {{ Id: {Id}, Email: {Email}, IsPrimary: {IsPrimary} }}";
        }
    }
}