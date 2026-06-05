using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Numeric
{
    /// <summary>
    /// Tests for Assert.That.IsInRange and Assert.That.IsOutOfRange
    /// </summary>
    [TestClass]
    [TestCategory("Numeric")]
    public sealed class AssertThatRangeTests
    {
        // ============================================================
        // IsInRange Tests - Integer
        // ============================================================

        [TestMethod]
        public void IsInRange_WhenValueIsWithinRange_ShouldPass()
        {
            // Arrange
            var value = 50;
            var min = 0;
            var max = 100;

            // Act & Assert - Should NOT throw
            Assert.That.IsInRange(value, min, max,
                                  because: "Value is within the valid range",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WhenValueIsAtMinBoundary_ShouldPass()
        {
            // Arrange
            var value = 0;
            var min = 0;
            var max = 100;

            // Act & Assert - Should NOT throw
            Assert.That.IsInRange(value, min, max,
                                  because: "Value equals minimum boundary (inclusive)",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WhenValueIsAtMaxBoundary_ShouldPass()
        {
            // Arrange
            var value = 100;
            var min = 0;
            var max = 100;

            // Act & Assert - Should NOT throw
            Assert.That.IsInRange(value, min, max,
                                  because: "Value equals maximum boundary (inclusive)",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WhenValueIsBelowRange_ShouldFail()
        {
            // Arrange
            var value = -10;
            var min = 0;
            var max = 100;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsInRange(value, min, max,
                                      because: "Value should be within bounds",
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
        public void IsInRange_WhenValueIsAboveRange_ShouldFail()
        {
            // Arrange
            var value = 150;
            var min = 0;
            var max = 100;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsInRange(value, min, max,
                                      because: "Value should be within bounds",
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
        public void IsInRange_WhenValueIsOutOfRange_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var age = 150;
            var minAge = 0;
            var maxAge = 120;

            // Act
            try
            {
                Assert.That.IsInRange(age, minAge, maxAge,
                                      because: "Age must be within realistic human lifespan",
                                      fix: "Validate input data or check calculation logic for age");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("RANGE CHECK - VALUE OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Age must be within realistic human lifespan"));
                Assert.IsTrue(ex.Message.Contains("Validate input data or check calculation logic for age"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("age"));

                // Verify range values appear
                Assert.IsTrue(ex.Message.Contains("0"));
                Assert.IsTrue(ex.Message.Contains("120"));
                Assert.IsTrue(ex.Message.Contains("150"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsInRange Tests - Double
        // ============================================================

        [TestMethod]
        public void IsInRange_WithDoubleValues_WhenValueIsWithinRange_ShouldPass()
        {
            // Arrange
            var temperature = 22.5;
            var min = -40.0;
            var max = 50.0;

            // Act & Assert - Should NOT throw
            Assert.That.IsInRange(temperature, min, max,
                                  because: "Temperature is within acceptable range",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WithDoubleValues_WhenValueIsOutOfRange_ShouldFail()
        {
            // Arrange
            var temperature = 75.0;
            var min = -40.0;
            var max = 50.0;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsInRange(temperature, min, max,
                                      because: "Temperature should be within safe operating range",
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
        public void IsInRange_WithDoubleValues_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var price = 1500.99;
            var minPrice = 0.01;
            var maxPrice = 999.99;

            // Act
            try
            {
                Assert.That.IsInRange(price, minPrice, maxPrice,
                                      because: "Price must be within the allowed product range",
                                      fix: "Check pricing calculation or apply discount logic");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("RANGE CHECK - VALUE OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Price must be within the allowed product range"));
                Assert.IsTrue(ex.Message.Contains("Check pricing calculation or apply discount logic"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsInRange Tests - DateTime
        // ============================================================

        [TestMethod]
        public void IsInRange_WithDateTimeValues_WhenValueIsWithinRange_ShouldPass()
        {
            // Arrange
            var eventDate = new DateTime(2025, 6, 15);
            var minDate = new DateTime(2025, 1, 1);
            var maxDate = new DateTime(2025, 12, 31);

            // Act & Assert - Should NOT throw
            Assert.That.IsInRange(eventDate, minDate, maxDate,
                                  because: "Event date is within the current year",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WithDateTimeValues_WhenValueIsOutOfRange_ShouldFail()
        {
            // Arrange
            var eventDate = new DateTime(2026, 6, 15);
            var minDate = new DateTime(2025, 1, 1);
            var maxDate = new DateTime(2025, 12, 31);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsInRange(eventDate, minDate, maxDate,
                                      because: "Event date should be in 2025",
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
        public void IsInRange_WithDateTimeValues_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var appointmentDate = new DateTime(2020, 3, 15);
            var minDate = new DateTime(2025, 1, 1);
            var maxDate = new DateTime(2025, 12, 31);

            // Act
            try
            {
                Assert.That.IsInRange(appointmentDate, minDate, maxDate,
                                      because: "Appointment must be scheduled within the current year",
                                      fix: "Ensure date validation rejects past dates before saving");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("DATETIME RANGE - VALUE OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Appointment must be scheduled within the current year"));
                Assert.IsTrue(ex.Message.Contains("Ensure date validation rejects past dates before saving"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsOutOfRange Tests - Integer
        // ============================================================

        [TestMethod]
        public void IsOutOfRange_WhenValueIsOutOfRange_ShouldPass()
        {
            // Arrange
            var value = 150;
            var min = 0;
            var max = 100;

            // Act & Assert - Should NOT throw
            Assert.That.IsOutOfRange(value, min, max,
                                     because: "Value is outside the excluded range",
                                     fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsOutOfRange_WhenValueIsBelowRange_ShouldPass()
        {
            // Arrange
            var value = -10;
            var min = 0;
            var max = 100;

            // Act & Assert - Should NOT throw
            Assert.That.IsOutOfRange(value, min, max,
                                     because: "Value is below the excluded range",
                                     fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsOutOfRange_WhenValueIsWithinRange_ShouldFail()
        {
            // Arrange
            var value = 50;
            var min = 0;
            var max = 100;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsOutOfRange(value, min, max,
                                         because: "Value should be outside the range",
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
        public void IsOutOfRange_WhenValueIsAtMinBoundary_ShouldFail()
        {
            // Arrange
            var value = 0;
            var min = 0;
            var max = 100;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsOutOfRange(value, min, max,
                                         because: "Value should be outside the range",
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
        public void IsOutOfRange_WhenValueIsAtMaxBoundary_ShouldFail()
        {
            // Arrange
            var value = 100;
            var min = 0;
            var max = 100;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsOutOfRange(value, min, max,
                                         because: "Value should be outside the range",
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
        public void IsOutOfRange_WhenValueIsInRange_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var port = 8080;
            var reservedMin = 8000;
            var reservedMax = 9000;

            // Act
            try
            {
                Assert.That.IsOutOfRange(port, reservedMin, reservedMax,
                                         because: "Port must not be in the reserved range",
                                         fix: "Select a port outside the reserved range [8000-9000]");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("RANGE CHECK - VALUE IN RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Port must not be in the reserved range"));
                Assert.IsTrue(ex.Message.Contains("Select a port outside the reserved range [8000-9000]"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("port"));

                // Verify range values appear
                Assert.IsTrue(ex.Message.Contains("8000"));
                Assert.IsTrue(ex.Message.Contains("9000"));
                Assert.IsTrue(ex.Message.Contains("8080"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsOutOfRange Tests - Double
        // ============================================================

        [TestMethod]
        public void IsOutOfRange_WithDoubleValues_WhenValueIsOutOfRange_ShouldPass()
        {
            // Arrange
            var value = 150.5;
            var min = 0.0;
            var max = 100.0;

            // Act & Assert - Should NOT throw
            Assert.That.IsOutOfRange(value, min, max,
                                     because: "Value is outside the excluded range",
                                     fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsOutOfRange_WithDoubleValues_WhenValueIsInRange_ShouldFail()
        {
            // Arrange
            var value = 50.5;
            var min = 0.0;
            var max = 100.0;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsOutOfRange(value, min, max,
                                         because: "Value should be outside the range",
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
        public void IsOutOfRange_WithDoubleValues_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var frequency = 50.0;
            var forbiddenMin = 45.0;
            var forbiddenMax = 55.0;

            // Act
            try
            {
                Assert.That.IsOutOfRange(frequency, forbiddenMin, forbiddenMax,
                                         because: "Frequency must avoid interference range [45-55 Hz]",
                                         fix: "Adjust frequency to be below 45 Hz or above 55 Hz");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("RANGE CHECK - VALUE IN RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Frequency must avoid interference range [45-55 Hz]"));
                Assert.IsTrue(ex.Message.Contains("Adjust frequency to be below 45 Hz or above 55 Hz"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsOutOfRange Tests - DateTime
        // ============================================================

        [TestMethod]
        public void IsOutOfRange_WithDateTimeValues_WhenValueIsOutOfRange_ShouldPass()
        {
            // Arrange
            var maintenanceDate = new DateTime(2025, 3, 15);
            var blackoutStart = new DateTime(2025, 6, 1);
            var blackoutEnd = new DateTime(2025, 8, 31);

            // Act & Assert - Should NOT throw
            Assert.That.IsOutOfRange(maintenanceDate, blackoutStart, blackoutEnd,
                                     because: "Maintenance should be scheduled outside blackout period",
                                     fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsOutOfRange_WithDateTimeValues_WhenValueIsInRange_ShouldFail()
        {
            // Arrange
            var maintenanceDate = new DateTime(2025, 7, 15);
            var blackoutStart = new DateTime(2025, 6, 1);
            var blackoutEnd = new DateTime(2025, 8, 31);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsOutOfRange(maintenanceDate, blackoutStart, blackoutEnd,
                                         because: "Maintenance should avoid blackout period",
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
        public void IsOutOfRange_WithDateTimeValues_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var deploymentDate = new DateTime(2025, 12, 24);
            var freezeStart = new DateTime(2025, 12, 20);
            var freezeEnd = new DateTime(2026, 1, 5);

            // Act
            try
            {
                Assert.That.IsOutOfRange(deploymentDate, freezeStart, freezeEnd,
                                         because: "Deployments are forbidden during holiday freeze period",
                                         fix: "Reschedule deployment to before Dec 20 or after Jan 5");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("RANGE CHECK - VALUE IN RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Deployments are forbidden during holiday freeze period"));
                Assert.IsTrue(ex.Message.Contains("Reschedule deployment to before Dec 20 or after Jan 5"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsOutOfRange Tests - String (alphabetical comparison)
        // ============================================================

        [TestMethod]
        public void IsOutOfRange_WithStringValues_WhenValueIsOutOfRange_ShouldPass()
        {
            // Arrange
            var value = "Zulu";
            var min = "Alpha";
            var max = "Charlie";

            // Act & Assert - Should NOT throw
            Assert.That.IsOutOfRange(value, min, max,
                                     because: "String is alphabetically outside the range",
                                     fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsOutOfRange_WithStringValues_WhenValueIsInRange_ShouldFail()
        {
            // Arrange
            var value = "Bravo";
            var min = "Alpha";
            var max = "Charlie";
            var threw = false;

            // Act
            try
            {
                Assert.That.IsOutOfRange(value, min, max,
                                         because: "String should be outside the range",
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
        public void IsOutOfRange_WithStringValues_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var username = "Mike";
            var reservedMin = "Admin";
            var reservedMax = "System";

            // Act
            try
            {
                Assert.That.IsOutOfRange(username, reservedMin, reservedMax,
                                         because: "Username must not be in reserved alphabetical range",
                                         fix: "Choose a username starting with T-Z or ending before Admin");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("RANGE CHECK - VALUE IN RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Username must not be in reserved alphabetical range"));
                Assert.IsTrue(ex.Message.Contains("Choose a username starting with T-Z or ending before Admin"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }
    }
}