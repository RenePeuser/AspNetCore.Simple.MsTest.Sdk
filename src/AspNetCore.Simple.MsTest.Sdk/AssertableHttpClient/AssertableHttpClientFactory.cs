using System.Net.Http;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddAssertableHttpClientFactoryExtension
    {
        public static void AddAssertableHttpClientFactory(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IAssertableHttpClientFactory, AssertableHttpClientFactory>();
        }
    }

    /// <summary>
    /// Factory for creating <see cref="IAssertableHttpClient"/> instances.
    /// This factory allows for dependency injection and proper management of HTTP client instances.
    /// </summary>
    public interface IAssertableHttpClientFactory
    {
        /// <summary>
        /// Creates an assertable HTTP client that wraps the provided HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient to wrap with assertion capabilities.</param>
        /// <returns>An assertable HTTP client instance.</returns>
        IAssertableHttpClient Create(HttpClient httpClient);
    }

    /// <summary>
    /// Factory implementation for creating <see cref="IAssertableHttpClient"/> instances.
    /// This factory can be registered in a dependency injection container and used throughout tests.
    /// </summary>
    public sealed class AssertableHttpClientFactory : IAssertableHttpClientFactory
    {
        /// <inheritdoc />
        public IAssertableHttpClient Create(HttpClient httpClient)
        {
            return new AssertableHttpClient(httpClient);
        }
    }
}
