using System.Reflection;
using AspNetCore.Simple.MsTest.Sdk.Decorators;

namespace AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers
{
    /// <summary>
    /// Shared helper for getting the text decorator inside the primitive assert extensions, which
    /// have neither a DI container nor an assert context to resolve from.
    /// </summary>
    internal static class TextDecoratorHelper
    {
        private static readonly TextDecoratorProvider Provider = new TextDecoratorProvider();

        /// <summary>
        /// Gets the decorator for the test assembly that made the assert.
        ///
        /// The assembly has to be handed in. This used to call <c>Assembly.GetCallingAssembly()</c>
        /// here instead, which always answered "the sdk": every caller of this method is an sdk
        /// internal Build* helper, never the test. A RELEASE built package therefore always chose
        /// colour and a DEBUG test project got escape codes in its output.
        /// </summary>
        public static ITextDecorator GetTextDecorator(Assembly? consumerAssembly)
        {
            return Provider.For(consumerAssembly);
        }
    }
}
