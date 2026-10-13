using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
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

        private readonly string _bodyExpression;

        private readonly ImmutableList<(string Key, object? Value)> _parameters;

        private readonly ImmutableDictionary<string, string> _headers;

        private readonly HttpStatusCode _expectedStatusCode;

        internal HttpExpectationBuilder(HttpClient client,
                                        HttpMethod method,
                                        string url,
                                        string? body,
                                        string bodyExpression,
                                        ImmutableList<(string Key, object? Value)> parameters,
                                        ImmutableDictionary<string, string> headers,
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
            _bodyExpression = bodyExpression;
            _parameters = parameters;
            _headers = headers;
            _callingAssembly = callingAssembly;
            _callerFilePath = callerFilePath;
            _callerMemberName = callerMemberName;
            _callerLineNumber = callerLineNumber;
            _expectedStatusCode = expectedStatusCode;
        }

        public IHttpComparisonConfiguring<TExpected> ExpectedResponse<TExpected>(TExpected expected,
                                                                                 [CallerArgumentExpression(nameof(expected))] string expectedExpression = "")
        {
            // The type comes from the expected object, not from the endpoint - there is nothing declared
            // to validate against, so endpoint validation is skipped for the same reason as below.
            var responseBuilder = new HttpResponseBuilder<TExpected>(_client, _method, _url,
                                                                     _body, _bodyExpression, _parameters, _headers,
                                                                     _callingAssembly,
                                                                     _callerFilePath,
                                                                     _callerMemberName,
                                                                     _callerLineNumber,
                                                                     _expectedStatusCode,
                                                                     skipEndpointValidation: true);

            return responseBuilder.ExpectedResponse(expected, expectedExpression);
        }

        public Task ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var isSuccessTest = (int)_expectedStatusCode is >= 200 and < 300;

            return _client.AssertHttpCallAsync(url: _url,
                                               payloadAsJson: _body ?? string.Empty,
                                               httpMethod: _method,
                                               payloadAsJsonParameterName: _bodyExpression,
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
                                               cancellationToken: cancellationToken,
                                               callerMemberName: _callerMemberName,
                                               callerLineNumber: _callerLineNumber);
        }
    }
}