using System.Net;
using System.Reflection;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
{
    /// <summary>
    /// Builder for configuring HTTP requests in a fluent API style.
    /// Implements IHttpRequestConfiguring for the initial request configuration state.
    /// </summary>
    internal sealed class HttpRequestBuilder : IHttpRequestConfiguring
    {
        private readonly HttpClient _client;

        private readonly HttpMethod _method;

        private readonly string _url;

        private readonly Assembly _callingAssembly;

        private readonly string _callerFilePath;

        private string? _body;

        private readonly List<(string Key, object? Value)> _parameters = new();

        private readonly Dictionary<string, string> _headers = new();

        internal HttpRequestBuilder(
            HttpClient client,
            HttpMethod method,
            string url,
            Assembly callingAssembly,
            string callerFilePath)
        {
            _client = client;
            _method = method;
            _url = url;
            _callingAssembly = callingAssembly;
            _callerFilePath = callerFilePath;
        }

        // ============================================================
        // IHttpRequestConfiguring - Configuration Methods
        // ============================================================

        public IHttpRequestConfiguring WithBody(string bodyJson)
        {
            _body = bodyJson;

            return this;
        }

        public IHttpRequestConfiguring WithBody<T>(T body)
        {
            _body = JsonSerializer.Serialize(body, HttpClientAssertExtensions.JsonSerializerOptions);

            return this;
        }

        public IHttpRequestConfiguring WithParameters(params (string Key, object? Value)[] parameters)
        {
            _parameters.AddRange(parameters);

            return this;
        }

        public IHttpRequestConfiguring WithHeader(string key,
                                                  string value)
        {
            _headers[key] = value;

            return this;
        }

        // ============================================================
        // IHttpRequestConfiguring - Response Configuration Transition
        // ============================================================

        public IHttpResponseConfiguring<TResult> WithResponse<TResult>(string expectedJson)
        {
            return new HttpResponseBuilder<TResult>(_client,
                                                    _method,
                                                    _url,
                                                    _body,
                                                    _parameters,
                                                    _headers,
                                                    _callingAssembly,
                                                    _callerFilePath,
                                                    expectedJson);
        }

        // ============================================================
        // IHttpRequestConfiguring - Terminal Operations (Status Only)
        // ============================================================

        public IHttpStatusAssertable ExpectSuccess()
        {
            return new HttpStatusOnlyBuilder(_client,
                                             _method,
                                             _url,
                                             _body,
                                             _parameters,
                                             _headers,
                                             expectedStatusCodes: null); // null = IsSuccessStatusCode
        }

        public IHttpStatusAssertable Expect(params HttpStatusCode[] codes)
        {
            if (codes.Length == 0)
            {
                throw new ArgumentException("At least one status code must be provided.", nameof(codes));
            }

            return new HttpStatusOnlyBuilder(_client,
                                             _method,
                                             _url,
                                             _body,
                                             _parameters,
                                             _headers,
                                             expectedStatusCodes: codes);
        }

        public IHttpStatusAssertable ExpectNoContent()
        {
            return new HttpStatusOnlyBuilder(_client,
                                             _method,
                                             _url,
                                             _body,
                                             _parameters,
                                             _headers,
                                             expectedStatusCodes: new[] { HttpStatusCode.NoContent });
        }

        public IHttpStatusAssertable ExpectError(HttpStatusCode code)
        {
            return new HttpStatusOnlyBuilder(_client,
                                             _method,
                                             _url,
                                             _body,
                                             _parameters,
                                             _headers,
                                             expectedStatusCodes: new[] { code });
        }
    }
}