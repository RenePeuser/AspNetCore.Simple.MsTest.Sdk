using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Builds status-only expectations (no response body comparison) — MODEL B.
    /// <c>Expecting…</c> methods are composable config (return <c>this</c>); the single terminal is
    /// <see cref="ExecuteAsync"/>. There is no GetAwaiter: a forgotten terminal is a dangling
    /// fluent-builder statement caught by the analyzer, not a silently green test.
    /// </summary>
    internal sealed class HttpExpectationBuilder : IHttpExpectationConfiguring
    {
        private readonly HttpClient _client;

        private readonly HttpMethod _method;

        private readonly string _url;

        private readonly string? _body;

        private readonly List<(string Key, object? Value)> _parameters;

        private readonly Dictionary<string, string> _headers;

        private HttpStatusCode[]? _expectedStatusCodes;

        internal HttpExpectationBuilder(HttpClient client,
                                        HttpMethod method,
                                        string url,
                                        string? body,
                                        List<(string Key, object? Value)> parameters,
                                        Dictionary<string, string> headers)
        {
            _client = client;
            _method = method;
            _url = url;
            _body = body;
            _parameters = parameters;
            _headers = headers;
        }

        // ============================================================
        // Expectations — composable config (Model B).
        // ============================================================

        public IHttpExpectationConfiguring ExpectingSuccess()
        {
            _expectedStatusCodes = null; // null = any 2xx

            return this;
        }

        public IHttpExpectationConfiguring ExpectingStatus(HttpStatusCode code)
        {
            _expectedStatusCodes = new[] { code };

            return this;
        }

        public IHttpExpectationConfiguring ExpectingOneOf(params HttpStatusCode[] codes)
        {
            if (codes.Length == 0)
            {
                throw new ArgumentException("At least one status code must be provided.", nameof(codes));
            }

            _expectedStatusCodes = codes;

            return this;
        }

        public IHttpExpectationConfiguring ExpectingNoContent()
        {
            _expectedStatusCodes = new[] { HttpStatusCode.NoContent };

            return this;
        }

        public IHttpExpectationConfiguring ExpectingError(HttpStatusCode code)
        {
            _expectedStatusCodes = new[] { code };

            return this;
        }

        // ============================================================
        // THE one terminal.
        // ============================================================

        public async Task ExecuteAsync()
        {
            var response = await SendRequestAsync().ConfigureAwait(false);

            AssertStatusCode(response);
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

        private void AssertStatusCode(HttpResponseMessage response)
        {
            if (_expectedStatusCodes == null)
            {
                if (!response.IsSuccessStatusCode)
                {
                    Assert.Fail(BuildErrorOutput("Success (2xx)", $"{(int)response.StatusCode} {response.StatusCode}"));
                }
            }
            else if (_expectedStatusCodes.Length == 1)
            {
                var expectedCode = _expectedStatusCodes[0];

                if (response.StatusCode != expectedCode)
                {
                    Assert.Fail(BuildErrorOutput($"{(int)expectedCode} {expectedCode}",
                                                 $"{(int)response.StatusCode} {response.StatusCode}"));
                }
            }
            else if (!Enumerable.Contains(_expectedStatusCodes, response.StatusCode))
            {
                var expectedList = string.Join(" or ", _expectedStatusCodes.Select(c => $"{(int)c} {c}"));

                Assert.Fail(BuildErrorOutput(expectedList, $"{(int)response.StatusCode} {response.StatusCode}"));
            }
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
