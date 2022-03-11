using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static async Task GetAsAsync(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                return;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET '{url}' was not success full.").ConfigureAwait(false));
        }

        public static async Task<T> GetAsAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET '{url}' was not success full.").ConfigureAwait(false));
        }

        public static async Task<T> GetAsErrorResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"GET '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }
    }
}
