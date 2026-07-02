using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using AspNetCore.Simple.MsTest.Sdk.Decorators;

namespace AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers
{
    /// <summary>
    /// Shared helper for getting the appropriate text decorator based on build configuration.
    /// </summary>
    internal static class TextDecoratorHelper
    {
        /// <summary>
        /// Gets the appropriate text decorator based on the calling assembly's build configuration.
        /// Returns PlainTextDecorator if the calling assembly is in DEBUG mode, AnsiColorTextDecorator otherwise.
        /// This ensures the decorator matches the test project's build configuration, not the SDK's.
        /// </summary>
        /// <returns>The text decorator instance</returns>
#pragma warning disable CA1859 // Use concrete types when possible for improved performance - interface needed for flexibility
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ITextDecorator GetTextDecorator()
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var isDebugMode = IsAssemblyDebugBuild(callingAssembly);

            return isDebugMode ? new PlainTextDecorator() : new AnsiColorTextDecorator();
        }
#pragma warning restore CA1859

        /// <summary>
        /// Determines if an assembly was built in DEBUG mode by checking the DebuggableAttribute.
        /// </summary>
        private static bool IsAssemblyDebugBuild(Assembly assembly)
        {
            var debuggableAttribute = assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)
                                              .OfType<DebuggableAttribute>()
                                              .FirstOrDefault();

            if (debuggableAttribute == null)
            {
                return false;
            }

            return debuggableAttribute.IsJITTrackingEnabled;
        }
    }
}