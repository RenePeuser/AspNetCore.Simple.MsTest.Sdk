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
    ///     Immutable: every configuring call returns a NEW builder and leaves this one untouched. A base
    ///     request can be stored and branched (<c>baseRequest.WithParameter("Id", 1)</c> /
    ///     <c>baseRequest.WithParameter("Id", 2)</c>) without one branch leaking into the other or into a chain
    ///     already derived through <c>Produces…</c>.
    ///     </para>
    /// </summary>
    internal sealed class HttpRequestBuilder : IHttpRequestConfiguring
    {
        private readonly string _callerFilePath;

        private readonly string _callerMemberName;

        private readonly int _callerLineNumber;

        private readonly Assembly _callingAssembly;

        private readonly HttpClient _client;

        private ImmutableDictionary<string, string> _headers = ImmutableDictionary<string, string>.Empty;

        private readonly HttpMethod _method;

        private ImmutableList<(string Key, object? Value)> _parameters = ImmutableList<(string Key, object? Value)>.Empty;

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
        // Request body — explicit, no rate-heuristic (Schema A).
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
        // Placeholder parameters — naked names, SDK escapes internally (§10.1).
        // ============================================================

        public IHttpRequestConfiguring WithParameter(string key,
                                                     object? value)
        {
            return With(copy => copy._parameters = _parameters.Add((PlaceholderName.Wrap(key), value)));
        }

        public IHttpRequestConfiguring WithParameters(params (string Key, object? Value)[] parameters)
        {
            return With(copy => copy._parameters = _parameters.AddRange(parameters.Select(parameter => (PlaceholderName.Wrap(parameter.Key), parameter.Value))));
        }

        public IHttpRequestConfiguring WithParameters((string Key, object? Value) parameter)
        {
            return WithParameter(parameter.Key, parameter.Value);
        }

        public IHttpRequestConfiguring WithParameters(IReadOnlyDictionary<string, object?> parameters)
        {
            return With(copy => copy._parameters = _parameters.AddRange(parameters.Select(parameter => (PlaceholderName.Wrap(parameter.Key), parameter.Value))));
        }

        public IHttpRequestConfiguring WithParameters(object source)
        {
            ArgumentNullException.ThrowIfNull(source);

            // A Dictionary<string, string> (or any other value type) does not convert to
            // IReadOnlyDictionary<string, object?> and lands here. Reflecting over it would yield $Count$,
            // $Keys$, … and crash on the Item indexer - its ENTRIES are what the caller means.
            if (source is IDictionary dictionary)
            {
                // Through Keys + indexer: enumerating a generic dictionary as IEnumerable yields
                // KeyValuePair<,>, not DictionaryEntry.
                return With(copy => copy._parameters = _parameters.AddRange(dictionary.Keys
                                                                                      .Cast<object>()
                                                                                      .Select(key => (PlaceholderName.Wrap(Convert.ToString(key, CultureInfo.InvariantCulture)!), dictionary[key]))));
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

            return With(copy => copy._parameters = _parameters.AddRange(properties.Select(property => (PlaceholderName.Wrap(property.Name), property.GetValue(source)))));
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

        public IHttpRequestConfiguring WithHeader(string key,
                                                  string value)
        {
            return With(copy => copy._headers = _headers.SetItem(key, value));
        }

        public IHttpRequestConfiguring SkipEndpointValidation()
        {
            return With(copy => copy._skipEndpointValidation = true);
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
        // ============================================================

        public IHttpResponseConfiguring<T> Produces<T>(HttpStatusCode statusCode)
        {
            return new HttpResponseBuilder<T>(_client,
                                              _method,
                                              _url,
                                              _body,
                                              _bodyExpression,
                                              _parameters,
                                              _headers,
                                              _callingAssembly,
                                              _callerFilePath,
                                              _callerMemberName,
                                              _callerLineNumber,
                                              statusCode,
                                              _skipEndpointValidation);
        }

        public IHttpResponseConfiguring<T> Produces<T>(int statusCode)
        {
            return Produces<T>((HttpStatusCode)statusCode);
        }

        public IHttpExpectationConfiguring Produces(HttpStatusCode statusCode)
        {
            return new HttpExpectationBuilder(_client, _method, _url,
                                              _body, _bodyExpression, _parameters, _headers,
                                              _callingAssembly,
                                              _callerFilePath,
                                              _callerMemberName,
                                              _callerLineNumber,
                                              statusCode);
        }

        public IHttpExpectationConfiguring Produces(int statusCode)
        {
            return Produces((HttpStatusCode)statusCode);
        }
    }
}