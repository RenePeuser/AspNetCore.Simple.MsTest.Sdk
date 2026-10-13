using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        private const string UnauthorizedToken = "Unauthorized token";

        /// <summary>
        /// Sends with an invalid bearer token on THIS request only. The <c>Assert…AsUnauthorizedAsync</c> methods used to
        /// swap <see cref="HttpClient.DefaultRequestHeaders"/> and restore them afterwards: a test running in parallel on
        /// the same client sent the invalid token meanwhile, and a call that threw never restored the original - every
        /// later test of the session ran unauthorized. A header on the request message wins over the client's default
        /// and leaves the client untouched.
        /// </summary>
        private static async Task<HttpResponseMessage> SendWithInvalidTokenAsync(HttpClient httpClient,
                                                                                 HttpMethod method,
                                                                                 string url,
                                                                                 HttpContent? content)
        {
            using var request = new HttpRequestMessage(method, url) { Content = content };

            return await SendWithInvalidTokenAsync(httpClient, request).ConfigureAwait(false);
        }

        /// <inheritdoc cref="SendWithInvalidTokenAsync(HttpClient, HttpMethod, string, HttpContent)"/>
        private static Task<HttpResponseMessage> SendWithInvalidTokenAsync(HttpClient httpClient,
                                                                           HttpRequestMessage request)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", UnauthorizedToken);

            return httpClient.SendAsync(request);
        }
    }
}
