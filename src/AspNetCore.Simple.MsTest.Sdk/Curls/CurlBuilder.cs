using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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
        string BuildFrom(HttpMethod httpMethod,
                         string url,
                         string payloadAsJson,
                         AuthenticationHeaderValue? authenticationHeaderValue,
                         Assembly assembly,
                         bool showTokenInCurl);

        string BuildFrom(HttpResponseMessage httpResponseMessage,
                         string payloadAsJson,
                         AuthenticationHeaderValue? authenticationHeaderValue,
                         Assembly assembly,
                         bool showTokenInCurl);

        string BuildFrom(HttpRequestMessage httpRequestMessage,
                         string payloadAsJson,
                         AuthenticationHeaderValue? authenticationHeaderValue,
                         Assembly assembly,
                         bool showTokenInCurl);
    }

    internal sealed class CurlBuilder : ICurlBuilder
    {
        public string BuildFrom(HttpMethod httpMethod,
                                string url,
                                string payloadAsJson,
                                AuthenticationHeaderValue? authenticationHeaderValue,
                                Assembly assembly,
                                bool showTokenInCurl)
        {
            var curl = BuildCurl(httpMethod, url, payloadAsJson, authenticationHeaderValue, assembly, showTokenInCurl).Flatten(@$" \{Environment.NewLine}");
            return curl;

            static IEnumerable<string> BuildCurl(HttpMethod httpMethod,
                                                 string url,
                                                 string payloadAsJson,
                                                 AuthenticationHeaderValue? authenticationHeaderValue,
                                                 Assembly assembly,
                                                 bool showTokenInCurl)
            {
                // base curl call
                yield return "curl";
                yield return "--location";
                yield return $"--request {httpMethod} '{url}'";

                if (authenticationHeaderValue.IsNotNull())
                {
                    var token = showTokenInCurl ? authenticationHeaderValue.Parameter : "Sorry i am secret :)";
                    yield return $"--header 'Authorization: {authenticationHeaderValue.Scheme} {token}'";
                }

                if (payloadAsJson.IsNotNullOrWhiteSpace())
                {
                    var json = payloadAsJson.GetJsonString<object>(assembly);
                    yield return "--header 'Content-Type: application/json'";
                    yield return $"--data-raw '{json}'";
                }
            }
        }

        public string BuildFrom(HttpResponseMessage httpResponseMessage,
                                string payloadAsJson,
                                AuthenticationHeaderValue? authenticationHeaderValue,
                                Assembly assembly,
                                bool showTokenInCurl)
        {
            return BuildFrom(httpResponseMessage?.RequestMessage, payloadAsJson, authenticationHeaderValue, assembly, showTokenInCurl);
        }

        public string BuildFrom(HttpRequestMessage? httpRequestMessage,
                                string payloadAsJson,
                                AuthenticationHeaderValue? authenticationHeaderValue,
                                Assembly assembly,
                                bool showTokenInCurl)
        {
            if (httpRequestMessage.IsNull())
            {
                return string.Empty;
            }

            var curl = BuildCurl(httpRequestMessage, payloadAsJson, authenticationHeaderValue, assembly, showTokenInCurl).Flatten(@$" \{Environment.NewLine}");
            return curl;

            static IEnumerable<string> BuildCurl(HttpRequestMessage httpRequestMessage,
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
                    yield return $"--header '{requestMessageHeader.Key}: {requestMessageHeader.Value}";
                }
                
                if (payloadAsJson.IsNotNullOrWhiteSpace())
                {
                    var json = payloadAsJson.GetJsonString<object>(assembly);
                    yield return "--header 'Content-Type: application/json'";
                    yield return $"--data-raw '{json}'";
                }
            }
        }
    }
}
