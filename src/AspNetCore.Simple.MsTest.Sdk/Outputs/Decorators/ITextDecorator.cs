using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Decorators
{
    public static class AddTextDecoratorExtension
    {
        public static void AddTextDecorator(this IServiceCollection services)
        {
#if DEBUG
            services.AddPlainTextDecorator();
#else
            services.AddAnsiColorTextDecorator();
#endif
        }
    }

    /// <summary>
    /// Decorates text output with styling (colors, emphasis).
    /// Different implementations provide plain or colorized output.
    /// Used to make test output more readable without breaking Visual Studio Test Explorer.
    /// </summary>
    public interface ITextDecorator
    {
        /// <summary>
        /// Decorates error messages and critical information.
        /// Typically rendered in bold red.
        /// </summary>
        string Error(string text);

        /// <summary>
        /// Decorates section titles (HTTP CALL, DIFFERENCES, etc.).
        /// Typically rendered in bold cyan.
        /// </summary>
        string SectionTitle(string text);

        /// <summary>
        /// Highlights important values that should stand out.
        /// Typically rendered in bold white.
        /// </summary>
        string Highlight(string text);

        /// <summary>
        /// Dims less important content (borders, separators).
        /// Typically rendered in gray.
        /// </summary>
        string Dim(string text);

        /// <summary>
        /// Decorates positive/actionable content (curl commands).
        /// Typically rendered in green.
        /// </summary>
        string Success(string text);
    }
}