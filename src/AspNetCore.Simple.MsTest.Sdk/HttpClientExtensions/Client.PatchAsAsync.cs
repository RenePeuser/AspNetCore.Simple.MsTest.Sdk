using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static Task<T> PatchAsJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.PatchAsErrorResultWithJsonStringAsync<T>(url, jsonContent);
            }

            return httpClient.PatchAsResultWithJsonStringAsync<T>(url, jsonContent);
        }

        private static async Task<T> PatchAsResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PatchAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PATCH '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }

        public static async Task<T> PatchAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PatchAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode is false)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PATCH '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }

        public static async Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient httpClient, string url, T content)
        {
            var jsonContent = content.ToJson();
            var patchResponse = await httpClient.PatchAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            return patchResponse;
        }
    }
}
