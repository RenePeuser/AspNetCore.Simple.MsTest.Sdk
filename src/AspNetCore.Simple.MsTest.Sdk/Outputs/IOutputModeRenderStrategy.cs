using System.Collections.Immutable;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Strategy interface for mode-specific output rendering.
    /// Each implementation handles rendering for a specific output mode.
    /// </summary>
    internal interface IOutputModeRenderStrategy
    {
        /// <summary>
        /// Checks if this strategy can handle the given output mode.
        /// </summary>
        bool CanHandle(OutputMode mode);

        /// <summary>
        /// Renders the output for the given mode.
        /// </summary>
        string Render(IObjectAssertContext context,
                      ImmutableList<Difference> differences,
                      string expectedJson,
                      string currentJson);
    }
}