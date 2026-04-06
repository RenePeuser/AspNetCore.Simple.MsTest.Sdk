namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    /// <summary>
    /// Represents a single step in the HTTP assertion pipeline.
    /// Each step performs a specific validation (e.g., status code, schema, values).
    /// Steps are executed sequentially and can fail fast by throwing an assertion exception.
    /// </summary>
    public interface IHttpAssertionStep
    {
        /// <summary>
        /// Executes this assertion step on the given HTTP response context.
        /// If the assertion fails, this method should call Assert.That.Fail() to stop the pipeline.
        /// If the assertion succeeds, the method returns normally and the pipeline continues.
        /// Steps are validators only - they never modify the result.
        /// </summary>
        /// <typeparam name="TResult">The expected result type.</typeparam>
        /// <param name="context">The HTTP response context containing all data needed for assertion.</param>
        void Execute<TResult>(HttpResponseContext<TResult> context);
    }
}
