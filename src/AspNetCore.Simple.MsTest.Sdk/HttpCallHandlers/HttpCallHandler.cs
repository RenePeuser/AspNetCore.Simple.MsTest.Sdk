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
        Task<HttpResponseMessage> CallAsync(IHttpAssertContext context,
                                            CancellationToken cancellationToken = default);
    }

    internal sealed class HttpCallHandler(IHttpRequestMessageBuilder httpRequestMessageBuilder) : IHttpCallHandler
    {
        public async Task<HttpResponseMessage> CallAsync(IHttpAssertContext context,
                                                         CancellationToken cancellationToken = default)
        {
            // 1. Set up the HttpRequestMessage and don't forget to dispose it
            using var message = httpRequestMessageBuilder.BuildFrom(context);

            // 2. Send the request and, and do NOT dispose here, because the processing
            //    of the response happens on consumer side.
            var response = await context.Client.SendAsync(message, cancellationToken).ConfigureAwait(false);

            // 3. Return the response
            return response;
        }
    }
}