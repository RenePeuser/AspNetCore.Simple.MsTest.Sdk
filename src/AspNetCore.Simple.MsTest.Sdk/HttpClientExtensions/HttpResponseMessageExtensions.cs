using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class HttpResponseMessageExtensions
    {

        internal static async Task<string> GetResponseInfoAsync(this HttpResponseMessage response, string title)
        {
            var result = await response.GetResponseInfosAsync(title).ToListAsync().ConfigureAwait(false);
            return result.Flatten(Environment.NewLine);
        }
        internal static async IAsyncEnumerable<string> GetResponseInfosAsync(this HttpResponseMessage response, string title)
        {
            yield return title;
            yield return $"{nameof(response.RequestMessage.Method.Method)}: {response.RequestMessage.Method.Method}";
            yield return $"{nameof(response.RequestMessage.RequestUri.AbsolutePath)}: {response.RequestMessage.RequestUri.AbsolutePath}";
            yield return $"{nameof(response.StatusCode)}: {response.StatusCode}";
            yield return $"{nameof(response.Content)}: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}";
        }
    }
}
