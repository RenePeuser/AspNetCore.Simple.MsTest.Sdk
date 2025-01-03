using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal sealed class HttpCallHandler(HttpRequestMessageBuilder httpRequestMessageBuilder)
    {
        public async Task<HttpResponseMessage> CallAsync(HttpClient httpClient,
                                                         HttpMethod httpMethod,
                                                         string url,
                                                         object? payload,
                                                         CancellationToken cancellationToken,
                                                         [CallerArgumentExpression(nameof(payload))]
                                                         string payloadParameterName = "")
        {
            var message = httpRequestMessageBuilder.BuildFrom(httpMethod, url, payload, payloadParameterName);

            var response = await httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);

            return response;
        }
    }
}
