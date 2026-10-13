using System.Net.Http;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Fluent state after <c>ExpectedHeaders…</c> on <see cref="IHttpOptionsConfiguring"/>.
    ///
    /// <para>
    /// Not awaitable itself → a forgotten terminal is a dangling <see cref="FluentBuilderAttribute"/>
    /// expression caught by MSTESTSDK001. The single terminal is <see cref="ExecuteAsync"/>.
    /// </para>
    /// </summary>
    [FluentBuilder]
#if FLUENT_ALPHA
    public interface IHttpOptionsExpectationConfiguring
#else
    internal interface IHttpOptionsExpectationConfiguring
#endif
    {
        /// <summary>
        /// Executes the OPTIONS request, asserts 204 NoContent without a body and compares the response
        /// headers. Returns the response for further checks. The single terminal.
        /// </summary>
        Task<HttpResponseMessage> ExecuteAsync();
    }
}
