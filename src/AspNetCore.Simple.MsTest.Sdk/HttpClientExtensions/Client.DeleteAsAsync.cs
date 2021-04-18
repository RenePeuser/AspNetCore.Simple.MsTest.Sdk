using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static Task<T> DeleteAsAsync<T>(this HttpClient httpClient, string url)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.DeleteAsErrorResultAsync<T>(url);
            }

            return httpClient.DeleteAsResultAsync<T>(url);
        }

        private static async Task<T> DeleteAsResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"DELETE with'{url}' was not success full. Error code: {result.StatusCode}");
        }

        public static async Task<T> DeleteAsErrorResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"DELETE '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }
    }
}
