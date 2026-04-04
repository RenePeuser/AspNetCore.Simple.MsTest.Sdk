using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpResponseAssertStrategyExtension
    {
        /// <summary>
        /// Registers the HTTP response assertion strategy and its dependencies.
        /// Feature-based registration following the dependency tree pattern.
        /// </summary>
        public static void AddHttpResponseAssertStrategy(this IServiceCollection services)
        {
            // 1. Register all specific status code strategies
            services.AddUnexpectedStatusCodeStrategy();
            services.AddExpectedStatusCodeStrategy();

            // 2. Register the main strategy coordinator
            services.AddSingletonIfNotExists<IHttpResponseAssertStrategy, HttpResponseAssertStrategy>();
        }
    }

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
            // Select the appropriate strategy based on the context
            // Each strategy decides if it can handle this scenario
            var strategy = strategies.FirstOrDefault(s => s.CanHandle(context));

            if (strategy.IsNull())
            {
                throw new InvalidOperationException($"No strategy found for IsExpectedStatusCode={context.IsExpectedStatusCode}. " +
                                                    "Ensure both UnexpectedStatusCodeStrategy and ExpectedStatusCodeStrategy are registered.");
            }

            // Delegate to the selected strategy
            return strategy.ProcessAsync(context);
        }
    }
}
