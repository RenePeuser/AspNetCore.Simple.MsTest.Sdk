using System.Collections.Immutable;
using AspNetCore.Simple.MsTest.Sdk.Strategies;
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
    /// Renders assertion failure output based on the configured output mode.
    /// Wraps the human-readable output builder and delegates to the appropriate renderer.
    /// </summary>
    public interface IOutputModeRenderer
    {
        /// <summary>
        /// Renders assertion failure output based on the current output mode.
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
    /// Implementation that switches between output modes (Human, AI, Hybrid).
    /// Wraps the existing human output builder for backward compatibility.
    /// </summary>
    internal sealed class OutputModeRenderer(IAssertOutputBuilder humanOutputBuilder,
                                            IAiOutputTransformer aiOutputTransformer,
                                            IOutputModeService outputModeService) : IOutputModeRenderer
    {
        public string Render(IObjectAssertContext context,
                            ImmutableList<Difference> differences,
                            string expectedJson,
                            string currentJson)
        {
            var mode = outputModeService.GetOutputMode();

            return mode switch
            {
                OutputMode.Ai => aiOutputTransformer.TransformToJson(context, differences, expectedJson,
                                                                     currentJson),
                OutputMode.Hybrid => BuildHybridOutput(context, differences, expectedJson, currentJson),
                _ => humanOutputBuilder.BuildOutput(context, differences, expectedJson, currentJson)
            };
        }

        private string BuildHybridOutput(IObjectAssertContext context,
                                        ImmutableList<Difference> differences,
                                        string expectedJson,
                                        string currentJson)
        {
            // Build both human and AI outputs
            var humanOutput = humanOutputBuilder.BuildOutput(context, differences, expectedJson,
                                                             currentJson);
            var aiOutput = aiOutputTransformer.TransformToJson(context, differences, expectedJson,
                                                              currentJson);

            // Combine with clear separator
            return $"{humanOutput}\n\n{'='.Repeat(80)}\n===== AI OUTPUT (JSON) =====\n{'='.Repeat(80)}\n\n{aiOutput}";
        }
    }
}
