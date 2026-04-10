using AspNetCore.Simple.MsTest.Sdk;
using Extensions.Pack;

namespace Controller.Test
{
    [TestClass]
    public class UrlExists : ApiTestBase
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
