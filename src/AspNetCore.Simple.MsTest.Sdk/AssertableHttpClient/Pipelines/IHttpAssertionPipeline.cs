namespace AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient
{
    /// <summary>
    /// Represents a pipeline of HTTP assertion steps.
    /// Executes steps sequentially until one fails (Assert.Fail) or all succeed.
    /// Replaces the strategy pattern with a simpler, more extensible pipeline approach.
    /// </summary>
    public interface IHttpAssertionPipeline
    {
        /// <summary>
        /// Executes all assertion steps in sequence on the given HTTP response context.
        /// Returns the filtered current result if all assertions pass.
        /// If any step fails, the pipeline stops immediately (Assert.Fail throws).
        /// </summary>
        /// <typeparam name="TResult">The expected result type.</typeparam>
        /// <param name="context">The HTTP response context containing all data needed for assertions.</param>
        /// <returns>The filtered current result from the context.</returns>
        TResult Execute<TResult>(HttpResponseContext<TResult> context);
    }
}
