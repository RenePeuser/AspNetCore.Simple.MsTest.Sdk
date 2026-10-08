using System;
using Extensions.Pack;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Snapshots exist in two shapes. The writers always produce the full response envelope
    /// (<c>{ content: { headers, value }, statusCode, headers, isSuccessStatusCode }</c>), while a lot
    /// of older snapshots are the bare response body. Both are read correctly - the comparison wraps a
    /// bare body into an envelope before diffing - but writing always emitted the envelope, so the
    /// first write to an old snapshot silently migrated it to the other shape. With a globally enabled
    /// write response that happens to whole folders at once, in a diff nobody asked for.
    ///
    /// An existing snapshot therefore keeps the shape it has. Only a snapshot that is created now gets
    /// the envelope.
    /// </summary>
    internal static class SnapshotShape
    {
        /// <summary>
        /// Reduces <paramref name="currentResponseAsString"/> to the shape <paramref name="existingContent"/>
        /// already uses. Anything that cannot be read as json, and any existing envelope, is returned
        /// unchanged.
        /// </summary>
        public static string MatchExisting(string currentResponseAsString,
                                           string? existingContent)
        {
            if (existingContent.IsNullOrWhiteSpace() ||
                currentResponseAsString.IsNullOrWhiteSpace())
            {
                return currentResponseAsString;
            }

            var existing = TryParse(existingContent);

            // No shape to preserve, or the file is an envelope already.
            if (existing.IsNull() || IsEnvelope(existing))
            {
                return currentResponseAsString;
            }

            var current = TryParse(currentResponseAsString);

            if (current.IsNull() || IsEnvelope(current).IsFalse())
            {
                return currentResponseAsString;
            }

            var body = current["content"]?["value"];

            // An envelope without a body says nothing - leave the writer with what it had.
            return body.IsNull()
                       ? currentResponseAsString
                       : body.ToString(Newtonsoft.Json.Formatting.None);
        }

        /// <summary>
        /// True for the response envelope: an object carrying a <c>content</c> object with a
        /// <c>value</c>. A bare body could only look like this by carrying those very names itself,
        /// which is why <c>statusCode</c> is required as a second marker.
        /// </summary>
        public static bool IsEnvelope(string json)
        {
            return IsEnvelope(TryParse(json));
        }

        /// <inheritdoc cref="IsEnvelope(string)" />
        public static bool IsEnvelope(JToken? token)
        {
            if (token is not JObject jsonObject)
            {
                return false;
            }

            var content = jsonObject.GetValue("content", StringComparison.OrdinalIgnoreCase);

            if (content is not JObject contentObject ||
                (contentObject.ContainsKey("value").IsFalse() &&
                 contentObject.GetValue("value", StringComparison.OrdinalIgnoreCase).IsNull()))
            {
                return false;
            }

            return jsonObject.GetValue("statusCode", StringComparison.OrdinalIgnoreCase).IsNotNull() ||
                   jsonObject.GetValue("isSuccessStatusCode", StringComparison.OrdinalIgnoreCase).IsNotNull();
        }

        private static JToken? TryParse(string json)
        {
            try
            {
                return JToken.Parse(json);
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                // A text snapshot, or a file somebody is in the middle of editing.
                return null;
            }
        }
    }
}