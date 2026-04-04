using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Strategy for handling expected HTTP status codes (both success and error).
    /// Performs full response processing: deserialization, filtering, schema validation, and assertion.
    /// </summary>
    internal sealed class ExpectedStatusCodeStrategy : IHttpStatusCodeProcessingStrategy
    {
        /// <inheritdoc />
        public bool CanHandle(bool isExpectedStatusCode) => isExpectedStatusCode;

        /// <inheritdoc />
        public Task<TResult> ProcessAsync<TResult>(HttpResponseContext<TResult> context)
        {
            // TODO: Implement full processing logic (Lines 133-303 from AssertableHttpClient)
            // - Deserialize response
            // - Apply filter function
            // - Build SimpleHttpResponseMessage
            // - Schema validation
            // - Final assertion
            // - Optional response writing

            throw new System.NotImplementedException("ExpectedStatusCodeStrategy will be implemented in next steps");
        }
    }
}
