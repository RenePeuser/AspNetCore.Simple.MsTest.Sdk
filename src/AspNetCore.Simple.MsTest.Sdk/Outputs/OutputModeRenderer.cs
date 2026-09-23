using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddOutputModeRendererExtension
    {
        public static void AddOutputModeRenderer(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IOutputModeRenderer, OutputModeRenderer>();
        }
    }

    /// <summary>
    ///     Renders assertion failure output based on the configured output mode.
    ///     Uses the Strategy Pattern to delegate to mode-specific render strategies.
    /// </summary>
    public interface IOutputModeRenderer
    {
        /// <summary>
        ///     Renders assertion failure output based on the current output mode.
        /// </summary>
        /// <param name="context">The assertion context</param>
        /// <param name="differences">List of differences found</param>
        /// <param name="expectedJson">Expected result as JSON</param>
        /// <param name="currentJson">Current/actual result as JSON</param>
        /// <returns>Formatted error message (human-readable or JSON based on mode)</returns>
        string Render(IObjectAssertContext context,
                      ImmutableList<Difference> differences,
                      string expectedJson,
                      string currentJson);
    }

    /// <summary>
    ///     Implementation that resolves and delegates to the appropriate mode-specific render strategy.
    ///     Uses the Strategy Pattern - similar to AssertOutputBuilder but for output modes.
    ///     Allows consumers to register custom render strategies for extensibility.
    /// </summary>
    internal sealed class OutputModeRenderer(IEnumerable<IOutputModeRenderStrategy> renderStrategies,
                                             IOutputModeService outputModeService) : IOutputModeRenderer
    {
        public string Render(IObjectAssertContext context,
                             ImmutableList<Difference> differences,
                             string expectedJson,
                             string currentJson)
        {
            var mode = outputModeService.GetOutputMode();

            // Find ALL strategies that can handle this mode
            var matchingStrategies = renderStrategies.Where(s => s.CanHandle(mode)).ToList();

            // Ensure exactly ONE strategy matches (same pattern as AssertOutputBuilder)
            if (matchingStrategies.Count == 0)
            {
                throw new InvalidOperationException($"No render strategy found for output mode '{mode}'. " +
                                                    $"Ensure the appropriate strategy is registered in DI. " +
                                                    $"Available modes: {string.Join(", ", Enum.GetValues<OutputMode>())}");
            }

            if (matchingStrategies.Count > 1)
            {
                var strategyNames = string.Join(", ", matchingStrategies.Select(s => s.GetType().Name));

                throw new InvalidOperationException($"Multiple render strategies found for output mode '{mode}': {strategyNames}. " +
                                                    $"Ensure only ONE strategy can handle each mode. " +
                                                    $"This indicates a registration conflict in DI.");
            }

            // Exactly one strategy found - use it
            return matchingStrategies[0].Render(context, differences, expectedJson,
                                                currentJson);
        }
    }
}