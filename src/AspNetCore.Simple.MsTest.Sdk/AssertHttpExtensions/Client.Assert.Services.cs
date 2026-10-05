using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    ///     The one place every static assert entry - http and object route alike - resolves its services
    ///     and its <see cref="TestSdkSettings" /> from. There is no second container, no hand-wired copy
    ///     and no global setting anywhere else.
    /// </summary>
    public static partial class HttpClientAssertExtensions
    {
        private static readonly Lock ServiceProviderGate = new();

        private static IServiceProvider? _serviceProvider;

        /// <summary>
        ///     Hands the host's provider to all static assert extensions.
        ///     Call this method once during test initialization (e.g., in [AssemblyInitialize])
        ///     after registering services via services.AddAssertableHttpClient().
        ///     The provider has to stay alive for the whole test run - it is resolved from on every assert.
        /// </summary>
        /// <param name="serviceProvider">The service provider containing registered services</param>
        public static void Setup(IServiceProvider serviceProvider)
        {
            lock (ServiceProviderGate)
            {
                _serviceProvider = serviceProvider;
            }
        }

        /// <summary>
        ///     Configures the sdk for test projects without a host - pure object asserts, for example.
        ///     Call it once in [AssemblyInitialize]; calling it again replaces the settings.
        ///     With a host, pass the settings to <c>AddAssertableHttpClient</c> / <c>AddTestSdkSettings</c> instead.
        /// </summary>
        /// <param name="configureSettings">Code-only settings applied on top of the environment variables.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void Setup(Action<TestSdkSettings> configureSettings)
        {
            // The last frame where the consumer is still the caller - see ITextDecoratorProvider.
            var consumerAssembly = Assembly.GetCallingAssembly();

            lock (ServiceProviderGate)
            {
                _serviceProvider = CreateDefaultServiceProvider(consumerAssembly, configureSettings);
            }
        }

        /// <summary>
        ///     Resolves a service for an assert issued by <paramref name="consumerAssembly" />.
        /// </summary>
        internal static T GetService<T>(Assembly consumerAssembly)
            where T : notnull
        {
            var serviceProvider = Volatile.Read(ref _serviceProvider) ?? EnsureDefaultServiceProvider(consumerAssembly);

            return serviceProvider.GetRequiredService<T>();
        }

        /// <summary>
        ///     The json options of the application - they live in <see cref="TestSdkSettings" />.
        /// </summary>
        internal static JsonSerializerOptions JsonSerializerOptionsFor(Assembly consumerAssembly)
        {
            return GetService<JsonSerializerOptions>(consumerAssembly);
        }

        private static IServiceProvider EnsureDefaultServiceProvider(Assembly consumerAssembly)
        {
            lock (ServiceProviderGate)
            {
                return _serviceProvider ??= CreateDefaultServiceProvider(consumerAssembly, configureSettings: null);
            }
        }

        /// <summary>
        ///     Until a test hands over its host's provider - and for pure object asserts, which never do -
        ///     the extensions run on the sdk's own container, built from the very same registrations.
        ///     It is bound to the first consumer assembly asking: every test assembly runs in its own
        ///     test host process, so that one decides plain vs ANSI output for the whole run.
        ///     Without a host there is no endpoint registry - the first endpoint validation then reports
        ///     the missing Setup instead of failing cryptically.
        /// </summary>
        private static ServiceProvider CreateDefaultServiceProvider(Assembly consumerAssembly,
                                                                    Action<TestSdkSettings>? configureSettings)
        {
            // Without a host the environment is the only configuration source - TestSdkSettings__OutputMode
            // and friends bind exactly as they do in a host.
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            var services = new ServiceCollection();

            services.AddAssertableHttpClient(configuration, configureSettings, consumerAssembly);

            // No EndpointDataSource without a host.
            services.Replace(ServiceDescriptor.Singleton<IEndpointProvider, EmptyEndpointProvider>());

            return services.BuildServiceProvider();
        }
    }
}