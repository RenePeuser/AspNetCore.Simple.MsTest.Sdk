using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    ///     Builds the request stage of a fluent HTTP assertion chain (Endpoint-Stil, §15.6).
    ///     Body input is explicit (object / raw JSON / embedded file); all three funnel to a single
    ///     <c>_body</c> string, and the engine's file localizer resolves file-vs-raw downstream.
    ///
    ///     <para>
    ///     Serves both request stages: <see cref="IHttpBodyRequestConfiguring"/> (POST/PUT/PATCH/QUERY, body not
    ///     set yet) and <see cref="IHttpRequestConfiguring"/> (GET/DELETE, or a body verb after <c>Accepts…</c>).
    ///     Which methods a caller can reach is decided by the interface the previous call returned - the entry
    ///     points and <c>Accepts…</c> pick it, so a body can be set at most once and never on GET/DELETE.
    ///     </para>
    ///
    ///     <para>
    ///     Immutable: every configuring call returns a NEW builder and leaves this one untouched. A base
    ///     request can be stored and branched (<c>baseRequest.WithParameter("Id", 1)</c> /
    ///     <c>baseRequest.WithParameter("Id", 2)</c>) without one branch leaking into the other or into a chain
    ///     already derived through <c>Produces…</c>.
    ///     </para>
    /// </summary>
    internal sealed class HttpRequestBuilder : IHttpRequestConfiguring, IHttpBodyRequestConfiguring
    {
        private readonly string _callerFilePath;

        private readonly string _callerMemberName;

        private readonly int _callerLineNumber;

        private readonly Assembly _callingAssembly;

        private readonly HttpClient _client;

        // Header names are case-insensitive - "x-id" and "X-Id" are the same header.
        private ImmutableDictionary<string, string> _headers = ImmutableDictionary.Create<string, string>(StringComparer.OrdinalIgnoreCase);

        private readonly HttpMethod _method;

        private ImmutableList<(string Key, object? Value)> _parameters = ImmutableList<(string Key, object? Value)>.Empty;

        // Query entries in call order, NOT encoded yet - RequestUrl() encodes them when the chain leaves the request
        // stage. A collection value contributes one entry per element under the same key.
        private ImmutableList<(string Key, string Value)> _query = ImmutableList<(string Key, string Value)>.Empty;

        private readonly string _url;

        private string? _body;

        // The caller's own argument expression for the body (e.g. "CreatePerson.json" or fileName). Failure
        // output shows it exactly like the classic API's [CallerArgumentExpression] does - without it the
        // engine would capture the builder's internal field expression instead.
        private string _bodyExpression = string.Empty;

        private bool _skipEndpointValidation;

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
        // Request body — explicit, no rate-heuristic (Schema A). Only on IHttpBodyRequestConfiguring, and
        // every Accepts… returns IHttpRequestConfiguring, which has none: once per chain by type-state.
        // ============================================================

        public IHttpRequestConfiguring Accepts<T>(T body,
                                                  [CallerArgumentExpression(nameof(body))] string bodyExpression = "")
        {
            return WithBody(JsonSerializer.Serialize(body, HttpClientAssertExtensions.JsonSerializerOptionsFor(_callingAssembly)),
                            bodyExpression);
        }

        [Obsolete("Accepts(string) is not a body. In the Minimal API the string argument is the CONTENT TYPE, here it would be sent AS the body. Use AcceptsFromJsonString(json) for raw JSON, AcceptsFromEmbeddedJson(fileName) for an embedded file, or Accepts<T>(obj) for an object.", error: true)]
        public IHttpRequestConfiguring Accepts(string bodyJson)
        {
            throw new NotSupportedException("Accepts(string) is not a request body. Use AcceptsFromJsonString or AcceptsFromEmbeddedJson.");
        }

        public IHttpRequestConfiguring AcceptsFromJsonString(string bodyJson,
                                                             [CallerArgumentExpression(nameof(bodyJson))] string bodyJsonExpression = "")
        {
            return WithBody(bodyJson, bodyJsonExpression);
        }

        public IHttpRequestConfiguring AcceptsFromEmbeddedJson(string embeddedFileName,
                                                               [CallerArgumentExpression(nameof(embeddedFileName))] string embeddedFileNameExpression = "")
        {
            return WithBody(embeddedFileName, embeddedFileNameExpression);
        }

        // ============================================================
        // Parameters / headers / validation. Both request stages offer them, each with its OWN return type
        // (the body stage keeps the body open) - so they are implemented explicitly per interface and share
        // one implementation each.
        // ============================================================

        IHttpRequestConfiguring IHttpRequestConfiguring.WithParameter(string key,
                                                                      object? value)
        {
            return AddParameters([(key, value)]);
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.WithParameter(string key,
                                                                              object? value)
        {
            return AddParameters([(key, value)]);
        }

        IHttpRequestConfiguring IHttpRequestConfiguring.WithParameters(params (string Key, object? Value)[] parameters)
        {
            return AddParameters(parameters);
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.WithParameters(params (string Key, object? Value)[] parameters)
        {
            return AddParameters(parameters);
        }

        IHttpRequestConfiguring IHttpRequestConfiguring.WithParameters((string Key, object? Value) parameter)
        {
            return AddParameters([parameter]);
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.WithParameters((string Key, object? Value) parameter)
        {
            return AddParameters([parameter]);
        }

        IHttpRequestConfiguring IHttpRequestConfiguring.WithParameters(IReadOnlyDictionary<string, object?> parameters)
        {
            return AddParameters(parameters.Select(parameter => (parameter.Key, parameter.Value)));
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.WithParameters(IReadOnlyDictionary<string, object?> parameters)
        {
            return AddParameters(parameters.Select(parameter => (parameter.Key, parameter.Value)));
        }

        IHttpRequestConfiguring IHttpRequestConfiguring.WithParameters(object source)
        {
            return AddParametersFrom(source);
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.WithParameters(object source)
        {
            return AddParametersFrom(source);
        }

        IHttpRequestConfiguring IHttpRequestConfiguring.WithQueryParameter(string key,
                                                                           object? value)
        {
            return AddQueryParameter(key, value);
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.WithQueryParameter(string key,
                                                                                   object? value)
        {
            return AddQueryParameter(key, value);
        }

        IHttpRequestConfiguring IHttpRequestConfiguring.WithHeader(string key,
                                                                   string value)
        {
            return AddHeader(key, value);
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.WithHeader(string key,
                                                                           string value)
        {
            return AddHeader(key, value);
        }

        IHttpRequestConfiguring IHttpRequestConfiguring.WithBearerToken(string token)
        {
            return AddBearerToken(token);
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.WithBearerToken(string token)
        {
            return AddBearerToken(token);
        }

        IHttpRequestConfiguring IHttpRequestConfiguring.WithoutAuthorization()
        {
            return AddHeader(AuthorizationHeader, WithoutAuthorizationValue);
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.WithoutAuthorization()
        {
            return AddHeader(AuthorizationHeader, WithoutAuthorizationValue);
        }

        IHttpRequestConfiguring IHttpRequestConfiguring.SkipEndpointValidation()
        {
            return With(copy => copy._skipEndpointValidation = true);
        }

        IHttpBodyRequestConfiguring IHttpBodyRequestConfiguring.SkipEndpointValidation()
        {
            return With(copy => copy._skipEndpointValidation = true);
        }

        // ============================================================
        // Placeholder parameters — naked names, SDK escapes internally (§10.1).
        // ============================================================

        // A placeholder takes ONE value: ParameterReplacer replaces the token with the first entry, the second
        // finds nothing left to replace. A name given twice - in one call or across calls - would silently send
        // the first value, so it fails right here, at the line that repeats it.
        private HttpRequestBuilder AddParameters(IEnumerable<(string Key, object? Value)> parameters)
        {
            var result = _parameters;

            foreach (var (key, value) in parameters)
            {
                var placeholder = PlaceholderName.Wrap(key);
                var existing = result.FindIndex(parameter => string.Equals(parameter.Key, placeholder, StringComparison.Ordinal));

                if (existing >= 0)
                {
                    throw FluentUsageError.DuplicateParameter(EntryPoint(),
                                                              placeholder,
                                                              result[existing].Value,
                                                              value,
                                                              _callingAssembly,
                                                              _callerFilePath,
                                                              _callerMemberName,
                                                              _callerLineNumber);
                }

                result = result.Add((placeholder, value));
            }

            return With(copy => copy._parameters = result);
        }

        // How the test started this chain - "Client.AssertPost(\"api/persons/$Id$\")" - for fix hints.
        private string EntryPoint()
        {
            var verb = _method.Method[..1] + _method.Method[1..].ToLowerInvariant();

            return $"Client.Assert{verb}(\"{_url}\")";
        }

        private HttpRequestBuilder AddParametersFrom(object source)
        {
            ArgumentNullException.ThrowIfNull(source);

            // A Dictionary<string, string> (or any other value type) does not convert to
            // IReadOnlyDictionary<string, object?> and lands here. Reflecting over it would yield $Count$,
            // $Keys$, … and crash on the Item indexer - its ENTRIES are what the caller means.
            if (source is IDictionary dictionary)
            {
                // Through Keys + indexer: enumerating a generic dictionary as IEnumerable yields
                // KeyValuePair<,>, not DictionaryEntry.
                return AddParameters(dictionary.Keys
                                               .Cast<object>()
                                               .Select(key => (Convert.ToString(key, CultureInfo.InvariantCulture)!, dictionary[key])));
            }

            // Values and collections have no properties that name placeholders: a string would crash on its
            // Chars indexer, an int would silently add nothing, a list would add $Capacity$/$Count$.
            if (IsValue(source.GetType()) || source is IEnumerable)
            {
                throw new ArgumentException($"WithParameters(object) maps the public PROPERTIES of an object to placeholders (e.g. new {{ Id = 1, Name = \"Goku\" }}). A {source.GetType().Name} has no properties to map. Use WithParameter(\"Name\", value) for a single value, or WithParameters(dictionary) / WithParameters((\"Name\", value), …) for several.",
                                            nameof(source));
            }

            var properties = source.GetType()
                                   .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                   .Where(property => property.CanRead && property.GetIndexParameters().Length == 0);

            return AddParameters(properties.Select(property => (property.Name, property.GetValue(source))));
        }

        private static bool IsValue(Type type)
        {
            return type.IsPrimitive
                || type.IsEnum
                || type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(DateTimeOffset)
                || type == typeof(DateOnly)
                || type == typeof(TimeOnly)
                || type == typeof(TimeSpan)
                || type == typeof(Guid)
                || type == typeof(Uri);
        }

        // ============================================================
        // Query string — formatted and encoded by the SDK, never by hand in the url.
        // ============================================================

        // null adds nothing - an optional filter needs no if. A key given twice would leave the endpoint to pick
        // one of the two values (ASP.NET Core binds the first to a scalar), so it fails at the repeating line;
        // several values come as ONE collection. Keys compare case-insensitively like the model binding does.
        private HttpRequestBuilder AddQueryParameter(string key,
                                                     object? value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);

            var values = QueryValues(value);

            if (values.Count == 0)
            {
                return this;
            }

            var existing = _query.FindIndex(entry => string.Equals(entry.Key, key, StringComparison.OrdinalIgnoreCase));

            if (existing >= 0)
            {
                throw FluentUsageError.DuplicateQueryParameter(_query[existing].Key,
                                                               _query[existing].Value,
                                                               value,
                                                               _callingAssembly,
                                                               _callerFilePath,
                                                               _callerMemberName,
                                                               _callerLineNumber);
            }

            return With(copy => copy._query = _query.AddRange(values.Select(formatted => (key, formatted))));
        }

        private static List<string> QueryValues(object? value)
        {
            if (value is null)
            {
                return [];
            }

            if (value is not string && value is IEnumerable elements)
            {
                return elements.Cast<object?>()
                               .Where(element => element is not null)
                               .Select(element => FormatQueryValue(element!))
                               .ToList();
            }

            return [FormatQueryValue(value)];
        }

        // Culture-invariant and in the shape ASP.NET Core's model binding parses back - "1,5" or "13.10.2026" from a
        // German machine would bind to something else or fail. An object has no single query value.
        private static string FormatQueryValue(object value)
        {
            if (!IsValue(value.GetType()))
            {
                throw new ArgumentException($"WithQueryParameter sends ONE value per key (string, number, bool, enum, Guid, date, …) or a collection of them. A {value.GetType().Name} has no single query value. Add one WithQueryParameter per property you want to send.",
                                            nameof(value));
            }

            return value switch
            {
                bool flag => flag ? "true" : "false",
                DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
                DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O", CultureInfo.InvariantCulture),
                DateOnly date => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                TimeOnly time => time.ToString("HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture),
                TimeSpan timeSpan => timeSpan.ToString("c", CultureInfo.InvariantCulture),
                Enum => value.ToString()!,
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString() ?? string.Empty
            };
        }

        // The url the request is sent to - and what endpoint validation, api-version resolution and the curl output
        // see. A url that already carries a query ("persons?api-version=1") is continued with '&'.
        private string RequestUrl()
        {
            if (_query.IsEmpty)
            {
                return _url;
            }

            var query = string.Join('&', _query.Select(entry => $"{Uri.EscapeDataString(entry.Key)}={Uri.EscapeDataString(entry.Value)}"));

            var separator = !_url.Contains('?')
                                ? "?"
                                : _url.EndsWith('?') || _url.EndsWith('&')
                                    ? string.Empty
                                    : "&";

            return _url + separator + query;
        }

        // ============================================================
        // Credentials — one more request header, so they share the once-per-chain rule with WithHeader.
        // ============================================================

        private const string AuthorizationHeader = "Authorization";

        // An empty Authorization header on the request keeps HttpClient from merging its default one in (it only adds
        // default headers the request does not carry), and authentication handlers read it as no credentials.
        private const string WithoutAuthorizationValue = "";

        private HttpRequestBuilder AddBearerToken(string token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(token);

            return AddHeader(AuthorizationHeader, $"Bearer {token}");
        }

        // Overwriting the first value - what SetItem used to do - would silently send something else than
        // the earlier line of the test reads. Fails at the line that repeats the header.
        private HttpRequestBuilder AddHeader(string key,
                                             string value)
        {
            if (_headers.TryGetValue(key, out var existing) && string.Equals(key, AuthorizationHeader, StringComparison.OrdinalIgnoreCase))
            {
                throw FluentUsageError.DuplicateAuthorization(existing,
                                                              value,
                                                              _callingAssembly,
                                                              _callerFilePath,
                                                              _callerMemberName,
                                                              _callerLineNumber);
            }

            if (_headers.TryGetValue(key, out existing))
            {
                throw FluentUsageError.DuplicateHeader(key,
                                                       existing,
                                                       value,
                                                       _callingAssembly,
                                                       _callerFilePath,
                                                       _callerMemberName,
                                                       _callerLineNumber);
            }

            return With(copy => copy._headers = _headers.Add(key, value));
        }

        private HttpRequestBuilder WithBody(string body,
                                            string bodyExpression)
        {
            return With(copy =>
                        {
                            copy._body = body;
                            copy._bodyExpression = bodyExpression;
                        });
        }

        // Copy-on-write. A shallow copy is enough: every field is either immutable itself or an immutable
        // collection, so the copy and the original can never observe each other's changes.
        private HttpRequestBuilder With(Action<HttpRequestBuilder> change)
        {
            var copy = (HttpRequestBuilder)MemberwiseClone();
            change(copy);

            return copy;
        }

        // ============================================================
        // Transition to the response stage. Produces<T>(code) carries type + status + return type.
        // Same signature and return type on both request stages → one public implementation.
        // ============================================================

        public IHttpResponseConfiguring<T> Produces<T>(HttpStatusCode statusCode)
        {
            return new HttpResponseBuilder<T>(_client,
                                              _method,
                                              RequestUrl(),
                                              _body,
                                              _bodyExpression,
                                              _parameters,
                                              _headers,
                                              _callingAssembly,
                                              _callerFilePath,
                                              _callerMemberName,
                                              _callerLineNumber,
                                              statusCode,
                                              _skipEndpointValidation,
                                              validateRouteOnly: false);
        }

        public IHttpResponseConfiguring<T> Produces<T>(int statusCode)
        {
            return Produces<T>((HttpStatusCode)statusCode);
        }

        public IHttpExpectationConfiguring Produces(HttpStatusCode statusCode)
        {
            return new HttpExpectationBuilder(_client, _method, RequestUrl(),
                                              _body, _bodyExpression, _parameters, _headers,
                                              _callingAssembly,
                                              _callerFilePath,
                                              _callerMemberName,
                                              _callerLineNumber,
                                              statusCode,
                                              _skipEndpointValidation);
        }

        public IHttpExpectationConfiguring Produces(int statusCode)
        {
            return Produces((HttpStatusCode)statusCode);
        }
    }
}
