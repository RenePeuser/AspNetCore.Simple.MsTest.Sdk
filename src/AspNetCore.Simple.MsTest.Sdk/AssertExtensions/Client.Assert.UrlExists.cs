using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static partial class HttpClientAssertExtensions
    {
        public static void AssertUrlExists(this HttpClient client,
                                           string url)
        {
            Assert.IsTrue(client.UrlExists(url), $"The given url: '{url}' was not reachable or does not exists");
        }

        public static void AssertUrlNotExists(this HttpClient client,
                                              string url)
        {
            Assert.IsFalse(client.UrlExists(url), $"The given url: '{url}' exists");
        }

        public static async Task AssertUrlNotExistsAsync(this HttpClient client,
                                                         string url)
        {
            var urlExistsAsync = await client.UrlExistsAsync(url).ConfigureAwait(false);

            Assert.IsFalse(urlExistsAsync, $"The given url: '{url}' exists");
        }

        public static async Task AssertUrlExistsAsync(this HttpClient client,
                                                      string url)
        {
            var urlExistsAsync = await client.UrlExistsAsync(url).ConfigureAwait(false);

            Assert.IsTrue(urlExistsAsync, $"The given url: '{url}' was not reachable or does not exists");
        }
    }
}