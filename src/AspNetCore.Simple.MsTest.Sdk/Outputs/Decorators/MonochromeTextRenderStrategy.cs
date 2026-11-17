using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddMonochromeTextRenderStrategyExtension
    {
        public static void AddMonochromeTextRenderStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITextRenderStrategy, MonochromeTextRenderStrategy>();
        }
    }

    /// <summary>
    /// Emojis without colour - <see cref="OutputRendering.Auto"/> for a DEBUG consumer or under a
    /// debugger. The Visual Studio Test Explorer shows ANSI codes as raw characters, but renders emojis.
    /// </summary>
    internal sealed class MonochromeTextRenderStrategy : TextRenderStrategy
    {
        public override bool CanHandle(TextStyle style)
        {
            return style == TextStyle.Monochrome;
        }

        public override string Error(string text)
        {
            return text;
        }

        public override string SectionTitle(string text)
        {
            return text;
        }

        public override string Highlight(string text)
        {
            return text;
        }

        public override string Dim(string text)
        {
            return text;
        }

        public override string Success(string text)
        {
            return text;
        }

        public override string Symbol(OutputSymbol symbol)
        {
            return OutputSymbols.Emoji(symbol);
        }

        protected override string LeadSymbol(OutputSymbol symbol)
        {
            return OutputSymbols.Emoji(symbol);
        }
    }
}
