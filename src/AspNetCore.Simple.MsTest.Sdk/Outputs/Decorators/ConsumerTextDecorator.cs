using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk.Decorators
{
    /// <summary>
    /// An <see cref="ITextDecorator"/> permanently bound to one consumer assembly, for the injected
    /// consumers that have no context to resolve from. The assembly is fixed at construction - there
    /// is no shared state to set - while the style is still resolved per call so that attaching a
    /// debugger mid run takes effect.
    /// </summary>
    internal sealed class ConsumerTextDecorator(ITextDecoratorProvider textDecoratorProvider,
                                                Assembly? consumerAssembly) : ITextDecorator
    {
        public string Error(string text)
        {
            return textDecoratorProvider.For(consumerAssembly).Error(text);
        }

        public string SectionTitle(string text)
        {
            return textDecoratorProvider.For(consumerAssembly).SectionTitle(text);
        }

        public string Highlight(string text)
        {
            return textDecoratorProvider.For(consumerAssembly).Highlight(text);
        }

        public string Dim(string text)
        {
            return textDecoratorProvider.For(consumerAssembly).Dim(text);
        }

        public string Success(string text)
        {
            return textDecoratorProvider.For(consumerAssembly).Success(text);
        }
    }
}