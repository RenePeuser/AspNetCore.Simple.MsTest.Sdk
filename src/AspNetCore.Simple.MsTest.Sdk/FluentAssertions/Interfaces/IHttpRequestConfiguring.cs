using System;
using System.Net;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Initial fluent state after <c>Client.AssertPost/AssertGet/…(url)</c> (Endpoint-Stil, §15).
    /// Configures the request (body, parameters, headers) and transitions to the response stage
    /// via <see cref="Produces{T}(HttpStatusCode)"/>.
    ///
    /// <para>
    /// Body input is EXPLICIT — no rate-heuristic (Schema A, §11). Pick the method that matches intent:
    /// <list type="bullet">
    /// <item><see cref="Accepts{T}"/> — a C# object (serialized to JSON).</item>
    /// <item><see cref="AcceptsFromJsonString"/> — a raw JSON string, verbatim.</item>
    /// <item><see cref="AcceptsFromEmbeddedJson"/> — an embedded-resource file name.</item>
    /// </list>
    /// </para>
    /// </summary>
    [FluentBuilder]
#if FLUENT_ALPHA
    public interface IHttpRequestConfiguring
#else
    internal interface IHttpRequestConfiguring
#endif
    {
        // ============================================================
        // Request body — explicit, no rate-heuristic (Schema A).
        // ============================================================

        /// <summary>Sets the request body from a C# object (serialized to JSON).</summary>
        IHttpRequestConfiguring Accepts<T>(T body);

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
        IHttpRequestConfiguring AcceptsFromJsonString(string bodyJson);

        /// <summary>Sets the request body from an embedded-resource JSON file name.</summary>
        IHttpRequestConfiguring AcceptsFromEmbeddedJson(string embeddedFileName);

        // ============================================================
        // Placeholder parameters ($Token$ substitution). Naked names — SDK escapes internally (§10.1).
        // ============================================================

        /// <summary>Sets one placeholder parameter. Use the naked name ("Id"); the SDK adds the delimiters.</summary>
        IHttpRequestConfiguring WithParameter(string key,
                                              object? value);

        /// <summary>Sets placeholder parameters as key/value tuples.</summary>
        IHttpRequestConfiguring WithParameters(params (string Key, object? Value)[] parameters);

        /// <summary>
        /// Sets a single placeholder parameter given as a tuple.
        ///
        /// <para>
        /// Exists so that <c>WithParameters(("Name", "Goku"))</c> cannot bind to
        /// <see cref="WithParameters(object)"/>: with only the params-array and the object overload
        /// present, C# prefers the normal form over the expanded one, so a single tuple would be
        /// reflected over as an object and produce the placeholders <c>$Item1$</c>/<c>$Item2$</c>
        /// instead of <c>$Name$</c> — silently wrong.
        /// </para>
        /// </summary>
        IHttpRequestConfiguring WithParameters((string Key, object? Value) parameter);

        /// <summary>Sets placeholder parameters from all public properties of an object (§10.1.1, PascalCase).</summary>
        IHttpRequestConfiguring WithParameters(object source);

        /// <summary>Adds a custom HTTP request header.</summary>
        IHttpRequestConfiguring WithHeader(string key,
                                           string value);

        // ============================================================
        // Transition to the response stage. Produces<T>(code) carries TYPE + STATUS + RETURN TYPE (§15.6).
        // ============================================================

        /// <summary>Expects this status code with a body of type <typeparamref name="T"/>.</summary>
        IHttpResponseConfiguring<T> Produces<T>(HttpStatusCode statusCode);

        /// <summary>Expects this status code (int, e.g. <c>StatusCodes.Status201Created</c>) with a body of type <typeparamref name="T"/>.</summary>
        IHttpResponseConfiguring<T> Produces<T>(int statusCode);

        /// <summary>Expects this status code with no response body (e.g. 204). No <c>ExpectedResponse…</c> reachable (type-state).</summary>
        IHttpExpectationConfiguring Produces(HttpStatusCode statusCode);

        /// <summary>Expects this status code (int) with no response body.</summary>
        IHttpExpectationConfiguring Produces(int statusCode);
    }
}