using System.Collections.Immutable;
using AspNetCore.Simple.MsTest.Sdk.Strategies;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddHumanModeRenderStrategyExtension
    {
        public static void AddHumanModeRenderStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IOutputModeRenderStrategy, HumanModeRenderStrategy>();
        }
    }

    /// <summary>
    /// Renders output in Human mode - beautiful formatted text with sections, tables, and emojis.
    /// </summary>
    internal sealed class HumanModeRenderStrategy(IAssertOutputBuilder humanOutputBuilder) : IOutputModeRenderStrategy
    {
        public bool CanHandle(OutputMode mode)
        {
            return mode == OutputMode.Human;
        }

        public string Render(IObjectAssertContext context,
                             ImmutableList<Difference> differences,
                             string expectedJson,
                             string currentJson)
        {
            return humanOutputBuilder.BuildOutput(context, differences, expectedJson,
                                                  currentJson);
        }
    }
}