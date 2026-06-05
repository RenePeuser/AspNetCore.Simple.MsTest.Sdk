using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Core
{
    /// <summary>
    /// Tests for Assert.That.AreEqual, AreNotEqual, AreSame, and AreNotSame
    /// </summary>
    [TestClass]
    [TestCategory("Equality")]
    public sealed class AssertThatEqualityTests
    {
        // ============================================================
        // AreEqual Tests
        // ============================================================

        [TestMethod]
        public void AreEqual_WhenValuesAreEqual_ShouldPass()
        {
            // Arrange
            var expected = 42;
            var actual = 42;

            // Act & Assert - Should NOT throw
            Assert.That.AreEqual(expected, actual,
                                 because: "Testing that equal integers pass",
                                 fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreEqual_WhenValuesAreDifferent_ShouldFail()
        {
            // Arrange
            var expected = 42;
            var actual = 99;
            var threw = false;

            // Act
            try
            {
                Assert.That.AreEqual(expected, actual,
                                     because: "Testing that different values fail",
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
        public void AreEqual_WhenStringsDiffer_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expected = "Goku";
            var actual = "Vegeta";

            // Act
            try
            {
                Assert.That.AreEqual(expected, actual,
                                     because: "User name should match the expected value from the database",
                                     fix: "Verify the GetUserName method returns the correct user");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EQUALITY FAILED - VALUES NOT EQUAL"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User name should match the expected value from the database"));
                Assert.IsTrue(ex.Message.Contains("Verify the GetUserName method returns the correct user"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreEqual_WhenComplexObjectsAreEqual_ShouldPass()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku"
            };

            var actual = new TestUser
            {
                Id = 1,
                Name = "Goku"
            };

            // Act & Assert - Should NOT throw
            Assert.That.AreEqual(expected, actual,
                                 because: "User objects with same values should be equal",
                                 fix: "Check TestUser.Equals implementation");
        }

        [TestMethod]
        public void AreEqual_WhenComplexObjectsDiffer_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku",
                Email = "goku@saiyan.com"
            };

            var actual = new TestUser
            {
                Id = 2,
                Name = "Vegeta",
                Email = "vegeta@saiyan.com"
            };

            // Act
            try
            {
                Assert.That.AreEqual(expected, actual,
                                     because: "User retrieved from API should match expected user from test data",
                                     fix: "Check the API endpoint and ensure it returns the correct user by ID");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EQUALITY FAILED - VALUES NOT EQUAL"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("User retrieved from API should match expected user from test data"));
                Assert.IsTrue(ex.Message.Contains("Check the API endpoint and ensure it returns the correct user by ID"));

                // Verify variable names captured
                Assert.IsTrue(ex.Message.Contains("expected"));
                Assert.IsTrue(ex.Message.Contains("actual"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreEqual_WhenNullValuesAreEqual_ShouldPass()
        {
            // Arrange
            string? expected = null;
            string? actual = null;

            // Act & Assert - Should NOT throw
            Assert.That.AreEqual(expected, actual,
                                 because: "Both null values should be considered equal",
                                 fix: "N/A - this should pass");
        }

        // ============================================================
        // AreNotEqual Tests
        // ============================================================

        [TestMethod]
        public void AreNotEqual_WhenValuesAreDifferent_ShouldPass()
        {
            // Arrange
            var expected = 42;
            var actual = 99;

            // Act & Assert - Should NOT throw
            Assert.That.AreNotEqual(expected, actual,
                                    because: "Testing that different integers pass",
                                    fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreNotEqual_WhenValuesAreEqual_ShouldFail()
        {
            // Arrange
            var expected = 42;
            var actual = 42;
            var threw = false;

            // Act
            try
            {
                Assert.That.AreNotEqual(expected, actual,
                                        because: "Testing that equal values fail",
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
        public void AreNotEqual_WhenStringsAreEqual_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expected = "Goku";
            var actual = "Goku";

            // Act
            try
            {
                Assert.That.AreNotEqual(expected, actual,
                                        because: "Each user should have a unique name in the system",
                                        fix: "Verify that the GenerateUniqueName method is working correctly");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EQUALITY FAILED - VALUES ARE EQUAL"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Each user should have a unique name in the system"));
                Assert.IsTrue(ex.Message.Contains("Verify that the GenerateUniqueName method is working correctly"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreNotEqual_WhenComplexObjectsAreDifferent_ShouldPass()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku"
            };

            var actual = new TestUser
            {
                Id = 2,
                Name = "Vegeta"
            };

            // Act & Assert - Should NOT throw
            Assert.That.AreNotEqual(expected, actual,
                                    because: "User objects with different values should not be equal",
                                    fix: "Check TestUser.Equals implementation");
        }

        [TestMethod]
        public void AreNotEqual_WhenComplexObjectsAreEqual_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku",
                Email = "goku@saiyan.com"
            };

            var actual = new TestUser
            {
                Id = 1,
                Name = "Goku",
                Email = "goku@saiyan.com"
            };

            // Act
            try
            {
                Assert.That.AreNotEqual(expected, actual,
                                        because: "Two separate user creation calls should produce distinct user objects",
                                        fix: "Ensure CreateUser generates unique IDs and doesn't return cached instances");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("EQUALITY FAILED - VALUES ARE EQUAL"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Two separate user creation calls should produce distinct user objects"));
                Assert.IsTrue(ex.Message.Contains("Ensure CreateUser generates unique IDs and doesn't return cached instances"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // AreSame Tests
        // ============================================================

        [TestMethod]
        public void AreSame_WhenReferencesAreSame_ShouldPass()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku"
            };

            var actual = expected; // Same reference

            // Act & Assert - Should NOT throw
            Assert.That.AreSame(expected, actual,
                                because: "Testing that same references pass",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreSame_WhenReferencesAreDifferent_ShouldFail()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku"
            };

            var actual = new TestUser
            {
                Id = 1,
                Name = "Goku"
            }; // Different reference

            var threw = false;

            // Act
            try
            {
                Assert.That.AreSame(expected, actual,
                                    because: "Testing that different references fail",
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
        public void AreSame_WhenReferencesAreDifferent_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku",
                Email = "goku@saiyan.com"
            };

            var actual = new TestUser
            {
                Id = 1,
                Name = "Goku",
                Email = "goku@saiyan.com"
            };

            // Act
            try
            {
                Assert.That.AreSame(expected, actual,
                                    because: "Singleton service should return the same instance across calls",
                                    fix: "Verify DI registration uses AddSingleton, not AddTransient or AddScoped");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("REFERENCE EQUALITY FAILED - DIFFERENT INSTANCES"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Singleton service should return the same instance across calls"));
                Assert.IsTrue(ex.Message.Contains("Verify DI registration uses AddSingleton, not AddTransient or AddScoped"));

                // Verify variable names captured
                Assert.IsTrue(ex.Message.Contains("expected"));
                Assert.IsTrue(ex.Message.Contains("actual"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreSame_WhenStringsAreSameReference_ShouldPass()
        {
            // Arrange
            var expected = "Goku";
            var actual = expected; // Same string reference

            // Act & Assert - Should NOT throw
            Assert.That.AreSame(expected, actual,
                                because: "String references should be the same",
                                fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreSame_WhenStringsAreDifferentInstances_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expected = new string(new[]
                                      {
                                          'G', 'o', 'k',
                                          'u'
                                      });

            var actual = new string(new[]
                                    {
                                        'G', 'o', 'k',
                                        'u'
                                    });

            // Act
            try
            {
                Assert.That.AreSame(expected, actual,
                                    because: "String constant should be interned and return same reference",
                                    fix: "Use string.Intern() to ensure string interning for constants");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("REFERENCE EQUALITY FAILED - DIFFERENT INSTANCES"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // AreNotSame Tests
        // ============================================================

        [TestMethod]
        public void AreNotSame_WhenReferencesAreDifferent_ShouldPass()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku"
            };

            var actual = new TestUser
            {
                Id = 1,
                Name = "Goku"
            }; // Different reference

            // Act & Assert - Should NOT throw
            Assert.That.AreNotSame(expected, actual,
                                   because: "Testing that different references pass",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreNotSame_WhenReferencesAreSame_ShouldFail()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku"
            };

            var actual = expected; // Same reference
            var threw = false;

            // Act
            try
            {
                Assert.That.AreNotSame(expected, actual,
                                       because: "Testing that same references fail",
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
        public void AreNotSame_WhenReferencesAreSame_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var expected = new TestUser
            {
                Id = 1,
                Name = "Goku",
                Email = "goku@saiyan.com"
            };

            var actual = expected; // Same reference

            // Act
            try
            {
                Assert.That.AreNotSame(expected, actual,
                                       because: "Cloning method should create a new independent instance",
                                       fix: "Ensure Clone method creates deep copy, not returning 'this' reference");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("REFERENCE EQUALITY FAILED - SAME INSTANCE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("Cloning method should create a new independent instance"));
                Assert.IsTrue(ex.Message.Contains("Ensure Clone method creates deep copy, not returning 'this' reference"));

                // Verify variable names captured
                Assert.IsTrue(ex.Message.Contains("expected"));
                Assert.IsTrue(ex.Message.Contains("actual"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void AreNotSame_WhenListsAreDifferentInstances_ShouldPass()
        {
            // Arrange
            var expected = new List<int>
                           {
                               1,
                               2,
                               3
                           };

            var actual = new List<int>
                         {
                             1,
                             2,
                             3
                         }; // Different list instance

            // Act & Assert - Should NOT throw
            Assert.That.AreNotSame(expected, actual,
                                   because: "Each method call should return new list instance",
                                   fix: "N/A - this should pass");
        }

        [TestMethod]
        public void AreNotSame_WhenCollectionReturnsCache_ShouldFailWithBeautifulOutput()
        {
            // Arrange
            var cachedList = new List<int>
                             {
                                 1,
                                 2,
                                 3
                             };

            var expected = cachedList;
            var actual = cachedList; // Same cached instance

            // Act
            try
            {
                Assert.That.AreNotSame(expected, actual,
                                       because: "GetActiveUsers should return fresh collection, not cached reference",
                                       fix: "Return new List<User>(cachedUsers) instead of cached collection directly");

                Assert.Fail("Expected AssertFailedException");
            }
            catch (AssertFailedException ex)
            {
                // Verify output contains key sections
                Assert.IsTrue(ex.Message.Contains("REFERENCE EQUALITY FAILED - SAME INSTANCE"));
                Assert.IsTrue(ex.Message.Contains("📦 Test Information"));
                Assert.IsTrue(ex.Message.Contains("⚠️ Problem"));
                Assert.IsTrue(ex.Message.Contains("📊 Details"));
                Assert.IsTrue(ex.Message.Contains("💭 Context"));
                Assert.IsTrue(ex.Message.Contains("✅ Suggested Fix"));
                Assert.IsTrue(ex.Message.Contains("GetActiveUsers should return fresh collection, not cached reference"));
                Assert.IsTrue(ex.Message.Contains("Return new List<User>(cachedUsers) instead of cached collection directly"));

                // Print the beautiful output to console
                Console.WriteLine(ex.Message);
            }
        }

        // ============================================================
        // Test Helper Classes
        // ============================================================

        private sealed class TestUser : IEquatable<TestUser>
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;

            public string? Email { get; set; }

            public override string ToString() => $"TestUser {{ Id: {Id}, Name: {Name} }}";

            // Implement value equality for AreEqual tests
            public bool Equals(TestUser? other)
            {
                if (other is null) return false;
                if (ReferenceEquals(this, other)) return true;

                return Id == other.Id && Name == other.Name && Email == other.Email;
            }

            public override bool Equals(object? obj) => obj is TestUser other && Equals(other);

            public override int GetHashCode() => HashCode.Combine(Id, Name, Email);
        }
    }
}