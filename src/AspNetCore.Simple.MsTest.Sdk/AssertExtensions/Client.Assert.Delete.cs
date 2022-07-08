using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertDeleteAsync(this HttpClient client,
                                                   string url)
        {
            return client.AssertHttpCall(url, string.Empty, (client, url, _) => client.DeleteAsAsync(url), Assembly.GetCallingAssembly());
        }

        public static Task AssertDeleteAsync<TResult>(this HttpClient client,
                                                      string url,
                                                      string resultAsJson) where TResult : class
        {
            return client.AssertDeleteAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertDeleteAsync<TResult>(this HttpClient client,
                                                      string url,
                                                      string resultAsJson,
                                                      Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.DeleteAsAsync<TResult>, HttpMethod.Delete, callingAssembly);
        }


        public static Task AssertDeleteErrorAsync<TResult>(this HttpClient client,
                                                           string url,
                                                           string resultAsJson) where TResult : class
        {
            return client.AssertDeleteErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertDeleteErrorAsync<TResult>(this HttpClient client,
                                                           string url,
                                                           string resultAsJson,
                                                           Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.DeleteAsErrorResultAsync<TResult>, HttpMethod.Delete, callingAssembly);
        }

        public static async Task AssertDeleteAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.DeleteAsync(url).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, $"DELETE with'{url}' was successful, but unauthorized was expected");
        }
    }
}
