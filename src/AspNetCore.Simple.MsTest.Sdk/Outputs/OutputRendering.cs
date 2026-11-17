namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// How assertion failure output is rendered - independent of <see cref="OutputMode"/>, which picks
    /// WHAT is written. This picks how it LOOKS.
    /// </summary>
    public enum OutputRendering
    {
        /// <summary>
        /// Decided per consumer test assembly: a DEBUG build or an attached debugger gets emojis without
        /// colour (the Visual Studio Test Explorer shows ANSI codes as raw characters), everything else
        /// gets colour and emojis. This is the default.
        /// </summary>
        Auto = 0,

        /// <summary>
        /// ANSI colours and emojis, always - also in a DEBUG build.
        /// </summary>
        Graphics = 1,

        /// <summary>
        /// No colours, no emojis - plain text only. Box drawing borders stay, they are valid in every
        /// target. Use this when the message ends up in a TRX file and a Markdown summary
        /// (dorny/test-reporter, ...): the VSTest TRX logger writes ANSI escape codes and emojis as
        /// literal <c>\u001b[1;31m</c> / <c>📋</c> text.
        /// </summary>
        PlainText = 2
    }
}
