using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddTestSdkSettingsExtension
    {
        // Identifies the fallback registration, so a configured one can replace exactly that.
        private static readonly Func<IServiceProvider, TestSdkSettings> DefaultSettings = _ => new TestSdkSettings();

        /// <summary>
        /// Registers the default settings for a service that is registered on its own. Every service
        /// depending on <see cref="TestSdkSettings"/> calls this; a configured registration replaces it,
        /// no matter which one came first.
        /// </summary>
        public static void AddTestSdkSettings(this IServiceCollection services)
        {
            services.TryAddSingleton(DefaultSettings);
        }

        /// <summary>
        /// Registers the one settings instance of the sdk. Precedence: defaults, then the
        /// <c>TestSdkSettings</c> configuration section (appsettings, environment variables such as
        /// <c>TestSdkSettings__OutputMode</c>), then <paramref name="configureSettings"/>.
        /// The first configured registration wins - call it before <c>AddAssertableHttpClient</c> to
        /// configure the settings from your own <c>registerServices</c> callback.
        /// </summary>
        public static void AddTestSdkSettings(this IServiceCollection services,
                                              IConfiguration configuration,
                                              Action<TestSdkSettings>? configureSettings = null)
        {
            var registered = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(TestSdkSettings));

            if (registered is not null && ReferenceEquals(registered.ImplementationFactory, DefaultSettings).IsFalse())
            {
                return;
            }

            if (configuration.TryGetSettings<TestSdkSettings>(out var settings).IsFalse())
            {
                settings = new TestSdkSettings();
            }

            configureSettings?.Invoke(settings);

            services.Replace(ServiceDescriptor.Singleton(settings));
        }
    }

#pragma warning disable CA1819 // Properties should not return arrays
    public record TestSdkSettings
    {
        public string TestMethodAttribute { get; set; } = "[TestMethod]";

        public string ResponseFolderName { get; set; } = "Responses";

        public string RequestFolderName { get; set; } = "Requests";

        public string[] LegacyResponseFolderNames { get; set; } =
            [
                "Result",
                "Response",
                "Results",
                "Output"
            ];

        public string[] LegacyRequestFolderName { get; set; } =
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
        public string[] VolatileHeaderNames { get; set; } =
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

        /// <summary>
        /// Controls the output format for assertion failures.
        ///
        /// - Human: Beautiful console output optimized for human debugging (default)
        /// - Ai: Structured JSON output optimized for AI agents and automated debugging
        /// - Hybrid: Both human-readable and JSON output combined
        ///
        /// Configure via:
        /// - Environment variable: TestSdkSettings__OutputMode=ai
        /// - appsettings.json: "TestSdkSettings": { "OutputMode": "Ai" }
        /// </summary>
        public OutputMode OutputMode { get; set; } = OutputMode.Human;

        /// <summary>
        /// Records the current response into its snapshot for every assert - a Debug-only developer
        /// feature. Configure via environment variable <c>TestSdkSettings__WriteResponse=true</c>.
        /// </summary>
        public bool WriteResponse { get; set; }

        /// <summary>
        /// Skips the endpoint validation for every assert, not only for the ones passing
        /// <c>skipEndpointValidation: true</c>.
        /// </summary>
        public bool SkipEndpointValidation { get; set; }

        /// <summary>
        /// Prints the bearer token in the curl output instead of masking it.
        /// </summary>
        public bool ShowTokenInCurl { get; set; }

        /// <summary>
        /// The api's json options - both sides of every diff are written with them. Code only.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions { get; set; } = JsonSerializerExtension.CreateDefaultOptions();

        /// <summary>
        /// Global list transform applied to every set of differences before the per-assert
        /// <c>differenceFunc</c>. Code only.
        /// </summary>
        public Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; set; } = differences => differences;

        /// <summary>
        /// Global per-difference predicate. Return <c>true</c> to keep a difference,
        /// <c>false</c> to ignore it. The SDK iterates internally, so you only describe
        /// the condition (e.g. <c>d => d.MemberPath != "id"</c>) instead of writing a loop.
        /// Applied in addition to (and after) <see cref="DifferenceFunc"/> and any
        /// per-assert filter. Defaults to keeping every difference. Code only.
        /// </summary>
        public Predicate<Difference> DifferenceFilter { get; set; } = _ => true;

        /// <summary>
        /// Global predicate marking arrays whose element ORDER carries no meaning - an OpenAPI
        /// <c>anyOf</c>, a set of tags, anything a producer emits in a different order per run.
        /// Their elements are MATCHED against each other instead of compared index by index, so a
        /// pure reordering is no longer a difference while a missing or changed element still is.
        /// Neither document is reordered, which keeps every reported path pointing at the element
        /// it names.
        /// <para>
        /// Decide per array, not per bare name: <c>array => array.PropertyName is "anyOf"</c> makes
        /// EVERY anyOf order blind, <c>array => array.Path == "components.schemas.Pet.anyOf"</c>
        /// only that one. <see cref="JsonArrayContext.Path"/> is index free.
        /// </para>
        /// <para>
        /// This is the escape hatch for payloads you do not control. When the type is yours, the
        /// per-assert <c>orderFunc</c> is the better tool: it is type safe and it also normalizes
        /// what gets WRITTEN into the snapshot. And when the producer's order is nondeterministic
        /// at all, every client sees that - fixing it at the source beats hiding it in the test.
        /// </para>
        /// Defaults to <c>null</c>, which compares every array by index. Code only.
        /// </summary>
        public Predicate<JsonArrayContext>? OrderIndependentArrayFilter { get; set; }

        /// <summary>
        /// Where the sdk writes curl commands and diagnostic lines. Code only.
        /// </summary>
        public Action<string> LogAction { get; set; } = Console.WriteLine;

    }
#pragma warning restore CA1819 // Properties should not return arrays
}