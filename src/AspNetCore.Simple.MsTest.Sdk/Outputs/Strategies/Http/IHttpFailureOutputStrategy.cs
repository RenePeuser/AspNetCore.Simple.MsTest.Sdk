using System.Text;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    /// <summary>
    /// Strategy interface for building HTTP failure headers based on specific failure types.
    /// Each implementation handles one specific HttpAssertionFailureType and builds the header section
    /// (icon, title, test information, and failure-specific details).
    /// </summary>
    public interface IHttpFailureOutputStrategy
    {
        /// <summary>
        /// Determines if this strategy can handle the given failure type.
        /// </summary>
        /// <param name="failureType">The HTTP assertion failure type to check</param>
        /// <returns>True if this strategy handles the failure type, false otherwise</returns>
        bool CanHandle(HttpAssertionFailureType failureType);

        /// <summary>
        /// Builds the header section for this specific failure type.
        /// This includes the error icon/title banner, test information, and failure-specific details.
        /// </summary>
        /// <param name="sb">StringBuilder to append the header to</param>
        /// <param name="context">The HTTP response context containing all failure information</param>
        void BuildHeader(StringBuilder sb,
                         IHttpResponseContext context);
    }
}