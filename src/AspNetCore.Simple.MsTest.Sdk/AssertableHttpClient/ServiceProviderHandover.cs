using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    internal static class AddServiceProviderHandoverExtension
    {
        internal static void AddServiceProviderHandover(this IServiceCollection services)
        {
            // Enumerable - AddAssertableHttpClient may run more than once, the handover must not.
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IHostedService, ServiceProviderHandover>());
        }
    }

    /// <summary>
    ///     Hands the host's own provider to the static assert extensions the moment the host starts -
    ///     one container, so the asserts see exactly the singletons the application under test sees.
    ///     Any host works: <c>ApiTestBase&lt;T&gt;</c>, a custom <c>WebApplicationFactory</c>, a generic host.
    /// </summary>
    internal sealed class ServiceProviderHandover(IServiceProvider serviceProvider) : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            HttpClientAssertExtensions.Setup(serviceProvider);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
