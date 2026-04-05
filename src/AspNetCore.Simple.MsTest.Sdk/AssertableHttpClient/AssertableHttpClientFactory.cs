using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    /// <summary>
    /// Extension methods for registering assertable HTTP client services.
    /// </summary>
    public static class AddAssertableHttpClientFactoryExtension
    {
        /// <summary>
        /// Registers the assertable HTTP client services in the DI container.
        /// The HttpClient is passed via the context when calling AssertAsync, not in the constructor.
        /// </summary>
        public static void AddAssertableHttpClientFactory(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IAssertableHttpClient, AssertableHttpClient>();
        }
    }
}
