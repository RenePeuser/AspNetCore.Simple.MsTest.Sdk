using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public class TestCreatorMiddleware : IMiddleware
    {
        private readonly IRequestTestCreator _requestTestCreator;

        public TestCreatorMiddleware(IRequestTestCreator requestTestCreator)
        {
            _requestTestCreator = requestTestCreator;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            //First, get the incoming request
            var request = await GetRequestInfoUltraAsync(context.Request).ConfigureAwait(false);

            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream...
            await using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;
            await next(context).ConfigureAwait(false);

            //Format the response from the server
            var response = await GetResponseInfoUltraAsync(context.Response).ConfigureAwait(false);

            var test = _requestTestCreator.CreateTestFor(request, response);
            // context.Response.Headers.Add("test", test);

            //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalBodyStream).ConfigureAwait(false);
        }

        private async Task<RequestInfo> GetRequestInfoUltraAsync(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body);
            var bodyAsText = await reader.ReadToEndAsync().ConfigureAwait(false);
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(bodyAsText));

            return new RequestInfo(request.Method, request.Path.Value, bodyAsText);
        }

        private async Task<ResponseInfoUltra> GetResponseInfoUltraAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var bodyAsText = await new StreamReader(response.Body).ReadToEndAsync().ConfigureAwait(false);
            response.Body.Seek(0, SeekOrigin.Begin);

            // Here we need a solutions for inumerable
            var responseType = typeof(object);
            if (response.Headers.TryGetValue("returntype-assembly", out var returnTypeString))
            {
                responseType = Type.GetType(returnTypeString);
            }

            return new ResponseInfoUltra(responseType, bodyAsText, response.StatusCode);
        }
    }
}
