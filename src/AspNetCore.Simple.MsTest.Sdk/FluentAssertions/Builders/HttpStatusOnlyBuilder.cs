using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
{
    /// <summary>
    /// Builder for HTTP assertions that only validate status codes without response body checks.
    /// Implements IHttpStatusAssertable as a terminal operation.
    /// </summary>
    internal sealed class HttpStatusOnlyBuilder : IHttpStatusAssertable
    {
        private readonly HttpClient _client;

        private readonly HttpMethod _method;

        private readonly string _url;

        private readonly string? _body;

        private readonly List<(string Key, object? Value)> _parameters;

        private readonly Dictionary<string, string> _headers;

        private readonly HttpStatusCode[]? _expectedStatusCodes;

        internal HttpStatusOnlyBuilder(HttpClient client,
                                       HttpMethod method,
                                       string url,
                                       string? body,
                                       List<(string Key, object? Value)> parameters,
                                       Dictionary<string, string> headers,
                                       HttpStatusCode[]? expectedStatusCodes)
        {
            _client = client;
            _method = method;
            _url = url;
            _body = body;
            _parameters = parameters;
            _headers = headers;
            _expectedStatusCodes = expectedStatusCodes;
        }

        // ============================================================
        // IHttpStatusAssertable - Terminal Operation
        // ============================================================

        public async Task ExecuteAsync()
        {
            var response = await SendRequestAsync().ConfigureAwait(false);

            AssertStatusCode(response);
        }

        public TaskAwaiter GetAwaiter()
        {
            return ExecuteAsync().GetAwaiter();
        }

        // ============================================================
        // Private Execution Logic
        // ============================================================

        private async Task<HttpResponseMessage> SendRequestAsync()
        {
            using var request = new HttpRequestMessage(_method, _url);

            // Apply body if present
            if (_body != null)
            {
                var resolvedBody = _body.ResolveParameters(_parameters.ToArray());

                request.Content = new StringContent(resolvedBody,
                                                    Encoding.UTF8,
                                                    "application/json");
            }

            // Apply headers
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
                // Expect any success status code (2xx)
                if (!response.IsSuccessStatusCode)
                {
                    var errorOutput = BuildErrorOutput(expected: "Success (2xx)",
                                                       actual: $"{(int)response.StatusCode} {response.StatusCode}");

                    Assert.Fail(errorOutput);
                }
            }
            else if (_expectedStatusCodes.Length == 1)
            {
                // Expect specific status code
                var expectedCode = _expectedStatusCodes[0];

                if (response.StatusCode != expectedCode)
                {
                    var errorOutput = BuildErrorOutput(expected: $"{(int)expectedCode} {expectedCode}",
                                                       actual: $"{(int)response.StatusCode} {response.StatusCode}");

                    Assert.Fail(errorOutput);
                }
            }
            else
            {
                // Expect one of multiple status codes
                if (!Enumerable.Contains(_expectedStatusCodes, response.StatusCode))
                {
                    var expectedList = string.Join(" or ", _expectedStatusCodes.Select(c => $"{(int)c} {c}"));

                    var errorOutput = BuildErrorOutput(expected: expectedList,
                                                       actual: $"{(int)response.StatusCode} {response.StatusCode}");

                    Assert.Fail(errorOutput);
                }
            }
        }

        private string BuildErrorOutput(string expected,
                                        string actual)
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