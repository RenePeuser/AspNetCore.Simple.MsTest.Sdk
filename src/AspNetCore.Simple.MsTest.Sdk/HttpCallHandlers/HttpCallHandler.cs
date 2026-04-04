using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpCallHandlerExtension
    {
        public static void AddHttpCallHandler(this IServiceCollection services)
        {
            // Register dependency
            services.AddHttpRequestMessageBuilder();

            // Register service itself
            services.AddSingletonIfNotExists<IHttpCallHandler, HttpCallHandler>();
        }
    }

    public interface IHttpCallHandler
    {
        Task<HttpResponseMessage> CallAsync(HttpClient httpClient,
                                            HttpMethod httpMethod,
                                            string url,
                                            object? payload,
                                            CancellationToken cancellationToken,
                                            [CallerArgumentExpression(nameof(payload))]
                                            string payloadParameterName = "");

        /// <summary>
        /// Executes an HTTP call using the HTTP context's properties.
        /// This is the preferred method for context-based operations.
        /// </summary>
        Task<HttpResponseMessage> CallAsync(IHttpAssertContext context,
                                            CancellationToken cancellationToken = default);
    }

    internal sealed class HttpCallHandler(IHttpRequestMessageBuilder httpRequestMessageBuilder) : IHttpCallHandler
    {
        public async Task<HttpResponseMessage> CallAsync(HttpClient httpClient,
                                                         HttpMethod httpMethod,
                                                         string url,
                                                         object? payload,
                                                         CancellationToken cancellationToken,
                                                         [CallerArgumentExpression(nameof(payload))]
                                                         string payloadParameterName = "")
        {
            // 1. Set up the HttpRequestMessage and don't forget to dispose it
            using var message = httpRequestMessageBuilder.BuildFrom(httpMethod, url, payload,
                                                                    payloadParameterName);

            // 2. Send the request and, and do NOT dispose here, because the processing
            //    of the response happens on consumer side.
            var response = await httpClient.SendAsync(message, cancellationToken).ConfigureAwait(false);

            // 3. Return the response
            return response;
        }

        public Task<HttpResponseMessage> CallAsync(IHttpAssertContext context,
                                                   CancellationToken cancellationToken = default)
        {
            return CallAsync(context.Client,
                             context.HttpMethod,
                             context.Url,
                             context.ResolvedPayload ?? context.PayloadAsJson,
                             cancellationToken,
                             context.PayloadParameterName);
        }
    }
}
