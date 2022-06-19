using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpExtensions
    {
        public static Task<bool> UrlExistsAsync(this HttpClient httpClient, string url)
        {
            var httpClientAvoidClosure = httpClient;
            var relativeUrlAvoidClosure = url;
            return Task.Run(() => UrlExists(httpClient, relativeUrlAvoidClosure));
        }

        public static bool UrlExists(this HttpClient httpClient, string url)
        {
            var absolutePath = url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : Path.Combine(httpClient.BaseAddress?.ToString()!, url);
#pragma warning disable SYSLIB0014
            var webRequest = System.Net.WebRequest.Create(absolutePath);
#pragma warning restore SYSLIB0014
            webRequest.Method = "HEAD";
            try
            {
                using var response = (System.Net.HttpWebResponse)webRequest.GetResponse();
                return response.StatusCode.ToString() == "OK";
            }
            catch
            {
                Debug.WriteLine($"Url: '{absolutePath}' does not exists or was not reachable");
                return false;
            }
        }
    }
}
