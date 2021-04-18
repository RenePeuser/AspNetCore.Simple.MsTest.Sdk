using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertGetAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson) where TResult : class
        {
            return client.AssertGetAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertGetAsync<TResult>(this HttpClient client,
                                                   string url,
                                                   string resultAsJson,
                                                   Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsAsync<TResult>, HttpMethod.Get, callingAssembly);
        }


        public static Task AssertGetErrorAsync<TResult>(this HttpClient client,
                                                        string url,
                                                        string resultAsJson) where TResult : class
        {
            return client.AssertGetErrorAsync<TResult>(url, resultAsJson, Assembly.GetCallingAssembly());
        }

        public static Task AssertGetErrorAsync<TResult>(this HttpClient client,
                                                        string url,
                                                        string resultAsJson,
                                                        Assembly callingAssembly) where TResult : class
        {
            return client.AssertHttpCall(url, string.Empty, resultAsJson, item => item, HttpExtensions.GetAsErrorResultAsync<TResult>, HttpMethod.Get, callingAssembly);
        }
    }
}
