using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddPlainTextRenderStrategyExtension
    {
        public static void AddPlainTextRenderStrategy(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITextRenderStrategy, PlainTextRenderStrategy>();
        }
    }

    /// <summary>
    /// No colours, no emojis - <see cref="OutputRendering.PlainText"/>. Made for messages that end up
    /// in a TRX file: the VSTest TRX logger writes ESC and every emoji surrogate half as literal
    /// <c>\uXXXX</c> text, which a Markdown summary then shows verbatim. Box drawing borders are
    /// valid XML and stay.
    /// </summary>
    internal sealed class PlainTextRenderStrategy : TextRenderStrategy
    {
        public override bool CanHandle(TextStyle style)
        {
            return style == TextStyle.PlainText;
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
            return OutputSymbols.Ascii(symbol);
        }

        protected override string? LeadSymbol(OutputSymbol symbol)
        {
            return null;
        }
    }
}
