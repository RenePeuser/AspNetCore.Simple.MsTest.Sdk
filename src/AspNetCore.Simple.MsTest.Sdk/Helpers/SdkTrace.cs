using System;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Diagnostic tracing for the snapshot and write-response pipeline.
    ///
    /// These lines are what makes a misrouted snapshot debuggable - which writer declined, which mode
    /// was chosen, which resource name was resolved. They are invaluable when something goes wrong and
    /// pure noise on every one of the hundreds of green tests in a normal run, so they are off unless
    /// asked for.
    ///
    /// Enable with the environment variable <c>AspNetCoreSimpleMsTestSdk__Trace=true</c>, or by setting
    /// <see cref="Enabled"/> from a test.
    /// </summary>
    public static class SdkTrace
    {
        private static bool? _enabled;

        public static bool Enabled
        {
            get
            {
                _enabled ??= Environment.GetEnvironmentVariable("AspNetCoreSimpleMsTestSdk__Trace")?.ToBool() ?? false;

                return _enabled.Value;
            }

            set => _enabled = value;
        }

        public static void WriteLine(string message)
        {
            if (Enabled.IsFalse())
            {
                return;
            }

            Console.WriteLine(message);
        }
    }
}
