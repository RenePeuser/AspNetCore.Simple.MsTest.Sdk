using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.ExceptionAssertions
{
    /// <summary>
    /// Tests for Assert.That.DoesNotThrow and Assert.That.DoesNotThrowAsync
    /// </summary>
    [TestClass]
    [TestCategory("Exception")]
    public sealed class AssertThatDoesNotThrowTests
    {
        // ============================================================
        // DoesNotThrow Tests
        // ============================================================

        [TestMethod]
        public void DoesNotThrow_WhenActionDoesNotThrow_ShouldPass()
        {
            // Arrange
            var counter = 0;
            Action action = () => counter++;

            // Act & Assert - Should NOT throw
            Assert.That.DoesNotThrow(action,
                                     because: "Simple increment operations should not throw",
                                     fix: "N/A - this should pass");

            // Verify action was executed
            Assert.AreEqual(1, counter, "Action should have been executed");
        }

        [TestMethod]
        public void DoesNotThrow_WhenActionThrowsException_ShouldFail()
        {
            // Arrange
            Action action = () => throw new InvalidOperationException("Test exception");
            var threw = false;

            // Act
            try
            {
                Assert.That.DoesNotThrow(action,
                                         because: "Testing that throwing actions fail",
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
        public void DoesNotThrow_WhenActionThrowsException_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var calculator = new TestCalculator();
            Action action = () => calculator.Divide(10, 0);

            // Act
            try
            {
                Assert.That.DoesNotThrow(action,
                                         because: "Division operation should handle zero divisor gracefully",
                                         fix: "Add validation to check for zero divisor before performing division");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EXCEPTION THROWN - EXPECTED NO EXCEPTION"),
                              "Should contain header");

                Assert.IsTrue(ex.Message.Contains("📦 Test Information"),
                              "Should contain Test Information section");

                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"),
                              "Should contain Problem section");

                Assert.IsTrue(ex.Message.Contains("📊 Details"),
                              "Should contain Details section");

                Assert.IsTrue(ex.Message.Contains("💭 Context"),
                              "Should contain Context section");

                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"),
                              "Should contain Suggested Fix section");

                // Verify custom because/fix messages appear in output
                Assert.IsTrue(ex.Message.Contains("Division operation should handle zero divisor gracefully"),
                              "Should contain because message");

                Assert.IsTrue(ex.Message.Contains("Add validation to check for zero divisor before performing division"),
                              "Should contain fix message");

                // Verify exception details
                Assert.IsTrue(ex.Message.Contains("DivideByZeroException"),
                              "Should contain exception type");

                Assert.IsTrue(ex.Message.Contains("Action"),
                              "Should contain action name");

                // Print the beautiful output to console
                Console.WriteLine("=== DoesNotThrow Beautiful Output ===");
                Console.WriteLine(ex.Message);
                Console.WriteLine("=====================================");
            }
        }

        [TestMethod]
        public void DoesNotThrow_WhenActionWithComplexException_ShouldShowInnerException()
        {
            // Arrange
            Action action = () =>
            {
                try
                {
                    throw new ArgumentException("Inner error");
                }
                catch (Exception inner)
                {
                    throw new InvalidOperationException("Outer error", inner);
                }
            };

            // Act
            try
            {
                Assert.That.DoesNotThrow(action,
                                         because: "Complex operation should not throw nested exceptions",
                                         fix: "Review exception handling chain and fix root cause");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify inner exception is shown
                Assert.IsTrue(ex.Message.Contains("Inner Exception"),
                              "Should show inner exception label");

                Assert.IsTrue(ex.Message.Contains("ArgumentException"),
                              "Should show inner exception type");

                Assert.IsTrue(ex.Message.Contains("Inner error"),
                              "Should show inner exception message");

                // Print to console
                Console.WriteLine("=== DoesNotThrow with Inner Exception ===");
                Console.WriteLine(ex.Message);
                Console.WriteLine("=========================================");
            }
        }

        [TestMethod]
        public void DoesNotThrow_WhenActionIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Action? action = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                                                     Assert.That.DoesNotThrow(action,
                                                                              because: "Testing null action",
                                                                              fix: "Provide a valid action"));
        }

        [TestMethod]
        public void DoesNotThrow_WithVoidMethod_ShouldPass()
        {
            // Arrange
            var list = new List<string>();
            Action action = () => list.Add("item");

            // Act & Assert - Should NOT throw
            Assert.That.DoesNotThrow(action,
                                     because: "Adding to list should not throw",
                                     fix: "Check list initialization");

            Assert.AreEqual(1, list.Count, "Item should have been added");
        }

        // ============================================================
        // DoesNotThrowAsync Tests
        // ============================================================

        [TestMethod]
        public async Task DoesNotThrowAsync_WhenActionDoesNotThrow_ShouldPass()
        {
            // Arrange
            var counter = 0;

            Func<Task> action = async () =>
            {
                await Task.Delay(1);
                counter++;
            };

            // Act & Assert - Should NOT throw
            await Assert.That.DoesNotThrowAsync(action,
                                                because: "Async increment should not throw",
                                                fix: "N/A - this should pass");

            // Verify action was executed
            Assert.AreEqual(1, counter, "Async action should have been executed");
        }

        [TestMethod]
        public async Task DoesNotThrowAsync_WhenActionThrowsException_ShouldFail()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                await Task.Delay(1);

                throw new InvalidOperationException("Async test exception");
            };

            var threw = false;

            // Act
            try
            {
                await Assert.That.DoesNotThrowAsync(action,
                                                    because: "Testing that async throwing actions fail",
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
        public async Task DoesNotThrowAsync_WhenActionThrowsException_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var service = new TestAsyncService();
            Func<Task> action = async () => await service.ProcessDataAsync(null!);

            // Act
            try
            {
                await Assert.That.DoesNotThrowAsync(action,
                                                    because: "ProcessDataAsync should validate input before processing",
                                                    fix: "Add null check at the beginning of ProcessDataAsync method");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EXCEPTION THROWN - EXPECTED NO EXCEPTION"),
                              "Should contain header");

                Assert.IsTrue(ex.Message.Contains("📦 Test Information"),
                              "Should contain Test Information section");

                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"),
                              "Should contain Problem section");

                Assert.IsTrue(ex.Message.Contains("📊 Details"),
                              "Should contain Details section");

                Assert.IsTrue(ex.Message.Contains("💭 Context"),
                              "Should contain Context section");

                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"),
                              "Should contain Suggested Fix section");

                // Verify custom because/fix messages appear in output
                Assert.IsTrue(ex.Message.Contains("ProcessDataAsync should validate input before processing"),
                              "Should contain because message");

                Assert.IsTrue(ex.Message.Contains("Add null check at the beginning of ProcessDataAsync method"),
                              "Should contain fix message");

                // Verify exception details
                Assert.IsTrue(ex.Message.Contains("ArgumentNullException"),
                              "Should contain exception type");

                Assert.IsTrue(ex.Message.Contains("Action"),
                              "Should contain action name");

                // Print the beautiful output to console
                Console.WriteLine("=== DoesNotThrowAsync Beautiful Output ===");
                Console.WriteLine(ex.Message);
                Console.WriteLine("==========================================");
            }
        }

        [TestMethod]
        public async Task DoesNotThrowAsync_WhenActionWithTaskCanceledException_ShouldFail()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                var cts = new CancellationTokenSource();
                cts.Cancel();
                await Task.Delay(1000, cts.Token);
            };

            // Act
            try
            {
                await Assert.That.DoesNotThrowAsync(action,
                                                    because: "Operation should complete without cancellation",
                                                    fix: "Ensure cancellation token is not cancelled during operation");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify TaskCanceledException is shown
                Assert.IsTrue(ex.Message.Contains("TaskCanceledException") ||
                              ex.Message.Contains("OperationCanceledException"),
                              "Should show cancellation exception type");

                // Print to console
                Console.WriteLine("=== DoesNotThrowAsync with Cancellation ===");
                Console.WriteLine(ex.Message);
                Console.WriteLine("===========================================");
            }
        }

        [TestMethod]
        public async Task DoesNotThrowAsync_WhenActionIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            Func<Task>? action = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
                                                                await Assert.That.DoesNotThrowAsync(action,
                                                                                                    because: "Testing null action",
                                                                                                    fix: "Provide a valid action"));
        }

        [TestMethod]
        public async Task DoesNotThrowAsync_WithHttpClientOperation_ShouldPass()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                await Task.Delay(10); // Simulate async HTTP call
                var result = "OK";
                Assert.AreEqual("OK", result);
            };

            // Act & Assert - Should NOT throw
            await Assert.That.DoesNotThrowAsync(action,
                                                because: "HTTP client operation should succeed",
                                                fix: "Check network connectivity and endpoint availability");
        }

        [TestMethod]
        public async Task DoesNotThrowAsync_WithInnerException_ShouldShowFullExceptionChain()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                await Task.Delay(1);

                try
                {
                    throw new FormatException("Invalid format detected");
                }
                catch (Exception inner)
                {
                    throw new ApplicationException("Failed to process data", inner);
                }
            };

            // Act
            try
            {
                await Assert.That.DoesNotThrowAsync(action,
                                                    because: "Data processing should handle format errors",
                                                    fix: "Add format validation before processing");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify both outer and inner exceptions are shown
                Assert.IsTrue(ex.Message.Contains("ApplicationException"),
                              "Should show outer exception type");

                Assert.IsTrue(ex.Message.Contains("Inner Exception"),
                              "Should show inner exception section");

                Assert.IsTrue(ex.Message.Contains("FormatException"),
                              "Should show inner exception type");

                Assert.IsTrue(ex.Message.Contains("Invalid format detected"),
                              "Should show inner exception message");

                // Print to console
                Console.WriteLine("=== DoesNotThrowAsync with Inner Exception ===");
                Console.WriteLine(ex.Message);
                Console.WriteLine("==============================================");
            }
        }

        [TestMethod]
        public async Task DoesNotThrowAsync_WithAggregateException_ShouldShowExceptionDetails()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                var tasks = new[] { Task.Run(() => throw new InvalidOperationException("Task 1 failed")), Task.Run(() => throw new ArgumentException("Task 2 failed")) };

                await Task.WhenAll(tasks);
            };

            // Act
            try
            {
                await Assert.That.DoesNotThrowAsync(action,
                                                    because: "Parallel tasks should complete successfully",
                                                    fix: "Handle exceptions in parallel tasks individually");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify exception type is shown (might be AggregateException or first inner)
                Assert.IsTrue(ex.Message.Contains("Exception Type"),
                              "Should show exception type label");

                // Print to console
                Console.WriteLine("=== DoesNotThrowAsync with Aggregate Exception ===");
                Console.WriteLine(ex.Message);
                Console.WriteLine("==================================================");
            }
        }

        // ============================================================
        // Test Helper Classes
        // ============================================================

        private sealed class TestCalculator
        {
            public int Divide(int numerator,
                              int denominator)
            {
                return numerator / denominator; // Will throw DivideByZeroException if denominator is 0
            }
        }

        private sealed class TestAsyncService
        {
            public async Task ProcessDataAsync(string data)
            {
                if (data == null)
                {
                    throw new ArgumentNullException(nameof(data), "Data cannot be null");
                }

                await Task.Delay(10);

                // Process data...
            }

            public async Task<string> FetchDataAsync()
            {
                await Task.Delay(10);

                return "Data";
            }
        }
    }
}