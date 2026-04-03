using System.Net.Http;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddAssertableHttpClientExtension
    {
        public static void AddAssertableHttpClient(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IAssertableHttpClient, AssertableHttpClient>();
        }
    }

    /// <summary>
    /// Represents an HTTP client with assertion capabilities for API testing.
    /// Core service that performs HTTP calls with automatic response assertions.
    /// The HTTP method (GET, POST, PUT, PATCH, DELETE) is determined by the context.
    /// </summary>
    public interface IAssertableHttpClient
    {
        /// <summary>
        /// Performs an HTTP request and asserts the response without deserializing to a specific type.
        /// The HTTP method is determined by the context's HttpMethod property.
        /// </summary>
        /// <param name="context">The assertion context containing all request parameters including the HTTP method.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task AssertAsync(HttpAssertContext context);

        /// <summary>
        /// Performs an HTTP request and asserts the response against an expected result.
        /// The HTTP method is determined by the context's HttpMethod property.
        /// </summary>
        /// <typeparam name="TResult">The expected result type to deserialize the response to.</typeparam>
        /// <param name="context">The assertion context containing all request parameters including the HTTP method.</param>
        /// <returns>A task containing the deserialized response.</returns>
        Task<TResult> AssertAsync<TResult>(HttpAssertContext<TResult> context);
    }


    /// <summary>
    /// Implementation of <see cref="IAssertableHttpClient"/> that wraps an HttpClient
    /// and delegates to the existing extension methods for backward compatibility.
    /// This allows for dependency injection while maintaining the existing assertion logic.
    /// </summary>
    internal sealed class AssertableHttpClient : IAssertableHttpClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssertableHttpClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HttpClient to wrap with assertion capabilities.</param>
        public AssertableHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public Task AssertAsync(HttpAssertContext context)
        {
            // Convert to internal context and delegate to the master assert method
            var internalContext = HttpAssertContextInternalFactory.FromContext(context);
            return HttpClientAssertExtensions.AssertHttpCallAsync(internalContext);
        }

        /// <inheritdoc />
        public Task<TResult> AssertAsync<TResult>(HttpAssertContext<TResult> context)
        {
            // Convert to internal context and delegate to the master assert method
            var internalContext = HttpAssertContextInternalFactory.FromContext(context);
            return HttpClientAssertExtensions.AssertHttpCallAsync(internalContext);
        }
    }
}
