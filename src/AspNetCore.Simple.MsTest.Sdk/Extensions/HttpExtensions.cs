using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class HttpExtensions
    {
        public static async Task<T> GetAsAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"GET to '{url}' was not success full. Error code: {result.StatusCode}");
        }

        public static async Task<T> DeleteAsAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"DELETE with'{url}' was not success full. Error code: {result.StatusCode}");
        }

        public static async Task<T> PutAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PutAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PUT to '{url}' was not success full. Error code: {result.StatusCode}");
        }

        public static async Task<T> PostAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST to '{url}' was not success full. Error code: {result.StatusCode}");
        }

        public static async Task<T> PostAsAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST to '{url}' was not success full. Error code: {result.StatusCode}");
        }

        public static async Task<T> GetAsExceptionAsync<T>(this HttpClient httpClient, string url) where T : Exception
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (!result.IsSuccessStatusCode)
            {
                return await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
            }

            throw new InvalidOperationException($"GET to '{url}' was success full, but exception was expected.");
        }

        public static async Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient httpClient, string url, T content)
        {
            var jsonContent = content.ToJson();
            var patchResponse = await httpClient.PatchAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            return patchResponse;
        }

        public static Task<HttpResponseMessage> PostAsJsonStringAsync(this HttpClient httpClient, string url, string jsonContent)
        {
            return httpClient.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json));
        }

        public static Task<HttpResponseMessage> PutAsJsonStringAsync(this HttpClient httpClient, string url, string jsonContent)
        {
            return httpClient.PutAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json));
        }

        public static async Task<T> PostAsJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST to '{url}' was not success full. Error code: {postResponse.StatusCode}");
        }

        public static async Task<T> PutAsJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PutAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PUT to '{url}' was not success full. Error code: {postResponse.StatusCode}");
        }
    }
}
