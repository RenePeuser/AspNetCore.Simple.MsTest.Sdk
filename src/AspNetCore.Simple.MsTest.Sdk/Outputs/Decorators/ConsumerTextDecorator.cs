using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// An <see cref="ITextDecorator"/> permanently bound to one consumer assembly, for the injected
    /// consumers that have no context to resolve from. The assembly is fixed at construction - there
    /// is no shared state to set - while the style is still resolved per call so that attaching a
    /// debugger mid run, or a changed <see cref="TestSdkSettings.OutputRendering"/>, takes effect.
    /// </summary>
    internal sealed class ConsumerTextDecorator(ITextDecoratorProvider textDecoratorProvider,
                                                Assembly? consumerAssembly) : ITextDecorator
    {
        private ITextDecorator Current => textDecoratorProvider.For(consumerAssembly);

        public string Error(string text)
        {
            return Current.Error(text);
        }

        public string Error(OutputSymbol symbol,
                            string text)
        {
            return Current.Error(symbol, text);
        }

        public string SectionTitle(string text)
        {
            return Current.SectionTitle(text);
        }

        public string SectionTitle(OutputSymbol symbol,
                                   string text)
        {
            return Current.SectionTitle(symbol, text);
        }

        public string Highlight(string text)
        {
            return Current.Highlight(text);
        }

        public string Highlight(OutputSymbol symbol,
                                string text)
        {
            return Current.Highlight(symbol, text);
        }

        public string Symbol(OutputSymbol symbol)
        {
            return Current.Symbol(symbol);
        }

        public string Dim(string text)
        {
            return Current.Dim(text);
        }

        public string Success(string text)
        {
            return Current.Success(text);
        }
    }
}
