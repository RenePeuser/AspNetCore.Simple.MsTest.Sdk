using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class CurlBuilder
    {
        public string BuildFrom(System.Net.Http.HttpMethod httpMethod,
                                string url,
                                string payloadAsJson,
                                AuthenticationHeaderValue? authenticationHeaderValue,
                                Assembly assembly,
                                bool showTokenInCurl)
        {
            var curl = BuildCurl(httpMethod, url, payloadAsJson, authenticationHeaderValue, assembly, showTokenInCurl).Flatten(@$" \{Environment.NewLine}");
            return curl;

            static IEnumerable<string> BuildCurl(System.Net.Http.HttpMethod httpMethod,
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
                    var json = payloadAsJson.GetJsonString(assembly);
                    yield return "--header 'Content-Type: application/json'";
                    yield return $"--data-raw '{json}'";
                }
            }
        }
    }
}
