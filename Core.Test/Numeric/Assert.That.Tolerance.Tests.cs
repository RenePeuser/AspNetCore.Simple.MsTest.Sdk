using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Numeric
{
    /// <summary>
    /// Tests for Assert.That.IsCloseTo tolerance assertions
    /// </summary>
    [TestClass]
    [TestCategory("Numeric")]
    public sealed class AssertThatToleranceTests
    {
        // ============================================================
        // IsCloseTo (double) Tests
        // ============================================================

        [TestMethod]
        public void IsCloseTo_Double_WhenWithinTolerance_ShouldPass()
        {
            // Arrange
            var actual = 10.05;
            var expected = 10.0;
            var tolerance = 0.1;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that values within tolerance pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Double_WhenExactlyAtTolerance_ShouldPass()
        {
            // Arrange
            var actual = 10.1;
            var expected = 10.0;
            var tolerance = 0.1;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that values exactly at tolerance boundary pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Double_WhenEqualToExpected_ShouldPass()
        {
            // Arrange
            var actual = 42.0;
            var expected = 42.0;
            var tolerance = 0.01;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that equal values pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Double_WhenBelowToleranceRange_ShouldPass()
        {
            // Arrange
            var actual = 9.95;
            var expected = 10.0;
            var tolerance = 0.1;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that values below expected within tolerance pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Double_WhenOutsideTolerance_ShouldFail()
        {
            // Arrange
            var actual = 10.2;
            var expected = 10.0;
            var tolerance = 0.1;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsCloseTo(actual, expected, tolerance,
                                      because: "Testing that values outside tolerance fail",
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
        public void IsCloseTo_Double_WhenNegativeTolerance_ShouldThrowArgumentException()
        {
            // Arrange
            var actual = 10.0;
            var expected = 10.0;
            var tolerance = -0.1;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
            {
                Assert.That.IsCloseTo(actual, expected, tolerance,
                                      because: "Testing negative tolerance",
                                      fix: "Use positive tolerance");
            });

            Assert.IsTrue(exception.Message.Contains("Tolerance must be non-negative"));
        }

        [TestMethod]
        public void IsCloseTo_Double_WhenOutsideTolerance_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var temperature = 98.9;
            var expected = 98.6;
            var tolerance = 0.2;

            // Act
            try
            {
                Assert.That.IsCloseTo(temperature, expected, tolerance,
                                      because: "Body temperature should be within normal range",
                                      fix: "Check thermometer calibration or patient condition");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("TOLERANCE CHECK FAILED - VALUE OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Body temperature should be within normal range"));
                Assert.IsTrue(ex.Message.Contains("Check thermometer calibration or patient condition"));

                // Verify details are present
                Assert.IsTrue(ex.Message.Contains("temperature"));
                Assert.IsTrue(ex.Message.Contains("double"));
                Assert.IsTrue(ex.Message.Contains("98.9"));
                Assert.IsTrue(ex.Message.Contains("98.6"));
                Assert.IsTrue(ex.Message.Contains("±0.2"));
                Assert.IsTrue(ex.Message.Contains("Valid Range"));
                Assert.IsTrue(ex.Message.Contains("Exceeded By"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsCloseTo_Double_WithLargeValues_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var distance = 384400500.0; // Moon distance in meters (slightly off)
            var expected = 384400000.0;
            var tolerance = 100.0;

            // Act
            try
            {
                Assert.That.IsCloseTo(distance, expected, tolerance,
                                      because: "Measured distance to the moon should match expected value",
                                      fix: "Recalibrate the laser rangefinder");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify all sections are present
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));

                // Verify custom messages appear
                Assert.IsTrue(ex.Message.Contains("Measured distance to the moon should match expected value"));
                Assert.IsTrue(ex.Message.Contains("Recalibrate the laser rangefinder"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsCloseTo (decimal) Tests
        // ============================================================

        [TestMethod]
        public void IsCloseTo_Decimal_WhenWithinTolerance_ShouldPass()
        {
            // Arrange
            var actual = 99.99m;
            var expected = 100.00m;
            var tolerance = 0.05m;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that decimal values within tolerance pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Decimal_WhenExactlyAtTolerance_ShouldPass()
        {
            // Arrange
            var actual = 100.05m;
            var expected = 100.00m;
            var tolerance = 0.05m;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that decimal values exactly at tolerance boundary pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Decimal_WhenEqualToExpected_ShouldPass()
        {
            // Arrange
            var actual = 1234.5678m;
            var expected = 1234.5678m;
            var tolerance = 0.0001m;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that equal decimal values pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Decimal_WhenOutsideTolerance_ShouldFail()
        {
            // Arrange
            var actual = 100.10m;
            var expected = 100.00m;
            var tolerance = 0.05m;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsCloseTo(actual, expected, tolerance,
                                      because: "Testing that decimal values outside tolerance fail",
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
        public void IsCloseTo_Decimal_WhenNegativeTolerance_ShouldThrowArgumentException()
        {
            // Arrange
            var actual = 100.0m;
            var expected = 100.0m;
            var tolerance = -0.01m;

            // Act & Assert
            var testAction = () => Assert.That.IsCloseTo(actual, expected, tolerance,
                                                         because: "Testing negative tolerance",
                                                         fix: "Use positive tolerance");

            Assert.That.Throws<ArgumentException>(testAction,
                                                  because: "Negative tolerance should throw",
                                                  fix: "N/A");
        }

        [TestMethod]
        public void IsCloseTo_Decimal_WhenOutsideTolerance_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var price = 19.99m;
            var expected = 19.95m;
            var tolerance = 0.02m;

            // Act
            try
            {
                Assert.That.IsCloseTo(price, expected, tolerance,
                                      because: "Price should match catalog value within rounding tolerance",
                                      fix: "Verify pricing calculation includes all discounts");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("TOLERANCE CHECK FAILED - VALUE OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Price should match catalog value within rounding tolerance"));
                Assert.IsTrue(ex.Message.Contains("Verify pricing calculation includes all discounts"));

                // Verify details are present
                Assert.IsTrue(ex.Message.Contains("price"));
                Assert.IsTrue(ex.Message.Contains("decimal"));
                Assert.IsTrue(ex.Message.Contains("Valid Range"));
                Assert.IsTrue(ex.Message.Contains("Exceeded By"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsCloseTo_Decimal_WithHighPrecision_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var exchangeRate = 1.234568m;
            var expected = 1.234567m;
            var tolerance = 0.000000m;

            // Act
            try
            {
                Assert.That.IsCloseTo(exchangeRate, expected, tolerance,
                                      because: "Exchange rate must match exactly",
                                      fix: "Update exchange rate API endpoint");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify all sections are present
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));

                // Verify custom messages appear
                Assert.IsTrue(ex.Message.Contains("Exchange rate must match exactly"));
                Assert.IsTrue(ex.Message.Contains("Update exchange rate API endpoint"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsCloseTo (float) Tests
        // ============================================================

        [TestMethod]
        public void IsCloseTo_Float_WhenWithinTolerance_ShouldPass()
        {
            // Arrange
            var actual = 3.14f;
            var expected = 3.14159f;
            var tolerance = 0.01f;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that float values within tolerance pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Float_WhenExactlyAtTolerance_ShouldPass()
        {
            // Arrange
            var actual = 5.5f;
            var expected = 5.0f;
            var tolerance = 0.5f;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that float values exactly at tolerance boundary pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Float_WhenEqualToExpected_ShouldPass()
        {
            // Arrange
            var actual = 2.71828f;
            var expected = 2.71828f;
            var tolerance = 0.0001f;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that equal float values pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Float_WhenOutsideTolerance_ShouldFail()
        {
            // Arrange
            var actual = 1.5f;
            var expected = 1.0f;
            var tolerance = 0.4f;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsCloseTo(actual, expected, tolerance,
                                      because: "Testing that float values outside tolerance fail",
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
        public void IsCloseTo_Float_WhenNegativeTolerance_ShouldThrowArgumentException()
        {
            // Arrange
            var actual = 1.0f;
            var expected = 1.0f;
            var tolerance = -0.1f;

            // Act & Assert
            var testAction = () => Assert.That.IsCloseTo(actual, expected, tolerance,
                                                         because: "Testing negative tolerance",
                                                         fix: "Use positive tolerance");

            Assert.That.Throws<ArgumentException>(testAction,
                                                  because: "Negative tolerance should throw",
                                                  fix: "N/A");
        }

        [TestMethod]
        public void IsCloseTo_Float_WhenOutsideTolerance_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var velocity = 9.8f;
            var expected = 9.81f;
            var tolerance = 0.005f;

            // Act
            try
            {
                Assert.That.IsCloseTo(velocity, expected, tolerance,
                                      because: "Gravity acceleration should match standard value",
                                      fix: "Verify sensor calibration and environmental factors");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("TOLERANCE CHECK FAILED - VALUE OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Gravity acceleration should match standard value"));
                Assert.IsTrue(ex.Message.Contains("Verify sensor calibration and environmental factors"));

                // Verify details are present
                Assert.IsTrue(ex.Message.Contains("velocity"));
                Assert.IsTrue(ex.Message.Contains("float"));
                Assert.IsTrue(ex.Message.Contains("Valid Range"));
                Assert.IsTrue(ex.Message.Contains("Exceeded By"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsCloseTo_Float_WithVerySmallTolerance_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var ratio = 1.618034f; // Golden ratio (slightly off)
            var expected = 1.618033f;
            var tolerance = 0.0000001f;

            // Act
            try
            {
                Assert.That.IsCloseTo(ratio, expected, tolerance,
                                      because: "Golden ratio calculation should be precise",
                                      fix: "Increase calculation precision or adjust tolerance");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify all sections are present
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));

                // Verify custom messages appear
                Assert.IsTrue(ex.Message.Contains("Golden ratio calculation should be precise"));
                Assert.IsTrue(ex.Message.Contains("Increase calculation precision or adjust tolerance"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // Edge Case Tests
        // ============================================================

        [TestMethod]
        public void IsCloseTo_Double_WithZeroTolerance_ShouldRequireExactMatch()
        {
            // Arrange
            var actual = 1.0;
            var expected = 1.0;
            var tolerance = 0.0;

            // Act & Assert - Should NOT throw for exact match
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing zero tolerance with exact match",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Double_WithZeroTolerance_ShouldFailForAnyDifference()
        {
            // Arrange
            var actual = 1.0000001;
            var expected = 1.0;
            var tolerance = 0.0;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsCloseTo(actual, expected, tolerance,
                                      because: "Testing zero tolerance with difference",
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
        public void IsCloseTo_Double_WithNegativeValues_ShouldWork()
        {
            // Arrange
            var actual = -10.05;
            var expected = -10.0;
            var tolerance = 0.1;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing negative values within tolerance",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Decimal_WithVeryLargeValues_ShouldWork()
        {
            // Arrange
            var actual = 1000000000.01m;
            var expected = 1000000000.00m;
            var tolerance = 0.05m;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing very large decimal values within tolerance",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_Float_WithVerySmallValues_ShouldWork()
        {
            // Arrange
            var actual = 0.000001f;
            var expected = 0.0000009f;
            var tolerance = 0.0000002f;

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing very small float values within tolerance",
                                  fix: "N/A - this should pass");
        }
    }
}