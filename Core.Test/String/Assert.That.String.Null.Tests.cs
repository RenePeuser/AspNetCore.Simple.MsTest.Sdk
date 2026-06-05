using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.String
{
    /// <summary>
    /// Tests for Assert.That string null/empty/whitespace methods
    /// </summary>
    [TestClass]
    [TestCategory("String")]
    public sealed class AssertThatStringNullTests
    {
        // ============================================================
        // IsNullOrEmpty Tests
        // ============================================================

        [TestMethod]
        public void IsNullOrEmpty_WhenValueIsNull_ShouldPass()
        {
            // Arrange
            string? value = null;

            // Act & Assert - Should NOT throw
            Assert.That.IsNullOrEmpty(value,
                                      because: "Testing that null values pass",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNullOrEmpty_WhenValueIsEmpty_ShouldPass()
        {
            // Arrange
            var value = string.Empty;

            // Act & Assert - Should NOT throw
            Assert.That.IsNullOrEmpty(value,
                                      because: "Testing that empty string passes",
                                      fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNullOrEmpty_WhenValueHasContent_ShouldFail()
        {
            // Arrange
            var value = "Some content";
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNullOrEmpty(value,
                                          because: "Testing that non-empty values fail",
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
        public void IsNullOrEmpty_WhenStringHasContent_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var userName = "Goku";

            // Act
            try
            {
                Assert.That.IsNullOrEmpty(userName,
                                          because: "Username should be null or empty for anonymous users",
                                          fix: "Ensure GetCurrentUser returns null for unauthenticated requests");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING CHECK FAILED - EXPECTED NULL OR EMPTY"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Username should be null or empty for anonymous users"));
                Assert.IsTrue(ex.Message.Contains("Ensure GetCurrentUser returns null for unauthenticated requests"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("userName"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsNotNullOrEmpty Tests
        // ============================================================

        [TestMethod]
        public void IsNotNullOrEmpty_WhenValueHasContent_ShouldPass()
        {
            // Arrange
            var value = "Valid content";

            // Act & Assert - Should NOT throw
            Assert.That.IsNotNullOrEmpty(value,
                                         because: "Testing that non-empty values pass",
                                         fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNotNullOrEmpty_WhenValueIsNull_ShouldFail()
        {
            // Arrange
            string? value = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNotNullOrEmpty(value,
                                             because: "Testing that null values fail",
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
        public void IsNotNullOrEmpty_WhenValueIsEmpty_ShouldFail()
        {
            // Arrange
            var value = string.Empty;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNotNullOrEmpty(value,
                                             because: "Testing that empty values fail",
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
        public void IsNotNullOrEmpty_WhenStringIsNull_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            string? email = null;

            // Act
            try
            {
                Assert.That.IsNotNullOrEmpty(email,
                                             because: "Email must be provided for user registration",
                                             fix: "Check form validation in RegisterUser.cshtml");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING CHECK FAILED - EXPECTED NON-NULL OR EMPTY"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Email must be provided for user registration"));
                Assert.IsTrue(ex.Message.Contains("Check form validation in RegisterUser.cshtml"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("email"));

                // Verify type info
                Assert.IsTrue(ex.Message.Contains("string"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNotNullOrEmpty_WhenStringIsEmpty_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var productName = "";

            // Act
            try
            {
                Assert.That.IsNotNullOrEmpty(productName,
                                             because: "Product name is required for catalog display",
                                             fix: "Add validation to ProductController.Create to reject empty names");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING CHECK FAILED - EXPECTED NON-NULL OR EMPTY"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Product name is required for catalog display"));
                Assert.IsTrue(ex.Message.Contains("Add validation to ProductController.Create to reject empty names"));

                // Verify the empty string is displayed correctly
                Assert.IsTrue(ex.Message.Contains("\"\" (empty string)"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsNullOrWhiteSpace Tests
        // ============================================================

        [TestMethod]
        public void IsNullOrWhiteSpace_WhenValueIsNull_ShouldPass()
        {
            // Arrange
            string? value = null;

            // Act & Assert - Should NOT throw
            Assert.That.IsNullOrWhiteSpace(value,
                                           because: "Testing that null values pass",
                                           fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNullOrWhiteSpace_WhenValueIsEmpty_ShouldPass()
        {
            // Arrange
            var value = string.Empty;

            // Act & Assert - Should NOT throw
            Assert.That.IsNullOrWhiteSpace(value,
                                           because: "Testing that empty string passes",
                                           fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNullOrWhiteSpace_WhenValueIsWhiteSpace_ShouldPass()
        {
            // Arrange
            var value = "   \t\n  ";

            // Act & Assert - Should NOT throw
            Assert.That.IsNullOrWhiteSpace(value,
                                           because: "Testing that whitespace-only string passes",
                                           fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNullOrWhiteSpace_WhenValueHasContent_ShouldFail()
        {
            // Arrange
            var value = "Some content";
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNullOrWhiteSpace(value,
                                               because: "Testing that non-whitespace values fail",
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
        public void IsNullOrWhiteSpace_WhenStringHasContent_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var description = "A valid description";

            // Act
            try
            {
                Assert.That.IsNullOrWhiteSpace(description,
                                               because: "Description should be blank for draft posts",
                                               fix: "Ensure CreateDraftPost clears the description field");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING CHECK FAILED - EXPECTED NULL OR WHITESPACE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Description should be blank for draft posts"));
                Assert.IsTrue(ex.Message.Contains("Ensure CreateDraftPost clears the description field"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("description"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsNotNullOrWhiteSpace Tests
        // ============================================================

        [TestMethod]
        public void IsNotNullOrWhiteSpace_WhenValueHasContent_ShouldPass()
        {
            // Arrange
            var value = "Valid content";

            // Act & Assert - Should NOT throw
            Assert.That.IsNotNullOrWhiteSpace(value,
                                              because: "Testing that non-whitespace values pass",
                                              fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNotNullOrWhiteSpace_WhenValueHasContentWithSpaces_ShouldPass()
        {
            // Arrange
            var value = "  Content with spaces  ";

            // Act & Assert - Should NOT throw
            Assert.That.IsNotNullOrWhiteSpace(value,
                                              because: "Testing that strings with content and spaces pass",
                                              fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNotNullOrWhiteSpace_WhenValueIsNull_ShouldFail()
        {
            // Arrange
            string? value = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNotNullOrWhiteSpace(value,
                                                  because: "Testing that null values fail",
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
        public void IsNotNullOrWhiteSpace_WhenValueIsEmpty_ShouldFail()
        {
            // Arrange
            var value = string.Empty;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNotNullOrWhiteSpace(value,
                                                  because: "Testing that empty values fail",
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
        public void IsNotNullOrWhiteSpace_WhenValueIsWhiteSpace_ShouldFail()
        {
            // Arrange
            var value = "   \t\n  ";
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNotNullOrWhiteSpace(value,
                                                  because: "Testing that whitespace-only values fail",
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
        public void IsNotNullOrWhiteSpace_WhenStringIsNull_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            string? comment = null;

            // Act
            try
            {
                Assert.That.IsNotNullOrWhiteSpace(comment,
                                                  because: "Comment content is required to post a review",
                                                  fix: "Add client-side validation to prevent empty comment submission");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING CHECK FAILED - EXPECTED NON-NULL OR WHITESPACE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Comment content is required to post a review"));
                Assert.IsTrue(ex.Message.Contains("Add client-side validation to prevent empty comment submission"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("comment"));

                // Verify type info
                Assert.IsTrue(ex.Message.Contains("string"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNotNullOrWhiteSpace_WhenStringIsEmpty_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var title = "";

            // Act
            try
            {
                Assert.That.IsNotNullOrWhiteSpace(title,
                                                  because: "Blog post title must not be empty",
                                                  fix: "Ensure BlogPostDto.Title has [Required] attribute");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING CHECK FAILED - EXPECTED NON-NULL OR WHITESPACE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Blog post title must not be empty"));
                Assert.IsTrue(ex.Message.Contains("Ensure BlogPostDto.Title has [Required] attribute"));

                // Verify the empty string is displayed correctly
                Assert.IsTrue(ex.Message.Contains("\"\" (empty string)"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNotNullOrWhiteSpace_WhenStringIsWhiteSpace_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var address = "   \t\n  ";

            // Act
            try
            {
                Assert.That.IsNotNullOrWhiteSpace(address,
                                                  because: "Shipping address must contain actual content",
                                                  fix: "Add .Trim() validation to OrderValidator.ValidateAddress");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("STRING CHECK FAILED - EXPECTED NON-NULL OR WHITESPACE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Shipping address must contain actual content"));
                Assert.IsTrue(ex.Message.Contains("Add .Trim() validation to OrderValidator.ValidateAddress"));

                // Verify whitespace is displayed correctly with length
                Assert.IsTrue(ex.Message.Contains("whitespace only"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }
    }
}