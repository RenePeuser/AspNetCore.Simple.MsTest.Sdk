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
            var expected = new
                           {
                               Name = "Goku",
                               Age = 30,
                               Power = 9000
                           };

            var actual = new
                         {
                             Name = "Goku",
                             Age = 30,
                             Power = 9000
                         };

            // Act & Assert - Should pass now
            Assert.That.ObjectsAreEqual(expected, actual);
        }

        [TestMethod]
        public void Test_HumanMode_PrimitiveComparison_ShouldShowFormattedOutput()
        {
            // Arrange
            var expected = "Hello World";
            var actual = "Hello World";

            // Act & Assert - Should pass now
            Assert.That.ObjectsAreEqual(expected, actual);
        }
    }
}