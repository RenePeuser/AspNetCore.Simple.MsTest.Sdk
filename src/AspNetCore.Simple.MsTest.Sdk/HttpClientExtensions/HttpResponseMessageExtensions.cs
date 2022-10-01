using System;
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
        internal static async Task<T> ParseResultAsync<T>(this HttpResponseMessage responseMessage)
        {
            var jsonString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            T typeResult;
            try
            {
                typeResult = jsonString.FromJsonStringAs<T>();
            }
            catch (Exception)
            {
                var errorResponse = new { Url = $"{responseMessage.RequestMessage?.Method} {responseMessage.RequestMessage?.RequestUri}", ExpectedResponse = typeof(T).Name, CurrentResponse = jsonString }.ToIList();
                var table = ConsoleTable.From(errorResponse).ToString();
                throw new UnexpectedResultException($"{Environment.NewLine}{Environment.NewLine}Your expected response type: '{typeof(T).Name}' can not be deserialized from current response json string{Environment.NewLine}{Environment.NewLine}{table}{Environment.NewLine}{Environment.NewLine}Current result: {jsonString}");
            }

            return typeResult;
        }

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
