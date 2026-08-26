using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddTestSdkSettingsExtension
    {
        internal static void AddTestSdkSettings(this IServiceCollection services,
                                                IConfiguration configuration)
        {
            if (configuration.TryGetSettings<TestSdkSettings>(out var settings).IsFalse())
            {
                settings = new TestSdkSettings();
            }

            services.AddSingletonIfNotExists(settings);
        }
    }

#pragma warning disable CA1819 // Properties should not return arrays
    public record TestSdkSettings
    {
        public string TestMethodAttribute { get; init; } = "[TestMethod]";

        public string ResponseFolderName { get; init; } = "Responses";

        public string RequestFolderName { get; init; } = "Requests";

        public string[] LegacyResponseFolderNames { get; init; } =
            [
                "Result",
                "Response",
                "Results",
                "Output"
            ];

        public string[] LegacyRequestFolderName { get; init; } =
            [
                "Payloads",
                "Payload",
                "Requests",
                "Request"
            ];

        /// <summary>
        /// Response headers that change on every single call. They carry no comparison value, but they
        /// used to be recorded into the snapshot envelope and then had to match - so a re-recorded
        /// snapshot showed up as noise in every diff and every review.
        ///
        /// They are dropped both when a snapshot is written and before it is compared, so existing
        /// snapshots that still carry one do not turn red.
        ///
        /// Override per project via configuration to add your own (a correlation id, a build stamp):
        /// <code>
        /// "TestSdkSettings": { "VolatileHeaderNames": [ "traceparent", "X-My-Correlation-Id" ] }
        /// </code>
        /// Note that this REPLACES the defaults - list every name you want dropped.
        /// </summary>
        public string[] VolatileHeaderNames { get; init; } =
            [
                // W3C trace context - a new value per request by definition.
                "traceparent",
                "tracestate",
                "baggage",

                // Vendor tracing and correlation.
                "X-Amzn-Trace-Id",
                "X-Cloud-Trace-Context",
                "X-Correlation-Id",
                "X-Request-Id",
                "Request-Id",
                "Request-Context",

                // Wall clock and timing.
                "Date",
                "Age",
                "Server-Timing",
                "X-Runtime",

                // Changes with every build or host, never with the behaviour under test.
                "X-Powered-By"
            ];
    }
#pragma warning restore CA1819 // Properties should not return arrays
}