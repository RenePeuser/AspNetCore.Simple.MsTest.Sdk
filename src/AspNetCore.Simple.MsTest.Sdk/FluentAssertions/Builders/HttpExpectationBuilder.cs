using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
{
    /// <summary>
    /// Builds a status-only expectation (body-less <c>Produces(code)</c>, Endpoint-Stil §15.6) — no
    /// response body comparison. The single terminal is <see cref="ExecuteAsync"/>; there is no
    /// GetAwaiter, so a forgotten terminal is a dangling fluent-builder statement (MSTESTSDK001), not a
    /// silently green test.
    /// </summary>
    internal sealed class HttpExpectationBuilder : IHttpExpectationConfiguring
    {
        private readonly HttpClient _client;

        private readonly HttpMethod _method;

        private readonly string _url;

        private readonly string? _body;

        private readonly List<(string Key, object? Value)> _parameters;

        private readonly Dictionary<string, string> _headers;

        private readonly HttpStatusCode _expectedStatusCode;

        internal HttpExpectationBuilder(HttpClient client,
                                        HttpMethod method,
                                        string url,
                                        string? body,
                                        List<(string Key, object? Value)> parameters,
                                        Dictionary<string, string> headers,
                                        HttpStatusCode expectedStatusCode)
        {
            _client = client;
            _method = method;
            _url = url;
            _body = body;
            _parameters = parameters;
            _headers = headers;
            _expectedStatusCode = expectedStatusCode;
        }

        public async Task ExecuteAsync()
        {
            var response = await SendRequestAsync().ConfigureAwait(false);

            if (response.StatusCode != _expectedStatusCode)
            {
                Assert.Fail(BuildErrorOutput($"{(int)_expectedStatusCode} {_expectedStatusCode}",
                                             $"{(int)response.StatusCode} {response.StatusCode}"));
            }
        }

        private async Task<HttpResponseMessage> SendRequestAsync()
        {
            using var request = new HttpRequestMessage(_method, _url);

            if (_body != null)
            {
                var resolvedBody = _body.ResolveParameters(_parameters.ToArray());

                request.Content = new StringContent(resolvedBody, Encoding.UTF8, "application/json");
            }

            foreach (var (key, value) in _headers)
            {
                request.Headers.TryAddWithoutValidation(key, value);
            }

            return await _client.SendAsync(request).ConfigureAwait(false);
        }

        private string BuildErrorOutput(string expected, string actual)
        {
            var data = new[]
                       {
                           new
                           {
                               Request = $"{_method.Method} {_url}",
                               Expected = expected,
                               Actual = actual
                           }
                       };

            var table = TableFormatter.From(data);

            return $"{Environment.NewLine}{Environment.NewLine}{table}";
        }
    }
}
