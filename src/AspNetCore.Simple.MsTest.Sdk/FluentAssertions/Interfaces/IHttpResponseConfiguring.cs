using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Fluent state after <c>Produces&lt;T&gt;(code)</c> (Endpoint-Stil, §15.6). The status + return type
    /// are already fixed by <c>Produces</c>. Here you optionally attach an expected body to compare
    /// against — via <c>ExpectedResponse…</c> (Schema A: object / raw JSON / embedded file).
    ///
    /// <para>
    /// <b>Type-state:</b> comparison config (<c>IgnoreProperty</c>, …) lives on
    /// <see cref="IHttpComparisonConfiguring{TResult}"/>, reachable ONLY after an <c>ExpectedResponse…</c>.
    /// Without an expected body there is nothing to configure — so you cannot even write it (§4).
    /// </para>
    ///
    /// <para>
    /// Omitting <c>ExpectedResponse…</c> and going straight to <see cref="ExecuteAsync"/> is the
    /// body-less path: the real response is still deserialized and returned as <typeparamref name="TResult"/>,
    /// but no body comparison happens.
    /// </para>
    /// </summary>
    /// <typeparam name="TResult">The response type (also the return type of <see cref="ExecuteAsync"/>).</typeparam>
    [FluentBuilder]
    public interface IHttpResponseConfiguring<TResult>
    {
        /// <summary>Sets the expected response body from a C# object (serialized to JSON).</summary>
        IHttpComparisonConfiguring<TResult> ExpectedResponse(TResult expected,
                                                             [CallerArgumentExpression(nameof(expected))] string expectedExpression = "");

        /// <summary>Sets the expected response body from a raw JSON string (used verbatim).</summary>
        IHttpComparisonConfiguring<TResult> ExpectedResponseFromJsonString(string expectedJson,
                                                                           [CallerArgumentExpression(nameof(expectedJson))] string expectedJsonExpression = "");

        /// <summary>Sets the expected response body from an embedded-resource JSON file name.</summary>
        IHttpComparisonConfiguring<TResult> ExpectedResponseFromEmbeddedJson(string embeddedFileName,
                                                                             [CallerArgumentExpression(nameof(embeddedFileName))] string embeddedFileNameExpression = "");

        /// <summary>
        /// Skips the endpoint validation for this call - both the route check (does an endpoint for this verb,
        /// url and version exist?) and the check of the declared return type against <c>Produces&lt;T&gt;</c>.
        /// For endpoints the host cannot describe (external APIs, errors raised by middleware, a route that
        /// deliberately does not exist). Mirrors the classic <c>skipEndpointValidation</c>. Idempotent.
        ///
        /// <para>
        /// Only offered after <c>Produces…</c>: it switches off a check of the whole contract - request side AND
        /// response type - so it sits behind both. Also reachable after an <c>ExpectedResponse…</c>.
        /// </para>
        /// </summary>
        IHttpResponseConfiguring<TResult> SkipEndpointValidation();

        /// <summary>
        /// Executes the request WITHOUT body comparison — asserts only the status code, deserializes
        /// and returns the real response. The single terminal.
        /// </summary>
        /// <param name="cancellationToken">Cancels the HTTP call; a cancellation surfaces as <see cref="System.OperationCanceledException"/>, not as an assertion failure.</param>
        Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}