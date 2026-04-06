using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddPlainTextDecoratorExtension
    {
        public static void AddPlainTextDecorator(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITextDecorator, PlainTextDecorator>();
        }
    }

    /// <summary>
    /// Plain text decorator that returns text unchanged.
    /// Used in DEBUG builds for Visual Studio Test Explorer compatibility.
    /// Prevents ANSI escape sequence artifacts in VS Test output.
    /// </summary>
    internal sealed class PlainTextDecorator : ITextDecorator
    {
        public string Error(string text) => text;

        public string SectionTitle(string text) => text;

        public string Highlight(string text) => text;

        public string Dim(string text) => text;

        public string Success(string text) => text;
    }
}
