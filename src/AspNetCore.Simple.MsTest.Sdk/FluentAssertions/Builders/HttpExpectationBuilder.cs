using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
{
    /// <summary>
    /// Builds a status-only expectation (body-less <c>Produces(code)</c>, Endpoint-Stil §15.6) — no
    /// response body comparison. The single terminal is <see cref="ExecuteAsync"/>; there is no
    /// GetAwaiter, so a forgotten terminal is a dangling fluent-builder statement (MSTESTSDK001), not a
    /// silently green test.
    ///
    /// <para>
    /// Runs through the same engine as the typed path (<c>AssertHttpCallAsync</c>) rather than sending
    /// the request itself. That is what makes the two paths behave alike: embedded-resource bodies get
    /// localized, <c>$Token$</c> placeholders get replaced in body AND url, PATCH gets its
    /// merge-patch content type, and a failure produces the full diagnostic output (curl, response body,
    /// ProblemDetails) instead of a bare status table.
    /// </para>
    /// </summary>
    internal sealed class HttpExpectationBuilder : IHttpExpectationConfiguring
    {
        private readonly Assembly _callingAssembly;

        private readonly string _callerFilePath;

        private readonly string _callerMemberName;

        private readonly int _callerLineNumber;

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
                                        Assembly callingAssembly,
                                        string callerFilePath,
                                        string callerMemberName,
                                        int callerLineNumber,
                                        HttpStatusCode expectedStatusCode)
        {
            _client = client;
            _method = method;
            _url = url;
            _body = body;
            _parameters = parameters;
            _headers = headers;
            _callingAssembly = callingAssembly;
            _callerFilePath = callerFilePath;
            _callerMemberName = callerMemberName;
            _callerLineNumber = callerLineNumber;
            _expectedStatusCode = expectedStatusCode;
        }

        public Task ExecuteAsync()
        {
            var isSuccessTest = (int)_expectedStatusCode is >= 200 and < 300;

            return _client.AssertHttpCallAsync(url: _url,
                                               payloadAsJson: _body ?? string.Empty,
                                               httpMethod: _method,
                                               parameters: _parameters.ToArray(),
                                               callingAssembly: _callingAssembly,
                                               callerFilePath: _callerFilePath,
                                               isSuccessStatusCode: isSuccessTest,

                                               // Produces(code) deliberately names NO response type — it asserts the
                                               // status and nothing else. The endpoint validator compares the declared
                                               // return type against the expected one, so with nothing to compare it
                                               // would reject every endpoint that does return a body. Skipping it here
                                               // keeps "status only" meaning status only; the typed Produces<T>(code)
                                               // path carries a type and validates as before.
                                               skipEndpointValidation: true,
                                               expectedHttpStatusCode: _expectedStatusCode,
                                               requestHeaders: _headers,
                                               callerMemberName: _callerMemberName,
                                               callerLineNumber: _callerLineNumber);
        }
    }
}