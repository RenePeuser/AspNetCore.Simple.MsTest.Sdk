using System.Text.RegularExpressions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Contains compiled regular expressions for API version pattern matching.
    /// Uses source-generated regex for better performance.
    /// </summary>
    internal static partial class GlobalRegex
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

        /// <summary>
        /// Matches the key indexer JsonDiffer produces for key-value arrays: ["theme"] in
        /// settings["theme"].Value. Captures the key without its quotes in group 1.
        /// See MemberPathQuery - the segment is its own notation and has to be translated
        /// before Newtonsoft can resolve it.
        /// </summary>
        [GeneratedRegex(@"\[""([^""]*)""\]")]
        internal static partial Regex KeyIndexer();

        /// <summary>
        /// Matches Assert method names followed by opening parenthesis.
        /// Examples: AssertGetAsync(, AssertPostAsync(, AssertDeleteAsync(, etc.
        /// Captures the method name in group 1.
        /// Pattern: (Assert\w+Async)\s*\(
        /// </summary>
        [GeneratedRegex(@"(Assert\w+Async)\s*\(")]
        internal static partial Regex AssertMethodPattern();
    }
}