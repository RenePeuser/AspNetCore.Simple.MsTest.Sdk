using System.Text.RegularExpressions;
using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.String
{
    /// <summary>
    /// Tests for Assert.That.Matches and Assert.That.DoesNotMatch
    /// </summary>
    [TestClass]
    [TestCategory("String")]
    public sealed class AssertThatStringPatternTests
    {
        // ============================================================
        // Matches Tests
        // ============================================================

        [TestMethod]
        public void Matches_WhenTextMatchesPattern_ShouldPass()
        {
            // Arrange
            var email = "test@example.com";
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            // Act & Assert - Should NOT throw
            Assert.That.Matches(email,
                                emailPattern,
                                because: "Email should follow standard email format",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Matches_WhenTextDoesNotMatchPattern_ShouldFail()
        {
            // Arrange
            var invalidEmail = "not-an-email";
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            var threw = false;

            // Act
            try
            {
                Assert.That.Matches(invalidEmail,
                                    emailPattern,
                                    because: "Testing that invalid emails fail validation",
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
        public void Matches_WhenTextDoesNotMatchPattern_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var phoneNumber = "555-ABCD";
            var phonePattern = @"^\d{3}-\d{4}$";

            // Act
            try
            {
                Assert.That.Matches(phoneNumber,
                                    phonePattern,
                                    because: "Phone number must be in XXX-XXXX format for database validation",
                                    fix: "Ensure phone input is validated and formatted before saving to database");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("PATTERN MISMATCH - EXPECTED MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Phone number must be in XXX-XXXX format for database validation"));
                Assert.IsTrue(ex.Message.Contains("Ensure phone input is validated and formatted before saving to database"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("phoneNumber"));

                // Verify pattern details
                Assert.IsTrue(ex.Message.Contains(phonePattern));
                Assert.IsTrue(ex.Message.Contains("555-ABCD"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void Matches_WhenTextIsNull_ShouldFail()
        {
            // Arrange
            string? nullText = null;
            var pattern = @"^\d+$";
            var threw = false;

            // Act
            try
            {
                Assert.That.Matches(nullText,
                                    pattern,
                                    because: "Testing that null text fails pattern matching",
                                    fix: "Ensure text is not null before validation");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void Matches_WithRegexOptions_WhenTextMatchesCaseInsensitive_ShouldPass()
        {
            // Arrange
            var text = "HELLO World";
            var pattern = @"^hello world$";

            // Act & Assert - Should NOT throw
            Assert.That.Matches(text,
                                pattern,
                                because: "Pattern should match case-insensitively",
                                fix: "N/A - this should pass",
                                options: RegexOptions.IgnoreCase);
        }

        [TestMethod]
        public void Matches_WithComplexPattern_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var url = "ftp://example.com";
            var urlPattern = @"^https?://[^\s/$.?#].[^\s]*$";

            // Act
            try
            {
                Assert.That.Matches(url,
                                    urlPattern,
                                    because: "API endpoints must use HTTP or HTTPS protocol for security",
                                    fix: "Update the URL generation logic to enforce HTTPS protocol");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("PATTERN MISMATCH - EXPECTED MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("API endpoints must use HTTP or HTTPS protocol for security"));
                Assert.IsTrue(ex.Message.Contains("Update the URL generation logic to enforce HTTPS protocol"));

                // Verify helpful suggestions
                Assert.IsTrue(ex.Message.Contains("regex101.com"));
                Assert.IsTrue(ex.Message.Contains("Expected") && ex.Message.Contains("Match"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // DoesNotMatch Tests
        // ============================================================

        [TestMethod]
        public void DoesNotMatch_WhenTextDoesNotMatchPattern_ShouldPass()
        {
            // Arrange
            var safeText = "Hello World";
            var sqlInjectionPattern = @"(--|;|'|""|\bOR\b|\bAND\b|\bDROP\b|\bDELETE\b)";

            // Act & Assert - Should NOT throw
            Assert.That.DoesNotMatch(safeText,
                                     sqlInjectionPattern,
                                     because: "User input should not contain SQL injection patterns",
                                     fix: "N/A - this should pass",
                                     options: RegexOptions.IgnoreCase);
        }

        [TestMethod]
        public void DoesNotMatch_WhenTextMatchesPattern_ShouldFail()
        {
            // Arrange
            var suspiciousText = "DROP TABLE users";
            var sqlInjectionPattern = @"\bDROP\b";
            var threw = false;

            // Act
            try
            {
                Assert.That.DoesNotMatch(suspiciousText,
                                         sqlInjectionPattern,
                                         because: "Testing that SQL keywords are detected",
                                         fix: "This is expected to fail",
                                         options: RegexOptions.IgnoreCase);
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void DoesNotMatch_WhenTextMatchesPattern_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var password = "pass123";
            var weakPasswordPattern = @"^(password|pass|123|admin)";

            // Act
            try
            {
                Assert.That.DoesNotMatch(password,
                                         weakPasswordPattern,
                                         because: "Password must not contain common weak patterns for security compliance",
                                         fix: "Implement password strength validation that rejects common weak passwords",
                                         options: RegexOptions.IgnoreCase);

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("PATTERN MATCH - EXPECTED NO MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Password must not contain common weak patterns for security compliance"));
                Assert.IsTrue(ex.Message.Contains("Implement password strength validation that rejects common weak passwords"));

                // Verify variable name was captured
                Assert.IsTrue(ex.Message.Contains("password"));

                // Verify pattern details
                Assert.IsTrue(ex.Message.Contains(weakPasswordPattern));
                Assert.IsTrue(ex.Message.Contains("pass123"));

                // Verify match details are shown
                Assert.IsTrue(ex.Message.Contains("Match Value"));
                Assert.IsTrue(ex.Message.Contains("Match Index"));
                Assert.IsTrue(ex.Message.Contains("Expected") && ex.Message.Contains("No Match"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void DoesNotMatch_WhenTextIsNull_ShouldPass()
        {
            // Arrange
            string? nullText = null;
            var pattern = @"^\d+$";

            // Act & Assert - Should NOT throw (null does not match any pattern)
            Assert.That.DoesNotMatch(nullText,
                                     pattern,
                                     because: "Null text should not match any pattern",
                                     fix: "N/A - this should pass");
        }

        [TestMethod]
        public void DoesNotMatch_WithRegexOptions_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var username = "ADMIN";
            var reservedPattern = @"^(admin|root|system)$";

            // Act
            try
            {
                Assert.That.DoesNotMatch(username,
                                         reservedPattern,
                                         because: "Usernames cannot use reserved system names",
                                         fix: "Add validation to reject reserved usernames during registration",
                                         options: RegexOptions.IgnoreCase);

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("PATTERN MATCH - EXPECTED NO MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Usernames cannot use reserved system names"));
                Assert.IsTrue(ex.Message.Contains("Add validation to reject reserved usernames during registration"));

                // Verify regex options are shown
                Assert.IsTrue(ex.Message.Contains("IgnoreCase"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void DoesNotMatch_WithSpecialCharacters_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var comment = "Check out https://malicious-site.com";
            var urlPattern = @"https?://[^\s]+";

            // Act
            try
            {
                Assert.That.DoesNotMatch(comment,
                                         urlPattern,
                                         because: "User comments should not contain external URLs to prevent spam",
                                         fix: "Implement URL detection and filtering in comment validation middleware");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("PATTERN MATCH - EXPECTED NO MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User comments should not contain external URLs to prevent spam"));
                Assert.IsTrue(ex.Message.Contains("Implement URL detection and filtering in comment validation middleware"));

                // Verify match information
                Assert.IsTrue(ex.Message.Contains("Match Value"));
                Assert.IsTrue(ex.Message.Contains("https://malicious-site.com"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // Edge Case Tests
        // ============================================================

        [TestMethod]
        public void Matches_WithEmptyString_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var emptyText = "";
            var requiredPattern = @".+";

            // Act
            try
            {
                Assert.That.Matches(emptyText,
                                    requiredPattern,
                                    because: "Field value is required and cannot be empty",
                                    fix: "Add client-side and server-side validation for required fields");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("PATTERN MISMATCH - EXPECTED MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Field value is required and cannot be empty"));
                Assert.IsTrue(ex.Message.Contains("Add client-side and server-side validation for required fields"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void DoesNotMatch_WithEmptyString_ShouldPass()
        {
            // Arrange
            var emptyText = "";
            var pattern = @"[A-Z]+";

            // Act & Assert - Should NOT throw (empty string does not match)
            Assert.That.DoesNotMatch(emptyText,
                                     pattern,
                                     because: "Empty string should not match uppercase pattern",
                                     fix: "N/A - this should pass");
        }

        [TestMethod]
        public void Matches_WithMultilinePattern_ShouldPass()
        {
            // Arrange
            var multilineText = "Line 1\nLine 2\nLine 3";
            var multilinePattern = @"^Line 1$.*^Line 3$";

            // Act & Assert - Should NOT throw
            Assert.That.Matches(multilineText,
                                multilinePattern,
                                because: "Multi-line text should match pattern with correct flags",
                                fix: "N/A - this should pass",
                                options: RegexOptions.Multiline | RegexOptions.Singleline);
        }

        [TestMethod]
        public void Matches_WithComplexEmailPattern_ShouldPass()
        {
            // Arrange
            var email = "user.name+tag@example.co.uk";
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            // Act & Assert - Should NOT throw
            Assert.That.Matches(email,
                                emailPattern,
                                because: "Email format should support complex valid patterns",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void DoesNotMatch_WithWhitespacePattern_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var textWithSpaces = "Hello   World";
            var multiSpacePattern = @"\s{2,}";

            // Act
            try
            {
                Assert.That.DoesNotMatch(textWithSpaces,
                                         multiSpacePattern,
                                         because: "Text should not contain multiple consecutive spaces for formatting consistency",
                                         fix: "Normalize whitespace in text processing pipeline");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("PATTERN MATCH - EXPECTED NO MATCH"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Text should not contain multiple consecutive spaces for formatting consistency"));
                Assert.IsTrue(ex.Message.Contains("Normalize whitespace in text processing pipeline"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }
    }
}