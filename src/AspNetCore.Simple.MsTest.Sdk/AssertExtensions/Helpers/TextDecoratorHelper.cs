using AspNetCore.Simple.MsTest.Sdk.Decorators;

namespace AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers
{
    /// <summary>
    /// Shared helper for getting the appropriate text decorator based on build configuration.
    /// </summary>
    internal static class TextDecoratorHelper
    {
        /// <summary>
        /// Gets the appropriate text decorator based on build configuration.
        /// Returns PlainTextDecorator in DEBUG, AnsiColorTextDecorator in RELEASE.
        /// </summary>
        /// <returns>The text decorator instance</returns>
#pragma warning disable CA1859 // Use concrete types when possible for improved performance - interface needed for flexibility
        public static ITextDecorator GetTextDecorator()
        {
#if DEBUG
            return new PlainTextDecorator();
#else
            return new AnsiColorTextDecorator();
#endif
        }
#pragma warning restore CA1859
    }
}