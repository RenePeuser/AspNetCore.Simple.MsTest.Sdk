using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Collection
{
    /// <summary>
    /// Tests for Assert.That.HasCount, Assert.That.HasCountInRange, and Assert.That.HasCountGreaterThan
    /// </summary>
    [TestClass]
    [TestCategory("Collection")]
    public sealed class AssertThatCollectionCountTests
    {
        // ============================================================
        // HasCount Tests
        // ============================================================

        [TestMethod]
        public void HasCount_WhenCollectionHasExactCount_ShouldPass()
        {
            // Arrange
            var numbers = new List<int> { 1, 2, 3, 4, 5 };

            // Act & Assert - Should NOT throw
            Assert.That.HasCount(numbers,
                                 expectedCount: 5,
                                 because: "Testing that exact count passes",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasCount_WhenCollectionCountMismatch_ShouldFail()
        {
            // Arrange
            var fruits = new List<string> { "apple", "banana" };
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCount(fruits,
                                     expectedCount: 5,
                                     because: "Testing that wrong count fails",
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
        public void HasCount_WhenCollectionCountMismatch_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var userIds = new List<int> { 1, 2, 3 };

            // Act
            try
            {
                Assert.That.HasCount(userIds,
                                     expectedCount: 5,
                                     because: "Expected 5 users to be created during test setup",
                                     fix: "Check the SeedTestUsers method ensures 5 users are added");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION COUNT - MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Expected 5 users to be created during test setup"));
                Assert.IsTrue(ex.Message.Contains("Check the SeedTestUsers method ensures 5 users are added"));

                // Verify details are present
                Assert.IsTrue(ex.Message.Contains("Actual Count"));
                Assert.IsTrue(ex.Message.Contains("Expected Count"));
                Assert.IsTrue(ex.Message.Contains("Difference"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasCount_WhenCollectionEmpty_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var items = new List<string>();

            // Act
            try
            {
                Assert.That.HasCount(items,
                                     expectedCount: 3,
                                     because: "Cart should contain items after AddToCart was called",
                                     fix: "Verify AddToCart method is working correctly and items persist");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION COUNT - MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Cart should contain items after AddToCart was called"));
                Assert.IsTrue(ex.Message.Contains("Verify AddToCart method is working correctly and items persist"));

                // Verify collection name was captured
                Assert.IsTrue(ex.Message.Contains("items"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasCount_WhenCollectionHasTooMany_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var tags = new List<string> { "urgent", "bug", "frontend", "backend", "critical" };

            // Act
            try
            {
                Assert.That.HasCount(tags,
                                     expectedCount: 2,
                                     because: "Only 2 tags should be allowed per issue",
                                     fix: "Add validation to reject more than 2 tags");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION COUNT - MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Only 2 tags should be allowed per issue"));
                Assert.IsTrue(ex.Message.Contains("Add validation to reject more than 2 tags"));

                // Verify preview is shown
                Assert.IsTrue(ex.Message.Contains("Preview"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasCount_WhenCollectionIsZeroAndExpectedZero_ShouldPass()
        {
            // Arrange
            var empty = new List<int>();

            // Act & Assert - Should NOT throw
            Assert.That.HasCount(empty,
                                 expectedCount: 0,
                                 because: "Testing empty collection with expected zero",
                                 fix: "N/A - this should pass");
        }

        // ============================================================
        // HasCountInRange Tests
        // ============================================================

        [TestMethod]
        public void HasCountInRange_WhenCollectionIsInRange_ShouldPass()
        {
            // Arrange
            var numbers = new List<int> { 1, 2, 3 };

            // Act & Assert - Should NOT throw
            Assert.That.HasCountInRange(numbers,
                                        minCount: 2,
                                        maxCount: 5,
                                        because: "Testing that count in range passes",
                                        fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasCountInRange_WhenCollectionIsAtMinimum_ShouldPass()
        {
            // Arrange
            var items = new List<string> { "a", "b" };

            // Act & Assert - Should NOT throw
            Assert.That.HasCountInRange(items,
                                        minCount: 2,
                                        maxCount: 10,
                                        because: "Testing minimum boundary",
                                        fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasCountInRange_WhenCollectionIsAtMaximum_ShouldPass()
        {
            // Arrange
            var items = new List<string> { "a", "b", "c", "d", "e" };

            // Act & Assert - Should NOT throw
            Assert.That.HasCountInRange(items,
                                        minCount: 1,
                                        maxCount: 5,
                                        because: "Testing maximum boundary",
                                        fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasCountInRange_WhenCollectionBelowMin_ShouldFail()
        {
            // Arrange
            var scores = new List<int> { 85 };
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCountInRange(scores,
                                            minCount: 3,
                                            maxCount: 10,
                                            because: "Testing below minimum fails",
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
        public void HasCountInRange_WhenCollectionAboveMax_ShouldFail()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3, 4, 5, 6 };
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCountInRange(items,
                                            minCount: 1,
                                            maxCount: 3,
                                            because: "Testing above maximum fails",
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
        public void HasCountInRange_WhenCollectionBelowMin_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var participants = new List<string> { "Alice" };

            // Act
            try
            {
                Assert.That.HasCountInRange(participants,
                                            minCount: 3,
                                            maxCount: 10,
                                            because: "Meeting requires at least 3 participants to proceed",
                                            fix: "Ensure test data includes minimum required participants");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION COUNT - OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Meeting requires at least 3 participants to proceed"));
                Assert.IsTrue(ex.Message.Contains("Ensure test data includes minimum required participants"));

                // Verify range details
                Assert.IsTrue(ex.Message.Contains("Min Count"));
                Assert.IsTrue(ex.Message.Contains("Max Count"));
                Assert.IsTrue(ex.Message.Contains("Below Min By"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasCountInRange_WhenCollectionAboveMax_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var attachments = new List<string> { "file1.pdf", "file2.pdf", "file3.pdf", "file4.pdf", "file5.pdf", "file6.pdf" };

            // Act
            try
            {
                Assert.That.HasCountInRange(attachments,
                                            minCount: 1,
                                            maxCount: 3,
                                            because: "Email attachments are limited to 3 files maximum",
                                            fix: "Add validation to reject uploads beyond 3 files");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION COUNT - OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Email attachments are limited to 3 files maximum"));
                Assert.IsTrue(ex.Message.Contains("Add validation to reject uploads beyond 3 files"));

                // Verify range details
                Assert.IsTrue(ex.Message.Contains("Min Count"));
                Assert.IsTrue(ex.Message.Contains("Max Count"));
                Assert.IsTrue(ex.Message.Contains("Above Max By"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasCountInRange_WhenEmptyAndMinIsZero_ShouldPass()
        {
            // Arrange
            var optional = new List<string>();

            // Act & Assert - Should NOT throw
            Assert.That.HasCountInRange(optional,
                                        minCount: 0,
                                        maxCount: 5,
                                        because: "Testing empty with min zero",
                                        fix: "N/A - this should pass");
        }

        // ============================================================
        // HasCountGreaterThan Tests
        // ============================================================

        [TestMethod]
        public void HasCountGreaterThan_WhenCollectionIsGreater_ShouldPass()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3, 4, 5 };

            // Act & Assert - Should NOT throw
            Assert.That.HasCountGreaterThan(items,
                                            minCount: 3,
                                            because: "Testing greater than passes",
                                            fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasCountGreaterThan_WhenCollectionEqualToMin_ShouldFail()
        {
            // Arrange
            var items = new List<string> { "a", "b", "c" };
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCountGreaterThan(items,
                                                minCount: 3,
                                                because: "Testing equal to min fails (must be GREATER)",
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
        public void HasCountGreaterThan_WhenCollectionLessThanMin_ShouldFail()
        {
            // Arrange
            var items = new List<int> { 1, 2 };
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCountGreaterThan(items,
                                                minCount: 5,
                                                because: "Testing less than min fails",
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
        public void HasCountGreaterThan_WhenCollectionNotGreater_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var comments = new List<string> { "Great post!", "Thanks!" };

            // Act
            try
            {
                Assert.That.HasCountGreaterThan(comments,
                                                minCount: 5,
                                                because: "Popular posts should have more than 5 comments",
                                                fix: "Seed more test comments or adjust popularity threshold");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION COUNT - NOT GREATER THAN"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Popular posts should have more than 5 comments"));
                Assert.IsTrue(ex.Message.Contains("Seed more test comments or adjust popularity threshold"));

                // Verify details
                Assert.IsTrue(ex.Message.Contains("Min Count"));
                Assert.IsTrue(ex.Message.Contains("Short By"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasCountGreaterThan_WhenEmptyCollection_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var results = new List<int>();

            // Act
            try
            {
                Assert.That.HasCountGreaterThan(results,
                                                minCount: 0,
                                                because: "Search should return at least 1 result",
                                                fix: "Check search query parameters and ensure test data exists");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION COUNT - NOT GREATER THAN"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Search should return at least 1 result"));
                Assert.IsTrue(ex.Message.Contains("Check search query parameters and ensure test data exists"));

                // Verify collection name captured
                Assert.IsTrue(ex.Message.Contains("results"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasCountGreaterThan_WhenExactlyAtThreshold_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var votes = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            // Act
            try
            {
                Assert.That.HasCountGreaterThan(votes,
                                                minCount: 10,
                                                because: "Super popular posts need MORE than 10 votes to qualify",
                                                fix: "Either add more votes or change the threshold to >= 10");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION COUNT - NOT GREATER THAN"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Super popular posts need MORE than 10 votes to qualify"));
                Assert.IsTrue(ex.Message.Contains("Either add more votes or change the threshold to >= 10"));

                // Verify actual count shows 10
                Assert.IsTrue(ex.Message.Contains("10"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasCountGreaterThan_WhenMinIsZero_ShouldRequireAtLeastOne()
        {
            // Arrange
            var items = new List<string> { "item1" };

            // Act & Assert - Should NOT throw (1 > 0)
            Assert.That.HasCountGreaterThan(items,
                                            minCount: 0,
                                            because: "At least one item is required",
                                            fix: "N/A - this should pass");
        }

        // ============================================================
        // Edge Cases and Null Handling
        // ============================================================

        [TestMethod]
        public void HasCount_WhenCollectionIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            List<int>? nullCollection = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCount(nullCollection!,
                                     expectedCount: 5,
                                     because: "Testing null handling",
                                     fix: "Don't pass null");
            }
            catch (ArgumentNullException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected ArgumentNullException to be thrown");
        }

        [TestMethod]
        public void HasCountInRange_WhenCollectionIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            List<string>? nullCollection = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCountInRange(nullCollection!,
                                            minCount: 1,
                                            maxCount: 5,
                                            because: "Testing null handling",
                                            fix: "Don't pass null");
            }
            catch (ArgumentNullException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected ArgumentNullException to be thrown");
        }

        [TestMethod]
        public void HasCountGreaterThan_WhenCollectionIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            List<int>? nullCollection = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCountGreaterThan(nullCollection!,
                                                minCount: 0,
                                                because: "Testing null handling",
                                                fix: "Don't pass null");
            }
            catch (ArgumentNullException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected ArgumentNullException to be thrown");
        }

        [TestMethod]
        public void HasCountInRange_WhenMinCountIsNegative_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3 };
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCountInRange(items,
                                            minCount: -1,
                                            maxCount: 5,
                                            because: "Testing negative min",
                                            fix: "Use non-negative min");
            }
            catch (ArgumentOutOfRangeException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected ArgumentOutOfRangeException to be thrown");
        }

        [TestMethod]
        public void HasCountInRange_WhenMaxLessThanMin_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3 };
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCountInRange(items,
                                            minCount: 10,
                                            maxCount: 5,
                                            because: "Testing invalid range",
                                            fix: "Ensure max >= min");
            }
            catch (ArgumentOutOfRangeException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected ArgumentOutOfRangeException to be thrown");
        }

        [TestMethod]
        public void HasCountGreaterThan_WhenMinCountIsNegative_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var items = new List<int> { 1, 2, 3 };
            var threw = false;

            // Act
            try
            {
                Assert.That.HasCountGreaterThan(items,
                                                minCount: -1,
                                                because: "Testing negative min",
                                                fix: "Use non-negative min");
            }
            catch (ArgumentOutOfRangeException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected ArgumentOutOfRangeException to be thrown");
        }

        // ============================================================
        // Different Collection Types
        // ============================================================

        [TestMethod]
        public void HasCount_WithArray_ShouldWork()
        {
            // Arrange
            var array = new[] { 1, 2, 3 };

            // Act & Assert - Should NOT throw
            Assert.That.HasCount(array,
                                 expectedCount: 3,
                                 because: "Testing arrays work",
                                 fix: "N/A");
        }

        [TestMethod]
        public void HasCount_WithHashSet_ShouldWork()
        {
            // Arrange
            var hashSet = new HashSet<string> { "a", "b", "c" };

            // Act & Assert - Should NOT throw
            Assert.That.HasCount(hashSet,
                                 expectedCount: 3,
                                 because: "Testing HashSet works",
                                 fix: "N/A");
        }

        [TestMethod]
        public void HasCountInRange_WithLinqQuery_ShouldWork()
        {
            // Arrange
            var numbers = Enumerable.Range(1, 10).Where(x => x % 2 == 0);

            // Act & Assert - Should NOT throw
            Assert.That.HasCountInRange(numbers,
                                        minCount: 4,
                                        maxCount: 6,
                                        because: "Testing LINQ queries work",
                                        fix: "N/A");
        }

        [TestMethod]
        public void HasCountGreaterThan_WithComplexObjects_ShouldWork()
        {
            // Arrange
            var users = new List<TestUser>
            {
                new TestUser { Id = 1, Name = "Alice" },
                new TestUser { Id = 2, Name = "Bob" },
                new TestUser { Id = 3, Name = "Charlie" },
                new TestUser { Id = 4, Name = "Diana" }
            };

            // Act & Assert - Should NOT throw
            Assert.That.HasCountGreaterThan(users,
                                            minCount: 3,
                                            because: "Testing complex objects work",
                                            fix: "N/A");
        }

        // ============================================================
        // Test Helper Classes
        // ============================================================

        private sealed class TestUser
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;

            public override string ToString() => $"User(Id={Id}, Name={Name})";
        }
    }
}
