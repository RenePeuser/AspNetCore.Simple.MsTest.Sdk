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
        public static async Task<T> PostAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"POST '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }

        public static async Task<T> PostAsErrorResultAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"POST '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }

        public static async Task<T> PostAsAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"POST '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }

        public static async Task<T> PostAsErrorResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"POST '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }



        public static async Task<T> PostAsJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? Assembly.GetCallingAssembly().GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var postResponse = await httpClient.PostAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await postResponse.GetResponseInfoAsync($"POST '{url}' was not success full").ConfigureAwait(false));
        }

        public static async Task<T> PostAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? Assembly.GetCallingAssembly().GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var postResponse = await httpClient.PostAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode is false)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await postResponse.GetResponseInfoAsync($"POST '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }


        public static Task<HttpResponseMessage> PostAsJsonStringAsync(this HttpClient httpClient, string url, string jsonContent)
        {
            return httpClient.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json));
        }
    }
}
