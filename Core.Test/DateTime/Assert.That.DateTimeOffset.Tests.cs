using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.DateTimeOffsetAssertions
{
    /// <summary>
    /// Tests for Assert.That DateTimeOffset assertions: IsAfter, IsBefore, IsInRange, IsCloseTo, HasOffset, IsUtc, IsLocal
    /// </summary>
    [TestClass]
    [TestCategory("DateTimeOffset")]
    public sealed class AssertThatDateTimeOffsetTests
    {
        // ============================================================
        // IsAfter Tests
        // ============================================================

        [TestMethod]
        public void IsAfter_WhenActualIsAfterExpected_ShouldPass()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 10,
                                            12, 0, 0,
                                            TimeSpan.Zero);

            var expected = new DateTimeOffset(2026, 6, 5,
                                              12, 0, 0,
                                              TimeSpan.Zero);

            // Act & Assert - Should NOT throw
            Assert.That.IsAfter(actual, expected,
                                because: "Testing that later dates pass the IsAfter check",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsAfter_WhenActualIsBeforeExpected_ShouldFail()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 1,
                                            12, 0, 0,
                                            TimeSpan.Zero);

            var expected = new DateTimeOffset(2026, 6, 5,
                                              12, 0, 0,
                                              TimeSpan.Zero);

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
        public void IsAfter_WithDifferentTimezones_ShouldCompareCorrectly()
        {
            // Arrange - Same absolute time, different offsets
            var actual = new DateTimeOffset(2026, 6, 5,
                                            14, 0, 0,
                                            TimeSpan.FromHours(2)); // 12:00 UTC

            var expected = new DateTimeOffset(2026, 6, 5,
                                              12, 0, 0,
                                              TimeSpan.Zero); // 12:00 UTC

            var threw = false;

            // Act - These are the same absolute moment, so IsAfter should fail
            try
            {
                Assert.That.IsAfter(actual, expected,
                                    because: "Testing timezone-aware comparison",
                                    fix: "This is expected to fail as they represent the same moment");
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
            var eventTime = new DateTimeOffset(2026, 6, 1,
                                               10, 30, 0,
                                               TimeSpan.FromHours(1));

            var deadline = new DateTimeOffset(2026, 6, 5,
                                              17, 0, 0,
                                              TimeSpan.Zero);

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
                Assert.IsTrue(ex.Message.Contains("DATETIMEOFFSET COMPARISON - EXPECTED AFTER"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Event must occur after the project deadline"));
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
            var actual = new DateTimeOffset(2026, 6, 1,
                                            12, 0, 0,
                                            TimeSpan.Zero);

            var expected = new DateTimeOffset(2026, 6, 5,
                                              12, 0, 0,
                                              TimeSpan.Zero);

            // Act & Assert - Should NOT throw
            Assert.That.IsBefore(actual, expected,
                                 because: "Testing that earlier dates pass the IsBefore check",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsBefore_WhenActualIsAfterExpected_ShouldFail()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 10,
                                            12, 0, 0,
                                            TimeSpan.Zero);

            var expected = new DateTimeOffset(2026, 6, 5,
                                              12, 0, 0,
                                              TimeSpan.Zero);

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

        // ============================================================
        // IsInRange Tests
        // ============================================================

        [TestMethod]
        public void IsInRange_WhenActualIsWithinRange_ShouldPass()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 5,
                                            12, 0, 0,
                                            TimeSpan.Zero);

            var start = new DateTimeOffset(2026, 6, 1,
                                           0, 0, 0,
                                           TimeSpan.Zero);

            var end = new DateTimeOffset(2026, 6, 30,
                                         23, 59, 59,
                                         TimeSpan.Zero);

            // Act & Assert - Should NOT throw
            Assert.That.IsInRange(actual, start, end,
                                  because: "Testing that dates within range pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsInRange_WhenActualIsBeforeRange_ShouldFail()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 5, 31,
                                            23, 59, 59,
                                            TimeSpan.Zero);

            var start = new DateTimeOffset(2026, 6, 1,
                                           0, 0, 0,
                                           TimeSpan.Zero);

            var end = new DateTimeOffset(2026, 6, 30,
                                         23, 59, 59,
                                         TimeSpan.Zero);

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

        // ============================================================
        // IsCloseTo Tests
        // ============================================================

        [TestMethod]
        public void IsCloseTo_WhenActualIsWithinTolerance_ShouldPass()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 5,
                                            12, 0, 5,
                                            TimeSpan.Zero);

            var expected = new DateTimeOffset(2026, 6, 5,
                                              12, 0, 0,
                                              TimeSpan.Zero);

            var tolerance = TimeSpan.FromSeconds(10);

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing that dates within tolerance pass",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsCloseTo_WhenActualExceedsTolerance_ShouldFail()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 5,
                                            12, 0, 15,
                                            TimeSpan.Zero);

            var expected = new DateTimeOffset(2026, 6, 5,
                                              12, 0, 0,
                                              TimeSpan.Zero);

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

        // ============================================================
        // HasOffset Tests
        // ============================================================

        [TestMethod]
        public void HasOffset_WhenOffsetMatches_ShouldPass()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 5,
                                            12, 0, 0,
                                            TimeSpan.FromHours(2));

            var expectedOffset = TimeSpan.FromHours(2);

            // Act & Assert - Should NOT throw
            Assert.That.HasOffset(actual, expectedOffset,
                                  because: "Testing that matching offset passes",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasOffset_WhenOffsetDoesNotMatch_ShouldFail()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 5,
                                            12, 0, 0,
                                            TimeSpan.FromHours(1));

            var expectedOffset = TimeSpan.FromHours(2);
            var threw = false;

            // Act
            try
            {
                Assert.That.HasOffset(actual, expectedOffset,
                                      because: "Testing that mismatched offset fails",
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
        public void HasOffset_WhenFails_ShouldHaveBeautifulOutput()
        {
            // Arrange
            var timestamp = new DateTimeOffset(2026, 6, 5,
                                               12, 0, 0,
                                               TimeSpan.FromHours(1));

            var expectedOffset = TimeSpan.FromHours(2);

            // Act
            try
            {
                Assert.That.HasOffset(timestamp, expectedOffset,
                                      because: "API must return timestamps in Central European Time (UTC+2)",
                                      fix: "Configure the API to return timestamps with UTC+2 offset");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("DATETIMEOFFSET OFFSET"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("API must return timestamps in Central European Time (UTC+2)"));
                Assert.IsTrue(ex.Message.Contains("timestamp"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsUtc Tests
        // ============================================================

        [TestMethod]
        public void IsUtc_WhenOffsetIsZero_ShouldPass()
        {
            // Arrange
            var actual = DateTimeOffset.UtcNow;

            // Act & Assert - Should NOT throw
            Assert.That.IsUtc(actual,
                              because: "Testing that UTC DateTimeOffset passes IsUtc check",
                              fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsUtc_WhenOffsetIsNotZero_ShouldFail()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 5,
                                            12, 0, 0,
                                            TimeSpan.FromHours(2));

            var threw = false;

            // Act
            try
            {
                Assert.That.IsUtc(actual,
                                  because: "Testing that non-UTC DateTimeOffset fails IsUtc check",
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
        public void IsUtc_WhenFails_ShouldHaveBeautifulOutput()
        {
            // Arrange - Use explicit non-UTC offset (works on all systems)
            var timestamp = new DateTimeOffset(2026, 6, 5,
                                               12, 0, 0,
                                               TimeSpan.FromHours(2));

            // Act
            try
            {
                Assert.That.IsUtc(timestamp,
                                  because: "Database timestamps must be stored in UTC for consistency",
                                  fix: "Use DateTimeOffset.UtcNow or convert to UTC with .ToUniversalTime()");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("DATETIMEOFFSET OFFSET"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Database timestamps must be stored in UTC for consistency"));
                Assert.IsTrue(ex.Message.Contains("timestamp"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsLocal Tests
        // ============================================================

        [TestMethod]
        public void IsLocal_WhenOffsetMatchesLocalTimezone_ShouldPass()
        {
            // Arrange
            var actual = DateTimeOffset.Now;

            // Act & Assert - Should NOT throw
            Assert.That.IsLocal(actual,
                                because: "Testing that local DateTimeOffset passes IsLocal check",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsLocal_WhenOffsetIsDifferent_ShouldFail()
        {
            // Arrange - Create an offset that's definitely not the local offset
            var localOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);

            var differentOffset = localOffset == TimeSpan.FromHours(5)
                                      ? TimeSpan.FromHours(10)
                                      : TimeSpan.FromHours(5);

            var actual = new DateTimeOffset(2026, 6, 5,
                                            12, 0, 0,
                                            differentOffset);

            var threw = false;

            // Act
            try
            {
                Assert.That.IsLocal(actual,
                                    because: "Testing that non-local DateTimeOffset fails IsLocal check",
                                    fix: "This is expected to fail");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        // ============================================================
        // Edge Case Tests
        // ============================================================

        [TestMethod]
        public void IsCloseTo_WithMillisecondPrecision_ShouldWork()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 5,
                                            12, 0, 0,
                                            500,
                                            TimeSpan.Zero);

            var expected = new DateTimeOffset(2026, 6, 5,
                                              12, 0, 0,
                                              0,
                                              TimeSpan.Zero);

            var tolerance = TimeSpan.FromMilliseconds(600);

            // Act & Assert - Should NOT throw
            Assert.That.IsCloseTo(actual, expected, tolerance,
                                  because: "Testing millisecond-level precision",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasOffset_WithNegativeOffset_ShouldWork()
        {
            // Arrange
            var actual = new DateTimeOffset(2026, 6, 5,
                                            12, 0, 0,
                                            TimeSpan.FromHours(-5)); // EST

            var expectedOffset = TimeSpan.FromHours(-5);

            // Act & Assert - Should NOT throw
            Assert.That.HasOffset(actual, expectedOffset,
                                  because: "Testing negative offset (western hemisphere)",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsAfter_WithMixedTimezones_ShouldCompareAbsoluteTime()
        {
            // Arrange - Different local times but later absolute time
            var actual = new DateTimeOffset(2026, 6, 5,
                                            10, 0, 0,
                                            TimeSpan.FromHours(-5)); // 15:00 UTC

            var expected = new DateTimeOffset(2026, 6, 5,
                                              12, 0, 0,
                                              TimeSpan.Zero); // 12:00 UTC

            // Act & Assert - Should NOT throw (actual is 3 hours after expected in absolute time)
            Assert.That.IsAfter(actual, expected,
                                because: "Testing that timezone-aware comparison works correctly",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsLocal_ShouldWorkRegardlessOfSystemTimezone()
        {
            // Arrange - Test works on both UTC and non-UTC systems
            var localOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now);

            var localTime = new DateTimeOffset(2026, 6, 5,
                                               12, 0, 0,
                                               localOffset);

            // Act & Assert - Should NOT throw regardless of system timezone
            Assert.That.IsLocal(localTime,
                                because: "Testing timezone-agnostic behavior",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsUtc_ShouldWorkOnUtcAndNonUtcSystems()
        {
            // Arrange
            var utcTime = DateTimeOffset.UtcNow;

            // Act & Assert - Should NOT throw on any system (UTC or non-UTC)
            Assert.That.IsUtc(utcTime,
                              because: "UTC validation should work on any system timezone",
                              fix: "N/A - this should pass");
        }
    }
}