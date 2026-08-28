using System.Diagnostics;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Decorators
{
    public static class AddRuntimeTextDecoratorExtension
    {
        public static void AddRuntimeTextDecorator(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITextDecorator, RuntimeTextDecorator>();
        }
    }

    /// <summary>
    /// Runtime text decorator that delegates to PlainTextDecorator or AnsiColorTextDecorator
    /// based on debugger state and build configuration.
    /// This ensures no ANSI escape codes appear when a debugger is attached (IDE test execution)
    /// or when built in DEBUG mode, preventing them from being rendered as ASCII artifacts in test output.
    /// </summary>
    internal sealed class RuntimeTextDecorator : ITextDecorator
    {
        private readonly PlainTextDecorator _plainDecorator = new();
#if !DEBUG
        private readonly AnsiColorTextDecorator _ansiDecorator = new();
#endif

#pragma warning disable CA1859 // Use concrete types when possible for improved performance - interface needed for flexibility
        private ITextDecorator GetDecorator()
        {
            // Check debugger state OR build configuration
            // If either is true, use plain text to avoid ANSI codes in IDE test output
#if DEBUG
            return _plainDecorator;
#else
            return Debugger.IsAttached ? _plainDecorator : _ansiDecorator;
#endif
        }
#pragma warning restore CA1859

        public string Error(string text)
        {
            return GetDecorator().Error(text);
        }

        public string SectionTitle(string text)
        {
            return GetDecorator().SectionTitle(text);
        }

        public string Highlight(string text)
        {
            return GetDecorator().Highlight(text);
        }

        public string Dim(string text)
        {
            return GetDecorator().Dim(text);
        }

        public string Success(string text)
        {
            return GetDecorator().Success(text);
        }
    }
}
