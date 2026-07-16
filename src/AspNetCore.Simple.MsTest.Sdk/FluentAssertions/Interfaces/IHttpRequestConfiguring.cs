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
    public interface IHttpRequestConfiguring
    {
        // ============================================================
        // Request body — explicit, no rate-heuristic (Schema A).
        // ============================================================

        /// <summary>Sets the request body from a C# object (serialized to JSON).</summary>
        IHttpRequestConfiguring Accepts<T>(T body);

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