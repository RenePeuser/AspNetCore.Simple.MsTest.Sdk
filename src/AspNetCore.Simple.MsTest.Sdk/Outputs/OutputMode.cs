namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Defines the output mode for assertion failure messages.
    /// </summary>
    public enum OutputMode
    {
        /// <summary>
        /// Human-readable output with formatted sections, tables, and colors.
        /// This is the default mode for developer consumption.
        /// </summary>
        Human = 0,

        /// <summary>
        /// Structured JSON output optimized for AI agents.
        /// Contains error codes, severity, differences, and suggested fixes.
        /// </summary>
        Ai = 1,

        /// <summary>
        /// Hybrid mode that outputs both Human and AI formats.
        /// Reserved for future implementation.
        /// </summary>
        Hybrid = 2
    }
}