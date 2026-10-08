using System.Text;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    /// <summary>
    /// Orchestrator that coordinates HTTP failure output strategies.
    /// Selects the appropriate strategy based on failure type and delegates header building.
    /// </summary>
    internal interface IHttpFailureOutputBuilder
    {
        /// <summary>
        /// Builds the header section by delegating to the appropriate strategy.
        /// Falls back to default strategy if no specific strategy can handle the failure type.
        /// </summary>
        /// <param name="sb">StringBuilder to append the header to</param>
        /// <param name="context">The HTTP response context containing all failure information</param>
        void BuildHeader(StringBuilder sb,
                         IHttpResponseContext context);
    }
}