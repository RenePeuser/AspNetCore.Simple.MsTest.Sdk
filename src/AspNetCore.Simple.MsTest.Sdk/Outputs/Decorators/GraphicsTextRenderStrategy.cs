using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddGraphicsTextRenderStrategyExtension
    {
        public static void AddGraphicsTextRenderStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITextRenderStrategy, GraphicsTextRenderStrategy>();
        }
    }

    /// <summary>
    /// ANSI colours and emojis - <see cref="OutputRendering.Graphics"/>, and
    /// <see cref="OutputRendering.Auto"/> for a RELEASE consumer without a debugger (the CI case).
    /// </summary>
    internal sealed class GraphicsTextRenderStrategy : TextRenderStrategy
    {
        // ANSI escape codes
        private const string Reset = "\x1b[0m";

        private const string BoldRed = "\x1b[1;31m";

        private const string BoldCyan = "\x1b[1;36m";

        private const string BoldWhite = "\x1b[1;37m";

        private const string Gray = "\x1b[90m";

        private const string Green = "\x1b[32m";

        public override bool CanHandle(TextStyle style)
        {
            return style == TextStyle.Graphics;
        }

        public override string Error(string text)
        {
            return $"{BoldRed}{text}{Reset}";
        }

        public override string SectionTitle(string text)
        {
            return $"{BoldCyan}{text}{Reset}";
        }

        public override string Highlight(string text)
        {
            return $"{BoldWhite}{text}{Reset}";
        }

        public override string Dim(string text)
        {
            return $"{Gray}{text}{Reset}";
        }

        public override string Success(string text)
        {
            return $"{Green}{text}{Reset}";
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
