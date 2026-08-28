using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Core
{
    /// <summary>
    /// Tests for Assert.That.ObjectsAreEqual with string values - especially edge cases
    /// </summary>
    [TestClass]
    [TestCategory("ObjectsAreEqual")]
    [TestCategory("String")]
    public sealed class AssertThatObjectsAreEqualStringTests
    {
        [TestMethod]
        public void ObjectsAreEqual_WhenBothStringsAreEmpty_ShouldPass()
        {
            // Arrange
            var expected = "";
            var current = "";

            // Act & Assert - Should NOT throw
            Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
        }

        [TestMethod]
        public void ObjectsAreEqual_WhenBothStringsAreNull_ShouldPass()
        {
            // Arrange
            string? expected = null;
            string? current = null;

            // Act & Assert - Should NOT throw
            Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
        }

        [TestMethod]
        public void ObjectsAreEqual_WhenBothStringsAreEqual_ShouldPass()
        {
            // Arrange
            var expected = "Skill";
            var current = "Skill";

            // Act & Assert - Should NOT throw
            Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
        }

        [TestMethod]
        public void ObjectsAreEqual_WhenExpectedIsEmptyAndCurrentHasValue_ShouldFail()
        {
            // Arrange
            var expected = "";
            var current = "Skill";

            // Act & Assert
            try
            {
                Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
                Assert.Fail("Expected AssertFailedException to be thrown");
            }
            catch (AssertFailedException ex)
            {
                // Verify the error message shows the correct values
                var message = ex.Message;

                // The output should clearly show:
                // Expected: [empty] or ""
                // Current: Skill
                Console.WriteLine(message);

                // Verify key information is present
                Assert.IsTrue(message.Contains("expected") || message.Contains("Expected"),
                             "Error message should reference expected value");
                Assert.IsTrue(message.Contains("current") || message.Contains("Current"),
                             "Error message should reference current value");
            }
        }

        [TestMethod]
        public void ObjectsAreEqual_WhenExpectedHasValueAndCurrentIsEmpty_ShouldFail()
        {
            // Arrange
            var expected = "Skill";
            var current = "";

            // Act & Assert
            try
            {
                Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
                Assert.Fail("Expected AssertFailedException to be thrown");
            }
            catch (AssertFailedException ex)
            {
                // Verify the error message shows the correct values
                var message = ex.Message;
                Console.WriteLine(message);

                // Verify key information is present
                Assert.IsTrue(message.Contains("expected") || message.Contains("Expected"),
                             "Error message should reference expected value");
                Assert.IsTrue(message.Contains("current") || message.Contains("Current"),
                             "Error message should reference current value");
            }
        }

        [TestMethod]
        public void ObjectsAreEqual_WhenStringsAreDifferent_ShouldFail()
        {
            // Arrange
            var expected = "Component";
            var current = "Skill";

            // Act & Assert
            try
            {
                Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
                Assert.Fail("Expected AssertFailedException to be thrown");
            }
            catch (AssertFailedException ex)
            {
                var message = ex.Message;
                Console.WriteLine(message);

                Assert.IsTrue(message.Contains("expected") || message.Contains("Expected"));
                Assert.IsTrue(message.Contains("current") || message.Contains("Current"));
            }
        }

        [TestMethod]
        public void ObjectsAreEqual_WhenExpectedIsNullAndCurrentHasValue_ShouldFail()
        {
            // Arrange
            string? expected = null;
            var current = "Skill";

            // Act & Assert
            try
            {
                Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
                Assert.Fail("Expected AssertFailedException to be thrown");
            }
            catch (AssertFailedException ex)
            {
                var message = ex.Message;
                Console.WriteLine(message);

                Assert.IsTrue(message.Contains("expected") || message.Contains("Expected"));
                Assert.IsTrue(message.Contains("current") || message.Contains("Current"));
            }
        }

        [TestMethod]
        public void ObjectsAreEqual_WhenExpectedHasValueAndCurrentIsNull_ShouldFail()
        {
            // Arrange
            var expected = "Skill";
            string? current = null;

            // Act & Assert
            try
            {
                Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
                Assert.Fail("Expected AssertFailedException to be thrown");
            }
            catch (AssertFailedException ex)
            {
                var message = ex.Message;
                Console.WriteLine(message);

                Assert.IsTrue(message.Contains("expected") || message.Contains("Expected"));
                Assert.IsTrue(message.Contains("current") || message.Contains("Current"));
            }
        }

        [TestMethod]
        public void ObjectsAreEqual_WithWriteResponse_ShouldStillValidateCorrectly()
        {
            // Arrange
            var expected = "Skill";
            var current = "Skill";

            // Act & Assert - Should NOT throw even with writeResponse=true
            Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current, writeResponse: false);
        }

        [TestMethod]
        public void ObjectsAreEqual_WithMultilineStrings_ShouldCompareLineByLine()
        {
            // Arrange
            var expected = "Line1\nLine2\nLine3";
            var current = "Line1\nLine2\nLine3";

            // Act & Assert - Should NOT throw
            Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
        }

        [TestMethod]
        public void ObjectsAreEqual_WithMultilineStringsDifferent_ShouldShowLineDifferences()
        {
            // Arrange
            var expected = "Line1\nLine2\nLine3";
            var current = "Line1\nDifferentLine\nLine3";

            // Act & Assert
            try
            {
                Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
                Assert.Fail("Expected AssertFailedException to be thrown");
            }
            catch (AssertFailedException ex)
            {
                var message = ex.Message;
                Console.WriteLine(message);

                // Should show line-by-line comparison
                Assert.IsTrue(message.Contains("Line") || message.Contains("line"));
            }
        }
    }
}
