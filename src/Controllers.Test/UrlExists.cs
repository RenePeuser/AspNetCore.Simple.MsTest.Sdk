using AspNetCore.Simple.MsTest.Sdk;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test
{
    [TestClass]
    [TestCategory("Controller")]
    public class UrlExists : ApiTestBase
    {
        [TestMethod]
        public void Should_Return_True_If_Absolute_Url_Exists()
        {
            Assert.That.IsTrue(Client.UrlExists("https://www.google.de/"),
                               because: "UrlExists has to follow an absolute url out of the test host instead of treating it as a relative route - this test uses a public site as the reachable reference.",
                               fix: "Check that UrlExists passes an absolute uri straight to the HttpClient. If the machine has no internet access this test cannot pass - point it at a locally reachable absolute url instead.");
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