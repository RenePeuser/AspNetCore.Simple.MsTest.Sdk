using System.Reflection;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddWriteResponseServiceExtension
    {
        public static void AddWriteResponseService(this IServiceCollection services)
        {
            services.AddTestSdkSettings();
            services.AddSingletonIfNotExists<IWriteResponseService, WriteResponseService>();
        }
    }

    internal interface IWriteResponseService
    {
        // Very important we do only write in DEBUG mode this is a pure Developer feature !
        bool ShouldWriteResponse(bool scopedWriteResponse,
                                 Assembly callingAssembly);

        /// <summary>
        /// Determines if the response should be written using the context's properties.
        /// This is the preferred method for context-based operations.
        /// </summary>
        bool ShouldWriteResponse(IObjectAssertContext context);
    }

    internal sealed class WriteResponseService(TestSdkSettings testSdkSettings) : IWriteResponseService
    {
        public bool ShouldWriteResponse(bool scopedWriteResponse,
                                        Assembly callingAssembly)
        {
            var requested = WasRequested(scopedWriteResponse);

            if (callingAssembly.IsCompiledInDebug().IsFalse())
            {
                // Dropping an explicit request without a word is what makes a Release run look like a
                // broken sdk: the author passes writeResponse, no file appears, no reason is given, and
                // the next run fails exactly as before. The not-found message says the same thing, but
                // only when a snapshot is missing - a recording that was meant to UPDATE one is silent
                // otherwise, so the trace has to carry it.
                if (requested)
                {
                    SdkTrace.WriteLine($"[WriteResponseService] writeResponse was requested but '{callingAssembly.GetName().Name}' "
                                       + "is not compiled in Debug - recording is a Debug-only feature and is skipped.");
                }

                return false;
            }

            return requested;
        }

        /// <summary>
        /// Whether recording was asked for at all, ignoring the Debug gate. Kept apart from the gate so
        /// "you did not ask" and "you asked and it was refused" stay two different answers.
        /// </summary>
        private bool WasRequested(bool scopedWriteResponse)
        {
            // Bound from the TestSdkSettings__WriteResponse environment variable as well.
            return scopedWriteResponse || testSdkSettings.WriteResponse;
        }

        public bool ShouldWriteResponse(IObjectAssertContext context)
        {
            return ShouldWriteResponse(context.WriteResponse, context.CallingAssembly);
        }
    }
}