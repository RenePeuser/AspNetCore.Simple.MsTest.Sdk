using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static async Task GetAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.GetAsync(url).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET with'{url}' was successful, but unauthorized was expected").ConfigureAwait(false));
        }

        public static async Task GetAsAsync(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET '{url}' was not successful.").ConfigureAwait(false));
        }

        public static async Task<T> GetAsAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET '{url}' was not successful.").ConfigureAwait(false));
        }

        public static async Task<T> GetAsErrorResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET '{url}' was successful, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }
    }
}
