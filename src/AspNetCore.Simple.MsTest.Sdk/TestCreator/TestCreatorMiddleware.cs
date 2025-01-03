using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddTestCreatorMiddlewareExtension
    {
        internal static void AddTestCreatorMiddleware(this IServiceCollection services)
        {
            services.AddSingleton<TestCreatorMiddleware>();
        }
    }

    internal sealed class TestCreatorMiddleware(IRequestTestCreator requestTestCreator) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            ResponseInfoUltra? response;
            var request = await GetRequestInfoUltraAsync(context.Request).ConfigureAwait(false);
            try
            {
                //First, get the incoming request

                //Copy a pointer to the original response body stream
                var originalBodyStream = context.Response.Body;

                //Create a new memory stream...
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                await using var responseBody = new MemoryStream();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
                context.Response.Body = responseBody;
                await next(context).ConfigureAwait(false);

                //Format the response from the server
                response = await GetResponseInfoUltraAsync(context).ConfigureAwait(false);
                if (response.IsNotNull())
                {
                    requestTestCreator.CreateTestFor(request, response);
                }

                //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
                await responseBody.CopyToAsync(originalBodyStream).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //Format the response from the server
                response = await GetResponseInfoUltraAsync(context).ConfigureAwait(false);
                if (response.IsNotNull())
                {
                    requestTestCreator.CreateTestFor(request, response);
                }

                throw;
            }
        }

        private async Task<RequestInfo> GetRequestInfoUltraAsync(HttpRequest request)
        {
            var bodyAsText = "Was not able to read request body";

            if (request.ContentType.IsNotNull())
            {
                if (request.Body.CanRead)
                {
                    if (request.ContentType.Contains(MediaTypeNames.Application.Json))
                    {
                        using var reader = new StreamReader(request.Body);
                        bodyAsText = await reader.ReadToEndAsync().ConfigureAwait(false);
                        request.Body = new MemoryStream(Encoding.UTF8.GetBytes(bodyAsText));
                    }
                    else
                    {
                        bodyAsText = $"Request which are not type of {MediaTypeNames.Application.Json} makes no sense to read";
                    }
                }
            }
            else
            {
                bodyAsText = string.Empty;
            }

            var absoluteUrl = $"{request.Scheme}://{request.Host}{request.Path.Value}";

            return new RequestInfo(request.Method, request.Path.Value!, absoluteUrl, bodyAsText);
        }

        private async Task<ResponseInfoUltra?> GetResponseInfoUltraAsync(HttpContext response)
        {
            var bodyAsText = "Was not able to read response stream";

            if (response.Response.Body.CanRead)
            {
                response.Response.Body.Seek(0, SeekOrigin.Begin);
                bodyAsText = await new StreamReader(response.Response.Body).ReadToEndAsync().ConfigureAwait(false);
                response.Response.Body.Seek(0, SeekOrigin.Begin);
            }

            // Yes cool new shit
            var controllerActionDescriptor = response.GetEndpoint()?
                                                     .Metadata
                                                     .GetMetadata<ControllerActionDescriptor>();

            if (controllerActionDescriptor.IsNull())
            {
                return null;
            }

            var returnType = controllerActionDescriptor.GetReturnType();
            var producesResponseTypes = controllerActionDescriptor.EndpointMetadata.OfType<ProducesResponseTypeAttribute>();
            var returnTypes = producesResponseTypes.Select(pr => new ResponseType(pr.StatusCode, pr.Type)).ToImmutableList();

            if (returnTypes.IsEmpty())
            {
                returnTypes = ImmutableList.Create(new ResponseType(200, returnType), new ResponseType(400, typeof(ProblemDetails)));
            }

            return new ResponseInfoUltra(returnTypes, bodyAsText, response.Response.StatusCode, response.Response);
        }
    }

    internal sealed record ResponseType(int StatusCode, Type Type);
}
