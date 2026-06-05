using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.ExceptionAssertions
{
    /// <summary>
    /// Tests for Assert.That.Throws, Assert.That.ThrowsAsync, and Assert.That.ThrowsWithMessage
    /// </summary>
    [TestClass]
    [TestCategory("Exception")]
    public sealed class AssertThatThrowsTests
    {
        // ============================================================
        // Throws Tests
        // ============================================================

        [TestMethod]
        public void Throws_WhenActionThrowsExpectedException_ShouldPass()
        {
            // Arrange
            Action action = () => throw new InvalidOperationException("Test exception");

            // Act & Assert - Should NOT throw
            Assert.That.Throws<InvalidOperationException>(action,
                because: "Testing that expected exception is properly caught",
                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Throws_WhenActionDoesNotThrow_ShouldFail()
        {
            // Arrange
            Action action = () => { }; // Does nothing
            var threw = false;

            // Act
            try
            {
                Assert.That.Throws<InvalidOperationException>(action,
                    because: "Testing that missing exception fails",
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
        public void Throws_WhenActionThrowsWrongException_ShouldFail()
        {
            // Arrange
            Action action = () => throw new ArgumentException("Wrong exception");
            var threw = false;

            // Act
            try
            {
                Assert.That.Throws<InvalidOperationException>(action,
                    because: "Testing that wrong exception type fails",
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
        public void Throws_WhenNoExceptionThrown_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            Action action = () =>
            {
                // Simulating a method that should throw but doesn't
                var result = 10 / 2; // No exception
            };

            // Act
            try
            {
                Assert.That.Throws<DivideByZeroException>(action,
                    because: "Division by zero should always throw DivideByZeroException",
                    fix: "Check the divisor value in the calculation - ensure it's actually zero");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EXCEPTION ASSERTION - TYPE MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Division by zero should always throw DivideByZeroException"));
                Assert.IsTrue(ex.Message.Contains("Check the divisor value in the calculation - ensure it's actually zero"));
                Assert.IsTrue(ex.Message.Contains("no exception was thrown"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Throws_WhenWrongExceptionThrown_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            Action action = () => throw new ArgumentNullException("userId", "User ID cannot be null");

            // Act
            try
            {
                Assert.That.Throws<InvalidOperationException>(action,
                    because: "User service should throw InvalidOperationException for invalid operations",
                    fix: "Update UserService.GetUser to throw InvalidOperationException instead of ArgumentNullException");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EXCEPTION ASSERTION - TYPE MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User service should throw InvalidOperationException for invalid operations"));
                Assert.IsTrue(ex.Message.Contains("Update UserService.GetUser to throw InvalidOperationException instead of ArgumentNullException"));
                Assert.IsTrue(ex.Message.Contains("Expected Type"));
                Assert.IsTrue(ex.Message.Contains("InvalidOperationException"));
                Assert.IsTrue(ex.Message.Contains("Actual Type"));
                Assert.IsTrue(ex.Message.Contains("ArgumentNullException"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Throws_WithDerivedExceptionType_ShouldPass()
        {
            // Arrange
            Action action = () => throw new ArgumentNullException("param");

            // Act & Assert - ArgumentNullException derives from ArgumentException
            Assert.That.Throws<ArgumentException>(action,
                because: "ArgumentNullException is a derived type of ArgumentException",
                fix: "N/A - this should pass");
        }

        // ============================================================
        // ThrowsAsync Tests
        // ============================================================

        [TestMethod]
        public async Task ThrowsAsync_WhenActionThrowsExpectedException_ShouldPass()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                await Task.Delay(1);
                throw new InvalidOperationException("Async test exception");
            };

            // Act & Assert - Should NOT throw
            await Assert.That.ThrowsAsync<InvalidOperationException>(action,
                because: "Testing that async exception is properly caught",
                fix: "N/A - this should pass");
        }

        [TestMethod]
        public async Task ThrowsAsync_WhenActionDoesNotThrow_ShouldFail()
        {
            // Arrange
            Func<Task> action = async () => await Task.CompletedTask;
            var threw = false;

            // Act
            try
            {
                await Assert.That.ThrowsAsync<InvalidOperationException>(action,
                    because: "Testing that missing async exception fails",
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
        public async Task ThrowsAsync_WhenActionThrowsWrongException_ShouldFail()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                await Task.Delay(1);
                throw new ArgumentException("Wrong async exception");
            };
            var threw = false;

            // Act
            try
            {
                await Assert.That.ThrowsAsync<InvalidOperationException>(action,
                    because: "Testing that wrong async exception type fails",
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
        public async Task ThrowsAsync_WhenNoExceptionThrown_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                await Task.Delay(10);
                // Completes successfully without throwing
            };

            // Act
            try
            {
                await Assert.That.ThrowsAsync<TimeoutException>(action,
                    because: "Long-running operation should timeout after 5ms",
                    fix: "Add proper timeout handling with CancellationToken to async operations");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EXCEPTION ASSERTION - TYPE MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Long-running operation should timeout after 5ms"));
                Assert.IsTrue(ex.Message.Contains("Add proper timeout handling with CancellationToken to async operations"));
                Assert.IsTrue(ex.Message.Contains("no exception was thrown"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public async Task ThrowsAsync_WhenWrongExceptionThrown_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                await Task.Delay(1);
                throw new HttpRequestException("Network error");
            };

            // Act
            try
            {
                await Assert.That.ThrowsAsync<TimeoutException>(action,
                    because: "Network calls should throw TimeoutException when they exceed duration limit",
                    fix: "Update HttpClient configuration to use proper timeout handling");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EXCEPTION ASSERTION - TYPE MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Network calls should throw TimeoutException when they exceed duration limit"));
                Assert.IsTrue(ex.Message.Contains("Update HttpClient configuration to use proper timeout handling"));
                Assert.IsTrue(ex.Message.Contains("Expected Type"));
                Assert.IsTrue(ex.Message.Contains("TimeoutException"));
                Assert.IsTrue(ex.Message.Contains("Actual Type"));
                Assert.IsTrue(ex.Message.Contains("HttpRequestException"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public async Task ThrowsAsync_WithTaskException_ShouldPass()
        {
            // Arrange
            Func<Task> action = () => Task.FromException(new TaskCanceledException());

            // Act & Assert - Should NOT throw
            await Assert.That.ThrowsAsync<TaskCanceledException>(action,
                because: "Cancelled tasks should throw TaskCanceledException",
                fix: "N/A - this should pass");
        }

        // ============================================================
        // ThrowsWithMessage Tests
        // ============================================================

        [TestMethod]
        public void ThrowsWithMessage_WhenExceptionAndMessageMatch_ShouldPass()
        {
            // Arrange
            const string expectedMessage = "User not found";
            Action action = () => throw new InvalidOperationException(expectedMessage);

            // Act & Assert - Should NOT throw
            Assert.That.ThrowsWithMessage<InvalidOperationException>(action,
                expectedMessage: expectedMessage,
                because: "Testing that exception type and message both match",
                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void ThrowsWithMessage_WhenMessageDoesNotMatch_ShouldFail()
        {
            // Arrange
            Action action = () => throw new InvalidOperationException("Actual message");
            var threw = false;

            // Act
            try
            {
                Assert.That.ThrowsWithMessage<InvalidOperationException>(action,
                    expectedMessage: "Expected message",
                    because: "Testing that mismatched message fails",
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
        public void ThrowsWithMessage_WhenNoExceptionThrown_ShouldFail()
        {
            // Arrange
            Action action = () => { }; // Does nothing
            var threw = false;

            // Act
            try
            {
                Assert.That.ThrowsWithMessage<InvalidOperationException>(action,
                    expectedMessage: "Expected message",
                    because: "Testing that missing exception fails",
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
        public void ThrowsWithMessage_WhenWrongExceptionType_ShouldFail()
        {
            // Arrange
            Action action = () => throw new ArgumentException("Test message");
            var threw = false;

            // Act
            try
            {
                Assert.That.ThrowsWithMessage<InvalidOperationException>(action,
                    expectedMessage: "Test message",
                    because: "Testing that wrong exception type fails even with correct message",
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
        public void ThrowsWithMessage_WhenMessageMismatch_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            const string actualMessage = "User with ID 42 was not found in the database";
            const string expectedMessage = "User not found";
            Action action = () => throw new InvalidOperationException(actualMessage);

            // Act
            try
            {
                Assert.That.ThrowsWithMessage<InvalidOperationException>(action,
                    expectedMessage: expectedMessage,
                    because: "Exception message should match the standardized error format",
                    fix: "Update UserService.GetUser to use consistent error messages: 'User not found'");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EXCEPTION ASSERTION - TYPE AND MESSAGE MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Exception message should match the standardized error format"));
                Assert.IsTrue(ex.Message.Contains("Update UserService.GetUser to use consistent error messages"));
                Assert.IsTrue(ex.Message.Contains("Expected Msg"));
                Assert.IsTrue(ex.Message.Contains(expectedMessage));
                Assert.IsTrue(ex.Message.Contains("Actual Message"));
                Assert.IsTrue(ex.Message.Contains(actualMessage));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ThrowsWithMessage_WhenNoException_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            Action action = () =>
            {
                // Simulating a validation method that should throw but doesn't
                var email = "user@example.com";
                var isValid = email.Contains("@"); // Returns true, doesn't throw
            };

            // Act
            try
            {
                Assert.That.ThrowsWithMessage<ArgumentException>(action,
                    expectedMessage: "Email address is invalid",
                    because: "Email validation should throw ArgumentException for invalid formats",
                    fix: "Add proper email format validation to EmailValidator.Validate method");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EXCEPTION ASSERTION - TYPE AND MESSAGE MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Email validation should throw ArgumentException for invalid formats"));
                Assert.IsTrue(ex.Message.Contains("Add proper email format validation to EmailValidator.Validate method"));
                Assert.IsTrue(ex.Message.Contains("no exception was thrown"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ThrowsWithMessage_WhenTypeAndMessageMismatch_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            Action action = () => throw new ArgumentNullException("userId", "Value cannot be null. (Parameter 'userId')");

            // Act
            try
            {
                Assert.That.ThrowsWithMessage<InvalidOperationException>(action,
                    expectedMessage: "User ID is required for this operation",
                    because: "Business validation should use InvalidOperationException with clear messages",
                    fix: "Replace ArgumentNullException with InvalidOperationException and update message");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EXCEPTION ASSERTION - TYPE AND MESSAGE MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Business validation should use InvalidOperationException with clear messages"));
                Assert.IsTrue(ex.Message.Contains("Replace ArgumentNullException with InvalidOperationException and update message"));
                Assert.IsTrue(ex.Message.Contains("Expected Type"));
                Assert.IsTrue(ex.Message.Contains("InvalidOperationException"));
                Assert.IsTrue(ex.Message.Contains("Actual Type"));
                Assert.IsTrue(ex.Message.Contains("ArgumentNullException"));
                Assert.IsTrue(ex.Message.Contains("Expected Msg"));
                Assert.IsTrue(ex.Message.Contains("Actual Message"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void ThrowsWithMessage_WithExactMatch_ShouldPass()
        {
            // Arrange
            const string message = "The operation cannot be completed because the resource is locked";
            Action action = () => throw new InvalidOperationException(message);

            // Act & Assert - Should NOT throw
            Assert.That.ThrowsWithMessage<InvalidOperationException>(action,
                expectedMessage: message,
                because: "Testing exact message matching for complex error messages",
                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void ThrowsWithMessage_WithEmptyMessage_ShouldPass()
        {
            // Arrange
            const string emptyMessage = "";
            Action action = () => throw new InvalidOperationException(emptyMessage);

            // Act & Assert - Should NOT throw
            Assert.That.ThrowsWithMessage<InvalidOperationException>(action,
                expectedMessage: emptyMessage,
                because: "Testing that empty messages are handled correctly",
                fix: "N/A - this should pass");
        }

        // ============================================================
        // Edge Case Tests
        // ============================================================

        [TestMethod]
        public void Throws_WithNestedException_ShouldCatchOuterException()
        {
            // Arrange
            Action action = () =>
            {
                try
                {
                    throw new ArgumentException("Inner exception");
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException("Outer exception", ex);
                }
            };

            // Act & Assert - Should catch the outer exception
            Assert.That.Throws<InvalidOperationException>(action,
                because: "Nested exceptions should be caught by their outer type",
                fix: "N/A - this should pass");
        }

        [TestMethod]
        public async Task ThrowsAsync_WithAggregateException_ShouldPass()
        {
            // Arrange
            Func<Task> action = async () =>
            {
                await Task.Run(() => throw new InvalidOperationException("Inner exception"));
            };

            // Act & Assert - Should catch the InvalidOperationException unwrapped from AggregateException
            await Assert.That.ThrowsAsync<InvalidOperationException>(action,
                because: "Task exceptions should be unwrapped from AggregateException",
                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void ThrowsWithMessage_IsCaseSensitive_ShouldFail()
        {
            // Arrange
            Action action = () => throw new InvalidOperationException("USER NOT FOUND");
            var threw = false;

            // Act
            try
            {
                Assert.That.ThrowsWithMessage<InvalidOperationException>(action,
                    expectedMessage: "user not found",
                    because: "Message comparison is case-sensitive",
                    fix: "This is expected to fail");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown due to case mismatch");
        }
    }
}
