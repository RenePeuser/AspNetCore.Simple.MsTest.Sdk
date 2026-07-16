using System.Net;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Fluent state for status-only expectations (no response body comparison).
    /// Reached via <c>ExpectingResponse()</c> on the request builder.
    ///
    /// <para>
    /// MODEL B: <c>Expecting…</c> methods are composable config (return the builder); the single
    /// terminal is <see cref="ExecuteAsync"/>. Not awaitable itself → a forgotten terminal is a
    /// dangling <see cref="FluentBuilderAttribute"/> expression caught by MSTESTSDK001.
    /// </para>
    /// </summary>
    [FluentBuilder]
    public interface IHttpExpectationConfiguring
    {
        /// <summary>Expects any 2xx success status.</summary>
        IHttpExpectationConfiguring ExpectingSuccess();

        /// <summary>Expects exactly this status code.</summary>
        IHttpExpectationConfiguring ExpectingStatus(HttpStatusCode code);

        /// <summary>Expects one of the given status codes.</summary>
        IHttpExpectationConfiguring ExpectingOneOf(params HttpStatusCode[] codes);

        /// <summary>Expects HTTP 204 No Content.</summary>
        IHttpExpectationConfiguring ExpectingNoContent();

        /// <summary>Expects an error status code (4xx/5xx).</summary>
        IHttpExpectationConfiguring ExpectingError(HttpStatusCode code);

        /// <summary>Executes the request and runs all configured expectations. The single terminal.</summary>
        Task ExecuteAsync();
    }
}
