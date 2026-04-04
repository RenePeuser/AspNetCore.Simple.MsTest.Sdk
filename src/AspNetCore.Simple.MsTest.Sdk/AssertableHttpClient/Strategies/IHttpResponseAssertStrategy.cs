using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Main strategy interface for HTTP response assertion processing.
    /// Coordinates the assertion flow by selecting and delegating to specific status code strategies.
    /// </summary>
    public interface IHttpResponseAssertStrategy
    {
        /// <summary>
        /// Performs HTTP response assertion based on the response context.
        /// Selects the appropriate processing strategy based on status code expectations.
        /// </summary>
        /// <typeparam name="TResult">The expected result type</typeparam>
        /// <param name="context">The HTTP response context containing all preprocessed data</param>
        /// <returns>The deserialized and validated result</returns>
        Task<TResult> AssertAsync<TResult>(HttpResponseContext<TResult> context);
    }
}
