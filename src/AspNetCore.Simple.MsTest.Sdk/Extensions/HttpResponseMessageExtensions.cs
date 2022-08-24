using System;
using System.Net.Http;
using System.Threading.Tasks;
using ConsoleTables;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk.Extensions
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
    }
}
