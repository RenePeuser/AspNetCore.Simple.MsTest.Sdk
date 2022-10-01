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
        public static async Task<T> PostAsAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            var responseInfoAsync = await result.GetResponseInfoAsync(nameof(result.IsSuccessStatusCode)).ConfigureAwait(false);

            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task<T> PostAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            var responseInfoAsync = await result.GetResponseInfoAsync(nameof(result.IsSuccessStatusCode)).ConfigureAwait(false);

            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task PostAsUnauhthorizedAsync(this HttpClient httpClient, string url, object body)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }

            var responseInfoAsync = await result.GetResponseInfoAsync($"POST '{url}' was successful, but unauthorized was expected").ConfigureAwait(false);
            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task<T> PostAsErrorResultAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                return await result.ParseResultAsync<T>().ConfigureAwait(false);
            }

            var responseInfoAsync = await result.GetResponseInfoAsync("Not successful").ConfigureAwait(false);
            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task<T> PostAsUnauthorizedAsync<T>(this HttpClient httpClient, string url)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            if (result.IsSuccessStatusCode)
            {
                return await result.ParseResultAsync<T>().ConfigureAwait(false);
            }

            var responseInfoAsync = await result.GetResponseInfoAsync(nameof(result.IsSuccessStatusCode)).ConfigureAwait(false);
            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task<T> PostAsErrorResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                return await result.ParseResultAsync<T>().ConfigureAwait(false);
            }

            var responseInfoAsync = await result.GetResponseInfoAsync("Not successful").ConfigureAwait(false);
            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static Task<T> PostAsJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            return httpClient.PostAsJsonStringAsync<T>(url, payloadAsJson, Assembly.GetCallingAssembly());
        }

        public static async Task<T> PostAsJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var postResponse = await httpClient.PostAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode)
            {
                return await postResponse.ParseResultAsync<T>().ConfigureAwait(false);
            }

            var responseInfoAsync = await postResponse.GetResponseInfoAsync(nameof(postResponse.IsSuccessStatusCode)).ConfigureAwait(false);
            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static async Task PostAsJsonStringUnauthorizedAsync(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var postResponse = await httpClient.PostAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            if (postResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return;
            }

            var responseInfoAsync = await postResponse.GetResponseInfoAsync(nameof(postResponse.IsSuccessStatusCode)).ConfigureAwait(false);
            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static Task<T> PostAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            return httpClient.PostAsErrorResultWithJsonStringAsync<T>(url, payloadAsJson, Assembly.GetCallingAssembly());
        }

        public static async Task<T> PostAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var postResponse = await httpClient.PostAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode is false)
            {
                return await postResponse.ParseResultAsync<T>().ConfigureAwait(false);
            }

            var responseInfoAsync = await postResponse.GetResponseInfoAsync("Not successful").ConfigureAwait(false);
            throw new UnexpectedResultException(responseInfoAsync);
        }

        public static Task<HttpResponseMessage> PostAsJsonStringAsync(this HttpClient httpClient, string url, string payloadAsJson)
        {
            return httpClient.PostAsJsonStringAsync(url, payloadAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<HttpResponseMessage> PostAsJsonStringAsync(this HttpClient httpClient, string url, string payloadAsJson, Assembly callingAssembly)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(payloadAsJson) : payloadAsJson;

            return httpClient.PostAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json));
        }
    }
}
