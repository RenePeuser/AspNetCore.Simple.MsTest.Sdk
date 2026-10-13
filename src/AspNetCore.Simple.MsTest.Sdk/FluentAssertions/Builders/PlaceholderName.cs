using System;

namespace AspNetCore.Simple.MsTest.Sdk
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

        /// <summary>
        /// Wraps a naked placeholder name in delimiters; leaves an already-wrapped name unchanged. A name that
        /// carries the delimiter anywhere else (<c>"$"</c>, <c>"$$"</c>, <c>"$Id"</c>, <c>"I$d"</c>) throws: wrapped
        /// it names a token no snapshot contains, so its value would never be sent.
        /// </summary>
        internal static string Wrap(string name)
        {
            var alreadyWrapped = name.Length >= 2 && name[0] == Delimiter && name[^1] == Delimiter;
            var naked = alreadyWrapped ? name[1..^1] : name;

            if (string.IsNullOrWhiteSpace(naked) || naked.Contains(Delimiter, StringComparison.Ordinal))
            {
                throw new ArgumentException($"'{name}' is no placeholder name. Give the name without the '{Delimiter}' delimiter - WithParameter(\"Id\", 1) fills $Id$; the sdk adds the delimiter itself.",
                                            nameof(name));
            }

            return alreadyWrapped ? name : $"{Delimiter}{name}{Delimiter}";
        }
    }
}