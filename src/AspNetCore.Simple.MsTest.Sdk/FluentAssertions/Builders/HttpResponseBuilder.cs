using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
{
    /// <summary>
    /// Builds the response stage of a fluent HTTP assertion chain (MODEL B).
    /// <c>Expecting…</c> methods are composable config (return <c>this</c>); the single terminal is
    /// <see cref="ExecuteAsync"/>. The builder is intentionally NOT awaitable.
    /// </summary>
    /// <typeparam name="TResult">The expected response type.</typeparam>
    internal sealed class HttpResponseBuilder<TResult> : IHttpResponseConfiguring<TResult>
    {
        private readonly HttpClient _client;

        private readonly HttpMethod _method;

        private readonly string _url;

        private readonly string? _body;

        private readonly List<(string Key, object? Value)> _requestParameters;

        private readonly Dictionary<string, string> _headers;

        private readonly Assembly _callingAssembly;

        private readonly string _callerFilePath;

        private readonly string _expectedJson;

        private Func<TResult?, TResult?>? _filterFunc;

        private Func<ImmutableList<Difference>, IEnumerable<Difference>>? _differenceFunc;

        private readonly List<(string Key, object? Value)> _expectedParameters = new();

        private bool _writeSnapshot;

        // Expectation state (Model B — configured, applied on ExecuteAsync).
        private HttpStatusCode[]? _expectedStatusCodes;

        internal HttpResponseBuilder(HttpClient client,
                                     HttpMethod method,
                                     string url,
                                     string? body,
                                     List<(string Key, object? Value)> requestParameters,
                                     Dictionary<string, string> headers,
                                     Assembly callingAssembly,
                                     string callerFilePath,
                                     string expectedJson)
        {
            _client = client;
            _method = method;
            _url = url;
            _body = body;
            _requestParameters = requestParameters;
            _headers = headers;
            _callingAssembly = callingAssembly;
            _callerFilePath = callerFilePath;
            _expectedJson = expectedJson;
        }

        // ============================================================
        // Response transformation / difference configuration.
        // ============================================================

        public IHttpResponseConfiguring<TResult> FilterResponse(Func<TResult?, TResult?> filter)
        {
            _filterFunc = filter;

            return this;
        }

        public IHttpResponseConfiguring<TResult> IgnoreDifferences(Func<ImmutableList<Difference>, IEnumerable<Difference>> filter)
        {
            _differenceFunc = filter;

            return this;
        }

        public IHttpResponseConfiguring<TResult> IgnoreProperty<T>(Expression<Func<T, object?>> propertySelector)
        {
            var propertyName = ExtractPropertyName(propertySelector);

            return IgnoreDifferences(diffs =>
                                         diffs.Where(d => !d.MemberPath.Equals(propertyName, StringComparison.OrdinalIgnoreCase)));
        }

        public IHttpResponseConfiguring<TResult> WithParameters(params (string Key, object? Value)[] parameters)
        {
            _expectedParameters.AddRange(parameters);

            return this;
        }

        public IHttpResponseConfiguring<TResult> WriteSnapshot(bool write = true)
        {
            _writeSnapshot = write;

            return this;
        }

        // ============================================================
        // Expectations — composable config (Model B).
        // ============================================================

        public IHttpResponseConfiguring<TResult> ExpectingSuccess()
        {
            _expectedStatusCodes = null; // null = any 2xx

            return this;
        }

        public IHttpResponseConfiguring<TResult> ExpectingStatus(HttpStatusCode code)
        {
            _expectedStatusCodes = new[] { code };

            return this;
        }

        public IHttpResponseConfiguring<TResult> ExpectingOneOf(params HttpStatusCode[] codes)
        {
            if (codes.Length == 0)
            {
                throw new ArgumentException("At least one status code must be provided.", nameof(codes));
            }

            _expectedStatusCodes = codes;

            return this;
        }

        public IHttpResponseConfiguring<TResult> ExpectingError(HttpStatusCode code)
        {
            _expectedStatusCodes = new[] { code };

            return this;
        }

        // ============================================================
        // THE one terminal.
        // ============================================================

        public Task<TResult> ExecuteAsync()
        {
            var allParameters = _requestParameters.Concat(_expectedParameters).ToArray();

            var isSuccessTest = _expectedStatusCodes == null ||
                                (_expectedStatusCodes.Length > 0 &&
                                 (int)_expectedStatusCodes[0] >= 200 &&
                                 (int)_expectedStatusCodes[0] < 300);

            return _client.AssertHttpCallAsync(url: _url,
                                               payloadAsJson: _body ?? string.Empty,
                                               expectedResult: _expectedJson,
                                               filterFunc: _filterFunc ?? (x => x),
                                               httpMethod: _method,
                                               differenceFunc: _differenceFunc ?? (d => d),
                                               parameters: allParameters,
                                               callingAssembly: _callingAssembly,
                                               callerFilePath: _callerFilePath,
                                               isSuccessStatusCode: isSuccessTest,
                                               writeResponse: _writeSnapshot,
                                               expectedHttpStatusCode: _expectedStatusCodes?.FirstOrDefault());
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
