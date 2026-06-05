using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Collection
{
    /// <summary>
    /// Tests for Assert.That.AreEqual and Assert.That.AreEquivalent (collection equality/equivalence)
    /// </summary>
    [TestClass]
    [TestCategory("Collection")]
    public sealed class AssertThatCollectionEqualityTests
    {
        // ============================================================
        // AreEqual Tests (Order Matters)
        // ============================================================

        [TestMethod]
        public void AreEqual_WhenCollectionsAreEqual_ShouldPass()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3, 4, 5 };
            var actual = new List<int> { 1, 2, 3, 4, 5 };

            // Act & Assert - Should NOT throw
            Assert.That.AreEqual(expected, actual,
                                 because: "Both collections contain the same elements in the same order",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreEqual_WhenCollectionsAreBothNull_ShouldPass()
        {
            // Arrange
            List<string>? expected = null;
            List<string>? actual = null;

            // Act & Assert - Should NOT throw
            Assert.That.AreEqual(expected, actual,
                                 because: "Both collections are null",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreEqual_WhenCollectionsAreBothEmpty_ShouldPass()
        {
            // Arrange
            var expected = new List<string>();
            var actual = new List<string>();

            // Act & Assert - Should NOT throw
            Assert.That.AreEqual(expected, actual,
                                 because: "Both collections are empty",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreEqual_WhenOrderDiffers_ShouldFail()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3 };
            var actual = new List<int> { 3, 2, 1 };
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEqual(expected, actual,
                                     because: "Testing that different order causes failure",
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
        public void AreEqual_WhenCountDiffers_ShouldFail()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3 };
            var actual = new List<int> { 1, 2, 3, 4, 5 };
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEqual(expected, actual,
                                     because: "Testing that different counts cause failure",
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
        public void AreEqual_WhenExpectedIsNull_ShouldFail()
        {
            // Arrange
            List<string>? expected = null;
            var actual = new List<string> { "a", "b" };
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEqual(expected, actual,
                                     because: "Expected collection is null but actual is not",
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
        public void AreEqual_WhenActualIsNull_ShouldFail()
        {
            // Arrange
            var expected = new List<string> { "a", "b" };
            List<string>? actual = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEqual(expected, actual,
                                     because: "Actual collection is null but expected is not",
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
        public void AreEqual_WhenElementsDiffer_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expectedUserIds = new List<int> { 10, 20, 30, 40, 50 };
            var actualUserIds = new List<int> { 10, 20, 99, 40, 50 };

            // Act
            try
            {
                Assert.That.AreEqual(expectedUserIds, actualUserIds,
                                     because: "User IDs should match the expected sequence from the API response",
                                     fix: "Check the API endpoint /users/list for correct ordering and IDs");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION EQUALITY FAILED - ORDER MATTERS"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User IDs should match the expected sequence from the API response"));
                Assert.IsTrue(ex.Message.Contains("Check the API endpoint /users/list for correct ordering and IDs"));

                // Verify variable names are captured
                Assert.IsTrue(ex.Message.Contains("expectedUserIds"));
                Assert.IsTrue(ex.Message.Contains("actualUserIds"));

                // Verify element comparison section
                Assert.IsTrue(ex.Message.Contains("Element Comparison:"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreEqual_WhenStringCollectionsDiffer_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expectedRoles = new List<string> { "Admin", "User", "Guest" };
            var actualRoles = new List<string> { "Admin", "PowerUser", "Guest" };

            // Act
            try
            {
                Assert.That.AreEqual(expectedRoles, actualRoles,
                                     because: "The roles assigned to the user must match the default roles",
                                     fix: "Verify the role assignment logic in UserService.AssignDefaultRoles()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION EQUALITY FAILED - ORDER MATTERS"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("The roles assigned to the user must match the default roles"));
                Assert.IsTrue(ex.Message.Contains("Verify the role assignment logic in UserService.AssignDefaultRoles()"));

                // Verify AreEquivalent suggestion for order-independent check
                Assert.IsTrue(ex.Message.Contains("Consider using AreEquivalent() if order doesn't matter"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreEqual_WhenCountDiffers_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expectedProducts = new List<string> { "Laptop", "Mouse", "Keyboard" };
            var actualProducts = new List<string> { "Laptop", "Mouse", "Keyboard", "Monitor", "Webcam" };

            // Act
            try
            {
                Assert.That.AreEqual(expectedProducts, actualProducts,
                                     because: "Shopping cart should contain exactly the items added by the user",
                                     fix: "Check if additional items are being auto-added in CartService.AddItem()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION EQUALITY FAILED - ORDER MATTERS"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Shopping cart should contain exactly the items added by the user"));
                Assert.IsTrue(ex.Message.Contains("Check if additional items are being auto-added in CartService.AddItem()"));

                // Verify counts are shown
                Assert.IsTrue(ex.Message.Contains("Expected Count"));
                Assert.IsTrue(ex.Message.Contains("Actual Count"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // AreEquivalent Tests (Order Doesn't Matter)
        // ============================================================

        [TestMethod]
        public void AreEquivalent_WhenCollectionsAreEquivalent_ShouldPass()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3, 4, 5 };
            var actual = new List<int> { 5, 4, 3, 2, 1 };

            // Act & Assert - Should NOT throw
            Assert.That.AreEquivalent(expected, actual,
                                      because: "Both collections contain the same elements regardless of order",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreEquivalent_WhenCollectionsAreBothNull_ShouldPass()
        {
            // Arrange
            List<string>? expected = null;
            List<string>? actual = null;

            // Act & Assert - Should NOT throw
            Assert.That.AreEquivalent(expected, actual,
                                      because: "Both collections are null",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreEquivalent_WhenCollectionsAreBothEmpty_ShouldPass()
        {
            // Arrange
            var expected = new List<string>();
            var actual = new List<string>();

            // Act & Assert - Should NOT throw
            Assert.That.AreEquivalent(expected, actual,
                                      because: "Both collections are empty",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreEquivalent_WhenOrderDiffers_ShouldPass()
        {
            // Arrange
            var expected = new List<string> { "apple", "banana", "cherry" };
            var actual = new List<string> { "cherry", "apple", "banana" };

            // Act & Assert - Should NOT throw
            Assert.That.AreEquivalent(expected, actual,
                                      because: "Same elements in different order should be equivalent",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreEquivalent_WhenElementsMissing_ShouldFail()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3, 4, 5 };
            var actual = new List<int> { 1, 2, 3 };
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEquivalent(expected!, actual!,
                                          because: "Testing that missing elements cause failure",
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
        public void AreEquivalent_WhenExtraElements_ShouldFail()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3 };
            var actual = new List<int> { 1, 2, 3, 4, 5 };
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEquivalent(expected!, actual!,
                                          because: "Testing that extra elements cause failure",
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
        public void AreEquivalent_WhenDifferentElements_ShouldFail()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 3 };
            var actual = new List<int> { 4, 5, 6 };
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEquivalent(expected!, actual!,
                                          because: "Testing that different elements cause failure",
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
        public void AreEquivalent_WhenExpectedIsNull_ShouldFail()
        {
            // Arrange
            List<string>? expected = null;
            var actual = new List<string> { "a", "b" };
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEquivalent(expected!, actual!,
                                          because: "Expected collection is null but actual is not",
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
        public void AreEquivalent_WhenActualIsNull_ShouldFail()
        {
            // Arrange
            var expected = new List<string> { "a", "b" };
            List<string>? actual = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEquivalent(expected!, actual!,
                                          because: "Actual collection is null but expected is not",
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
        public void AreEquivalent_WhenMissingElements_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expectedTags = new List<string> { "csharp", "dotnet", "testing", "mstest", "sdk" };
            var actualTags = new List<string> { "csharp", "dotnet", "testing" };

            // Act
            try
            {
                Assert.That.AreEquivalent(expectedTags, actualTags,
                                          because: "All required tags must be present on the blog post",
                                          fix: "Verify tag population in BlogPostService.CreatePost()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION EQUIVALENCE FAILED - ORDER IGNORED"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("All required tags must be present on the blog post"));
                Assert.IsTrue(ex.Message.Contains("Verify tag population in BlogPostService.CreatePost()"));

                // Verify variable names are captured
                Assert.IsTrue(ex.Message.Contains("expectedTags"));
                Assert.IsTrue(ex.Message.Contains("actualTags"));

                // Verify missing/extra elements section
                Assert.IsTrue(ex.Message.Contains("Missing in Actual"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreEquivalent_WhenExtraElements_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expectedPermissions = new List<string> { "read", "write" };
            var actualPermissions = new List<string> { "read", "write", "delete", "admin" };

            // Act
            try
            {
                Assert.That.AreEquivalent(expectedPermissions, actualPermissions,
                                          because: "User should only have basic permissions after registration",
                                          fix: "Check PermissionService.AssignDefaultPermissions() for over-assignment");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION EQUIVALENCE FAILED - ORDER IGNORED"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User should only have basic permissions after registration"));
                Assert.IsTrue(ex.Message.Contains("Check PermissionService.AssignDefaultPermissions() for over-assignment"));

                // Verify variable names are captured
                Assert.IsTrue(ex.Message.Contains("expectedPermissions"));
                Assert.IsTrue(ex.Message.Contains("actualPermissions"));

                // Verify missing/extra elements section
                Assert.IsTrue(ex.Message.Contains("Extra in Actual"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreEquivalent_WhenCompletelyDifferent_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expectedFeatures = new List<string> { "feature-a", "feature-b", "feature-c" };
            var actualFeatures = new List<string> { "feature-x", "feature-y", "feature-z" };

            // Act
            try
            {
                Assert.That.AreEquivalent(expectedFeatures, actualFeatures,
                                          because: "Feature flags should match the configuration for production environment",
                                          fix: "Review FeatureFlagService.GetActiveFeatures() and configuration file");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COLLECTION EQUIVALENCE FAILED - ORDER IGNORED"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Feature flags should match the configuration for production environment"));
                Assert.IsTrue(ex.Message.Contains("Review FeatureFlagService.GetActiveFeatures() and configuration file"));

                // Verify both missing and extra sections present
                Assert.IsTrue(ex.Message.Contains("Missing in Actual"));
                Assert.IsTrue(ex.Message.Contains("Extra in Actual"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // Complex Object Tests
        // ============================================================

        [TestMethod]
        public void AreEqual_WithComplexObjects_ShouldPass()
        {
            // Arrange
            var expected = new List<TestUser>
                           {
                               new TestUser { Id = 1, Name = "Goku" },
                               new TestUser { Id = 2, Name = "Vegeta" },
                               new TestUser { Id = 3, Name = "Gohan" }
                           };

            var actual = new List<TestUser>
                         {
                             new TestUser { Id = 1, Name = "Goku" },
                             new TestUser { Id = 2, Name = "Vegeta" },
                             new TestUser { Id = 3, Name = "Gohan" }
                         };

            // Act & Assert - Should NOT throw
            Assert.That.AreEqual(expected, actual,
                                 because: "User objects are equal when all properties match",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreEquivalent_WithComplexObjects_ShouldPass()
        {
            // Arrange
            var expected = new List<TestUser>
                           {
                               new TestUser { Id = 1, Name = "Goku" },
                               new TestUser { Id = 2, Name = "Vegeta" },
                               new TestUser { Id = 3, Name = "Gohan" }
                           };

            var actual = new List<TestUser>
                         {
                             new TestUser { Id = 3, Name = "Gohan" },
                             new TestUser { Id = 1, Name = "Goku" },
                             new TestUser { Id = 2, Name = "Vegeta" }
                         };

            // Act & Assert - Should NOT throw
            Assert.That.AreEquivalent(expected, actual,
                                      because: "User objects are equivalent when same elements present, regardless of order",
                                      fix: "N/A - this should pass");
        }

        // ============================================================
        // Edge Cases
        // ============================================================

        [TestMethod]
        public void AreEqual_WithLargeCollection_ShouldTruncateOutput()
        {
            // Arrange
            var expected = Enumerable.Range(1, 20).ToList();
            var actual = Enumerable.Range(1, 15).Concat(new[] { 99 }).Concat(Enumerable.Range(16, 4)).ToList();

            // Act
            try
            {
                Assert.That.AreEqual(expected, actual,
                                     because: "Large collections should show truncated comparison",
                                     fix: "Verify data generation logic");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify truncation message appears
                Assert.IsTrue(ex.Message.Contains("more elements") || ex.Message.Contains("Element Comparison:"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreEquivalent_WithDuplicates_ShouldHandleCorrectly()
        {
            // Arrange
            var expected = new List<int> { 1, 2, 2, 3 };
            var actual = new List<int> { 3, 2, 2, 1 };

            // Act & Assert - Should NOT throw
            Assert.That.AreEquivalent(expected, actual,
                                      because: "Collections with duplicates should be equivalent if same count of each element",
                                      fix: "N/A - this should pass");
        }

        // ============================================================
        // Test Helper Classes
        // ============================================================

        private sealed class TestUser : IEquatable<TestUser>, IComparable<TestUser>
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public override string ToString() => $"TestUser {{ Id: {Id}, Name: {Name} }}";

            public bool Equals(TestUser? other)
            {
                if (other is null)
                    return false;
                return Id == other.Id && Name == other.Name;
            }

            public override bool Equals(object? obj) => Equals(obj as TestUser);

            public override int GetHashCode() => HashCode.Combine(Id, Name);

            public int CompareTo(TestUser? other)
            {
                if (other is null)
                    return 1;
                var idComparison = Id.CompareTo(other.Id);
                return idComparison != 0 ? idComparison : string.Compare(Name, other.Name, StringComparison.Ordinal);
            }
        }
    }
}
