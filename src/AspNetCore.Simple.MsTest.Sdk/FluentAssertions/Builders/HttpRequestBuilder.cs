using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
{
    /// <summary>
    /// Builds the request stage of a fluent HTTP assertion chain.
    /// Body input is explicit (object / raw JSON / embedded file); all three funnel to a single
    /// <c>payloadAsJson</c> string, and the engine's file localizer resolves file-vs-raw downstream.
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

        internal HttpRequestBuilder(HttpClient client,
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
        // Body — explicit, no rate-heuristic.
        // ============================================================

        public IHttpRequestConfiguring WithBody<T>(T body)
        {
            _body = JsonSerializer.Serialize(body, HttpClientAssertExtensions.JsonSerializerOptions);

            return this;
        }

        public IHttpRequestConfiguring WithJsonString(string bodyJson)
        {
            _body = bodyJson;

            return this;
        }

        public IHttpRequestConfiguring WithEmbeddedJson(string embeddedFileName)
        {
            _body = embeddedFileName;

            return this;
        }

        public IHttpRequestConfiguring WithParameters(params (string Key, object? Value)[] parameters)
        {
            _parameters.AddRange(parameters);

            return this;
        }

        public IHttpRequestConfiguring WithHeader(string key, string value)
        {
            _headers[key] = value;

            return this;
        }

        // ============================================================
        // Transition to response configuration (expected body). <T> mandatory on all three.
        // ============================================================

        public IHttpResponseConfiguring<TResult> Returns<TResult>(TResult expected)
        {
            var expectedJson = JsonSerializer.Serialize(expected, HttpClientAssertExtensions.JsonSerializerOptions);

            return CreateResponseBuilder<TResult>(expectedJson);
        }

        public IHttpResponseConfiguring<TResult> ReturnsJsonString<TResult>(string expectedJson)
        {
            return CreateResponseBuilder<TResult>(expectedJson);
        }

        public IHttpResponseConfiguring<TResult> ReturnsEmbeddedJson<TResult>(string embeddedFileName)
        {
            return CreateResponseBuilder<TResult>(embeddedFileName);
        }

        // ============================================================
        // Transition to status-only expectation (no body comparison).
        // ============================================================

        public IHttpExpectationConfiguring ExpectingResponse()
        {
            return new HttpExpectationBuilder(_client, _method, _url, _body, _parameters, _headers);
        }

        private HttpResponseBuilder<TResult> CreateResponseBuilder<TResult>(string expectedJson)
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
    }
}
