using System.Net.Http;
using System.Threading.Tasks;
using ConsoleTables;
using Extensions.Pack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class HttpResponseMessageExtensions
    {
        internal static async Task<string> GetResponseInfoAsync(this HttpResponseMessage response, string expected)
        {
            var errorResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            errorResponse = JToken.Parse(errorResponse).ToString(Formatting.Indented);
            var errorResult = new
            {
                Request = $"{response.RequestMessage?.Method} {response.RequestMessage?.RequestUri}",
                Expected = expected,
                Current = response.StatusCode,
            }.ToIList();

            var table = ConsoleTable.From(errorResult).ToString();
            var errorOutput = $@"
{table}

Current response:

{errorResponse}";

            return errorOutput;
        }
    }
}
