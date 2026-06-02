using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Serializer.Json;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using static System.Net.Mime.MediaTypeNames;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpRequestMessageBuilderExtension
    {
        public static void AddHttpRequestMessageBuilder(this IServiceCollection services)
        {
            // Register dependency
            services.AddJsonSerializer();

            // Register service itself
            services.AddSingletonIfNotExists<IHttpRequestMessageBuilder, HttpRequestMessageBuilder>();
        }
    }

    public interface IHttpRequestMessageBuilder
    {
        HttpRequestMessage BuildFrom(HttpMethod method,
                                     string uri,
                                     object? payload,
                                     string payloadParameterName);

        HttpRequestMessage BuildFrom(IHttpAssertContext context);
    }

    internal sealed class HttpRequestMessageBuilder(JsonSerializer jsonSerializer) : IHttpRequestMessageBuilder
    {
        public HttpRequestMessage BuildFrom(HttpMethod method,
                                            string uri,
                                            object? payload,
                                            string payloadParameterName)
        {
            var content = payload.IsNotNull() ? GetContent() : null;

            var httpRequestMessage = new HttpRequestMessage(method, uri)
                                     {
                                         Version = HttpVersion.Version11,
                                         VersionPolicy = HttpVersionPolicy.RequestVersionOrLower,
                                         Content = content
                                     };

            return httpRequestMessage;

            HttpContent GetContent()
            {
                return payload switch
                {
                    string stringContent => new StringContent(stringContent, Encoding.UTF8, GetContentType(method)),
                    InMemoryFileAsStream inMemoryFileAsStream => inMemoryFileAsStream.ToMultipartFormDataContent(payloadParameterName),
                    byte[] byteArray => new ByteArrayContent(byteArray),
                    Stream streamContent => new StreamContent(streamContent),
                    _ => new StringContent(jsonSerializer.Serialize(payload), Encoding.UTF8, Application.Json)
                };
            }
        }

        private string GetContentType(HttpMethod httpMethod)
        {
            if (httpMethod.EqualsTo(HttpMethod.Patch))
            {
                // JSON Merge Patch (RFC 7386) is used by default cause of simplicity
                return "application/merge-patch+json";
            }

            return Application.Json;
        }

        public HttpRequestMessage BuildFrom(IHttpAssertContext context)
        {
            return BuildFrom(context.HttpMethod,
                             context.Url,
                             context.ResolvedPayload ?? context.PayloadAsJson,
                             context.PayloadParameterName);
        }
    }
}