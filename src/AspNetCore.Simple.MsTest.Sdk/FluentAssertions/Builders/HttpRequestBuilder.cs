using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
{
    /// <summary>
    ///     Builds the request stage of a fluent HTTP assertion chain (Endpoint-Stil, §15.6).
    ///     Body input is explicit (object / raw JSON / embedded file); all three funnel to a single
    ///     <c>_body</c> string, and the engine's file localizer resolves file-vs-raw downstream.
    /// </summary>
    internal sealed class HttpRequestBuilder : IHttpRequestConfiguring
    {
        private readonly string _callerFilePath;

        private readonly string _callerMemberName;

        private readonly int _callerLineNumber;

        private readonly Assembly _callingAssembly;

        private readonly HttpClient _client;

        private readonly Dictionary<string, string> _headers = new();

        private readonly HttpMethod _method;

        private readonly List<(string Key, object? Value)> _parameters = new();

        private readonly string _url;

        private string? _body;

        internal HttpRequestBuilder(HttpClient client,
                                    HttpMethod method,
                                    string url,
                                    Assembly callingAssembly,
                                    string callerFilePath,
                                    string callerMemberName,
                                    int callerLineNumber)
        {
            _client = client;
            _method = method;
            _url = url;
            _callingAssembly = callingAssembly;
            _callerFilePath = callerFilePath;
            _callerMemberName = callerMemberName;
            _callerLineNumber = callerLineNumber;
        }

        // ============================================================
        // Request body — explicit, no rate-heuristic (Schema A).
        // ============================================================

        public IHttpRequestConfiguring Accepts<T>(T body)
        {
            _body = JsonSerializer.Serialize(body, HttpClientAssertExtensions.JsonSerializerOptionsFor(_callingAssembly));

            return this;
        }

        [Obsolete("Accepts(string) is not a body. In the Minimal API the string argument is the CONTENT TYPE, here it would be sent AS the body. Use AcceptsFromJsonString(json) for raw JSON, AcceptsFromEmbeddedJson(fileName) for an embedded file, or Accepts<T>(obj) for an object.", error: true)]
        public IHttpRequestConfiguring Accepts(string bodyJson)
        {
            throw new NotSupportedException("Accepts(string) is not a request body. Use AcceptsFromJsonString or AcceptsFromEmbeddedJson.");
        }

        public IHttpRequestConfiguring AcceptsFromJsonString(string bodyJson)
        {
            _body = bodyJson;

            return this;
        }

        public IHttpRequestConfiguring AcceptsFromEmbeddedJson(string embeddedFileName)
        {
            _body = embeddedFileName;

            return this;
        }

        // ============================================================
        // Placeholder parameters — naked names, SDK escapes internally (§10.1).
        // ============================================================

        public IHttpRequestConfiguring WithParameter(string key,
                                                     object? value)
        {
            _parameters.Add((PlaceholderName.Wrap(key), value));

            return this;
        }

        public IHttpRequestConfiguring WithParameters(params (string Key, object? Value)[] parameters)
        {
            foreach (var (key, value) in parameters)
            {
                _parameters.Add((PlaceholderName.Wrap(key), value));
            }

            return this;
        }

        public IHttpRequestConfiguring WithParameters((string Key, object? Value) parameter)
        {
            return WithParameter(parameter.Key, parameter.Value);
        }

        public IHttpRequestConfiguring WithParameters(object source)
        {
            foreach (var property in source.GetType().GetProperties())
            {
                _parameters.Add((PlaceholderName.Wrap(property.Name), property.GetValue(source)));
            }

            return this;
        }

        public IHttpRequestConfiguring WithHeader(string key,
                                                  string value)
        {
            _headers[key] = value;

            return this;
        }

        // ============================================================
        // Transition to the response stage. Produces<T>(code) carries type + status + return type.
        // ============================================================

        public IHttpResponseConfiguring<T> Produces<T>(HttpStatusCode statusCode)
        {
            return new HttpResponseBuilder<T>(_client,
                                              _method,
                                              _url,
                                              _body,
                                              _parameters,
                                              _headers,
                                              _callingAssembly,
                                              _callerFilePath,
                                              _callerMemberName,
                                              _callerLineNumber,
                                              statusCode);
        }

        public IHttpResponseConfiguring<T> Produces<T>(int statusCode)
        {
            return Produces<T>((HttpStatusCode)statusCode);
        }

        public IHttpExpectationConfiguring Produces(HttpStatusCode statusCode)
        {
            return new HttpExpectationBuilder(_client, _method, _url,
                                              _body, _parameters, _headers,
                                              _callingAssembly,
                                              _callerFilePath,
                                              _callerMemberName,
                                              _callerLineNumber,
                                              statusCode);
        }

        public IHttpExpectationConfiguring Produces(int statusCode)
        {
            return Produces((HttpStatusCode)statusCode);
        }
    }
}