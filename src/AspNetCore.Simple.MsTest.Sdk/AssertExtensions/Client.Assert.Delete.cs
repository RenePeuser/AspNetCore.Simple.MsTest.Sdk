using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

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
    }
}
