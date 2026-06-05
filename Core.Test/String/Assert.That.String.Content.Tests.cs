using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.String
{
    /// <summary>
    /// Tests for Assert.That string content assertions: Contains, DoesNotContain, StartsWith, EndsWith
    /// </summary>
    [TestClass]
    [TestCategory("String")]
    public sealed class AssertThatStringContentTests
    {
        // ============================================================
        // Contains Tests
        // ============================================================

        [TestMethod]
        public void Contains_WhenSubstringExists_ShouldPass()
        {
            // Arrange
            var text = "The quick brown fox jumps over the lazy dog";

            // Act & Assert - Should NOT throw
            Assert.That.Contains(text, "quick brown",
                                 because: "Testing that existing substring passes",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Contains_WhenSubstringDoesNotExist_ShouldFail()
        {
            // Arrange
            var text = "The quick brown fox jumps over the lazy dog";
            var threw = false;

            // Act
            try
            {
                Assert.That.Contains(text, "cat",
                                     because: "Testing that non-existent substring fails",
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
        public void Contains_WhenSubstringDoesNotExist_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var apiResponse = "User not found in database";

            // Act
            try
            {
                Assert.That.Contains(apiResponse, "success",
                                     because: "API response should indicate successful operation",
                                     fix: "Verify API endpoint returns success status for valid requests");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING CONTENT - EXPECTED TO CONTAIN SUBSTRING"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("API response should indicate successful operation"));
                Assert.IsTrue(ex.Message.Contains("Verify API endpoint returns success status for valid requests"));

                // Verify variable name and substring details
                Assert.IsTrue(ex.Message.Contains("apiResponse"));
                Assert.IsTrue(ex.Message.Contains("success"));
                Assert.IsTrue(ex.Message.Contains("Not found"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Contains_WithCaseInsensitiveComparison_ShouldPass()
        {
            // Arrange
            var text = "Hello World";

            // Act & Assert - Should NOT throw
            Assert.That.Contains(text, "HELLO",
                                 because: "Testing case-insensitive comparison",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Contains_WithNullText_ShouldFail()
        {
            // Arrange
            string? text = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.Contains(text!, "test",
                                     because: "Null text should fail contains check",
                                     fix: "Ensure text is not null before assertion");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        // ============================================================
        // DoesNotContain Tests
        // ============================================================

        [TestMethod]
        public void DoesNotContain_WhenSubstringDoesNotExist_ShouldPass()
        {
            // Arrange
            var text = "The quick brown fox jumps over the lazy dog";

            // Act & Assert - Should NOT throw
            Assert.That.DoesNotContain(text, "cat",
                                       because: "Testing that non-existent substring passes",
                                       fix: "N/A - this should pass");
        }

        [TestMethod]
        public void DoesNotContain_WhenSubstringExists_ShouldFail()
        {
            // Arrange
            var text = "The quick brown fox jumps over the lazy dog";
            var threw = false;

            // Act
            try
            {
                Assert.That.DoesNotContain(text, "quick",
                                           because: "Testing that existing substring fails",
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
        public void DoesNotContain_WhenSubstringExists_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var logOutput = "ERROR: Database connection failed at 10:30 AM";

            // Act
            try
            {
                Assert.That.DoesNotContain(logOutput, "ERROR",
                                           because: "Log output should not contain error messages in successful tests",
                                           fix: "Check database connection configuration and ensure test database is running");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING CONTENT - EXPECTED NOT TO CONTAIN SUBSTRING"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Log output should not contain error messages in successful tests"));
                Assert.IsTrue(ex.Message.Contains("Check database connection configuration and ensure test database is running"));

                // Verify variable name and found index
                Assert.IsTrue(ex.Message.Contains("logOutput"));
                Assert.IsTrue(ex.Message.Contains("ERROR"));
                Assert.IsTrue(ex.Message.Contains("Index 0"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void DoesNotContain_WithNullText_ShouldPass()
        {
            // Arrange
            string? text = null;

            // Act & Assert - Should NOT throw (null doesn't contain anything)
            Assert.That.DoesNotContain(text!, "test",
                                       because: "Null text does not contain any substring",
                                       fix: "N/A - this should pass");
        }

        [TestMethod]
        public void DoesNotContain_WithCaseInsensitiveComparison_ShouldFail()
        {
            // Arrange
            var text = "Hello World";
            var threw = false;

            // Act
            try
            {
                Assert.That.DoesNotContain(text, "WORLD",
                                           because: "Testing case-insensitive comparison",
                                           fix: "Remove 'world' from text",
                                           comparison: StringComparison.OrdinalIgnoreCase);
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        // ============================================================
        // StartsWith Tests
        // ============================================================

        [TestMethod]
        public void StartsWith_WhenTextStartsWithPrefix_ShouldPass()
        {
            // Arrange
            var text = "Hello World";

            // Act & Assert - Should NOT throw
            Assert.That.StartsWith(text, "Hello",
                                   because: "Testing that correct prefix passes",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void StartsWith_WhenTextDoesNotStartWithPrefix_ShouldFail()
        {
            // Arrange
            var text = "Hello World";
            var threw = false;

            // Act
            try
            {
                Assert.That.StartsWith(text, "World",
                                       because: "Testing that incorrect prefix fails",
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
        public void StartsWith_WhenTextDoesNotStartWithPrefix_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var fileName = "report_2024.pdf";

            // Act
            try
            {
                Assert.That.StartsWith(fileName, "invoice_",
                                       because: "Generated filename should follow invoice naming convention",
                                       fix: "Update the filename generation logic to prefix with 'invoice_'");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING PREFIX - EXPECTED TO START WITH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Generated filename should follow invoice naming convention"));
                Assert.IsTrue(ex.Message.Contains("Update the filename generation logic to prefix with 'invoice_'"));

                // Verify variable name, prefix, and actual prefix shown
                Assert.IsTrue(ex.Message.Contains("fileName"));
                Assert.IsTrue(ex.Message.Contains("invoice_"));
                Assert.IsTrue(ex.Message.Contains("Actual Prefix"));
                Assert.IsTrue(ex.Message.Contains("report_2"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void StartsWith_WithCaseInsensitiveComparison_ShouldPass()
        {
            // Arrange
            var text = "Hello World";

            // Act & Assert - Should NOT throw
            Assert.That.StartsWith(text, "HELLO",
                                   because: "Testing case-insensitive prefix check",
                                   fix: "N/A - this should pass",
                                   comparison: StringComparison.OrdinalIgnoreCase);
        }

        [TestMethod]
        public void StartsWith_WithNullText_ShouldFail()
        {
            // Arrange
            string? text = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.StartsWith(text!, "test",
                                       because: "Null text should fail prefix check",
                                       fix: "Ensure text is not null before assertion");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void StartsWith_WithEmptyPrefix_ShouldPass()
        {
            // Arrange
            var text = "Hello World";

            // Act & Assert - Should NOT throw (any string starts with empty string)
            Assert.That.StartsWith(text, "",
                                   because: "All strings start with empty string",
                                   fix: "N/A - this should pass");
        }

        // ============================================================
        // EndsWith Tests
        // ============================================================

        [TestMethod]
        public void EndsWith_WhenTextEndsWithSuffix_ShouldPass()
        {
            // Arrange
            var text = "Hello World";

            // Act & Assert - Should NOT throw
            Assert.That.EndsWith(text, "World",
                                 because: "Testing that correct suffix passes",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void EndsWith_WhenTextDoesNotEndWithSuffix_ShouldFail()
        {
            // Arrange
            var text = "Hello World";
            var threw = false;

            // Act
            try
            {
                Assert.That.EndsWith(text, "Hello",
                                     because: "Testing that incorrect suffix fails",
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
        public void EndsWith_WhenTextDoesNotEndWithSuffix_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var apiUrl = "https://api.example.com/users";

            // Act
            try
            {
                Assert.That.EndsWith(apiUrl, "/products",
                                     because: "API URL should point to products endpoint",
                                     fix: "Verify URL construction logic uses correct endpoint suffix");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING SUFFIX - EXPECTED TO END WITH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("API URL should point to products endpoint"));
                Assert.IsTrue(ex.Message.Contains("Verify URL construction logic uses correct endpoint suffix"));

                // Verify variable name, suffix, and actual suffix shown
                Assert.IsTrue(ex.Message.Contains("apiUrl"));
                Assert.IsTrue(ex.Message.Contains("/products"));
                Assert.IsTrue(ex.Message.Contains("Actual Suffix"));
                Assert.IsTrue(ex.Message.Contains("/users"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void EndsWith_WithCaseInsensitiveComparison_ShouldPass()
        {
            // Arrange
            var text = "Hello World";

            // Act & Assert - Should NOT throw
            Assert.That.EndsWith(text, "WORLD",
                                 because: "Testing case-insensitive suffix check",
                                 fix: "N/A - this should pass",
                                 comparison: StringComparison.OrdinalIgnoreCase);
        }

        [TestMethod]
        public void EndsWith_WithNullText_ShouldFail()
        {
            // Arrange
            string? text = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.EndsWith(text!, "test",
                                     because: "Null text should fail suffix check",
                                     fix: "Ensure text is not null before assertion");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void EndsWith_WithEmptySuffix_ShouldPass()
        {
            // Arrange
            var text = "Hello World";

            // Act & Assert - Should NOT throw (any string ends with empty string)
            Assert.That.EndsWith(text, "",
                                 because: "All strings end with empty string",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void EndsWith_WithFileExtension_ShouldPass()
        {
            // Arrange
            var fileName = "document.pdf";

            // Act & Assert - Should NOT throw
            Assert.That.EndsWith(fileName, ".pdf",
                                 because: "File should have PDF extension",
                                 fix: "Ensure file generation produces PDF format");
        }
    }
}