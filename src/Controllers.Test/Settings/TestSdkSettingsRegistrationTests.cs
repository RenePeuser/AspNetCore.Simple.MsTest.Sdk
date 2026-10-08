using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Settings
{
    /// <summary>
    /// Several sdk registrations bind the configured settings on their own - AddEmbeddedFileLocalizer(configuration)
    /// among them. Consumers commonly call such a registration before
    /// <c>AddAssertableHttpClient(configuration, settings => ...)</c>. That delegate used to be dropped silently,
    /// because the first configured registration won: the global DifferenceFunc never ran and every createdAt
    /// suddenly showed up as a snapshot mismatch.
    /// </summary>
    [TestClass]
    [TestCategory("Settings")]
    public sealed class TestSdkSettingsRegistrationTests
    {
        [TestMethod]
        public void ConfigureSettingsMustReachTheInstanceEvenWhenAnEarlierRegistrationAlreadyConfiguredIt()
        {
            var configuration = new ConfigurationBuilder().Build();
            var services = new ServiceCollection();

            services.AddEmbeddedFileLocalizer(configuration);
            services.AddAssertableHttpClient(configuration, settings =>
            {
                settings.DifferenceFunc = IgnoreEverything;
                settings.WriteResponse = false;
            });

            using var provider = services.BuildServiceProvider();
            var settings = provider.GetRequiredService<TestSdkSettings>();

            Assert.That.AreEqual(1,
                                 services.Count(descriptor => descriptor.ServiceType == typeof(TestSdkSettings)),
                                 because: "The sdk works with exactly one settings instance.",
                                 fix: "Check AddTestSdkSettings - it must replace or reuse the registration, never add a second one.");

            Assert.That.IsFalse(settings.WriteResponse,
                                because: "WriteResponse = false was set by the consumer's configureSettings of AddAssertableHttpClient.",
                                fix: "AddTestSdkSettings(configuration, configureSettings) must apply configureSettings to an already configured instance instead of returning early.");

            Assert.That.IsTrue(settings.DifferenceFunc == IgnoreEverything,
                               because: "The consumer's global DifferenceFunc must survive an earlier configured registration.",
                               fix: "AddTestSdkSettings(configuration, configureSettings) must apply configureSettings to an already configured instance instead of returning early.");
        }

        [TestMethod]
        public void EveryConfigureSettingsMustBeAppliedInCallOrder()
        {
            var configuration = new ConfigurationBuilder().Build();
            var services = new ServiceCollection();

            services.AddTestSdkSettings(configuration, settings => settings.ResponseFolderName = "First");
            services.AddTestSdkSettings(configuration, settings => settings.RequestFolderName = "Second");
            services.AddTestSdkSettings(configuration, settings => settings.ResponseFolderName = "Third");

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

        [TestMethod]
        public void AddAssertableHttpClientCalledTwiceMustApplyBothSettingsAndHandOverOnce()
        {
            var configuration = new ConfigurationBuilder().Build();
            var services = new ServiceCollection();

            // The consumer from registerServices, then ApiTestBase<T> itself.
            services.AddAssertableHttpClient(configuration, settings => settings.WriteResponse = false);
            services.AddAssertableHttpClient(configuration, settings => settings.ResponseFolderName = "Snapshots");

            using var provider = services.BuildServiceProvider();
            var settings = provider.GetRequiredService<TestSdkSettings>();

            Assert.That.AreEqual(1,
                                 services.Count(descriptor => descriptor.ServiceType == typeof(IHostedService)),
                                 because: "The provider is handed to the static asserts once per host, no matter how often the sdk is registered.",
                                 fix: "Register the ServiceProviderHandover via TryAddEnumerable.");

            Assert.That.AreEqual("Snapshots",
                                 settings.ResponseFolderName,
                                 because: "The second configureSettings must reach the one settings instance as well.",
                                 fix: "AddTestSdkSettings(configuration, configureSettings) must apply configureSettings to an already configured instance.");

            Assert.That.IsFalse(settings.WriteResponse,
                                because: "The first configureSettings must survive the second registration.",
                                fix: "AddTestSdkSettings(configuration, configureSettings) must never replace an already configured instance.");
        }

        private static IEnumerable<Difference> IgnoreEverything(ImmutableList<Difference> differences)
        {
            return [];
        }
    }
}