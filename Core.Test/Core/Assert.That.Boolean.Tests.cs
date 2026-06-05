using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Core
{
    /// <summary>
    /// Tests for Assert.That.IsTrue and Assert.That.IsFalse
    /// </summary>
    [TestClass]
    [TestCategory("Boolean")]
    public sealed class AssertThatBooleanTests
    {
        // ============================================================
        // IsTrue Tests
        // ============================================================

        [TestMethod]
        public void IsTrue_WhenConditionIsTrue_ShouldPass()
        {
            // Arrange
            var condition = true;

            // Act & Assert - Should NOT throw
            Assert.That.IsTrue(condition,
                               because: "Testing that true conditions pass",
                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsTrue_WhenConditionIsFalse_ShouldFail()
        {
            // Arrange
            var condition = false;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsTrue(condition,
                                   because: "Testing that false conditions fail",
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
        public void IsTrue_WhenFails_ShouldHaveBeautifulOutput()
        {
            // Arrange
            var isAuthenticated = false;

            // Act
            try
            {
                Assert.That.IsTrue(isAuthenticated,
                                   because: "User must be authenticated to access premium features",
                                   fix: "Check authentication middleware configuration");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("CONDITION FAILED - EXPECTED TRUE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User must be authenticated to access premium features"));
                Assert.IsTrue(ex.Message.Contains("Check authentication middleware configuration"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("isAuthenticated"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsTrue_WithComplexCondition_ShouldCaptureExpression()
        {
            // Arrange
            var user = new TestUser
            {
                IsActive = false,
                IsVerified = true
            };

            // Act
            try
            {
                Assert.That.IsTrue(user.IsActive && user.IsVerified,
                                   because: "Active and verified users should have full access",
                                   fix: "Activate the user account in the admin panel");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify the entire condition expression was captured
                Assert.IsTrue(ex.Message.Contains("user.IsActive && user.IsVerified"));
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsFalse Tests
        // ============================================================

        [TestMethod]
        public void IsFalse_WhenConditionIsFalse_ShouldPass()
        {
            // Arrange
            var condition = false;

            // Act & Assert - Should NOT throw
            Assert.That.IsFalse(condition,
                                because: "Testing that false conditions pass",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsFalse_WhenConditionIsTrue_ShouldFail()
        {
            // Arrange
            var condition = true;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsFalse(condition,
                                    because: "Testing that true conditions fail",
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
        public void IsFalse_WhenFails_ShouldHaveBeautifulOutput()
        {
            // Arrange
            var hasErrors = true;

            // Act
            try
            {
                Assert.That.IsFalse(hasErrors,
                                    because: "System should be in a valid state after initialization",
                                    fix: "Review startup validation logic and ensure all checks pass");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("CONDITION FAILED - EXPECTED FALSE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("System should be in a valid state after initialization"));
                Assert.IsTrue(ex.Message.Contains("Review startup validation logic and ensure all checks pass"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("hasErrors"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsFalse_WithComplexCondition_ShouldCaptureExpression()
        {
            // Arrange
            var response = new ApiResponse
            {
                IsError = true,
                StatusCode = 500
            };

            // Act
            try
            {
                Assert.That.IsFalse(response.IsError || response.StatusCode >= 400,
                                    because: "API response should indicate success",
                                    fix: "Check the API endpoint implementation for error handling");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify the entire condition expression was captured
                Assert.IsTrue(ex.Message.Contains("response.IsError || response.StatusCode >= 400"));
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // Test Helper Classes
        // ============================================================

        private sealed class TestUser
        {
            public bool IsActive { get; set; }

            public bool IsVerified { get; set; }
        }

        private sealed class ApiResponse
        {
            public bool IsError { get; set; }

            public int StatusCode { get; set; }
        }
    }
}