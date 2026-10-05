using System;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Controllers.Test
{
    /// <summary>
    ///     Base class for tests that exercise sdk services directly, without a running host.
    ///     The container is built exactly the way a consumer without a host would build it.
    /// </summary>
    public abstract class SdkTestBase
    {
        private static readonly Lazy<ServiceProvider> LazyServices = new(CreateServices);

        protected static IServiceProvider Services => LazyServices.Value;

        private static ServiceProvider CreateServices()
        {
            var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            var services = new ServiceCollection();

            services.AddAssertableHttpClient(configuration, consumerAssembly: typeof(SdkTestBase).Assembly);

            return services.BuildServiceProvider();
        }
    }
}