using System;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Main strategy coordinator for HTTP response assertions.
    /// Selects and delegates to the appropriate status code processing strategy.
    /// Follows the Strategy Pattern: one strategy is chosen based on runtime conditions.
    /// </summary>
    internal sealed class HttpResponseAssertStrategy(IEnumerable<IHttpStatusCodeProcessingStrategy> strategies) : IHttpResponseAssertStrategy
    {
        /// <inheritdoc />
        public Task<TResult> AssertAsync<TResult>(HttpResponseContext<TResult> context)
        {
            // Select the appropriate strategy based on status code expectations
            var strategy = strategies.FirstOrDefault(s => s.CanHandle(context.IsExpectedStatusCode));

            if (strategy == null)
            {
                throw new InvalidOperationException($"No strategy found for IsExpectedStatusCode={context.IsExpectedStatusCode}. " +
                                                    "Ensure both UnexpectedStatusCodeStrategy and ExpectedStatusCodeStrategy are registered.");
            }

            // Delegate to the selected strategy
            var result =  strategy.ProcessAsync(context);

            return result;
        }
    }
}
