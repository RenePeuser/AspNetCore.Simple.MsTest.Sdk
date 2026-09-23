using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Core
{
    /// <summary>
    /// Tests for Assert.That.IsOfType, Assert.That.IsNotOfType, and Assert.That.IsAssignableTo
    /// </summary>
    [TestClass]
    [TestCategory("Type")]
    public sealed class AssertThatTypeTests
    {
        // ============================================================
        // IsOfType Tests
        // ============================================================

        [TestMethod]
        public void IsOfType_WhenExactTypeMatches_ShouldPass()
        {
            // Arrange
            var value = "Hello World";

            // Act & Assert - Should NOT throw
            Assert.That.IsOfType<string>(value,
                                         because: "Testing that exact type match passes",
                                         fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsOfType_WhenTypesDoNotMatch_ShouldFail()
        {
            // Arrange
            var value = 42;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsOfType<string>(value,
                                             because: "Testing that type mismatch fails",
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
        public void IsOfType_WhenValueIsNull_ShouldFail()
        {
            // Arrange
            string? value = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsOfType<string>(value,
                                             because: "Testing that null values fail",
                                             fix: "Ensure value is not null");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void IsOfType_WhenTypeMismatch_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var response = new ApiResponse
            {
                StatusCode = 200,
                Message = "Success"
            };

            // Act
            try
            {
                Assert.That.IsOfType<ErrorResponse>(response,
                                                    because: "API should return error response for invalid requests",
                                                    fix: "Check the request validation logic in the controller");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("TYPE MISMATCH - EXPECTED EXACT TYPE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("API should return error response for invalid requests"));
                Assert.IsTrue(ex.Message.Contains("Check the request validation logic in the controller"));

                // Verify type info
                Assert.IsTrue(ex.Message.Contains("ErrorResponse"));
                Assert.IsTrue(ex.Message.Contains("ApiResponse"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsOfType_WithComplexType_ShouldPass()
        {
            // Arrange
            var user = new TestUser
            {
                Id = 1,
                Name = "Goku"
            };

            // Act & Assert - Should NOT throw
            Assert.That.IsOfType<TestUser>(user,
                                           because: "User object should be exact TestUser type",
                                           fix: "Verify user creation logic");
        }

        [TestMethod]
        public void IsOfType_WithDerivedType_ShouldFail()
        {
            // Arrange
            BaseClass value = new DerivedClass();

            // Act
            try
            {
                Assert.That.IsOfType<BaseClass>(value,
                                                because: "Expected exact base class type",
                                                fix: "Use IsAssignableTo for inheritance checks");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify it fails because IsOfType requires exact match
                Assert.IsTrue(ex.Message.Contains("TYPE MISMATCH - EXPECTED EXACT TYPE"));
                Assert.IsTrue(ex.Message.Contains("BaseClass"));
                Assert.IsTrue(ex.Message.Contains("DerivedClass"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // IsNotOfType Tests
        // ============================================================

        [TestMethod]
        public void IsNotOfType_WhenTypesAreDifferent_ShouldPass()
        {
            // Arrange
            var value = "Hello World";

            // Act & Assert - Should NOT throw
            Assert.That.IsNotOfType<int>(value,
                                         because: "Testing that different types pass",
                                         fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNotOfType_WhenTypesMatch_ShouldFail()
        {
            // Arrange
            var value = 42;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsNotOfType<int>(value,
                                             because: "Testing that matching types fail",
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
        public void IsNotOfType_WhenValueIsNull_ShouldPass()
        {
            // Arrange
            string? value = null;

            // Act & Assert - Should NOT throw (null is not of any type)
            Assert.That.IsNotOfType<string>(value,
                                            because: "Testing that null values pass",
                                            fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsNotOfType_WhenTypesMatch_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var error = new ErrorResponse
            {
                ErrorCode = "ERR_001",
                ErrorMessage = "Invalid input"
            };

            // Act
            try
            {
                Assert.That.IsNotOfType<ErrorResponse>(error,
                                                       because: "Expected successful response, not an error",
                                                       fix: "Fix the validation logic to accept valid inputs");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("TYPE MISMATCH - EXPECTED DIFFERENT TYPE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Expected successful response, not an error"));
                Assert.IsTrue(ex.Message.Contains("Fix the validation logic to accept valid inputs"));

                // Verify type info
                Assert.IsTrue(ex.Message.Contains("ErrorResponse"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsNotOfType_WithDerivedType_ShouldPass()
        {
            // Arrange
            BaseClass value = new DerivedClass();

            // Act & Assert - Should NOT throw (DerivedClass is not exactly BaseClass)
            Assert.That.IsNotOfType<BaseClass>(value,
                                               because: "Derived type should not be exact match",
                                               fix: "N/A - this should pass");
        }

        // ============================================================
        // IsAssignableTo Tests
        // ============================================================

        [TestMethod]
        public void IsAssignableTo_WhenTypesMatch_ShouldPass()
        {
            // Arrange
            var value = "Hello World";

            // Act & Assert - Should NOT throw
            Assert.That.IsAssignableTo<string>(value,
                                               because: "Testing that exact type is assignable",
                                               fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsAssignableTo_WhenDerivedTypeIsAssignable_ShouldPass()
        {
            // Arrange
            BaseClass value = new DerivedClass();

            // Act & Assert - Should NOT throw
            Assert.That.IsAssignableTo<BaseClass>(value,
                                                  because: "Derived type should be assignable to base",
                                                  fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsAssignableTo_WhenImplementsInterface_ShouldPass()
        {
            // Arrange
            var value = new TestUser
            {
                Id = 1,
                Name = "Goku"
            };

            // Act & Assert - Should NOT throw
            Assert.That.IsAssignableTo<IEntity>(value,
                                                because: "TestUser implements IEntity",
                                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void IsAssignableTo_WhenNotAssignable_ShouldFail()
        {
            // Arrange
            var value = "Hello World";
            var threw = false;

            // Act
            try
            {
                Assert.That.IsAssignableTo<int>(value,
                                                because: "Testing that non-assignable types fail",
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
        public void IsAssignableTo_WhenValueIsNull_ShouldFail()
        {
            // Arrange
            string? value = null;
            var threw = false;

            // Act
            try
            {
                Assert.That.IsAssignableTo<string>(value,
                                                   because: "Testing that null values fail",
                                                   fix: "Ensure value is not null");
            }
            catch (AssertFailedException)
            {
                threw = true;
            }

            // Assert
            Assert.IsTrue(threw, "Expected AssertFailedException to be thrown");
        }

        [TestMethod]
        public void IsAssignableTo_WhenNotAssignable_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var data = new DataModel
            {
                Id = 1,
                Value = "Test"
            };

            // Act
            try
            {
                Assert.That.IsAssignableTo<IEntity>(data,
                                                    because: "All models should implement IEntity interface",
                                                    fix: "Add IEntity interface implementation to DataModel class");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("TYPE MISMATCH - EXPECTED ASSIGNABLE TYPE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("All models should implement IEntity interface"));
                Assert.IsTrue(ex.Message.Contains("Add IEntity interface implementation to DataModel class"));

                // Verify type info
                Assert.IsTrue(ex.Message.Contains("IEntity"));
                Assert.IsTrue(ex.Message.Contains("DataModel"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void IsAssignableTo_WithComplexInheritance_ShouldPass()
        {
            // Arrange
            var derived = new DerivedClass();

            // Act & Assert - Should NOT throw
            Assert.That.IsAssignableTo<BaseClass>(derived,
                                                  because: "DerivedClass inherits from BaseClass",
                                                  fix: "Verify inheritance chain");
        }

        [TestMethod]
        public void IsAssignableTo_NullWithBeautifulOutput_ShouldFail()
        {
            // Arrange
            IEntity? entity = null;

            // Act
            try
            {
                Assert.That.IsAssignableTo<IEntity>(entity,
                                                    because: "Repository should return valid entity",
                                                    fix: "Check repository GetById implementation");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("TYPE MISMATCH - EXPECTED ASSIGNABLE TYPE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Repository should return valid entity"));
                Assert.IsTrue(ex.Message.Contains("Check repository GetById implementation"));
                Assert.IsTrue(ex.Message.Contains("null"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // Test Helper Classes and Interfaces
        // ============================================================

        private interface IEntity
        {
            int Id { get; set; }
        }

        private sealed class TestUser : IEntity
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public override string ToString()
            {
                return $"TestUser {{ Id: {Id}, Name: {Name} }}";
            }
        }

        private sealed class ApiResponse
        {
            public int StatusCode { get; set; }

            public string Message { get; set; } = string.Empty;

            public override string ToString()
            {
                return $"ApiResponse {{ StatusCode: {StatusCode}, Message: {Message} }}";
            }
        }

        private sealed class ErrorResponse
        {
            public string ErrorCode { get; set; } = string.Empty;

            public string ErrorMessage { get; set; } = string.Empty;

            public override string ToString()
            {
                return $"ErrorResponse {{ ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage} }}";
            }
        }

        private class BaseClass
        {
            public virtual string Type => "Base";
        }

        private sealed class DerivedClass : BaseClass
        {
            public override string Type => "Derived";
        }

        private sealed class DataModel
        {
            public int Id { get; set; }

            public string Value { get; set; } = string.Empty;
        }
    }
}