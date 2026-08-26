using System;
using Extensions.Pack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// A <see cref="Difference.MemberPath"/> looks like JSONPath but is not. <see cref="JsonDiffer"/>
    /// addresses an element of a key-value array by its Key instead of by its index -
    /// <c>settings["theme"].Value</c> rather than <c>settings[0].Value</c> - and Newtonsoft rejects
    /// the double quotes while parsing the path:
    /// <c>JsonException: Unexpected character while parsing path indexer: "</c>.
    ///
    /// That exception used to escape <see cref="DifferenceResponseWriter"/> and take the whole write
    /// down with a message naming nothing that led back to the payload. The equivalent JSONPath is a
    /// filter over the Key property, so the segment is translated before the lookup; every other path
    /// passes through unchanged.
    /// </summary>
    internal static class MemberPathQuery
    {
        /// <summary>
        /// Resolves a member path against <paramref name="root"/>. Returns null when the path does not
        /// exist there or cannot be expressed as JSONPath - never throws, because a path nobody can
        /// address is not a reason to fail a snapshot write.
        /// </summary>
        public static JToken? SelectToken(JToken? root,
                                          string memberPath)
        {
            if (root.IsNull() || memberPath.IsNullOrWhiteSpace())
            {
                return null;
            }

            var jsonPath = ToJsonPath(memberPath);

            if (jsonPath.IsNull())
            {
                return null;
            }

            try
            {
                return root!.SelectToken(jsonPath!);
            }
            catch (JsonException)
            {
                // An unaddressable path leaves the current value in place - the same outcome as before
                // this translation existed, and a lot better than failing the write over it.
                return null;
            }
        }

        /// <summary>
        /// Translates the key indexers of a member path into JSONPath filters. Returns null when the
        /// path cannot be translated safely.
        /// </summary>
        public static string? ToJsonPath(string memberPath)
        {
            if (memberPath.Contains('"', StringComparison.Ordinal).IsFalse())
            {
                return memberPath;
            }

            var untranslatable = false;

            var jsonPath = GlobalRegex.KeyIndexer()
                                      .Replace(memberPath,
                                               match =>
                                               {
                                                   var key = match.Groups[1].Value;

                                                   // A key carrying a single quote would break out of the
                                                   // filter expression and address something else entirely.
                                                   if (key.Contains('\'', StringComparison.Ordinal))
                                                   {
                                                       untranslatable = true;

                                                       return match.Value;
                                                   }

                                                   return $"[?(@.Key=='{key}')]";
                                               });

            return untranslatable ? null : jsonPath;
        }
    }
}
