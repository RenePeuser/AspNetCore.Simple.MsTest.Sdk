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
            services.AddSingletonIfNotExists<TestCreatorMiddleware>();
        }
    }

    internal sealed class TestCreatorMiddleware(IRequestTestCreator requestTestCreator) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context,
                                      RequestDelegate next)
        {
            var request = await GetRequestInfoUltraAsync(context.Request).ConfigureAwait(false);

            //Copy a pointer to the original response body stream
            var originalBodyStream = context.Response.Body;

            //Create a new memory stream so the response can be read back after the pipeline ran
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await using var responseBody = new MemoryStream();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            context.Response.Body = responseBody;

            try
            {
                await next(context).ConfigureAwait(false);
            }
            finally
            {
                // Read the response BEFORE the buffer is handed back - on the exception path the
                // previous version inspected an already disposed stream and reported nothing.
                var response = await GetResponseInfoUltraAsync(context).ConfigureAwait(false);

                if (response.IsNotNull())
                {
                    requestTestCreator.CreateTestFor(request, response);
                }

                // Always hand the buffer back to the real response stream. Leaving the disposed
                // MemoryStream in place turns any later write into a confusing follow-up error that
                // hides whatever actually failed.
                if (responseBody.CanSeek)
                {
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream).ConfigureAwait(false);
                }

                context.Response.Body = originalBodyStream;
            }
        }

        private async Task<RequestInfo> GetRequestInfoUltraAsync(HttpRequest request)
        {
            var bodyAsText = "Was not able to read request body";

            if (request.ContentType.IsNotNull())
            {
                if (request.Body.CanRead)
                {
                    // PATCH is sent as "application/merge-patch+json" (RFC 7386), which does NOT
                    // contain "application/json" as a substring - patch bodies were invisible here.
                    if (IsJson(request.ContentType))
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

            return new RequestInfo(request.Method, request.Path.Value!, absoluteUrl,
                                   bodyAsText);
        }

        /// <summary>
        /// True for application/json and every "+json" structured suffix such as
        /// application/merge-patch+json or application/problem+json.
        /// </summary>
        private static bool IsJson(string contentType)
        {
            return contentType.Contains(MediaTypeNames.Application.Json, StringComparison.OrdinalIgnoreCase) ||
                   contentType.Contains("+json", StringComparison.OrdinalIgnoreCase);
        }

        private async Task<ResponseInfoUltra?> GetResponseInfoUltraAsync(HttpContext response)
        {
            var bodyAsText = "Was not able to read response stream";

            // Seek requires CanSeek - checking CanRead threw NotSupportedException ("The stream is
            // not seekable") for every response body that was not swapped for a buffer.
            if (response.Response.Body.CanRead && response.Response.Body.CanSeek)
            {
                response.Response.Body.Seek(0, SeekOrigin.Begin);

                // leaveOpen - the caller still needs this stream to copy the response back.
                using var streamReader = new StreamReader(response.Response.Body, Encoding.UTF8, true,
                                                          1024, leaveOpen: true);

                bodyAsText = await streamReader.ReadToEndAsync().ConfigureAwait(false);
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

            return new ResponseInfoUltra(returnTypes, bodyAsText, response.Response.StatusCode,
                                         response.Response);
        }
    }

    internal sealed record ResponseType(int StatusCode,
                                        Type Type);
}