using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddTextRenderStrategiesExtension
    {
        /// <summary>
        /// Registers one render strategy per <see cref="TextStyle"/>. A consumer replaces a style by
        /// registering its own strategy first - exactly one has to handle each style.
        /// </summary>
        public static void AddTextRenderStrategies(this IServiceCollection services)
        {
            services.AddGraphicsTextRenderStrategy();
            services.AddMonochromeTextRenderStrategy();
            services.AddPlainTextRenderStrategy();
        }
    }

    /// <summary>
    /// The concrete look an output is rendered in. <see cref="OutputRendering.Auto"/> is not one of
    /// them - it is resolved to one of these per consumer assembly.
    /// </summary>
    internal enum TextStyle
    {
        /// <summary>ANSI colours and emojis.</summary>
        Graphics,

        /// <summary>Emojis without colour - <see cref="OutputRendering.Auto"/> in a DEBUG build or under a debugger.</summary>
        Monochrome,

        /// <summary>Neither colours nor emojis.</summary>
        PlainText
    }

    /// <summary>
    /// Renders output in exactly one <see cref="TextStyle"/>. Same pattern as
    /// <see cref="IOutputModeRenderStrategy"/>: the <see cref="ITextDecoratorProvider"/> picks the one
    /// strategy whose <see cref="CanHandle"/> matches.
    /// </summary>
    internal interface ITextRenderStrategy : ITextDecorator
    {
        bool CanHandle(TextStyle style);
    }

    /// <summary>
    /// Shared shape of every strategy: a style only decides how a role is painted and how a symbol
    /// looks. Combining a symbol with its text is the same for all of them.
    /// </summary>
    internal abstract class TextRenderStrategy : ITextRenderStrategy
    {
        public abstract bool CanHandle(TextStyle style);

        public abstract string Error(string text);

        public abstract string SectionTitle(string text);

        public abstract string Highlight(string text);

        public abstract string Dim(string text);

        public abstract string Success(string text);

        public abstract string Symbol(OutputSymbol symbol);

        public string Error(OutputSymbol symbol,
                            string text)
        {
            return Error(Lead(symbol, text));
        }

        public string SectionTitle(OutputSymbol symbol,
                                   string text)
        {
            return SectionTitle(Lead(symbol, text));
        }

        public string Highlight(OutputSymbol symbol,
                                string text)
        {
            return Highlight(Lead(symbol, text));
        }

        /// <summary>
        /// The text led by <paramref name="symbol"/>. Null drops the lead symbol entirely - a symbol in
        /// front of a title is decoration, an ascii stand-in there would only be noise.
        /// </summary>
        protected abstract string? LeadSymbol(OutputSymbol symbol);

        private string Lead(OutputSymbol symbol,
                            string text)
        {
            var lead = LeadSymbol(symbol);

            return lead is null ? text : $"{lead} {text}";
        }
    }
}
