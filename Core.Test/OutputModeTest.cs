using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Test
{
    [TestClass]
    public class OutputModeTests
    {
    [TestMethod]
    public void Test_HumanMode_ObjectComparison_ShouldShowFormattedOutput()
    {
        // Arrange
        var expected = new { Name = "Goku", Age = 30, Power = 9000 };
        var actual = new { Name = "Vegeta", Age = 30, Power = 8500 };

        // Act & Assert - This will fail intentionally to show the output
        Assert.That.ObjectsAreEqual(expected, actual);
    }

    [TestMethod]
    public void Test_HumanMode_PrimitiveComparison_ShouldShowFormattedOutput()
    {
        // Arrange
        var expected = "Hello World";
        var actual = "Hello Universe";

        // Act & Assert - This will fail intentionally
        Assert.That.ObjectsAreEqual(expected, actual);
    }
    }
}
