using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Fluent state after an untyped <c>Produces(code)</c> (Endpoint-Stil, §15.6) — the status is fixed,
    /// but no response type is named.
    ///
    /// <para>
    /// Going straight to <see cref="ExecuteAsync"/> asserts the status only (e.g. 204). Alternatively
    /// <see cref="ExpectedResponse{TExpected}"/> takes the response type FROM the expected object —
    /// typically an anonymous object that lists just the properties worth checking. There is no
    /// file/raw-JSON variant here on purpose: those carry no type, so they need <c>Produces&lt;T&gt;</c>.
    /// </para>
    ///
    /// <para>
    /// Not awaitable itself → a forgotten terminal is a dangling <see cref="FluentBuilderAttribute"/>
    /// expression caught by MSTESTSDK001. The single terminal is <see cref="ExecuteAsync"/>.
    /// </para>
    /// </summary>
    [FluentBuilder]
    public interface IHttpExpectationConfiguring
    {
        /// <summary>
        /// Sets the expected response body from an object whose type becomes the response type —
        /// the fluent counterpart of the classic <c>AssertGetAsErrorAsync(url, new { … })</c>:
        /// <code>
        /// .Produces(HttpStatusCode.NotFound)
        /// .ExpectedResponse(new { Title = "Person not found", Status = 404 })
        /// </code>
        /// The response is deserialized into <typeparamref name="TExpected"/>, so only the properties
        /// of the expected object take part in the comparison. Endpoint validation is skipped, because
        /// <c>Produces(code)</c> named no type the endpoint could be checked against.
        /// </summary>
        /// <typeparam name="TExpected">The expected (often anonymous) type; also the return type of <c>ExecuteAsync()</c>.</typeparam>
        IHttpComparisonConfiguring<TExpected> ExpectedResponse<TExpected>(TExpected expected,
                                                                          [CallerArgumentExpression(nameof(expected))] string expectedExpression = "");

        /// <summary>
        /// Skips the endpoint validation for this call. <c>Produces(code)</c> names no response type, so only the
        /// route (verb, url, api version) is validated - this turns that off too, e.g. for a route that
        /// deliberately does not exist (404 / 405). Mirrors the classic <c>skipEndpointValidation</c>. Idempotent.
        /// </summary>
        IHttpExpectationConfiguring SkipEndpointValidation();

        /// <summary>Executes the request and asserts the status code. The single terminal.</summary>
        /// <param name="cancellationToken">Cancels the HTTP call; a cancellation surfaces as <see cref="System.OperationCanceledException"/>, not as an assertion failure.</param>
        Task ExecuteAsync(CancellationToken cancellationToken = default);
    }
}