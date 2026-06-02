using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Comparison
{
    public static class AddComparisonStrategyExtension
    {
        public static void AddComparisonStrategy(this IServiceCollection services)
        {
            // Register specific strategies (order matters - first match wins)
            services.AddStringComparisonStrategy();
            services.AddJsonComparisonStrategy();

            // Register orchestrator
            services.AddSingletonIfNotExists<IComparisonStrategy, ComparisonStrategy>();
        }
    }

    /// <summary>
    /// Orchestrator that delegates comparison to specific strategies.
    /// Uses first-match pattern: iterates through registered strategies until one can handle the comparison.
    /// Extensible: users can register custom ISpecificComparisonStrategy implementations in DI.
    /// </summary>
    internal sealed class ComparisonStrategy(IEnumerable<ISpecificComparisonStrategy> strategies) : IComparisonStrategy
    {
        public ComparisonResult Compare<T>(ObjectAssertContext<T> context)
        {
            // Find first strategy that can handle this comparison
            foreach (var strategy in strategies)
            {
                if (strategy.CanCompare(context))
                {
                    return strategy.Compare(context);
                }
            }

            // No strategy found - this should not happen if we have a fallback strategy
            // Fail with detailed error
            var error = $"No comparison strategy found for type '{typeof(T).Name}'. " +
                        $"Registered strategies: {strategies.Count()}. " +
                        $"Make sure a fallback strategy (like JsonComparisonStrategy) is registered.";

            throw new InvalidOperationException(error);
        }
    }
}