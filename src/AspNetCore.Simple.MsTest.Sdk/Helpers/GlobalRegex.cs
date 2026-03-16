using System.Text.RegularExpressions;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static partial class GlobalRegex
    {
        [GeneratedRegex(@"\[(\d+)\](?=\.)")]
        internal static partial Regex IndexReplacement();
    }
}
