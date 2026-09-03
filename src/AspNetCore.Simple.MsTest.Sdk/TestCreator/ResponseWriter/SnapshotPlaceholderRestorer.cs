using System;
using System.Linq;
using System.Text.RegularExpressions;
using Extensions.Pack;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Puts a snapshot's placeholders back where they stood, reading the spelling off the file being
    /// updated rather than guessing it.
    ///
    /// Two things forced this. A placeholder standing in for a number has to be written bare, one standing
    /// in for a string has to be quoted - and the SAME name can be both in one file (<c>"id": $id$</c>
    /// next to <c>"ref": "$id$"</c>). A decision taken per NAME gets one of the two wrong. And a
    /// placeholder does not have to align with a property at all: pass <c>$myId$</c> into a url and the
    /// api may hand it back inside a sentence - <c>"The person with the Id: $myId$ does not exist"</c> -
    /// where ParameterReplacer never finds it, because it matches property names and skips short values.
    ///
    /// Walking the two trees side by side answers both. The json path says which occurrence is meant, so
    /// bare stays bare and quoted stays quoted; and where the old value was a sentence carrying a
    /// placeholder, the new value is matched against that sentence as a template - if only the placeholder
    /// part differs, the template is written back untouched.
    /// </summary>
    internal static class SnapshotPlaceholderRestorer
    {
        private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Rewrites <paramref name="merged"/> in place so every placeholder the original snapshot had at a
        /// given path is spelled there again. Values the original did not describe are left alone.
        /// </summary>
        public static void ApplyOriginalSpelling(JToken? merged,
                                                 string? originalSnapshot)
        {
            if (merged.IsNull() || originalSnapshot.IsNullOrWhiteSpace())
            {
                return;
            }

            var original = TryParseTagged(originalSnapshot);

            if (original.IsNull())
            {
                return;
            }

            Walk(original, merged);
        }

        /// <summary>
        /// The original snapshot as a tree, with placeholders turned into markers that still remember
        /// whether they stood bare or quoted.
        /// </summary>
        private static JToken? TryParseTagged(string originalSnapshot)
        {
            try
            {
                return JToken.Parse(PlaceholderJson.MakeParseableKeepingShape(originalSnapshot));
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                // A text snapshot, or a file somebody is in the middle of editing.
                return null;
            }
        }

        private static void Walk(JToken original,
                                 JToken merged)
        {
            if (original is JObject originalObject && merged is JObject mergedObject)
            {
                foreach (var originalProperty in originalObject.Properties())
                {
                    var mergedProperty = mergedObject.Property(originalProperty.Name, StringComparison.OrdinalIgnoreCase);

                    if (mergedProperty.IsNull())
                    {
                        continue;
                    }

                    var replacement = ReplacementFor(originalProperty.Value, mergedProperty.Value);

                    if (replacement.IsNotNull())
                    {
                        mergedProperty.Value = replacement;

                        continue;
                    }

                    Walk(originalProperty.Value, mergedProperty.Value);
                }

                return;
            }

            if (original is JArray originalArray && merged is JArray mergedArray)
            {
                // Only positions both sides have can be spoken about - a response with a different number
                // of entries says nothing about which one used to carry the placeholder.
                foreach (var index in Enumerable.Range(0, Math.Min(originalArray.Count, mergedArray.Count)))
                {
                    var replacement = ReplacementFor(originalArray[index], mergedArray[index]);

                    if (replacement.IsNotNull())
                    {
                        mergedArray[index] = replacement;

                        continue;
                    }

                    Walk(originalArray[index], mergedArray[index]);
                }
            }
        }

        /// <summary>
        /// What the merged value at this path has to become, or null when the original said nothing about
        /// it and the recorded value stands.
        /// </summary>
        private static JValue? ReplacementFor(JToken originalValue,
                                              JToken mergedValue)
        {
            if (originalValue is not JValue { Type: JTokenType.String } value)
            {
                return null;
            }

            var originalText = value.Value?.ToString();

            if (originalText.IsNullOrWhiteSpace())
            {
                return null;
            }

            // The whole value was a placeholder. Which spelling it had is the whole point of the tagged
            // parse - it decides between a number and a string in the file that gets written.
            var tagged = PlaceholderJson.TaggedPlaceholder(originalText);

            if (tagged.IsNotNull())
            {
                return new JValue(tagged.Value.WasBare
                                      ? PlaceholderJson.BareMarkerFor(tagged.Value.Name)
                                      : PlaceholderJson.PlaceholderFor(tagged.Value.Name));
            }

            // The value was a sentence carrying a placeholder. It only survives when the recorded value
            // still looks like that sentence - otherwise the api changed the wording and the new text wins.
            return MatchesTemplate(originalText, mergedValue)
                       ? new JValue(originalText)
                       : null;
        }

        private static bool MatchesTemplate(string template,
                                            JToken mergedValue)
        {
            if (mergedValue is not JValue { Type: JTokenType.String } merged)
            {
                return false;
            }

            var text = merged.Value?.ToString();

            if (text.IsNullOrWhiteSpace() || PlaceholderJson.AllTokens(template).IsEmpty)
            {
                return false;
            }

            // The placeholder stands for whatever the api put there; everything around it has to be the
            // same text, or this is not the same sentence any more.
            var literals = PlaceholderJson.SplitOnPlaceholders(template).Select(Regex.Escape);
            var pattern = $"^{string.Join("(.+?)", literals)}$";

            return Regex.IsMatch(text, pattern, RegexOptions.Singleline, MatchTimeout);
        }
    }
}
