using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        /// Builds a curl command from HTTP response context interface.
        /// Non-generic overload for use with IHttpResponseContext.
        /// </summary>
        string BuildFrom(IHttpResponseContext context);

        /// <summary>
        /// Builds a curl command from HTTP assert context interface.
        /// Use this overload when no response is available yet (e.g., endpoint validation errors).
        /// </summary>
        string BuildFrom(IHttpAssertContext context);
    }

    internal sealed class CurlBuilder : ICurlBuilder
    {
        public string BuildFrom(IHttpResponseContext context)
        {
            var httpRequestMessage = context.HttpResponseMessage.RequestMessage;

            if (httpRequestMessage.IsNull())
            {
                return string.Empty;
            }

            var curl = BuildCurlFromResponse(context).Flatten(@$" \{Environment.NewLine}");

            return curl;
        }

        public string BuildFrom(IHttpAssertContext context)
        {
            var curl = BuildCurlFromContext(context).Flatten(@$" \{Environment.NewLine}");

            return curl;
        }

        private static IEnumerable<string> BuildCurlFromResponse(IHttpResponseContext context)
        {
            var httpRequestMessage = context.HttpResponseMessage.RequestMessage!;
            var payloadAsJson = context.ResolvedPayload ?? context.PayloadAsJson ?? string.Empty;
            var authenticationHeaderValue = context.Client.DefaultRequestHeaders.Authorization;
            var showTokenInCurl = context.ShowTokenInCurl;

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
                var token = JToken.Parse(payloadAsJson);
                var flattenedJson = token.ToString(Formatting.None);

                yield return "--header 'Content-Type: application/json'";
                yield return $"--data-raw '{flattenedJson}'";
            }
        }

        private static IEnumerable<string> BuildCurlFromContext(IHttpAssertContext context)
        {
            // Build full URL from client base address and relative URL
            var fullUrl = context.Client.BaseAddress.IsNotNull()
                              ? new Uri(context.Client.BaseAddress, context.Url).ToString()
                              : context.Url;

            var authenticationHeaderValue = context.Client.DefaultRequestHeaders.Authorization;
            var showTokenInCurl = context.ShowTokenInCurl;
            var payloadAsJson = context.ResolvedPayload ?? string.Empty;

            // base curl call
            yield return "curl";
            yield return "--location";
            yield return $"--request {context.HttpMethod.Method} '{fullUrl}'";

            // Add authorization header if present - RESPECT ShowTokenInCurl flag
            if (authenticationHeaderValue.IsNotNull())
            {
                var token = showTokenInCurl ? authenticationHeaderValue.Parameter : "Sorry i am secret :)";

                yield return $"--header 'Authorization: {authenticationHeaderValue.Scheme} {token}'";
            }

            // Add payload if present
            if (payloadAsJson.IsNotNullOrWhiteSpace())
            {
                var token = JToken.Parse(payloadAsJson);
                var flattenedJson = token.ToString(Formatting.None);

                yield return "--header 'Content-Type: application/json'";
                yield return $"--data-raw '{flattenedJson}'";
            }
        }
    }
}
