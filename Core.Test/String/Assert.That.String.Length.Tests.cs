using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.String
{
    /// <summary>
    /// Tests for Assert.That.HasLength and Assert.That.HasLengthInRange
    /// </summary>
    [TestClass]
    [TestCategory("String")]
    public sealed class AssertThatStringLengthTests
    {
        // ============================================================
        // HasLength Tests
        // ============================================================

        [TestMethod]
        public void HasLength_WhenStringHasExactLength_ShouldPass()
        {
            // Arrange
            var username = "GokuSon";

            // Act & Assert - Should NOT throw
            Assert.That.HasLength(username, 7,
                                  because: "Username must be exactly 7 characters for this test",
                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasLength_WhenStringLengthMismatch_ShouldFail()
        {
            // Arrange
            var password = "12345";
            var threw = false;

            // Act
            try
            {
                Assert.That.HasLength(password, 8,
                                      because: "Password must be exactly 8 characters",
                                      fix: "Update password validation logic");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void HasLength_WhenStringTooShort_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var apiKey = "ABC123";

            // Act
            try
            {
                Assert.That.HasLength(apiKey, 32,
                                      because: "API key must be 32 characters for security compliance",
                                      fix: "Generate a proper 32-character API key using the KeyGenerator.Generate() method");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("API key must be 32 characters for security compliance"));
                Assert.IsTrue(ex.Message.Contains("Generate a proper 32-character API key using the KeyGenerator.Generate() method"));

                // Verify details section contains expected information
                Assert.IsTrue(ex.Message.Contains("apiKey"));
                Assert.IsTrue(ex.Message.Contains("Actual Length"));
                Assert.IsTrue(ex.Message.Contains("Expected Length"));
                Assert.IsTrue(ex.Message.Contains("Difference"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasLength_WhenStringTooLong_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var displayName = "VeryLongDisplayNameThatExceedsTheMaximumAllowedLength";

            // Act
            try
            {
                Assert.That.HasLength(displayName, 20,
                                      because: "Display name is limited to 20 characters for UI consistency",
                                      fix: "Truncate the display name or update the database column max length");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Display name is limited to 20 characters for UI consistency"));
                Assert.IsTrue(ex.Message.Contains("Truncate the display name or update the database column max length"));

                // Verify details section contains expected information
                Assert.IsTrue(ex.Message.Contains("displayName"));
                Assert.IsTrue(ex.Message.Contains("Actual Length"));
                Assert.IsTrue(ex.Message.Contains("Expected Length"));
                Assert.IsTrue(ex.Message.Contains("Difference"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasLength_WhenStringIsNull_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            string? description = null;

            // Act
            try
            {
                Assert.That.HasLength(description, 50,
                                      because: "Product description should contain at least some content",
                                      fix: "Ensure the product description is populated from the database");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Product description should contain at least some content"));
                Assert.IsTrue(ex.Message.Contains("Ensure the product description is populated from the database"));

                // Verify null handling
                Assert.IsTrue(ex.Message.Contains("null"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasLength_WhenStringIsEmpty_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var errorMessage = string.Empty;

            // Act
            try
            {
                Assert.That.HasLength(errorMessage, 10,
                                      because: "Error messages should provide meaningful feedback to users",
                                      fix: "Update the error handling logic to return descriptive error messages");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH MISMATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Error messages should provide meaningful feedback to users"));
                Assert.IsTrue(ex.Message.Contains("Update the error handling logic to return descriptive error messages"));

                // Verify empty string handling
                Assert.IsTrue(ex.Message.Contains("(empty string)"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasLength_WhenStringIsVeryLong_ShouldTruncateInOutput()
        {
            // Arrange
            var longText = new string('A', 200); // 200 characters

            // Act
            try
            {
                Assert.That.HasLength(longText, 150,
                                      because: "Text content should not exceed processing limits",
                                      fix: "Implement text truncation before storage");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH MISMATCH"));

                // Verify truncation occurs (first 100 chars + ...)
                Assert.IsTrue(ex.Message.Contains("first 100"));
                Assert.IsTrue(ex.Message.Contains("..."));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // HasLengthInRange Tests
        // ============================================================

        [TestMethod]
        public void HasLengthInRange_WhenStringLengthWithinRange_ShouldPass()
        {
            // Arrange
            var username = "Vegeta";

            // Act & Assert - Should NOT throw
            Assert.That.HasLengthInRange(username, 3, 20,
                                         because: "Username must be between 3 and 20 characters",
                                         fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasLengthInRange_WhenStringLengthAtMinimum_ShouldPass()
        {
            // Arrange
            var code = "ABC";

            // Act & Assert - Should NOT throw
            Assert.That.HasLengthInRange(code, 3, 10,
                                         because: "Code must be between 3 and 10 characters",
                                         fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasLengthInRange_WhenStringLengthAtMaximum_ShouldPass()
        {
            // Arrange
            var code = "ABCDEFGHIJ";

            // Act & Assert - Should NOT throw
            Assert.That.HasLengthInRange(code, 3, 10,
                                         because: "Code must be between 3 and 10 characters",
                                         fix: "N/A - this should pass");
        }

        [TestMethod]
        public void HasLengthInRange_WhenStringTooShort_ShouldFail()
        {
            // Arrange
            var password = "12";
            var threw = false;

            // Act
            try
            {
                Assert.That.HasLengthInRange(password, 8, 64,
                                             because: "Password must be between 8 and 64 characters for security",
                                             fix: "Enforce minimum password length in validation");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void HasLengthInRange_WhenStringTooShort_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var emailSubject = "Hi";

            // Act
            try
            {
                Assert.That.HasLengthInRange(emailSubject, 5, 100,
                                             because: "Email subject must be descriptive and between 5 and 100 characters",
                                             fix: "Update email templates to include more descriptive subject lines");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Email subject must be descriptive and between 5 and 100 characters"));
                Assert.IsTrue(ex.Message.Contains("Update email templates to include more descriptive subject lines"));

                // Verify details section contains expected information
                Assert.IsTrue(ex.Message.Contains("emailSubject"));
                Assert.IsTrue(ex.Message.Contains("Actual Length"));
                Assert.IsTrue(ex.Message.Contains("Min Length"));
                Assert.IsTrue(ex.Message.Contains("Max Length"));
                Assert.IsTrue(ex.Message.Contains("too short"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasLengthInRange_WhenStringTooLong_ShouldFail()
        {
            // Arrange
            var biography = new string('X', 1001);
            var threw = false;

            // Act
            try
            {
                Assert.That.HasLengthInRange(biography, 10, 1000,
                                             because: "Biography must be between 10 and 1000 characters",
                                             fix: "Add character limit validation to biography input field");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void HasLengthInRange_WhenStringTooLong_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var comment = "This is an extremely long comment that exceeds the maximum allowed length for comments in the system";

            // Act
            try
            {
                Assert.That.HasLengthInRange(comment, 1, 50,
                                             because: "Comments are limited to 50 characters to maintain UI consistency",
                                             fix: "Add character counter and validation to comment input field");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Comments are limited to 50 characters to maintain UI consistency"));
                Assert.IsTrue(ex.Message.Contains("Add character counter and validation to comment input field"));

                // Verify details section contains expected information
                Assert.IsTrue(ex.Message.Contains("comment"));
                Assert.IsTrue(ex.Message.Contains("Actual Length"));
                Assert.IsTrue(ex.Message.Contains("Min Length"));
                Assert.IsTrue(ex.Message.Contains("Max Length"));
                Assert.IsTrue(ex.Message.Contains("too long"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasLengthInRange_WhenStringIsNull_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            string? productName = null;

            // Act
            try
            {
                Assert.That.HasLengthInRange(productName, 5, 100,
                                             because: "Product name is required and must be between 5 and 100 characters",
                                             fix: "Ensure product name is populated from the request body");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Product name is required and must be between 5 and 100 characters"));
                Assert.IsTrue(ex.Message.Contains("Ensure product name is populated from the request body"));

                // Verify null handling
                Assert.IsTrue(ex.Message.Contains("null"));
                Assert.IsTrue(ex.Message.Contains("too short"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasLengthInRange_WhenStringIsEmpty_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var title = string.Empty;

            // Act
            try
            {
                Assert.That.HasLengthInRange(title, 3, 200,
                                             because: "Article title must contain meaningful content between 3 and 200 characters",
                                             fix: "Add required field validation for article title");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH OUT OF RANGE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Article title must contain meaningful content between 3 and 200 characters"));
                Assert.IsTrue(ex.Message.Contains("Add required field validation for article title"));

                // Verify empty string handling
                Assert.IsTrue(ex.Message.Contains("(empty string)"));
                Assert.IsTrue(ex.Message.Contains("too short"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void HasLengthInRange_WhenVeryLongString_ShouldTruncateInOutput()
        {
            // Arrange
            var longDescription = new string('B', 250);

            // Act
            try
            {
                Assert.That.HasLengthInRange(longDescription, 10, 200,
                                             because: "Description should be concise and between 10 and 200 characters",
                                             fix: "Implement text truncation and provide character count feedback");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING LENGTH OUT OF RANGE"));

                // Verify truncation occurs (first 100 chars + ...)
                Assert.IsTrue(ex.Message.Contains("first 100"));
                Assert.IsTrue(ex.Message.Contains("..."));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }
    }
}