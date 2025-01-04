using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal sealed class HttpRequestMessageBuilder(Serializer.Json.JsonSerializer jsonSerializer)
    {
        internal HttpRequestMessage BuildFrom(HttpMethod method,
                                              string uri,
                                              object? payload,
                                              string payloadParameterName)
        {
            var content = payload.IsNotNull() ? GetContent() : null;

            return new HttpRequestMessage(method, uri) { Version = HttpVersion.Version11, VersionPolicy = HttpVersionPolicy.RequestVersionOrLower, Content = content };

            HttpContent GetContent()
            {
                return payload switch
                {
                    string stringContent => new StringContent(stringContent, Encoding.UTF8, MediaTypeNames.Application.Json),
                    InMemoryFileAsStream inMemoryFileAsStream => inMemoryFileAsStream.ToMultipartFormDataContent(payloadParameterName),
                    byte[] byteArray => new ByteArrayContent(byteArray),
                    Stream streamContent => new StreamContent(streamContent),
                    _ => new StringContent(jsonSerializer.Serialize(payload), Encoding.UTF8, MediaTypeNames.Application.Json)
                };
            }
        }
    }
}
