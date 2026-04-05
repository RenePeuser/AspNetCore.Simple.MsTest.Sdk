using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddCurlBuilderExtension
    {
        public static void AddCurlBuilder(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ICurlBuilder, CurlBuilder>();
        }
    }

    public interface ICurlBuilder
    {
        /// <summary>
        /// Builds a curl command from HTTP response context.
        /// Uses the actual request from HttpResponseMessage (includes all headers, query params, etc.).
        /// </summary>
        string BuildFrom<TResult>(HttpResponseContext<TResult> context);
    }

    internal sealed class CurlBuilder : ICurlBuilder
    {
        public string BuildFrom<TResult>(HttpResponseContext<TResult> context)
        {
            var httpRequestMessage = context.HttpResponseMessage.RequestMessage;

            if (httpRequestMessage.IsNull())
            {
                return string.Empty;
            }

            var payloadAsJson = context.Request.ResolvedPayload ?? context.Request.PayloadAsJson ?? string.Empty;
            var authenticationHeaderValue = context.Request.Client.DefaultRequestHeaders.Authorization;
            var assembly = context.Request.CallingAssembly;
            var showTokenInCurl = context.Request.ShowTokenInCurl;

            var curl = BuildCurl(httpRequestMessage, payloadAsJson, authenticationHeaderValue,
                                 assembly, showTokenInCurl).Flatten(@$" \{Environment.NewLine}");

            return curl;
        }

        private static IEnumerable<string> BuildCurl(HttpRequestMessage httpRequestMessage,
                                                     string payloadAsJson,
                                                     AuthenticationHeaderValue? authenticationHeaderValue,
                                                     Assembly assembly,
                                                     bool showTokenInCurl)
        {
            // base curl call
            yield return "curl";
            yield return "--location";
            yield return $"--request {httpRequestMessage.Method} '{httpRequestMessage.RequestUri}'";

            if (authenticationHeaderValue.IsNotNull())
            {
                var token = showTokenInCurl ? authenticationHeaderValue.Parameter : "Sorry i am secret :)";

                yield return $"--header 'Authorization: {authenticationHeaderValue.Scheme} {token}'";
            }

            foreach (var requestMessageHeader in httpRequestMessage.Headers)
            {
                yield return $"--header '{requestMessageHeader.Key}: {requestMessageHeader.Value.Flatten(", ")}'";
            }

            if (payloadAsJson.IsNotNullOrWhiteSpace())
            {
                var json = payloadAsJson.GetJsonStringFrom(assembly);

                yield return "--header 'Content-Type: application/json'";
                yield return $"--data-raw '{json}'";
            }
        }
    }
}
