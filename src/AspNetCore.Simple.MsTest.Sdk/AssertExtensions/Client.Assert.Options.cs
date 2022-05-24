using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static Task AssertOptionsAsync(this HttpClient client,
                                         string url,
                                         IImmutableDictionary<string, string> expectedHeaders)
        {
            IImmutableDictionary<string, IImmutableList<string>> expectedHeaderStructure = expectedHeaders.ToImmutableDictionary(item => item.Key, item => (IImmutableList<string>)ImmutableList.Create(item.Value));
            return client.AssertOptionsAsync(url, expectedHeaderStructure.ToJson());
        }

        public static Task AssertOptionsAsync(this HttpClient client,
                                         string url,
                                         IImmutableDictionary<string, IImmutableList<string>> expectedHeaders)
        {
            return client.AssertOptionsAsync(url, expectedHeaders.ToJson());
        }

        public static Task AssertOptionsAsync(this HttpClient client,
                                         string url,
                                         string expectedHeadersAsJson)
        {
            return client.AssertOptionsAsync(url, expectedHeadersAsJson, Assembly.GetCallingAssembly());
        }

        private static async Task AssertOptionsAsync(this HttpClient client,
                                                string url,
                                                string expectedHeadersAsJson,
                                                Assembly callingAssembly)
        {
            var expectedHeaders = expectedHeadersAsJson.EndsWith(".json", StringComparison.InvariantCulture) ? callingAssembly.GetFileContentFrom(expectedHeadersAsJson) : expectedHeadersAsJson;

            var request = new HttpRequestMessage(HttpMethod.Options, url);
            var result = await client.SendAsync(request).ConfigureAwait(false);
            var content = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            Assert.IsTrue(result.IsSuccessStatusCode, $"Option call to {request.RequestUri.AbsoluteUri} was not successful. ErrorCode: {result.StatusCode}");
            Assert.IsTrue(string.IsNullOrWhiteSpace(content), "Content of Options call should be null or empty");

            var headers = result.Headers.ToDictionary(item => item.Key, item => item.Value);

            Assert.That.ObjectsAreEqual(() => expectedHeaders, () => headers);
        }
    }
}
