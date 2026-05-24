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
            var showTokenInCurl = context.ShowTokenInCurl;

            // base curl call
            yield return "curl";
            yield return "--location";
            yield return $"--request {httpRequestMessage.Method} '{httpRequestMessage.RequestUri}'";

            // Handle all headers from the actual request message
            foreach (var requestMessageHeader in httpRequestMessage.Headers)
            {
                // Authorization header needs special handling - mask the token unless ShowTokenInCurl is true
                if (requestMessageHeader.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))
                {
                    var authValue = requestMessageHeader.Value.Flatten(", ");

                    // Parse scheme and token from "Bearer <token>" format
                    var parts = authValue.Split(' ', 2);

                    if (parts.Length == 2 && !showTokenInCurl)
                    {
                        yield return $"--header 'Authorization: {parts[0]} Sorry i am secret :)'";
                    }
                    else
                    {
                        yield return $"--header 'Authorization: {authValue}'";
                    }

                    continue;
                }

                yield return $"--header '{requestMessageHeader.Key}: {requestMessageHeader.Value.Flatten(", ")}'";
            }

            if (payloadAsJson.IsNotNullOrWhiteSpace())
            {
                string flattenedJson;

                try
                {
                    var token = JToken.Parse(payloadAsJson);
                    flattenedJson = token.ToString(Formatting.None);
                }
                catch (JsonReaderException)
                {
                    // If JSON parsing fails (e.g., invalid JSON like "[}"), use the raw payload
                    flattenedJson = payloadAsJson;
                }

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
                string flattenedJson;

                try
                {
                    var token = JToken.Parse(payloadAsJson);
                    flattenedJson = token.ToString(Formatting.None);
                }
                catch (JsonReaderException)
                {
                    // If JSON parsing fails (e.g., invalid JSON like "[}"), use the raw payload
                    flattenedJson = payloadAsJson;
                }

                yield return "--header 'Content-Type: application/json'";
                yield return $"--data-raw '{flattenedJson}'";
            }
        }
    }
}