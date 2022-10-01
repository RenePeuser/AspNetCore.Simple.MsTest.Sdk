using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static async Task DeleteAsAsync(this HttpClient httpClient, string url)
        {
            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return;
            }

            var responseInfoAsync = await result.GetResponseInfoAsync(nameof(result.IsSuccessStatusCode)).ConfigureAwait(false);

            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task<T> DeleteAsAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return await result.ParseResultAsync<T>().ConfigureAwait(false);
            }

            var responseInfoAsync = await result.GetResponseInfoAsync(nameof(result.IsSuccessStatusCode)).ConfigureAwait(false);

            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task<T> DeleteAsErrorResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                return await result.ParseResultAsync<T>().ConfigureAwait(false);
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync("Not successful").ConfigureAwait(false));
        }
    }
}
