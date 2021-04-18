using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static Task<T> GetAsAsync<T>(this HttpClient httpClient, string url)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.GetAsErrorResultAsync<T>(url);
            }

            return httpClient.GetAsResultAsync<T>(url);
        }

        private static async Task<T> GetAsResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"GET '{url}' was not success full. Error code: {result.StatusCode}");
        }

        public static async Task<T> GetAsErrorResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"GET '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }
    }
}
