using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
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
    /// </summary>
    /// <typeparam name="TResult">The expected response type.</typeparam>
    internal sealed class HttpResponseBuilder<TResult> : IHttpResponseConfiguring<TResult>, IHttpComparisonConfiguring<TResult>
    {
        private readonly HttpClient _client;

        private readonly HttpMethod _method;

        private readonly string _url;

        private readonly string? _body;

        private readonly List<(string Key, object? Value)> _requestParameters;

        private readonly Dictionary<string, string> _headers;

        private readonly Assembly _callingAssembly;

        private readonly string _callerFilePath;

        private readonly HttpStatusCode _expectedStatusCode;

        // Null until an ExpectedResponse… is set → body-less path (deserialize + return, no comparison).
        private string? _expectedJson;

        private Func<TResult?, TResult?>? _filterFunc;

        private Func<ImmutableList<Difference>, IEnumerable<Difference>>? _differenceFunc;

        private Predicate<Difference>? _differenceFilter;

        private bool _writeSnapshot;

        internal HttpResponseBuilder(HttpClient client,
                                     HttpMethod method,
                                     string url,
                                     string? body,
                                     List<(string Key, object? Value)> requestParameters,
                                     Dictionary<string, string> headers,
                                     Assembly callingAssembly,
                                     string callerFilePath,
                                     HttpStatusCode expectedStatusCode)
        {
            _client = client;
            _method = method;
            _url = url;
            _body = body;
            _requestParameters = requestParameters;
            _headers = headers;
            _callingAssembly = callingAssembly;
            _callerFilePath = callerFilePath;
            _expectedStatusCode = expectedStatusCode;
        }

        // ============================================================
        // Expected body (Schema A) → transition to comparison config.
        // ============================================================

        public IHttpComparisonConfiguring<TResult> ExpectedResponse(TResult expected)
        {
            _expectedJson = JsonSerializer.Serialize(expected, HttpClientAssertExtensions.JsonSerializerOptions);

            return this;
        }

        public IHttpComparisonConfiguring<TResult> ExpectedResponseFromJsonString(string expectedJson)
        {
            _expectedJson = expectedJson;

            return this;
        }

        public IHttpComparisonConfiguring<TResult> ExpectedResponseFromEmbeddedJson(string embeddedFileName)
        {
            _expectedJson = embeddedFileName;

            return this;
        }

        // ============================================================
        // Comparison config (only reachable after ExpectedResponse…).
        // ============================================================

        public IHttpComparisonConfiguring<TResult> FilterResponse(Func<TResult?, TResult?> filter)
        {
            _filterFunc = filter;

            return this;
        }

        public IHttpComparisonConfiguring<TResult> IgnoreDifferences(Func<ImmutableList<Difference>, IEnumerable<Difference>> filter)
        {
            _differenceFunc = filter;

            return this;
        }

        public IHttpComparisonConfiguring<TResult> DifferenceFilter(Predicate<Difference> filter)
        {
            _differenceFilter = filter;

            return this;
        }

        public IHttpComparisonConfiguring<TResult> IgnoreProperty<T>(Expression<Func<T, object?>> propertySelector)
        {
            var propertyName = ExtractPropertyName(propertySelector);

            return IgnoreDifferences(diffs =>
                                         diffs.Where(d => !d.MemberPath.Equals(propertyName, StringComparison.OrdinalIgnoreCase)));
        }

        public IHttpComparisonConfiguring<TResult> WriteSnapshot(bool write = true)
        {
            _writeSnapshot = write;

            return this;
        }

        // ============================================================
        // THE one terminal (shared by both interface views).
        // ============================================================

        public Task<TResult> ExecuteAsync()
        {
            var isSuccessTest = (int)_expectedStatusCode >= 200 && (int)_expectedStatusCode < 300;

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
                                               skipEndpointValidation: false,
                                               expectedHttpStatusCode: _expectedStatusCode,
                                               differenceFilter: _differenceFilter,
                                               callerFilePath: _callerFilePath);
        }

        private static string ExtractPropertyName<T>(Expression<Func<T, object?>> propertySelector)
        {
            if (propertySelector.Body is MemberExpression memberExpression)
            {
                return memberExpression.Member.Name;
            }

            if (propertySelector.Body is UnaryExpression { Operand: MemberExpression unaryMember })
            {
                return unaryMember.Member.Name;
            }

            throw new ArgumentException("Property selector must be a simple property access expression (e.g., p => p.PropertyName)",
                                        nameof(propertySelector));
        }
    }
}
