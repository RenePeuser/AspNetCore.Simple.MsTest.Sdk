using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Interfaces
{
    /// <summary>
    /// Fluent state after a body-less <c>Produces(code)</c> (Endpoint-Stil, §15.6) — status-only, no
    /// response body. There is deliberately no <c>ExpectedResponse…</c> here (type-state): you cannot
    /// compare a body that the endpoint does not return.
    ///
    /// <para>
    /// Not awaitable itself → a forgotten terminal is a dangling <see cref="FluentBuilderAttribute"/>
    /// expression caught by MSTESTSDK001. The single terminal is <see cref="ExecuteAsync"/>.
    /// </para>
    /// </summary>
    [FluentBuilder]
#if FLUENT_ALPHA
    public interface IHttpExpectationConfiguring
#else
    internal interface IHttpExpectationConfiguring
#endif
    {
        /// <summary>Executes the request and asserts the status code. The single terminal.</summary>
        Task ExecuteAsync();
    }
}