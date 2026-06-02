using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces;

// Import to access AssertHttpCallAsync extension method

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
{
    /// <summary>
    /// Builder for configuring HTTP response validation and transformation.
    /// Implements IHttpResponseConfiguring for response-specific configuration.
    /// </summary>
    /// <typeparam name="TResult">The expected response type</typeparam>
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

        private string _expectedJson;

        private Func<TResult?, TResult?>? _filterFunc;

        private Func<ImmutableList<Difference>, IEnumerable<Difference>>? _differenceFunc;

        private readonly List<(string Key, object? Value)> _expectedParameters = new();

        private bool _writeSnapshot;

        internal HttpResponseBuilder(
            HttpClient client,
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
        // IHttpResponseConfiguring - Configuration Methods
        // ============================================================

        public IHttpResponseConfiguring<TResult> FilterResponse(Func<TResult?, TResult?> filter)
        {
            _filterFunc = filter;

            return this;
        }

        public IHttpResponseConfiguring<TResult> IgnoreDifferences(
            Func<ImmutableList<Difference>, IEnumerable<Difference>> filter)
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
        // IHttpResponseConfiguring - Terminal Operations
        // ============================================================

        public Task<TResult> ExpectSuccess()
        {
            return ExecuteWithAssertionAsync(expectedStatusCodes: null);
        }

        public Task<TResult> Expect(params HttpStatusCode[] codes)
        {
            if (codes.Length == 0)
            {
                throw new ArgumentException("At least one status code must be provided.", nameof(codes));
            }

            return ExecuteWithAssertionAsync(expectedStatusCodes: codes);
        }

        public Task<TResult> ExpectStatus(HttpStatusCode code)
        {
            return ExecuteWithAssertionAsync(expectedStatusCodes: new[] { code });
        }

        public Task<TResult> ExpectError(HttpStatusCode code)
        {
            return ExecuteWithAssertionAsync(expectedStatusCodes: new[] { code });
        }

        // ============================================================
        // Private Execution Logic
        // ============================================================

        private Task<TResult> ExecuteWithAssertionAsync(HttpStatusCode[]? expectedStatusCodes)
        {
            // Merge request and expected parameters
            var allParameters = _requestParameters.Concat(_expectedParameters).ToArray();

            // Determine if this is a success test
            // If no status codes specified (null), it's a success test
            // If status codes are specified, check if the first one is in 2xx range
            var isSuccessTest = expectedStatusCodes == null ||
                                 (expectedStatusCodes.Length > 0 && (int)expectedStatusCodes[0] >= 200 && (int)expectedStatusCodes[0] < 300);

            // Call existing extension method
            return _client.AssertHttpCallAsync(url: _url,
                                                        payloadAsJson: _body ?? string.Empty,
                                                        expectedResult: _expectedJson,
                                                        filterFunc: _filterFunc ?? (x => x),
                                                        httpMethod: _method,
                                                        differenceFunc: _differenceFunc ?? (d => d),
                                                        parameters: allParameters,
                                                        callingAssembly: _callingAssembly,
                                                        payloadAsJsonParameterName: string.Empty,
                                                        expectedResultParameterName: string.Empty,
                                                        skipEndpointValidation: false,
                                                        callerFilePath: _callerFilePath,
                                                        isSuccessStatusCode: isSuccessTest,
                                                        writeResponse: _writeSnapshot,
                                                        expectedHttpStatusCode: expectedStatusCodes?.FirstOrDefault(),
                                                        callerMemberName: string.Empty,
                                                        callerLineNumber: 0);
        }

        // ============================================================
        // Internal Methods for Extensions
        // ============================================================

        /// <summary>
        /// Internal method used by extension methods to set expected JSON and execute with status code.
        /// This allows Produces(statusCode, json) to work on IHttpResponseConfiguring.
        /// </summary>
        internal Task<TResult> SetExpectedJsonAndExecute(string expectedJson,
                                                         HttpStatusCode statusCode)
        {
            _expectedJson = expectedJson;

            return ExpectStatus(statusCode);
        }

        // ============================================================
        // Helper Methods
        // ============================================================

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