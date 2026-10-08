using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Settings
{
    /// <summary>
    /// <c>AddAssertableHttpClient(configuration, settings => ...)</c> is the only registration call, and it
    /// is commonly made twice: by the consumer from <c>registerServices</c> and by <c>ApiTestBase&lt;T&gt;</c>
    /// right after. The second call used to drop the consumer's delegate silently, because the first
    /// configured registration won: the global DifferenceFunc never ran and every createdAt suddenly
    /// showed up as a snapshot mismatch.
    /// </summary>
    [TestClass]
    [TestCategory("Settings")]
    public sealed class TestSdkSettingsRegistrationTests
    {
        [TestMethod]
        public void AddAssertableHttpClientCalledTwiceMustApplyBothSettings()
        {
            var configuration = new ConfigurationBuilder().Build();
            var services = new ServiceCollection();

            // The consumer from registerServices, then ApiTestBase<T> itself.
            services.AddAssertableHttpClient(configuration, settings => settings.WriteResponse = false);
            services.AddAssertableHttpClient(configuration, settings => settings.ResponseFolderName = "Snapshots");

            using var provider = services.BuildServiceProvider();
            var settings = provider.GetRequiredService<TestSdkSettings>();

            Assert.That.AreEqual("Snapshots",
                                 settings.ResponseFolderName,
                                 because: "The second configureSettings must reach the one settings instance as well.",
                                 fix: "AddTestSdkSettings(configuration, configureSettings) must apply configureSettings to an already configured instance instead of returning early.");

            Assert.That.IsFalse(settings.WriteResponse,
                                because: "The first configureSettings must survive the second registration.",
                                fix: "AddTestSdkSettings(configuration, configureSettings) must never replace an already configured instance.");
        }

        [TestMethod]
        public void EveryConfigureSettingsMustBeAppliedInCallOrder()
        {
            var configuration = new ConfigurationBuilder().Build();
            var services = new ServiceCollection();

            services.AddAssertableHttpClient(configuration, settings => settings.ResponseFolderName = "First");
            services.AddAssertableHttpClient(configuration, settings => settings.RequestFolderName = "Second");
            services.AddAssertableHttpClient(configuration, settings => settings.ResponseFolderName = "Third");

            using var provider = services.BuildServiceProvider();
            var settings = provider.GetRequiredService<TestSdkSettings>();

            Assert.That.AreEqual("Third",
                                 settings.ResponseFolderName,
                                 because: "A later configureSettings overrides an earlier one for the same property.",
                                 fix: "Apply every configureSettings to the one instance, in call order.");

            Assert.That.AreEqual("Second",
                                 settings.RequestFolderName,
                                 because: "A configureSettings touching another property must not be lost.",
                                 fix: "Apply every configureSettings to the one instance, in call order.");
        }
    }
}
