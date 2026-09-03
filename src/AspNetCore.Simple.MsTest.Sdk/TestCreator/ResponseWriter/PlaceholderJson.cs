using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// A parameterized snapshot is only valid json AFTER its parameters are resolved. A parameter standing
    /// in for a number has to be written bare - <c>"resourceId": $resourceId$</c> - because quoting it
    /// would turn the resolved value into a string.
    ///
    /// The whole write response machinery reads the snapshot with Newtonsoft: the difference writer parses
    /// it to merge new values in, and <see cref="SnapshotShape"/> parses it to find out which shape the
    /// file uses. Neither can read a bare placeholder. So a snapshot with a numeric parameter could not be
    /// re-recorded at all - the writer threw a JsonReaderException, and the shape lookup silently fell back
    /// to "no shape" and replaced the file wholesale.
    ///
    /// This translates such a snapshot into something Newtonsoft can read, and back again. A placeholder
    /// becomes an ordinary json string carrying a sentinel, so a bare and a quoted placeholder end up as
    /// the very same token. That unification matters: the two sides of a merge do not spell it the same
    /// way - the snapshot has the bare form, while <c>ParameterReplacer.ReplaceWithPlaceholders</c> writes
    /// the quoted one into the current side. Without it every re-record would diff <c>"$resourceId$"</c>
    /// against <c>$resourceId$</c> and rewrite a value nobody changed.
    ///
    /// <see cref="Restore"/> puts the original spelling back, driven by the tokens that stood bare in the
    /// file being updated - so a snapshot keeps the exact form its author wrote.
    /// </summary>
    internal static partial class PlaceholderJson
    {
        private const string SentinelPrefix = "@@PH:";

        private const string SentinelSuffix = "@@";

        /// <summary>A json string whose whole content is a placeholder, quotes included.</summary>
        [GeneratedRegex(@"""\$[A-Za-z0-9_.\-]+\$""")]
        private static partial Regex QuotedPlaceholder();

        /// <summary>A placeholder on its own: two dollar signs around a name.</summary>
        [GeneratedRegex(@"\$[A-Za-z0-9_.\-]+\$")]
        private static partial Regex BarePlaceholder();

        /// <summary>The sentinel <see cref="MakeParseable"/> writes, quotes included.</summary>
        [GeneratedRegex(@"""@@PH:(?<name>[A-Za-z0-9_.\-]+)@@""")]
        private static partial Regex SentinelToken();

        /// <summary>
        /// The placeholder names the given snapshot spells bare, i.e. outside of a json string. Those are
        /// the ones that must not come back quoted - they stand in for numbers.
        /// </summary>
        public static ImmutableHashSet<string> BareTokens(string? json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return ImmutableHashSet<string>.Empty;
            }

            var tokens = ImmutableHashSet.CreateBuilder<string>();

            ScanOutsideStrings(json,
                               name =>
                               {
                                   tokens.Add(name);

                                   return null;
                               });

            return tokens.ToImmutable();
        }

        /// <summary>
        /// Rewrites every placeholder - bare or quoted - into a plain json string carrying the sentinel, so
        /// the text parses and both spellings compare equal. Running it twice changes nothing: a sentinel
        /// carries no dollar signs of its own.
        /// </summary>
        public static string MakeParseable(string? json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json ?? string.Empty;
            }

            // Bare first - that is the form which keeps the text from parsing at all.
            var result = ScanOutsideStrings(json, Sentinelize);

            // A string whose whole content is a placeholder means the same as the bare form and has to end
            // up as the same token, otherwise the two sides of a merge differ in nothing but quotes.
            return QuotedPlaceholder().Replace(result, match => Sentinelize(match.Value.Trim('"').Trim('$')) ?? match.Value);
        }

        /// <summary>
        /// Turns the sentinels back into placeholders. A name listed in <paramref name="bareTokens"/> is
        /// written without quotes, every other one keeps the quoted form.
        /// </summary>
        public static string Restore(string? json,
                                     IReadOnlySet<string> bareTokens)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json ?? string.Empty;
            }

            return SentinelToken().Replace(json,
                                           match =>
                                           {
                                               var name = match.Groups["name"].Value;

                                               return bareTokens.Contains(name)
                                                          ? $"${name}$"
                                                          : $"\"${name}$\"";
                                           });
        }

        private static string? Sentinelize(string name)
        {
            return $"\"{SentinelPrefix}{name}{SentinelSuffix}\"";
        }

        /// <summary>
        /// Walks the text once, tracking whether the current position sits inside a json string, and hands
        /// every placeholder found OUTSIDE a string to <paramref name="onBarePlaceholder"/>. Returning a
        /// replacement rewrites it, returning null leaves it alone - which is how the same walk serves both
        /// the collecting and the rewriting caller.
        /// </summary>
        private static string ScanOutsideStrings(string json,
                                                 Func<string, string?> onBarePlaceholder)
        {
            var builder = new StringBuilder(json.Length);
            var inString = false;
            var index = 0;

            while (index < json.Length)
            {
                var character = json[index];

                if (inString)
                {
                    builder.Append(character);

                    // A backslash escapes the next character - including a quote, which must not be read
                    // as the end of the string.
                    if (character == '\\' && index + 1 < json.Length)
                    {
                        builder.Append(json[index + 1]);
                        index += 2;

                        continue;
                    }

                    if (character == '"')
                    {
                        inString = false;
                    }

                    index++;

                    continue;
                }

                if (character == '"')
                {
                    inString = true;
                    builder.Append(character);
                    index++;

                    continue;
                }

                if (character == '$')
                {
                    var match = BarePlaceholder().Match(json, index);

                    if (match.Success && match.Index == index)
                    {
                        var name = match.Value.Trim('$');

                        builder.Append(onBarePlaceholder(name) ?? match.Value);
                        index += match.Length;

                        continue;
                    }
                }

                builder.Append(character);
                index++;
            }

            return builder.ToString();
        }
    }
}
