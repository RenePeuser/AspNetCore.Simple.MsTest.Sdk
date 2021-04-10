using System;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class HttpExtensions
    {
        public static Task<T> GetAsAsync<T>(this HttpClient httpClient, string url)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.GetAsExceptionAsync<T>(url);
            }

            return httpClient.GetAsResultAsync<T>(url);
        }

        private static async Task<T> GetAsResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"GET '{url}' was not success full. Error code: {result.StatusCode}");
        }

        private static async Task<T> GetAsExceptionAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"GET '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }



        public static Task<T> DeleteAsAsync<T>(this HttpClient httpClient, string url)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.DeleteAsExceptionAsync<T>(url);
            }

            return httpClient.DeleteAsResultAsync<T>(url);
        }

        private static async Task<T> DeleteAsResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"DELETE with'{url}' was not success full. Error code: {result.StatusCode}");
        }

        private static async Task<T> DeleteAsExceptionAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"DELETE '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }



        public static Task<T> PutAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.PutAsExceptionAsync<T>(url, body);
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

        private static async Task<T> PutAsExceptionAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PutAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PUT '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }



        public static Task<T> PostAsAsync<T>(this HttpClient httpClient, string url, object body)
        {
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.PostAsExceptionAsync<T>(url, body);
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

        private static async Task<T> PostAsExceptionAsync<T>(this HttpClient httpClient, string url, object body)
        {
            var result = await httpClient.PostAsJsonAsync(url, body).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
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
                return httpClient.PostAsExceptionAsync<T>(url);
            }

            return httpClient.PostAsResultAsync<T>(url);
        }

        private static async Task<T> PostAsResultAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (result.IsSuccessStatusCode is false)
            {
                var typeResult = await result.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }

        private static async Task<T> PostAsExceptionAsync<T>(this HttpClient httpClient, string url)
        {
            var result = await httpClient.PostAsync(url, null).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
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
                return httpClient.PostAsExceptionWithJsonStringAsync<T>(url, jsonContent);
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

        private static async Task<T> PostAsExceptionWithJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PostAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode is false)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"POST '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
        }



        public static Task<T> PutAsJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            // Nice here we check if the caller expect an exception
            if (typeof(Exception).IsAssignableFrom(typeof(T)))
            {
                return httpClient.PutAsExceptionWithJsonStringAsync<T>(url, jsonContent);
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

        private static async Task<T> PutAsExceptionWithJsonStringAsync<T>(this HttpClient httpClient, string url, string jsonContent)
        {
            var postResponse = await httpClient.PutAsync(url, new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json)).ConfigureAwait(false);
            if (postResponse.IsSuccessStatusCode is false)
            {
                var typeResult = await postResponse.Content.ReadAsAsync<T>().ConfigureAwait(false);
                return typeResult;
            }

            throw new InvalidOperationException($"PUT '{url}' was success full, but you expect an exception of type: '{typeof(T).Name}'");
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
    }
}
