using System.Text.RegularExpressions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Contains compiled regular expressions for API version pattern matching.
    /// Uses source-generated regex for better performance.
    /// </summary>
    internal static partial class ApiVersionRegex
    {
        /// <summary>
        /// Matches version patterns in URL path segments.
        /// Examples: /v1/, /v2/, /api/v1/, /api/v2.0/, etc.
        /// Captures the version number (without 'v' prefix) in group 1.
        /// Pattern: (?:^|/)v(\d+(?:\.\d+)?)(?:/|$|\?)
        /// </summary>
        [GeneratedRegex(@"(?:^|/)v(\d+(?:\.\d+)?)(?:/|$|\?)", RegexOptions.IgnoreCase)]
        internal static partial Regex UrlVersionPattern();

        [GeneratedRegex(@"\[(\d+)\](?=\.)")]
        internal static partial Regex IndexReplacement();
    }
}
