namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Strategy interface for context-specific output formatting.
    /// Each implementation handles formatting for a specific assertion context type.
    /// </summary>
    internal interface ISpecificOutputFormatter
    {
        /// <summary>
        /// Checks if this formatter can handle the given context.
        /// </summary>
        bool CanFormat(OutputContext context);

        /// <summary>
        /// Formats the output for the given context.
        /// </summary>
        string Format(OutputContext context);
    }
}