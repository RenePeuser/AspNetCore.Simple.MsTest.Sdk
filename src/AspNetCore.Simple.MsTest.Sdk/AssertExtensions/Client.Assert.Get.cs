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
        public static async Task AssertGetAsync(this HttpClient client,
                                          string url)
        {
            await client.AssertHttpCall(url, string.Empty, (client, url, _) => HttpExtensions.GetAsAsync(client, url), Assembly.GetCallingAssembly()).ConfigureAwait(false);
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                           string url,
                                                           string resultAsJson) where TResult : class
        {
            return client.AssertGetAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertGetAsync<TResult>(this HttpClient client,
                                                            string url,
                                                            string resultAsJson,
                                                            Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsAsync<TResult>, HttpMethod.Get, callingAssembly);
        }


        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                 string url,
                                                                 string resultAsJson) where TResult : class
        {
            return client.AssertGetAsErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task<TResult> AssertGetAsErrorAsync<TResult>(this HttpClient client,
                                                                 string url,
                                                                 string resultAsJson,
                                                                 Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsErrorResultAsync<TResult>, HttpMethod.Get, callingAssembly);
        }

        public static async Task AssertGetAsUnauthorizedAsync(this HttpClient httpClient, string url)
        {
            // Save original auth header
            var authenticationHeader = httpClient.DefaultRequestHeaders.Authorization;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "Unauthorized token");

            var result = await httpClient.GetAsync(url).ConfigureAwait(false);

            // Reset back to original
            httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;

            Assert.AreEqual(HttpStatusCode.Unauthorized, result.StatusCode, $"GET with'{url}' was successful, but unauthorized was expected");
        }
    }
}
