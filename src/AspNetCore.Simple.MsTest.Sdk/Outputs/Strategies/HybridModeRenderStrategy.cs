using System.Collections.Immutable;
using AspNetCore.Simple.MsTest.Sdk.Strategies;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHybridModeRenderStrategyExtension
    {
        public static void AddHybridModeRenderStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IOutputModeRenderStrategy, HybridModeRenderStrategy>();
        }
    }

    /// <summary>
    /// Renders output in Hybrid mode - both Human and AI output combined.
    /// </summary>
    internal sealed class HybridModeRenderStrategy(IAssertOutputBuilder humanOutputBuilder,
                                                   IAiOutputTransformer aiOutputTransformer) : IOutputModeRenderStrategy
    {
        public bool CanHandle(OutputMode mode)
        {
            return mode == OutputMode.Hybrid;
        }

        public string Render(IObjectAssertContext context,
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