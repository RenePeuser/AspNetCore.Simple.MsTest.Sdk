using System.Collections.Generic;
using System.Net;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Request stage WITHOUT a body to set (Endpoint-Stil, §15): the initial state of
    /// <c>Client.AssertGet/AssertDelete(url)</c>, and the state of <c>AssertPost/Put/Patch/Query(url)</c> once
    /// <c>Accepts…</c> has set the body. Configures parameters and headers and transitions to the response
    /// stage via <see cref="Produces{T}(HttpStatusCode)"/>.
    ///
    /// <para>
    /// <b>Type-state:</b> there is no <c>Accepts…</c> here. GET and DELETE carry no request body, so they never
    /// offer one, and a body verb cannot set its body a second time - <c>.Accepts(a).Accepts(b)</c> does not
    /// compile instead of silently sending <c>b</c>. The body verbs start on
    /// <see cref="IHttpBodyRequestConfiguring"/>.
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
        // Placeholder parameters ($Token$ substitution). Naked names — SDK escapes internally (§10.1).
        // ============================================================

        /// <summary>
        /// Sets one placeholder parameter. Use the naked name ("Id"); the SDK adds the delimiters.
        /// Each name can be set once per chain - a second value for the same name throws <see cref="System.ArgumentException"/>.
        /// </summary>
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

        /// <summary>Sets placeholder parameters from the entries of a dictionary — key = naked placeholder name.</summary>
        IHttpRequestConfiguring WithParameters(IReadOnlyDictionary<string, object?> parameters);

        /// <summary>
        /// Sets placeholder parameters from all public, non-indexed instance properties of an object
        /// (§10.1.1, PascalCase), e.g. <c>new { Id = 1, Name = "Goku" }</c>. Any other dictionary
        /// (<c>Dictionary&lt;string, string&gt;</c>, …) contributes its entries. A value (<c>string</c>, number,
        /// enum, <c>Guid</c>, …) or a collection has no properties to map and throws <see cref="System.ArgumentException"/>.
        /// </summary>
        IHttpRequestConfiguring WithParameters(object source);

        /// <summary>
        /// Adds a custom HTTP request header. Each header name (case-insensitive) can be set once per chain -
        /// a second value throws <see cref="System.ArgumentException"/>; send several values comma-separated.
        /// </summary>
        IHttpRequestConfiguring WithHeader(string key,
                                           string value);

        /// <summary>
        /// Skips the endpoint validation for this call - both the route check (does an endpoint for this verb,
        /// url and version exist?) and the check of the declared return type against <c>Produces&lt;T&gt;</c>.
        /// For endpoints the host cannot describe (external APIs, errors raised by middleware). Mirrors the
        /// classic <c>skipEndpointValidation</c>. Idempotent.
        /// </summary>
        IHttpRequestConfiguring SkipEndpointValidation();

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
