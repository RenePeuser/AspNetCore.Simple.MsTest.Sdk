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

        // ============================================================
        // Query string — built from parts, encoded by the SDK.
        // ============================================================

        /// <summary>
        /// Appends <c>key=value</c> to the url's query string, both URL-encoded (<c>"Son Goku &amp; Co"</c> is sent as
        /// <c>Son%20Goku%20%26%20Co</c>). Appends with <c>&amp;</c> when the url already has a query.
        ///
        /// <para>
        /// <c>null</c> adds nothing, so optional filters need no <c>if</c>. A collection repeats the key per element
        /// (<c>ids=1&amp;ids=2</c>). Values are formatted culture-invariant: <c>bool</c> as <c>true</c>/<c>false</c>,
        /// <c>DateTime</c>/<c>DateTimeOffset</c> round-trip (ISO 8601), <c>DateOnly</c> as <c>yyyy-MM-dd</c>, enums by name.
        /// The value is sent as given - no <c>$Token$</c> substitution.
        /// </para>
        ///
        /// <para>
        /// Each key (case-insensitive, like ASP.NET Core's model binding) can be set once per chain - a second value
        /// throws <see cref="System.ArgumentException"/>; pass a collection for several values.
        /// </para>
        /// </summary>
        IHttpRequestConfiguring WithQueryParameter(string key,
                                                   object? value);

        /// <summary>
        /// Adds a custom HTTP request header. Each header name (case-insensitive) can be set once per chain -
        /// a second value throws <see cref="System.ArgumentException"/>; send several values comma-separated.
        /// </summary>
        IHttpRequestConfiguring WithHeader(string key,
                                           string value);

        // ============================================================
        // Credentials — per request; the client's DefaultRequestHeaders are never touched.
        // ============================================================

        /// <summary>
        /// Sends <c>Authorization: Bearer {token}</c> with THIS request only. The header sits on the request message,
        /// so it wins over the client's default credentials for this call and leaves
        /// <see cref="System.Net.Http.HttpClient.DefaultRequestHeaders"/> untouched - a client shared by the whole test
        /// session (and by tests running in parallel) keeps its own token, also when the call throws.
        ///
        /// <para>
        /// Credentials are set once per chain: a second <c>WithBearerToken</c>, <c>WithoutAuthorization()</c> or a
        /// <c>WithHeader("Authorization", …)</c> in the same chain throws <see cref="System.ArgumentException"/>.
        /// </para>
        /// </summary>
        IHttpRequestConfiguring WithBearerToken(string token);

        /// <summary>
        /// Sends THIS request without the client's default credentials - for a 401 test on a client that is signed in
        /// for the rest of the session. The request message carries an empty <c>Authorization</c> header, which keeps
        /// <c>HttpClient</c> from adding its default one; the endpoint receives no <c>Authorization</c> header (an
        /// authentication handler that did see an empty one reads it as no credentials, too).
        /// <see cref="System.Net.Http.HttpClient.DefaultRequestHeaders"/> stay untouched.
        ///
        /// <para>Credentials are set once per chain - see <see cref="WithBearerToken"/>.</para>
        /// </summary>
        IHttpRequestConfiguring WithoutAuthorization();

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
