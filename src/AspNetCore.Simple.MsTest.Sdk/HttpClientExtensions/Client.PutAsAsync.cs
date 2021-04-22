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
        public static async Task<T> PutAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PutAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"PUT '{url}' was not success full. Error code: {result.StatusCode}").ConfigureAwait(false));
        }

        public static async Task<T> PutAsErrorResultAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PutAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await result.GetResponseInfoAsync($"PUT '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }


        public static async Task<T> PutAsJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? Assembly.GetCallingAssembly().GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var postResponse = await httpClient.PutAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await postResponse.GetResponseInfoAsync($"PUT '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }

        public static async Task<T> PutAsErrorResultWithJsonStringAsync<T>(this HttpClient httpClient, string url, string payloadAsJson)
        {
            var jsonPayload = payloadAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? Assembly.GetCallingAssembly().GetFileContentFrom(payloadAsJson) : payloadAsJson;

            var postResponse = await httpClient.PutAsync(url, new StringContent(jsonPayload, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode is false)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new UnexpectedResultException(await postResponse.GetResponseInfoAsync($"PUT '{url}' was success full, but you expect an error result of type: '{typeof(T).Name}'").ConfigureAwait(false));
        }


        public static Task<HttpResponseMessage> PutAsJsonStringAsync(this HttpClient httpClient, string url, string jsonContent)
        {
            return httpClient.PutAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json));
        }
    }
}
