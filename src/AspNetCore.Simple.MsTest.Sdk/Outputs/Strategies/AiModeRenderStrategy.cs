using System.Collections.Immutable;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddAiModeRenderStrategyExtension
    {
        public static void AddAiModeRenderStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IOutputModeRenderStrategy, AiModeRenderStrategy>();
        }
    }

    /// <summary>
    /// Renders output in AI mode - structured JSON optimized for AI agents.
    /// </summary>
    internal sealed class AiModeRenderStrategy(IAiOutputTransformer aiOutputTransformer) : IOutputModeRenderStrategy
    {
        public bool CanHandle(OutputMode mode)
        {
            return mode == OutputMode.Ai;
        }

        public string Render(IObjectAssertContext context,
                             ImmutableList<Difference> differences,
                             string expectedJson,
                             string currentJson)
        {
            return aiOutputTransformer.TransformToJson(context, differences, expectedJson,
                                                       currentJson);
        }
    }
}