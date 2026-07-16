namespace AspNetCore.Simple.MsTest.Sdk.FluentAssertions.Builders
{
    /// <summary>
    /// Owns the placeholder delimiter convention (§10.1). Test code uses the NAKED name (<c>"Id"</c>);
    /// this class adds the delimiters. Already-escaped input (<c>"$Id$"</c>) is detected and kept as-is
    /// (Postel's Law) so the ~1000-test migration stays a pure signature sweep. The delimiter here is the
    /// single source of truth — change it, and no test needs touching.
    /// </summary>
    internal static class PlaceholderName
    {
        private const char Delimiter = '$';

        /// <summary>Wraps a naked placeholder name in delimiters; leaves an already-wrapped name unchanged.</summary>
        internal static string Wrap(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var alreadyWrapped = name.Length >= 2 && name[0] == Delimiter && name[name.Length - 1] == Delimiter;

            return alreadyWrapped ? name : $"{Delimiter}{name}{Delimiter}";
        }
    }
}