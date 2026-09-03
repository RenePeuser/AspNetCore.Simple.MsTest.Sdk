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
        /// The marker asking for a placeholder to be written WITHOUT quotes, quotes included. A json
        /// string is the only way to carry that wish through a json tree - it is unwrapped in text form by
        /// <see cref="RestoreBareMarkers"/> once the tree has been serialized.
        /// </summary>
        [GeneratedRegex(@"""@@BARE:(?<name>[A-Za-z0-9_.\-]+)@@""")]
        private static partial Regex BareMarkerToken();

        /// <summary>
        /// Like <see cref="MakeParseable"/>, but the marker REMEMBERS the spelling: a bare placeholder
        /// becomes <c>@@PHB:name@@</c>, a quoted one <c>@@PHQ:name@@</c>. Used to read the shape of the
        /// file being updated - never for comparing, where the two spellings have to look identical.
        /// </summary>
        public static string MakeParseableKeepingShape(string? json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json ?? string.Empty;
            }

            var result = ScanOutsideStrings(json, name => $"\"@@PHB:{name}@@\"");

            return QuotedPlaceholder().Replace(result, match => $"\"@@PHQ:{match.Value.Trim('"').Trim('$')}@@\"");
        }

        /// <summary>
        /// Reads a marker written by <see cref="MakeParseableKeepingShape"/> back into its name and the
        /// spelling it had, or null when the text is not such a marker.
        /// </summary>
        public static (string Name, bool WasBare)? TaggedPlaceholder(string? value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return null;
            }

            var match = TaggedToken().Match(value);

            return match.Success
                       ? (match.Groups["name"].Value, match.Groups["kind"].Value == "B")
                       : null;
        }

        /// <summary>
        /// The literal pieces around every placeholder, in order - the fixed text of a sentence that
        /// carries one. <c>"Id: $x$ missing"</c> yields <c>["Id: ", " missing"]</c>.
        /// </summary>
        public static ImmutableList<string> SplitOnPlaceholders(string? text)
        {
            return text.IsNullOrWhiteSpace()
                       ? ImmutableList<string>.Empty
                       : BarePlaceholder().Split(text).ToImmutableList();
        }

        /// <summary>A shape remembering marker, without quotes.</summary>
        [GeneratedRegex(@"^@@PH(?<kind>[BQ]):(?<name>[A-Za-z0-9_.\-]+)@@$")]
        private static partial Regex TaggedToken();

        /// <summary>The value a tree node carries when it must be written bare.</summary>
        public static string BareMarkerFor(string name)
        {
            return $"@@BARE:{name}@@";
        }

        /// <summary>The value a tree node carries when it must be written as a quoted placeholder.</summary>
        public static string PlaceholderFor(string name)
        {
            return $"${name}$";
        }

        /// <summary>
        /// The placeholder name a sentinel or bare marker stands for, or null when the value is neither.
        /// </summary>
        public static string? NameOfSentinel(string? value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return null;
            }

            var sentinel = SentinelToken().Match($"\"{value}\"");

            if (sentinel.Success)
            {
                return sentinel.Groups["name"].Value;
            }

            var bare = BareMarkerToken().Match($"\"{value}\"");

            return bare.Success ? bare.Groups["name"].Value : null;
        }

        /// <summary>
        /// Unwraps the bare markers a tree carried, after it has been serialized: the quotes around them
        /// go away and what is left is the placeholder as the snapshot spelled it.
        /// </summary>
        public static string RestoreBareMarkers(string? json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json ?? string.Empty;
            }

            return BareMarkerToken().Replace(json, match => $"${match.Groups["name"].Value}$");
        }

        /// <summary>
        /// Every sentinel still sitting in the text after the per path restore ran - a placeholder the
        /// current side produced at a path the snapshot does not have. Nothing is known about how it
        /// should be spelled, so it becomes an ordinary quoted placeholder.
        /// </summary>
        public static string RestoreRemainingSentinelsAsQuoted(string? json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return json ?? string.Empty;
            }

            return SentinelToken().Replace(json, match => $"\"${match.Groups["name"].Value}$\"");
        }

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
        /// Every placeholder name in the given text, however it is spelled - bare, quoted, or embedded in
        /// a longer string. Unlike <see cref="BareTokens"/> this does not care where it sits; it answers
        /// "which placeholders does this text mention".
        /// </summary>
        public static ImmutableHashSet<string> AllTokens(string? json)
        {
            if (json.IsNullOrWhiteSpace())
            {
                return ImmutableHashSet<string>.Empty;
            }

            var tokens = ImmutableHashSet.CreateBuilder<string>();

            foreach (Match match in BarePlaceholder().Matches(json))
            {
                tokens.Add(match.Value.Trim('$'));
            }

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
