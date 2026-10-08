using System.Text.Json.Nodes;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Settings
{
    /// <summary>
    /// AddAssertableHttpClient is the only call a consumer makes. Once the host starts, the static asserts
    /// must resolve from that host's container - otherwise they silently run on the sdk's own fallback
    /// container: without the consumer's settings and without any endpoint to validate against.
    /// </summary>
    [TestClass]
    [TestCategory("Settings")]
    public sealed class ServiceProviderHandoverTests : ApiTestBase
    {
        [TestMethod]
        public void StaticAssertsMustUseTheSettingsOfTheStartedHost()
        {
            // ApiTestBase registers TestHelpers.IgnoreIdDifferences as the global DifferenceFunc. The
            // fallback container has no such func - there the differing id fails the assert.
            Assert.That.ObjectsAreEqual(JsonNode.Parse( /*lang=json,strict*/ """{"id":1,"name":"Son"}"""),
                                        JsonNode.Parse( /*lang=json,strict*/ """{"id":2,"name":"Son"}"""));
        }
    }
}
