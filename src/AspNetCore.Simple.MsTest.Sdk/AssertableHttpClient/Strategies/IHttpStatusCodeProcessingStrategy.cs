using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Strategy interface for processing HTTP responses based on status code expectations.
    /// Implementations handle either expected or unexpected status codes with different processing logic.
    /// </summary>
    public interface IHttpStatusCodeProcessingStrategy
    {
        /// <summary>
        /// Determines if this strategy can handle the given status code scenario.
        /// </summary>
        /// <param name="isExpectedStatusCode">True if the status code matches expectations</param>
        /// <returns>True if this strategy should handle this scenario</returns>
        bool CanHandle(bool isExpectedStatusCode);

        /// <summary>
        /// Processes the HTTP response and performs assertions.
        /// </summary>
        /// <typeparam name="TResult">The expected result type</typeparam>
        /// <param name="context">The HTTP response context containing all preprocessed data</param>
        /// <returns>The deserialized and validated result</returns>
        Task<TResult> ProcessAsync<TResult>(HttpResponseContext<TResult> context);
    }
}
