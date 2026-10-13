using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Initial fluent state after <c>Client.AssertPost/AssertPut/AssertPatch/AssertQuery(url)</c> — a verb that
    /// carries a request body, whose body is not set yet (Endpoint-Stil, §15). Configures the body, parameters
    /// and headers and transitions to the response stage via <see cref="Produces{T}(HttpStatusCode)"/>.
    ///
    /// <para>
    /// Body input is EXPLICIT — no rate-heuristic (Schema A, §11). Pick the method that matches intent:
    /// <list type="bullet">
    /// <item><see cref="Accepts{T}"/> — a C# object (serialized to JSON).</item>
    /// <item><see cref="AcceptsFromJsonString"/> — a raw JSON string, verbatim.</item>
    /// <item><see cref="AcceptsFromEmbeddedJson"/> — an embedded-resource file name.</item>
    /// </list>
    /// </para>
    ///
    /// <para>
    /// <b>Type-state:</b> every <c>Accepts…</c> leads to <see cref="IHttpRequestConfiguring"/>, which has no
    /// <c>Accepts…</c> anymore - the body is set at most once per chain, a second one does not compile. The
    /// order stays free: parameters and headers can come before or after the body.
    /// </para>
    /// </summary>
    [FluentBuilder]
#if FLUENT_ALPHA
    public interface IHttpBodyRequestConfiguring
#else
    internal interface IHttpBodyRequestConfiguring
#endif
    {
        // ============================================================
        // Request body — explicit, no rate-heuristic (Schema A). Once per chain (type-state).
        // ============================================================

        /// <summary>Sets the request body from a C# object (serialized to JSON).</summary>
        IHttpRequestConfiguring Accepts<T>(T body,
                                           [CallerArgumentExpression(nameof(body))] string bodyExpression = "");

        /// <summary>
        /// NOT a request body. Reserved to close the Minimal-API false friend: there,
        /// <c>Accepts&lt;T&gt;("application/json")</c> declares a CONTENT TYPE, while here the argument
        /// IS the body — so the reflex spelling would silently POST the string "application/json".
        /// Use <see cref="AcceptsFromJsonString"/> for raw JSON or <see cref="AcceptsFromEmbeddedJson"/>
        /// for a file name.
        /// </summary>
        [Obsolete("Accepts(string) is not a body. In the Minimal API the string argument is the CONTENT TYPE, here it would be sent AS the body. Use AcceptsFromJsonString(json) for raw JSON, AcceptsFromEmbeddedJson(fileName) for an embedded file, or Accepts<T>(obj) for an object.", error: true)]
        IHttpRequestConfiguring Accepts(string bodyJson);

        /// <summary>Sets the request body from a raw JSON string (used verbatim).</summary>
        IHttpRequestConfiguring AcceptsFromJsonString(string bodyJson,
                                                      [CallerArgumentExpression(nameof(bodyJson))] string bodyJsonExpression = "");

        /// <summary>Sets the request body from an embedded-resource JSON file name.</summary>
        IHttpRequestConfiguring AcceptsFromEmbeddedJson(string embeddedFileName,
                                                        [CallerArgumentExpression(nameof(embeddedFileName))] string embeddedFileNameExpression = "");

        // ============================================================
        // Parameters / headers / validation — same as IHttpRequestConfiguring, but the body stays open.
        // ============================================================

        /// <inheritdoc cref="IHttpRequestConfiguring.WithParameter"/>
        IHttpBodyRequestConfiguring WithParameter(string key,
                                                  object? value);

        /// <inheritdoc cref="IHttpRequestConfiguring.WithParameters(ValueTuple{string, object}[])"/>
        IHttpBodyRequestConfiguring WithParameters(params (string Key, object? Value)[] parameters);

        /// <inheritdoc cref="IHttpRequestConfiguring.WithParameters(ValueTuple{string, object})"/>
        IHttpBodyRequestConfiguring WithParameters((string Key, object? Value) parameter);

        /// <inheritdoc cref="IHttpRequestConfiguring.WithParameters(IReadOnlyDictionary{string, object})"/>
        IHttpBodyRequestConfiguring WithParameters(IReadOnlyDictionary<string, object?> parameters);

        /// <inheritdoc cref="IHttpRequestConfiguring.WithParameters(object)"/>
        IHttpBodyRequestConfiguring WithParameters(object source);

        /// <inheritdoc cref="IHttpRequestConfiguring.WithQueryParameter"/>
        IHttpBodyRequestConfiguring WithQueryParameter(string key,
                                                       object? value);

        /// <inheritdoc cref="IHttpRequestConfiguring.WithHeader"/>
        IHttpBodyRequestConfiguring WithHeader(string key,
                                               string value);

        /// <inheritdoc cref="IHttpRequestConfiguring.WithBearerToken"/>
        IHttpBodyRequestConfiguring WithBearerToken(string token);

        /// <inheritdoc cref="IHttpRequestConfiguring.WithoutAuthorization"/>
        IHttpBodyRequestConfiguring WithoutAuthorization();

        /// <inheritdoc cref="IHttpRequestConfiguring.SkipEndpointValidation"/>
        IHttpBodyRequestConfiguring SkipEndpointValidation();

        // ============================================================
        // Transition to the response stage WITHOUT a body (e.g. POST api/persons/1/activate).
        // ============================================================

        /// <inheritdoc cref="IHttpRequestConfiguring.Produces{T}(HttpStatusCode)"/>
        IHttpResponseConfiguring<T> Produces<T>(HttpStatusCode statusCode);

        /// <inheritdoc cref="IHttpRequestConfiguring.Produces{T}(int)"/>
        IHttpResponseConfiguring<T> Produces<T>(int statusCode);

        /// <inheritdoc cref="IHttpRequestConfiguring.Produces(HttpStatusCode)"/>
        IHttpExpectationConfiguring Produces(HttpStatusCode statusCode);

        /// <inheritdoc cref="IHttpRequestConfiguring.Produces(int)"/>
        IHttpExpectationConfiguring Produces(int statusCode);
    }
}
