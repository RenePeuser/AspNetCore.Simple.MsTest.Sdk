using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Core
{
    /// <summary>
    /// Tests for Assert.That.IsNull and Assert.That.IsNotNull
    /// </summary>
    [TestClass]
    [TestCategory("Null")]
    public sealed class AssertThatNullTests
    {
        // ============================================================
        // IsNull Tests
        // ============================================================

        [TestMethod]
        public void IsNull_WhenValueIsNull_ShouldPass()
        {
            // Arrange
            string? value = null;

            // Act & Assert - Should NOT throw
            Assert.That.IsNull(value,
                               because: "Testing that null values pass",
                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNull_WhenValueIsNotNull_ShouldFail()
        {
            // Arrange
            var value = "Not Null";
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNull(value,
                                   because: "Testing that non-null values fail",
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
        public void IsNull_WhenObjectIsNotNull_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var user = new TestUser
                       {
                           Id = 1,
                           Name = "Goku"
                       };

            // Act
            try
            {
                Assert.That.IsNull(user,
                                   because: "User should be null for this test scenario",
                                   fix: "Ensure the GetUser method returns null for invalid IDs");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("NULL REFERENCE - EXPECTED NULL"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User should be null for this test scenario"));
                Assert.IsTrue(ex.Message.Contains("Ensure the GetUser method returns null for invalid IDs"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsNotNull Tests
        // ============================================================

        [TestMethod]
        public void IsNotNull_WhenValueIsNotNull_ShouldPass()
        {
            // Arrange
            var value = "Not Null";

            // Act & Assert - Should NOT throw
            Assert.That.IsNotNull(value,
                                  because: "Testing that non-null values pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNotNull_WhenValueIsNull_ShouldFail()
        {
            // Arrange
            string? value = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNotNull(value,
                                      because: "Testing that null values fail",
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
        public void IsNotNull_WhenObjectIsNull_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            TestUser? user = null;

            // Act
            try
            {
                Assert.That.IsNotNull(user,
                                      because: "User must exist in the database after registration",
                                      fix: "Check database seeding in TestBase.InitializeAsync()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("NULL REFERENCE - EXPECTED NON-NULL"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User must exist in the database after registration"));
                Assert.IsTrue(ex.Message.Contains("Check database seeding in TestBase.InitializeAsync()"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("user"));

                // Verify type info
                Assert.IsTrue(ex.Message.Contains("TestUser"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNotNull_WithComplexObject_ShouldPass()
        {
            // Arrange
            var user = new TestUser
                       {
                           Id = 42,
                           Name = "Vegeta",
                           Email = "vegeta@saiyan.com"
                       };

            // Act & Assert - Should NOT throw
            Assert.That.IsNotNull(user,
                                  because: "User object should be properly initialized",
                                  fix: "Verify user creation logic");
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
    }
}