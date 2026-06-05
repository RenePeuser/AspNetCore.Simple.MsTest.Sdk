using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Numeric
{
    /// <summary>
    /// Tests for Assert.That mathematical property assertions:
    /// IsEven, IsOdd, IsPositive, IsNegative, IsZero
    /// </summary>
    [TestClass]
    [TestCategory("Numeric")]
    public sealed class AssertThatMathTests
    {
        // ============================================================
        // IsEven Tests
        // ============================================================

        [TestMethod]
        public void IsEven_WhenValueIsEven_ShouldPass()
        {
            // Arrange
            var value = 42;

            // Act & Assert - Should NOT throw
            Assert.That.IsEven(value,
                               because: "Testing that even values pass",
                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsEven_WhenValueIsOdd_ShouldFail()
        {
            // Arrange
            var value = 13;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsEven(value,
                                   because: "Testing that odd values fail",
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
        public void IsEven_WhenValueIsOdd_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var pageNumber = 7;

            // Act
            try
            {
                Assert.That.IsEven(pageNumber,
                                   because: "Page numbers must be even for two-sided printing layout",
                                   fix: "Adjust the pagination logic to ensure even page counts");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("MATHEMATICAL PROPERTY FAILED - EXPECTED EVEN"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Page numbers must be even for two-sided printing layout"));
                Assert.IsTrue(ex.Message.Contains("Adjust the pagination logic to ensure even page counts"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("pageNumber"));

                // Verify value is shown
                Assert.IsTrue(ex.Message.Contains("7"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsEven_WithZero_ShouldPass()
        {
            // Arrange
            var value = 0;

            // Act & Assert - Should NOT throw
            Assert.That.IsEven(value,
                               because: "Zero is even (divisible by 2)",
                               fix: "N/A - this should pass");
        }

        // ============================================================
        // IsOdd Tests
        // ============================================================

        [TestMethod]
        public void IsOdd_WhenValueIsOdd_ShouldPass()
        {
            // Arrange
            var value = 13;

            // Act & Assert - Should NOT throw
            Assert.That.IsOdd(value,
                              because: "Testing that odd values pass",
                              fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsOdd_WhenValueIsEven_ShouldFail()
        {
            // Arrange
            var value = 42;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsOdd(value,
                                  because: "Testing that even values fail",
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
        public void IsOdd_WhenValueIsEven_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var playerCount = 8;

            // Act
            try
            {
                Assert.That.IsOdd(playerCount,
                                  because: "Game requires an odd number of players for balanced teams",
                                  fix: "Update team formation logic to handle odd player counts");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("MATHEMATICAL PROPERTY FAILED - EXPECTED ODD"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Game requires an odd number of players for balanced teams"));
                Assert.IsTrue(ex.Message.Contains("Update team formation logic to handle odd player counts"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("playerCount"));

                // Verify value is shown
                Assert.IsTrue(ex.Message.Contains("8"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsOdd_WithNegativeOdd_ShouldPass()
        {
            // Arrange
            var value = -5;

            // Act & Assert - Should NOT throw
            Assert.That.IsOdd(value,
                              because: "Negative odd values are still odd",
                              fix: "N/A - this should pass");
        }

        // ============================================================
        // IsPositive Tests
        // ============================================================

        [TestMethod]
        public void IsPositive_WhenIntIsPositive_ShouldPass()
        {
            // Arrange
            var value = 100;

            // Act & Assert - Should NOT throw
            Assert.That.IsPositive(value,
                                   because: "Testing that positive values pass",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsPositive_WhenIntIsNegative_ShouldFail()
        {
            // Arrange
            var value = -42;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsPositive(value,
                                       because: "Testing that negative values fail",
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
        public void IsPositive_WhenIntIsNegative_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var accountBalance = -250;

            // Act
            try
            {
                Assert.That.IsPositive(accountBalance,
                                       because: "Account balance must be positive after deposit",
                                       fix: "Verify the deposit transaction was applied correctly");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("MATHEMATICAL PROPERTY FAILED - EXPECTED POSITIVE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Account balance must be positive after deposit"));
                Assert.IsTrue(ex.Message.Contains("Verify the deposit transaction was applied correctly"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("accountBalance"));

                // Verify value is shown
                Assert.IsTrue(ex.Message.Contains("-250"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsPositive_WhenIntIsZero_ShouldFail()
        {
            // Arrange
            var value = 0;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsPositive(value,
                                       because: "Testing that zero fails for positive check",
                                       fix: "This is expected to fail");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown for zero");
        }

        [TestMethod]
        public void IsPositive_WhenDoubleIsPositive_ShouldPass()
        {
            // Arrange
            var temperature = 36.5;

            // Act & Assert - Should NOT throw
            Assert.That.IsPositive(temperature,
                                   because: "Body temperature should be positive",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsPositive_WhenDecimalIsPositive_ShouldPass()
        {
            // Arrange
            var price = 99.99m;

            // Act & Assert - Should NOT throw
            Assert.That.IsPositive(price,
                                   because: "Product prices must be positive",
                                   fix: "N/A - this should pass");
        }

        // ============================================================
        // IsNegative Tests
        // ============================================================

        [TestMethod]
        public void IsNegative_WhenIntIsNegative_ShouldPass()
        {
            // Arrange
            var value = -100;

            // Act & Assert - Should NOT throw
            Assert.That.IsNegative(value,
                                   because: "Testing that negative values pass",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNegative_WhenIntIsPositive_ShouldFail()
        {
            // Arrange
            var value = 42;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNegative(value,
                                       because: "Testing that positive values fail",
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
        public void IsNegative_WhenIntIsPositive_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var temperatureDelta = 15;

            // Act
            try
            {
                Assert.That.IsNegative(temperatureDelta,
                                       because: "Temperature change must be negative during cooling phase",
                                       fix: "Check the cooling system calculations and sign conventions");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("MATHEMATICAL PROPERTY FAILED - EXPECTED NEGATIVE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Temperature change must be negative during cooling phase"));
                Assert.IsTrue(ex.Message.Contains("Check the cooling system calculations and sign conventions"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("temperatureDelta"));

                // Verify value is shown
                Assert.IsTrue(ex.Message.Contains("15"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNegative_WhenIntIsZero_ShouldFail()
        {
            // Arrange
            var value = 0;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNegative(value,
                                       because: "Testing that zero fails for negative check",
                                       fix: "This is expected to fail");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown for zero");
        }

        [TestMethod]
        public void IsNegative_WhenDoubleIsNegative_ShouldPass()
        {
            // Arrange
            var elevation = -42.5;

            // Act & Assert - Should NOT throw
            Assert.That.IsNegative(elevation,
                                   because: "Below sea level elevations are negative",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNegative_WhenDecimalIsNegative_ShouldPass()
        {
            // Arrange
            var profit = -1500.75m;

            // Act & Assert - Should NOT throw
            Assert.That.IsNegative(profit,
                                   because: "Company recorded a loss this quarter",
                                   fix: "N/A - this should pass");
        }

        // ============================================================
        // IsZero Tests
        // ============================================================

        [TestMethod]
        public void IsZero_WhenIntIsZero_ShouldPass()
        {
            // Arrange
            var value = 0;

            // Act & Assert - Should NOT throw
            Assert.That.IsZero(value,
                               because: "Testing that zero values pass",
                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsZero_WhenIntIsNonZero_ShouldFail()
        {
            // Arrange
            var value = 42;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsZero(value,
                                   because: "Testing that non-zero values fail",
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
        public void IsZero_WhenIntIsNonZero_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var remainingItems = 3;

            // Act
            try
            {
                Assert.That.IsZero(remainingItems,
                                   because: "All items must be processed and queue should be empty",
                                   fix: "Verify the queue processing loop completes all items");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("MATHEMATICAL PROPERTY FAILED - EXPECTED ZERO"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("All items must be processed and queue should be empty"));
                Assert.IsTrue(ex.Message.Contains("Verify the queue processing loop completes all items"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("remainingItems"));

                // Verify value is shown
                Assert.IsTrue(ex.Message.Contains("3"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsZero_WhenDoubleIsZero_ShouldPass()
        {
            // Arrange
            var delta = 0.0;

            // Act & Assert - Should NOT throw
            Assert.That.IsZero(delta,
                               because: "No change occurred in the measurement",
                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsZero_WhenDecimalIsZero_ShouldPass()
        {
            // Arrange
            var balance = 0.0m;

            // Act & Assert - Should NOT throw
            Assert.That.IsZero(balance,
                               because: "Account should be zeroed out after settlement",
                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsZero_WhenNegativeValue_ShouldFail()
        {
            // Arrange
            var value = -1;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsZero(value,
                                   because: "Testing that negative values fail",
                                   fix: "This is expected to fail");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown for negative value");
        }
    }
}