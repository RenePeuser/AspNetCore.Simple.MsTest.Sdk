using System;
using System.Net.Http;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static Task<T> PatchAsJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            return httpClient.PatchAsJsonStringAsync<T>(url, payloadAsJson, Assembly.GetCallingAssembly());
        }

        public static async Task<T> PatchAsJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var postResponse = await httpClient.PatchAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await postResponse.GetResponseInfoAsync($"PATCH '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }

        public static Task<T> PatchAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            return httpClient.PatchAsErrorResultWithJsonStringAsync<T>(url, payloadAsJson, Assembly.GetCallingAssembly());
        }

        public static async Task<T> PatchAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var postResponse = await httpClient.PatchAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode is false)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await postResponse.GetResponseInfoAsync($"PATCH '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }

        public static async Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient httpClient, string url, T content)
        {
            var jsonContent = content.ToJson();
            var patchResponse = await httpClient.PatchAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            return patchResponse;
        }
    }
}
