using System.Diagnostics;
using System.Reflection;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Decorators
{
    internal static class AddTextDecoratorProviderExtension
    {
        public static void AddTextDecoratorProvider(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ITextDecoratorProvider, TextDecoratorProvider>();
        }
    }

    /// <summary>
    /// Picks plain or coloured output for a given consumer assembly.
    ///
    /// The sdk ships as a RELEASE built nuget package, so a <c>#if DEBUG</c> inside it says nothing
    /// about the test project consuming it - it is evaluated when the sdk is compiled. The answer is
    /// a property of the CONSUMER assembly, which is why every caller has to hand it in.
    /// </summary>
    internal interface ITextDecoratorProvider
    {
        /// <summary>
        /// The decorator matching <paramref name="consumerAssembly"/>. A null assembly means the
        /// caller could not be identified and is treated as a CI run - colour stays on.
        /// </summary>
        ITextDecorator For(Assembly? consumerAssembly);
    }

    internal sealed class TextDecoratorProvider : ITextDecoratorProvider
    {
        // Both decorators are immutable and stateless, so one instance each serves every caller.
        private static readonly ITextDecorator PlainDecorator = new PlainTextDecorator();

        private static readonly ITextDecorator AnsiDecorator = new AnsiColorTextDecorator();

        public ITextDecorator For(Assembly? consumerAssembly)
        {
            // A debugger suffers the same escape code artifacts as the test explorer does.
            if (Debugger.IsAttached)
            {
                return PlainDecorator;
            }

            if (consumerAssembly.IsNull())
            {
                return AnsiDecorator;
            }

            return consumerAssembly.IsCompiledInDebug() ? PlainDecorator : AnsiDecorator;
        }
    }
}