using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddTextDecoratorExtension
    {
        /// <summary>
        /// Registers the decorator bound to the consumer test assembly. The assembly has to be handed
        /// in because the sdk cannot read it off itself - see <see cref="ITextDecoratorProvider"/>.
        /// </summary>
        public static void AddTextDecorator(this IServiceCollection services,
                                            Assembly? consumerAssembly = null)
        {
            // A consumer registering the sdk by hand is the direct caller here; the sdk's own
            // registration paths always pass the assembly explicitly, so this never masks them.
            consumerAssembly ??= Assembly.GetCallingAssembly();

            services.AddTextDecoratorProvider();

            // Resolved through the container, so a replaced ITextDecoratorProvider takes effect here too.
            services.TryAddSingleton<ITextDecorator>(serviceProvider => new ConsumerTextDecorator(serviceProvider.GetRequiredService<ITextDecoratorProvider>(),
                                                                                                  consumerAssembly));
        }
    }

    /// <summary>
    /// Decorates text output with styling (colors, emphasis) and symbols.
    /// Every <see cref="OutputRendering"/> style is its own <see cref="ITextRenderStrategy"/>.
    /// Output code never writes a colour code or an emoji itself - it names the role and the symbol,
    /// the strategy decides how that looks.
    /// </summary>
    internal interface ITextDecorator
    {
        /// <summary>
        /// Decorates error messages and critical information.
        /// Typically rendered in bold red.
        /// </summary>
        string Error(string text);

        /// <summary>
        /// <see cref="Error(string)"/> led by <paramref name="symbol"/> - dropped in plain text.
        /// </summary>
        string Error(OutputSymbol symbol,
                     string text);

        /// <summary>
        /// Decorates section titles (HTTP CALL, DIFFERENCES, etc.).
        /// Typically rendered in bold cyan.
        /// </summary>
        string SectionTitle(string text);

        /// <summary>
        /// <see cref="SectionTitle(string)"/> led by <paramref name="symbol"/> - dropped in plain text.
        /// </summary>
        string SectionTitle(OutputSymbol symbol,
                            string text);

        /// <summary>
        /// Highlights important values that should stand out.
        /// Typically rendered in bold white.
        /// </summary>
        string Highlight(string text);

        /// <summary>
        /// <see cref="Highlight(string)"/> led by <paramref name="symbol"/> - dropped in plain text.
        /// </summary>
        string Highlight(OutputSymbol symbol,
                         string text);

        /// <summary>
        /// A symbol standing on its own (a match column, a line pointer) - an emoji, or its ascii
        /// stand-in in plain text.
        /// </summary>
        string Symbol(OutputSymbol symbol);

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