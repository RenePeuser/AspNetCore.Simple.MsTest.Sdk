using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    public class UrlExists : MsTestBase
    {
        [TestMethod]
        public void Should_Return_True_If_Absolute_Url_Exists()
        {
            Assert.IsTrue(Client.UrlExists("https://www.google.de/"));
        }

        [TestMethod]
        public void Should_Assert_Url_Exists_Correctly()
        {
            Client.AssertUrlExists("https://www.google.de/");
        }

        [TestMethod]
        public void Should_Not_Assert_Url_Exists_Correctly()
        {
            Client.AssertUrlNotExists("https://www.g212le.de/");
        }
    }
}