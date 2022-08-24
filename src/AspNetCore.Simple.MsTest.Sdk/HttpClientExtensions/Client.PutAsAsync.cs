using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Extensions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static async Task<T> PutAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var putResponse = await httpClient.PutAsJsonAsync(url, body).ConfigureAwait(false);
            if (putResponse.IsSuccessStatusCode)
            {
                return await putResponse.ParseResultAsync<T>().ConfigureAwait(false);
            }

            throw new UnexpectedResultException(await putResponse.GetResponseInfoAsync(nameof(putResponse.IsSuccessStatusCode)).ConfigureAwait(false));
        }

        public static async Task PutAsUnauthorizedAsync<T>(this HttpClient httpClient, string url, object body)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.PutAsJsonAsync(url, body).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"PUT '{url}' was not successful. Error code: {result.StatusCode}").ConfigureAwait(false));
        }

        public static async Task<T> PutAsErrorResultAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var putResponse = await httpClient.PutAsJsonAsync(url, body).ConfigureAwait(false);
            if (putResponse.IsSuccessStatusCode is false)
            {
                return await putResponse.ParseResultAsync<T>().ConfigureAwait(false);
            }

            var responseInfoAsync = await putResponse.GetResponseInfoAsync("Not successful").ConfigureAwait(false);
            throw new UnexpectedResultException(responseInfoAsync);
        }


        public static Task<T> PutAsJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            return httpClient.PutAsJsonStringAsync<T>(url, payloadAsJson, Assembly.GetCallingAssembly());
        }

        public static async Task<T> PutAsJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var putResponse = await httpClient.PutAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (putResponse.IsSuccessStatusCode)
            {
                return await putResponse.ParseResultAsync<T>().ConfigureAwait(false);
            }

            throw new UnexpectedResultException(await putResponse.GetResponseInfoAsync(nameof(putResponse.IsSuccessStatusCode)).ConfigureAwait(false));
        }

        public static Task<T> PutAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            return httpClient.PutAsErrorResultWithJsonStringAsync<T>(url, payloadAsJson, Assembly.GetCallingAssembly());
        }

        public static async Task<T> PutAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var putResponse = await httpClient.PutAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (putResponse.IsSuccessStatusCode is false)
            {
                return await putResponse.ParseResultAsync<T>().ConfigureAwait(false);
            }

            var responseInfoAsync = await putResponse.GetResponseInfoAsync("Not successful").ConfigureAwait(false);

            throw new UnexpectedResultException(responseInfoAsync);
        }


        public static Task<HttpResponseMessage> PutAsJsonStringAsync(this HttpClient httpClient, string url, string jsonContent)
        {
            return httpClient.PutAsJsonStringAsync(url, jsonContent, Assembly.GetCallingAssembly());
        }

        public static Task<HttpResponseMessage> PutAsJsonStringAsync(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            return httpClient.PutAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json));
        }
    }
}
