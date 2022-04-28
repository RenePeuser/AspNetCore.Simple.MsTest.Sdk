using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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

        public static Task PatchAsJsonStringUnauthorizedAsync(this HttpClient httpClient, string url, string payloadAsJson)
        {
            return httpClient.PatchAsJsonStringUnauthorizdAsync(url, payloadAsJson, Assembly.GetCallingAssembly());
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

            throw new UnexpectedResultException(await postResponse.GetResponseInfoAsync($"PATCH '{url}' was successful, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }

        public static async Task PatchAsJsonStringUnauthorizdAsync(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var postResponse = await httpClient.PatchAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            if (postResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }

            throw new UnexpectedResultException(await postResponse.GetResponseInfoAsync($"PATCH '{url}' was successful, but you expect that this call is unauthorized 401").ConfigureAwait(false));
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

            throw new UnexpectedResultException(await postResponse.GetResponseInfoAsync($"PATCH '{url}' was successful, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }

        public static async Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient httpClient, string url, T content)
        {
            var jsonContent = content.ToJson();
            var patchResponse = await httpClient.PatchAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            return patchResponse;
        }
    }
}
