using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static Task<T> PutAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.PutAsErrorResultAsync<T>(url, body);
            }

            return httpClient.PutAsResultAsync<T>(url, body);
        }

        private static async Task<T> PutAsResultAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PutAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PUT '{url}' was not success full. Error code: {result.StatusCode}");
        }

        public static async Task<T> PutAsErrorResultAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PutAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PUT '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }


        public static Task<T> PutAsJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.PutAsErrorResultWithJsonStringAsync<T>(url, jsonContent);
            }

            return httpClient.PutAsResultWithJsonStringAsync<T>(url, jsonContent);
        }

        private static async Task<T> PutAsResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PutAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PUT '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }

        public static async Task<T> PutAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PutAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode is false)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PUT '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }


        public static Task<HttpResponseMessage> PutAsJsonStringAsync(this HttpClient httpClient, string url, string jsonContent)
        {
            return httpClient.PutAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json));
        }
    }
}
