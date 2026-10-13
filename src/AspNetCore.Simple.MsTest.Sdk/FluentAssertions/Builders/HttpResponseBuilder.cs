using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Builds the response + comparison stage of a fluent HTTP assertion chain (Endpoint-Stil, §15.6).
    /// The status + return type come from <c>Produces&lt;T&gt;(code)</c>; an optional <c>ExpectedResponse…</c>
    /// attaches a body to compare against. The single terminal is <see cref="ExecuteAsync"/>.
    ///
    /// <para>
    /// Implements both <see cref="IHttpResponseConfiguring{TResult}"/> (before an expected body) and
    /// <see cref="IHttpComparisonConfiguring{TResult}"/> (after) — but the type-state is enforced by the
    /// interfaces: comparison config is only reachable once <c>ExpectedResponse…</c> returned the
    /// comparison view.
    /// </para>
    ///
    /// <para>
    /// Immutable, like <see cref="HttpRequestBuilder"/>: every configuring call returns a NEW builder. A stored
    /// <c>Produces&lt;T&gt;(…)</c> or <c>ExpectedResponse…</c> stage can be branched into a strict and a lenient
    /// chain without the lenient ignore rules leaking into the strict one.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The expected response type.</typeparam>
    internal sealed class HttpResponseBuilder<TResult> : IHttpResponseConfiguring<TResult>, IHttpComparisonConfiguring<TResult>
    {
        private readonly HttpClient _client;

        private readonly HttpMethod _method;

        private readonly string _url;

        private readonly string? _body;

        private readonly string _bodyExpression;

        private readonly ImmutableList<(string Key, object? Value)> _requestParameters;

        private readonly ImmutableDictionary<string, string> _headers;

        private readonly Assembly _callingAssembly;

        private readonly string _callerFilePath;

        private readonly string _callerMemberName;

        private readonly int _callerLineNumber;

        private readonly HttpStatusCode _expectedStatusCode;

        private readonly bool _skipEndpointValidation;

        // Null until an ExpectedResponse… is set → body-less path (deserialize + return, no comparison).
        private string? _expectedJson;

        // The caller's own argument expression for the expected body - see HttpRequestBuilder._bodyExpression.
        private string _expectedExpression = string.Empty;

        private Func<TResult?, TResult?>? _filterFunc;

        private Func<ImmutableList<Difference>, IEnumerable<Difference>>? _differenceFunc;

        // True = ignore. Handed to the engine inverted, as its keep-predicate (differenceFilter).
        private Predicate<Difference>? _ignore;

        private bool _writeSnapshot;

        internal HttpResponseBuilder(HttpClient client,
                                     HttpMethod method,
                                     string url,
                                     string? body,
                                     string bodyExpression,
                                     ImmutableList<(string Key, object? Value)> requestParameters,
                                     ImmutableDictionary<string, string> headers,
                                     Assembly callingAssembly,
                                     string callerFilePath,
                                     string callerMemberName,
                                     int callerLineNumber,
                                     HttpStatusCode expectedStatusCode,
                                     bool skipEndpointValidation)
        {
            _client = client;
            _method = method;
            _url = url;
            _body = body;
            _bodyExpression = bodyExpression;
            _requestParameters = requestParameters;
            _headers = headers;
            _callingAssembly = callingAssembly;
            _callerFilePath = callerFilePath;
            _callerMemberName = callerMemberName;
            _callerLineNumber = callerLineNumber;
            _expectedStatusCode = expectedStatusCode;
            _skipEndpointValidation = skipEndpointValidation;
        }

        // ============================================================
        // Expected body (Schema A) → transition to comparison config.
        // ============================================================

        public IHttpComparisonConfiguring<TResult> ExpectedResponse(TResult expected,
                                                                    [CallerArgumentExpression(nameof(expected))] string expectedExpression = "")
        {
            return WithExpected(JsonSerializer.Serialize(expected, HttpClientAssertExtensions.JsonSerializerOptionsFor(_callingAssembly)),
                                expectedExpression);
        }

        public IHttpComparisonConfiguring<TResult> ExpectedResponseFromJsonString(string expectedJson,
                                                                                  [CallerArgumentExpression(nameof(expectedJson))] string expectedJsonExpression = "")
        {
            return WithExpected(expectedJson, expectedJsonExpression);
        }

        public IHttpComparisonConfiguring<TResult> ExpectedResponseFromEmbeddedJson(string embeddedFileName,
                                                                                    [CallerArgumentExpression(nameof(embeddedFileName))] string embeddedFileNameExpression = "")
        {
            return WithExpected(embeddedFileName, embeddedFileNameExpression);
        }

        // ============================================================
        // Comparison config (only reachable after ExpectedResponse…).
        // ============================================================

        public IHttpComparisonConfiguring<TResult> FilterResponse(Func<TResult?, TResult?> filter)
        {
            return With(copy => copy._filterFunc = filter);
        }

        // Ignore rules COMPOSE instead of overwriting: two IgnoreProperty calls (or IgnoreProperty next to
        // an IgnorePaths) each have to keep their effect. Overwriting would silently drop the earlier one
        // and let the difference it was hiding fail the test for no visible reason.
        public IHttpComparisonConfiguring<TResult> IgnoreDifferencesWhere(Predicate<Difference> ignore)
        {
            var previous = _ignore;

            return With(copy => copy._ignore = previous is null
                                                   ? ignore
                                                   : difference => previous(difference) || ignore(difference));
        }

        public IHttpComparisonConfiguring<TResult> IgnorePaths(params string[] paths)
        {
            var patterns = paths.Select(MemberPathPattern.Parse).ToImmutableList();

            return IgnoreDifferencesWhere(d => patterns.Any(pattern => pattern.Matches(d.MemberPath)));
        }

        public IHttpComparisonConfiguring<TResult> IgnoreProperty(Expression<Func<TResult, object?>> propertySelector)
        {
            var pattern = MemberPathPattern.FromExpression(propertySelector, HttpClientAssertExtensions.JsonSerializerOptionsFor(_callingAssembly));

            return IgnoreDifferencesWhere(d => pattern.Matches(d.MemberPath));
        }

        public IHttpComparisonConfiguring<TResult> FilterDifferences(Func<ImmutableList<Difference>, IEnumerable<Difference>> filter)
        {
            var previous = _differenceFunc;

            return With(copy => copy._differenceFunc = previous is null
                                                           ? filter
                                                           : diffs => filter(previous(diffs).ToImmutableList()));
        }

        public IHttpComparisonConfiguring<TResult> WriteSnapshot(bool write = true)
        {
            return With(copy => copy._writeSnapshot = write);
        }

        private HttpResponseBuilder<TResult> WithExpected(string expectedJson,
                                                          string expectedExpression)
        {
            return With(copy =>
                        {
                            copy._expectedJson = expectedJson;
                            copy._expectedExpression = expectedExpression;
                        });
        }

        // Copy-on-write - see HttpRequestBuilder.With. Delegates are immutable, so a shallow copy is enough.
        private HttpResponseBuilder<TResult> With(Action<HttpResponseBuilder<TResult>> change)
        {
            var copy = (HttpResponseBuilder<TResult>)MemberwiseClone();
            change(copy);

            return copy;
        }

        // ============================================================
        // THE one terminal (shared by both interface views).
        // ============================================================

        public Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            var isSuccessTest = (int)_expectedStatusCode is >= 200 and < 300;

            // No ExpectedResponse… → body-less path: empty expected + ignoreResponse=true means the
            // engine deserializes + returns the real response but skips the body comparison (§15.6).
            var expectedResult = _expectedJson ?? string.Empty;

            return _client.AssertHttpCallAsync(url: _url,
                                               payloadAsJson: _body ?? string.Empty,
                                               expectedResult: expectedResult,
                                               filterFunc: _filterFunc ?? (x => x),
                                               httpMethod: _method,
                                               differenceFunc: _differenceFunc ?? (d => d),
                                               parameters: _requestParameters.ToArray(),
                                               callingAssembly: _callingAssembly,
                                               isEmptyAnonymous: null,
                                               isSuccessStatusCode: isSuccessTest,
                                               writeResponse: _writeSnapshot,
                                               ignoreResponse: _expectedJson == null,
                                               skipEndpointValidation: _skipEndpointValidation,
                                               expectedHttpStatusCode: _expectedStatusCode,
                                               differenceFilter: _ignore is null ? null : d => _ignore(d).IsFalse(),
                                               requestHeaders: _headers,
                                               cancellationToken: cancellationToken,
                                               payloadAsJsonParameterName: _bodyExpression,
                                               expectedResultParameterName: _expectedExpression,
                                               callerFilePath: _callerFilePath,
                                               callerMemberName: _callerMemberName,
                                               callerLineNumber: _callerLineNumber);
        }
    }
}