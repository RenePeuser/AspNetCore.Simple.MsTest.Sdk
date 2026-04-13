using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Decorators
{
    public static class AddAnsiColorTextDecoratorExtension
    {
        public static void AddAnsiColorTextDecorator(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITextDecorator, AnsiColorTextDecorator>();
        }
    }

    /// <summary>
    /// ANSI color text decorator that adds color and style codes.
    /// Used in RELEASE builds for CI/terminal output with colors.
    /// </summary>
    internal sealed class AnsiColorTextDecorator : ITextDecorator
    {
        // ANSI escape codes
        private const string Reset = "\x1b[0m";

        private const string BoldRed = "\x1b[1;31m";

        private const string BoldCyan = "\x1b[1;36m";

        private const string BoldWhite = "\x1b[1;37m";

        private const string Gray = "\x1b[90m";

        private const string Green = "\x1b[32m";

        public string Error(string text) => $"{BoldRed}{text}{Reset}";

        public string SectionTitle(string text) => $"{BoldCyan}{text}{Reset}";

        public string Highlight(string text) => $"{BoldWhite}{text}{Reset}";

        public string Dim(string text) => $"{Gray}{text}{Reset}";

        public string Success(string text) => $"{Green}{text}{Reset}";
    }
}
