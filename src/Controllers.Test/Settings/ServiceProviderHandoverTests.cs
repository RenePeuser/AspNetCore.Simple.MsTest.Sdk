using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.Extensions.DependencyInjection;
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
        public void StaticAssertsMustResolveFromTheStartedHost()
        {
            var hostSettings = Services.GetRequiredService<TestSdkSettings>();
            var assertSettings = HttpClientAssertExtensions.GetService<TestSdkSettings>(typeof(ServiceProviderHandoverTests).Assembly);

            Assert.That.IsTrue(ReferenceEquals(hostSettings, assertSettings),
                               because: "The static asserts must see exactly the settings the consumer passed to AddAssertableHttpClient.",
                               fix: "AddAssertableHttpClient must register the ServiceProviderHandover, which hands the started host's provider over.");
        }
    }
}
