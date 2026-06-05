using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Numeric
{
    /// <summary>
    /// Tests for Assert.That comparison methods: IsGreaterThan, IsGreaterThanOrEqual, IsLessThan, IsLessThanOrEqual
    /// </summary>
    [TestClass]
    [TestCategory("Numeric")]
    public sealed class AssertThatComparisonTests
    {
        // ============================================================
        // IsGreaterThan Tests
        // ============================================================

        [TestMethod]
        public void IsGreaterThan_WhenValueIsGreater_ShouldPass()
        {
            // Arrange
            var value = 10;
            var threshold = 5;

            // Act & Assert - Should NOT throw
            Assert.That.IsGreaterThan(value, threshold,
                                      because: "Testing that greater values pass",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsGreaterThan_WhenValueIsNotGreater_ShouldFail()
        {
            // Arrange
            var value = 5;
            var threshold = 10;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsGreaterThan(value, threshold,
                                          because: "Testing that smaller values fail",
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
        public void IsGreaterThan_WhenValueIsEqual_ShouldFail()
        {
            // Arrange
            var value = 10;
            var threshold = 10;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsGreaterThan(value, threshold,
                                          because: "Testing that equal values fail",
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
        public void IsGreaterThan_WhenComparisonFails_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var score = 45;
            var passingScore = 50;

            // Act
            try
            {
                Assert.That.IsGreaterThan(score, passingScore,
                                          because: "Score must be greater than passing score to advance",
                                          fix: "Check score calculation logic in ScoreService.CalculateTotal()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COMPARISON FAILED - EXPECTED GREATER THAN"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Score must be greater than passing score to advance"));
                Assert.IsTrue(ex.Message.Contains("Check score calculation logic in ScoreService.CalculateTotal()"));
                Assert.IsTrue(ex.Message.Contains("score"));
                Assert.IsTrue(ex.Message.Contains("45"));
                Assert.IsTrue(ex.Message.Contains("50"));
                Assert.IsTrue(ex.Message.Contains(">"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsGreaterThan_WithDouble_ShouldWork()
        {
            // Arrange
            var temperature = 36.8;
            var threshold = 36.5;

            // Act & Assert - Should NOT throw
            Assert.That.IsGreaterThan(temperature, threshold,
                                      because: "Temperature should exceed normal body temperature",
                                      fix: "Check thermometer calibration");
        }

        // ============================================================
        // IsGreaterThanOrEqual Tests
        // ============================================================

        [TestMethod]
        public void IsGreaterThanOrEqual_WhenValueIsGreater_ShouldPass()
        {
            // Arrange
            var value = 10;
            var threshold = 5;

            // Act & Assert - Should NOT throw
            Assert.That.IsGreaterThanOrEqual(value, threshold,
                                             because: "Testing that greater values pass",
                                             fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsGreaterThanOrEqual_WhenValueIsEqual_ShouldPass()
        {
            // Arrange
            var value = 10;
            var threshold = 10;

            // Act & Assert - Should NOT throw
            Assert.That.IsGreaterThanOrEqual(value, threshold,
                                             because: "Testing that equal values pass",
                                             fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsGreaterThanOrEqual_WhenValueIsLess_ShouldFail()
        {
            // Arrange
            var value = 5;
            var threshold = 10;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsGreaterThanOrEqual(value, threshold,
                                                 because: "Testing that smaller values fail",
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
        public void IsGreaterThanOrEqual_WhenComparisonFails_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var userAge = 16;
            var minimumAge = 18;

            // Act
            try
            {
                Assert.That.IsGreaterThanOrEqual(userAge, minimumAge,
                                                 because: "User must be at least 18 years old to register",
                                                 fix: "Verify age validation in UserRegistrationService.ValidateAge()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COMPARISON FAILED - EXPECTED GREATER THAN OR EQUAL"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User must be at least 18 years old to register"));
                Assert.IsTrue(ex.Message.Contains("Verify age validation in UserRegistrationService.ValidateAge()"));
                Assert.IsTrue(ex.Message.Contains("userAge"));
                Assert.IsTrue(ex.Message.Contains("16"));
                Assert.IsTrue(ex.Message.Contains("18"));
                Assert.IsTrue(ex.Message.Contains(">="));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsGreaterThanOrEqual_WithDecimal_ShouldWork()
        {
            // Arrange
            var balance = 100.00m;
            var minimumBalance = 50.00m;

            // Act & Assert - Should NOT throw
            Assert.That.IsGreaterThanOrEqual(balance, minimumBalance,
                                             because: "Account balance must meet minimum requirement",
                                             fix: "Add funds to account");
        }

        // ============================================================
        // IsLessThan Tests
        // ============================================================

        [TestMethod]
        public void IsLessThan_WhenValueIsLess_ShouldPass()
        {
            // Arrange
            var value = 5;
            var threshold = 10;

            // Act & Assert - Should NOT throw
            Assert.That.IsLessThan(value, threshold,
                                   because: "Testing that smaller values pass",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsLessThan_WhenValueIsNotLess_ShouldFail()
        {
            // Arrange
            var value = 10;
            var threshold = 5;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsLessThan(value, threshold,
                                       because: "Testing that greater values fail",
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
        public void IsLessThan_WhenValueIsEqual_ShouldFail()
        {
            // Arrange
            var value = 10;
            var threshold = 10;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsLessThan(value, threshold,
                                       because: "Testing that equal values fail",
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
        public void IsLessThan_WhenComparisonFails_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var requestSize = 15_000_000; // 15 MB
            var maxSize = 10_000_000;     // 10 MB

            // Act
            try
            {
                Assert.That.IsLessThan(requestSize, maxSize,
                                       because: "Request payload must be under the 10MB limit",
                                       fix: "Reduce payload size or implement chunking in RequestHandler.ProcessUpload()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COMPARISON FAILED - EXPECTED LESS THAN"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Request payload must be under the 10MB limit"));
                Assert.IsTrue(ex.Message.Contains("Reduce payload size or implement chunking in RequestHandler.ProcessUpload()"));
                Assert.IsTrue(ex.Message.Contains("requestSize"));
                Assert.IsTrue(ex.Message.Contains("15000000"));
                Assert.IsTrue(ex.Message.Contains("10000000"));
                Assert.IsTrue(ex.Message.Contains("<"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsLessThan_WithString_ShouldWork()
        {
            // Arrange
            var firstName = "Alice";
            var lastName = "Bob";

            // Act & Assert - Should NOT throw
            Assert.That.IsLessThan(firstName, lastName,
                                   because: "First name should come before last name alphabetically",
                                   fix: "Check string comparison logic");
        }

        // ============================================================
        // IsLessThanOrEqual Tests
        // ============================================================

        [TestMethod]
        public void IsLessThanOrEqual_WhenValueIsLess_ShouldPass()
        {
            // Arrange
            var value = 5;
            var threshold = 10;

            // Act & Assert - Should NOT throw
            Assert.That.IsLessThanOrEqual(value, threshold,
                                          because: "Testing that smaller values pass",
                                          fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsLessThanOrEqual_WhenValueIsEqual_ShouldPass()
        {
            // Arrange
            var value = 10;
            var threshold = 10;

            // Act & Assert - Should NOT throw
            Assert.That.IsLessThanOrEqual(value, threshold,
                                          because: "Testing that equal values pass",
                                          fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsLessThanOrEqual_WhenValueIsGreater_ShouldFail()
        {
            // Arrange
            var value = 10;
            var threshold = 5;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsLessThanOrEqual(value, threshold,
                                              because: "Testing that greater values fail",
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
        public void IsLessThanOrEqual_WhenComparisonFails_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var responseTime = 2500; // milliseconds
            var slaLimit = 2000;     // 2 seconds

            // Act
            try
            {
                Assert.That.IsLessThanOrEqual(responseTime, slaLimit,
                                              because: "API response time must meet SLA of 2 seconds or less",
                                              fix: "Optimize database queries in ProductService.GetProductById()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("COMPARISON FAILED - EXPECTED LESS THAN OR EQUAL"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("API response time must meet SLA of 2 seconds or less"));
                Assert.IsTrue(ex.Message.Contains("Optimize database queries in ProductService.GetProductById()"));
                Assert.IsTrue(ex.Message.Contains("responseTime"));
                Assert.IsTrue(ex.Message.Contains("2500"));
                Assert.IsTrue(ex.Message.Contains("2000"));
                Assert.IsTrue(ex.Message.Contains("<="));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsLessThanOrEqual_WithDateTime_ShouldWork()
        {
            // Arrange
            var createdDate = new DateTime(2024, 1, 1);
            var expiryDate = new DateTime(2024, 12, 31);

            // Act & Assert - Should NOT throw
            Assert.That.IsLessThanOrEqual(createdDate, expiryDate,
                                          because: "Created date must be before or equal to expiry date",
                                          fix: "Check date validation logic");
        }

        // ============================================================
        // Edge Cases and Special Scenarios
        // ============================================================

        [TestMethod]
        public void IsGreaterThan_WithNegativeNumbers_ShouldWork()
        {
            // Arrange
            var value = -5;
            var threshold = -10;

            // Act & Assert - Should NOT throw
            Assert.That.IsGreaterThan(value, threshold,
                                      because: "Testing negative number comparison",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsLessThan_WithNegativeNumbers_ShouldWork()
        {
            // Arrange
            var value = -10;
            var threshold = -5;

            // Act & Assert - Should NOT throw
            Assert.That.IsLessThan(value, threshold,
                                   because: "Testing negative number comparison",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsGreaterThanOrEqual_WithZero_ShouldWork()
        {
            // Arrange
            var value = 0;
            var threshold = 0;

            // Act & Assert - Should NOT throw
            Assert.That.IsGreaterThanOrEqual(value, threshold,
                                             because: "Testing zero comparison",
                                             fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsLessThanOrEqual_WithMaxValue_ShouldWork()
        {
            // Arrange
            var value = int.MaxValue;
            var threshold = int.MaxValue;

            // Act & Assert - Should NOT throw
            Assert.That.IsLessThanOrEqual(value, threshold,
                                          because: "Testing max value comparison",
                                          fix: "N/A - this should pass");
        }
    }
}
