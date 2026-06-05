using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.DateTimeAssertions
{
    /// <summary>
    /// Tests for Assert.That DateTime assertions: IsAfter, IsBefore, IsInRange, IsCloseTo
    /// </summary>
    [TestClass]
    [TestCategory("DateTime")]
    public sealed class AssertThatDateTimeTests
    {
        // ============================================================
        // IsAfter Tests
        // ============================================================

        [TestMethod]
        public void IsAfter_WhenActualIsAfterExpected_ShouldPass()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 10, 12, 0, 0);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);

            // Act & Assert - Should NOT throw
            Assert.That.IsAfter(actual, expected,
                               because: "Testing that later dates pass the IsAfter check",
                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsAfter_WhenActualIsBeforeExpected_ShouldFail()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 1, 12, 0, 0);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsAfter(actual, expected,
                                   because: "Testing that earlier dates fail the IsAfter check",
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
        public void IsAfter_WhenActualIsEqualToExpected_ShouldFail()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsAfter(actual, expected,
                                   because: "Testing that equal dates fail the IsAfter check",
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
        public void IsAfter_WhenFails_ShouldHaveBeautifulOutput()
        {
            // Arrange
            var eventTime = new System.DateTime(2026, 6, 1, 10, 30, 0);
            var deadline = new System.DateTime(2026, 6, 5, 17, 0, 0);

            // Act
            try
            {
                Assert.That.IsAfter(eventTime, deadline,
                                   because: "Event must occur after the project deadline",
                                   fix: "Update the event scheduling logic to ensure events are created after the deadline");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("DATETIME COMPARISON - EXPECTED AFTER"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Event must occur after the project deadline"));
                Assert.IsTrue(ex.Message.Contains("Update the event scheduling logic to ensure events are created after the deadline"));
                Assert.IsTrue(ex.Message.Contains("eventTime"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsBefore Tests
        // ============================================================

        [TestMethod]
        public void IsBefore_WhenActualIsBeforeExpected_ShouldPass()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 1, 12, 0, 0);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);

            // Act & Assert - Should NOT throw
            Assert.That.IsBefore(actual, expected,
                                because: "Testing that earlier dates pass the IsBefore check",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsBefore_WhenActualIsAfterExpected_ShouldFail()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 10, 12, 0, 0);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsBefore(actual, expected,
                                    because: "Testing that later dates fail the IsBefore check",
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
        public void IsBefore_WhenActualIsEqualToExpected_ShouldFail()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsBefore(actual, expected,
                                    because: "Testing that equal dates fail the IsBefore check",
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
        public void IsBefore_WhenFails_ShouldHaveBeautifulOutput()
        {
            // Arrange
            var createdAt = new System.DateTime(2026, 6, 10, 14, 30, 0);
            var cutoffDate = new System.DateTime(2026, 6, 5, 23, 59, 59);

            // Act
            try
            {
                Assert.That.IsBefore(createdAt, cutoffDate,
                                    because: "Record must be created before the cutoff date for the report",
                                    fix: "Check the creation timestamp logic and ensure records are backdated correctly");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("DATETIME COMPARISON - EXPECTED BEFORE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Record must be created before the cutoff date for the report"));
                Assert.IsTrue(ex.Message.Contains("Check the creation timestamp logic and ensure records are backdated correctly"));
                Assert.IsTrue(ex.Message.Contains("createdAt"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsInRange Tests
        // ============================================================

        [TestMethod]
        public void IsInRange_WhenActualIsWithinRange_ShouldPass()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var start = new System.DateTime(2026, 6, 1, 0, 0, 0);
            var end = new System.DateTime(2026, 6, 30, 23, 59, 59);

            // Act & Assert - Should NOT throw
            Assert.That.IsInRange(actual, start, end,
                                 because: "Testing that dates within range pass",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WhenActualIsAtRangeStart_ShouldPass()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 1, 0, 0, 0);
            var start = new System.DateTime(2026, 6, 1, 0, 0, 0);
            var end = new System.DateTime(2026, 6, 30, 23, 59, 59);

            // Act & Assert - Should NOT throw (inclusive boundary)
            Assert.That.IsInRange(actual, start, end,
                                 because: "Testing that dates at range start pass (inclusive)",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WhenActualIsAtRangeEnd_ShouldPass()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 30, 23, 59, 59);
            var start = new System.DateTime(2026, 6, 1, 0, 0, 0);
            var end = new System.DateTime(2026, 6, 30, 23, 59, 59);

            // Act & Assert - Should NOT throw (inclusive boundary)
            Assert.That.IsInRange(actual, start, end,
                                 because: "Testing that dates at range end pass (inclusive)",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WhenActualIsBeforeRange_ShouldFail()
        {
            // Arrange
            var actual = new System.DateTime(2026, 5, 31, 23, 59, 59);
            var start = new System.DateTime(2026, 6, 1, 0, 0, 0);
            var end = new System.DateTime(2026, 6, 30, 23, 59, 59);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsInRange(actual, start, end,
                                     because: "Testing that dates before range fail",
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
        public void IsInRange_WhenActualIsAfterRange_ShouldFail()
        {
            // Arrange
            var actual = new System.DateTime(2026, 7, 1, 0, 0, 1);
            var start = new System.DateTime(2026, 6, 1, 0, 0, 0);
            var end = new System.DateTime(2026, 6, 30, 23, 59, 59);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsInRange(actual, start, end,
                                     because: "Testing that dates after range fail",
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
        public void IsInRange_WhenFails_ShouldHaveBeautifulOutput()
        {
            // Arrange
            var appointmentDate = new System.DateTime(2026, 7, 15, 10, 0, 0);
            var periodStart = new System.DateTime(2026, 6, 1, 0, 0, 0);
            var periodEnd = new System.DateTime(2026, 6, 30, 23, 59, 59);

            // Act
            try
            {
                Assert.That.IsInRange(appointmentDate, periodStart, periodEnd,
                                     because: "Appointment must be scheduled within the current billing period",
                                     fix: "Adjust the appointment date to fall within the valid date range or extend the billing period");

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
                Assert.IsTrue(ex.Message.Contains("Appointment must be scheduled within the current billing period"));
                Assert.IsTrue(ex.Message.Contains("Adjust the appointment date to fall within the valid date range or extend the billing period"));
                Assert.IsTrue(ex.Message.Contains("appointmentDate"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsCloseTo Tests
        // ============================================================

        [TestMethod]
        public void IsCloseTo_WhenActualIsWithinTolerance_ShouldPass()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 12, 0, 5);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var tolerance = TimeSpan.FromSeconds(10);

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                 because: "Testing that dates within tolerance pass",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_WhenActualIsExactlyAtTolerance_ShouldPass()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 12, 0, 10);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var tolerance = TimeSpan.FromSeconds(10);

            // Act & Assert - Should NOT throw (boundary case)
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                 because: "Testing that dates exactly at tolerance boundary pass",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_WhenActualIsEqualToExpected_ShouldPass()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var tolerance = TimeSpan.FromSeconds(1);

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                 because: "Testing that equal dates pass",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_WhenActualExceedsTolerance_ShouldFail()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 12, 0, 15);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var tolerance = TimeSpan.FromSeconds(10);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsCloseTo(actual, expected, tolerance,
                                     because: "Testing that dates outside tolerance fail",
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
        public void IsCloseTo_WhenActualIsBeforeExpectedAndExceedsTolerance_ShouldFail()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 11, 59, 45);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var tolerance = TimeSpan.FromSeconds(10);
            var threw = false;

            // Act
            try
            {
                Assert.That.IsCloseTo(actual, expected, tolerance,
                                     because: "Testing that dates before expected and outside tolerance fail",
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
        public void IsCloseTo_WhenFails_ShouldHaveBeautifulOutput()
        {
            // Arrange
            var actualTimestamp = new System.DateTime(2026, 6, 5, 12, 5, 30);
            var expectedTimestamp = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var tolerance = TimeSpan.FromMinutes(2);

            // Act
            try
            {
                Assert.That.IsCloseTo(actualTimestamp, expectedTimestamp, tolerance,
                                     because: "API response timestamp should be within 2 minutes of the request time for cache validation",
                                     fix: "Review the caching mechanism and reduce the time drift between request and response");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("DATETIME TOLERANCE - EXCEEDED THRESHOLD"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("API response timestamp should be within 2 minutes of the request time for cache validation"));
                Assert.IsTrue(ex.Message.Contains("Review the caching mechanism and reduce the time drift between request and response"));
                Assert.IsTrue(ex.Message.Contains("actualTimestamp"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // Edge Case Tests
        // ============================================================

        [TestMethod]
        public void IsCloseTo_WithMillisecondPrecision_ShouldWork()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 12, 0, 0, 500);
            var expected = new System.DateTime(2026, 6, 5, 12, 0, 0, 0);
            var tolerance = TimeSpan.FromMilliseconds(600);

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                 because: "Testing millisecond-level precision",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WithSingleDayRange_ShouldWork()
        {
            // Arrange
            var actual = new System.DateTime(2026, 6, 5, 12, 0, 0);
            var start = new System.DateTime(2026, 6, 5, 0, 0, 0);
            var end = new System.DateTime(2026, 6, 5, 23, 59, 59);

            // Act & Assert - Should NOT throw
            Assert.That.IsInRange(actual, start, end,
                                 because: "Testing single-day range",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsAfter_WithUtcDates_ShouldWork()
        {
            // Arrange
            var actual = System.DateTime.UtcNow.AddHours(1);
            var expected = System.DateTime.UtcNow;

            // Act & Assert - Should NOT throw
            Assert.That.IsAfter(actual, expected,
                               because: "Testing UTC date comparison",
                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsBefore_WithLargeDifference_ShouldWork()
        {
            // Arrange
            var actual = new System.DateTime(2020, 1, 1, 0, 0, 0);
            var expected = new System.DateTime(2026, 12, 31, 23, 59, 59);

            // Act & Assert - Should NOT throw
            Assert.That.IsBefore(actual, expected,
                                because: "Testing large time difference",
                                fix: "N/A - this should pass");
        }
    }
}
