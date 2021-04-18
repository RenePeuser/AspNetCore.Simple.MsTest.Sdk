using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static Task<T> PostAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.PostAsErrorResultAsync<T>(url, body);
            }

            return httpClient.PostAsResultAsync<T>(url, body);
        }

        private static async Task<T> PostAsResultAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST '{url}' was not success full. Error code: {result.StatusCode}");
        }

        public static async Task<T> PostAsErrorResultAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }



        public static Task<T> PostAsAsync<T>(this HttpClient httpClient, string url)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.PostAsErrorResultAsync<T>(url);
            }

            return httpClient.PostAsResultAsync<T>(url);
        }

        private static async Task<T> PostAsResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }

        public static async Task<T> PostAsErrorResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }



        public static Task<T> PostAsJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.PostAsErrorResultWithJsonStringAsync<T>(url, jsonContent);
            }

            return httpClient.PostAsResultWithJsonStringAsync<T>(url, jsonContent);
        }

        private static async Task<T> PostAsResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST '{url}' was not success full. Error code: {postResponse.StatusCode}");
        }

        public static async Task<T> PostAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode is false)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }


        public static Task<HttpResponseMessage> PostAsJsonStringAsync(this HttpClient httpClient, string url, string jsonContent)
        {
            return httpClient.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json));
        }
    }
}
