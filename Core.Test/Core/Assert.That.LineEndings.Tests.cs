using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test.Core
{
    /// <summary>
    /// Tests for line ending normalization in JSON comparisons
    /// </summary>
    [TestClass]
    [TestCategory("LineEndings")]
    public sealed class AssertThatLineEndingsTests
    {
        [TestMethod]
        public void ObjectsAreEqual_WithDifferentLineEndings_InStringProperty_ShouldPass()
        {
            // Arrange - Windows vs Unix line endings
            var expectedJson = "{\"content\":\"Line1\\r\\nLine2\\r\\nLine3\"}";
            var currentJson = "{\"content\":\"Line1\\nLine2\\nLine3\"}";

            var expected = JsonSerializer.Deserialize<JsonElement>(expectedJson);
            var current = JsonSerializer.Deserialize<JsonElement>(currentJson);

            // Act & Assert - Should pass because line endings are normalized
            Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
        }

        [TestMethod]
        public void ObjectsAreEqual_WithDifferentLineEndings_InNestedStringProperty_ShouldPass()
        {
            // Arrange - The exact scenario from the bug report
            var expectedJson = @"{
                ""content"": {
                    ""value"": {
                        ""content"": ""# Simple Skill V1\r\nThis is a simple skill with a readme attached.\r\n\r\n## Use cases\r\nThis skill does nothing, it is only referenced in tests.\r\n""
                    }
                }
            }";

            var currentJson = @"{
                ""content"": {
                    ""value"": {
                        ""content"": ""# Simple Skill V1\nThis is a simple skill with a readme attached.\n\n## Use cases\nThis skill does nothing, it is only referenced in tests.\n""
                    }
                }
            }";

            var expected = JsonSerializer.Deserialize<JsonElement>(expectedJson);
            var current = JsonSerializer.Deserialize<JsonElement>(currentJson);

            // Act & Assert - Should pass because line endings are normalized
            Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
        }

        [TestMethod]
        public void ObjectsAreEqual_WithDifferentLineEndings_InArray_ShouldPass()
        {
            // Arrange
            var expectedJson = "{\"lines\":[\"Line1\\r\\n\",\"Line2\\r\\n\",\"Line3\\r\\n\"]}";
            var currentJson = "{\"lines\":[\"Line1\\n\",\"Line2\\n\",\"Line3\\n\"]}";

            var expected = JsonSerializer.Deserialize<JsonElement>(expectedJson);
            var current = JsonSerializer.Deserialize<JsonElement>(currentJson);

            // Act & Assert - Should pass because line endings are normalized
            Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
        }

        [TestMethod]
        public void ObjectsAreEqual_WithActualDifferences_AfterNormalization_ShouldFail()
        {
            // Arrange - Different content, not just line endings
            var expectedJson = "{\"content\":\"Line1\\r\\nLine2\\r\\nLine3\"}";
            var currentJson = "{\"content\":\"Line1\\nDIFFERENT\\nLine3\"}";

            var expected = JsonSerializer.Deserialize<JsonElement>(expectedJson);
            var current = JsonSerializer.Deserialize<JsonElement>(currentJson);

            // Act & Assert - Should fail because content is actually different
            try
            {
                Assert.That.ObjectsAreEqual(expectedObject: expected, currentObject: current);
                Assert.Fail("Expected AssertFailedException to be thrown");
            }
            catch (AssertFailedException ex)
            {
                // Verify the error message shows the actual difference
                var message = ex.Message;
                Assert.IsTrue(message.Contains("content") || message.Contains("Content"));
            }
        }
    }
}
