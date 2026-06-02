using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Strategies
{
    public static class AddAssertOutputBuilderExtension
    {
        public static void AddAssertOutputBuilder(this IServiceCollection services)
        {
            // No dependencies needed - strategies are injected directly
            services.AddSingletonIfNotExists<IAssertOutputBuilder, AssertOutputBuilder>();
        }
    }

    /// <summary>
    /// Builds assertion failure output by resolving and delegating to the appropriate strategy.
    /// Ensures exactly one strategy matches - throws if zero or multiple strategies are found.
    /// Encapsulates both strategy resolution and output building.
    /// </summary>
    public interface IAssertOutputBuilder
    {
        /// <summary>
        /// Builds formatted assertion failure output using the appropriate strategy.
        /// </summary>
        /// <param name="context">The assertion context to build output for</param>
        /// <param name="differences">List of differences found</param>
        /// <param name="expectedJson">Expected JSON string</param>
        /// <param name="currentJson">Current/actual JSON string</param>
        /// <returns>Formatted error message for Assert.That.Fail()</returns>
        /// <exception cref="InvalidOperationException">Thrown when zero or multiple strategies match</exception>
        string BuildOutput(IObjectAssertContext context,
                           ImmutableList<Difference> differences,
                           string expectedJson,
                           string currentJson);
    }

    /// <summary>
    /// Implementation of assertion output building with strategy pattern.
    /// Resolves the appropriate strategy based on context type and delegates output building.
    /// Follows strict single-match validation with explicit failure on ambiguous matches.
    /// </summary>
    internal sealed class AssertOutputBuilder(IEnumerable<IAssertOutputStrategy> outputStrategies) : IAssertOutputBuilder
    {
        public string BuildOutput(IObjectAssertContext context,
                                  ImmutableList<Difference> differences,
                                  string expectedJson,
                                  string currentJson)
        {
            var matchingStrategies = outputStrategies.Where(s => s.CanHandle(context)).ToList();

            if (matchingStrategies.Count == 0)
            {
                throw new InvalidOperationException($"No output strategy found for context type: {context.GetType().Name}");
            }

            if (matchingStrategies.Count > 1)
            {
                var strategyNames = string.Join(", ", matchingStrategies.Select(s => s.GetType().Name));

                throw new InvalidOperationException($"Multiple output strategies found for context type {context.GetType().Name}: {strategyNames}");
            }

            var result = matchingStrategies[0].BuildOutput(context, differences, expectedJson,
                                                           currentJson);

            return result;
        }
    }
}