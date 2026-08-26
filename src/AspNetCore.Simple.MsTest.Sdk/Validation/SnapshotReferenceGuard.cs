using System;
using System.Reflection;
using System.Text.Json;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    /// <summary>
    /// The two checks that have to run before any comparison touches a snapshot reference.
    ///
    /// They used to live inside the http client only, which left the direct object route
    /// (<c>Assert.That.ObjectsAreEqual("Expected.json", obj)</c>) completely unguarded: a typo there
    /// still travelled on as content, and the downstream lookup matches manifest names by substring -
    /// "Persons.json" binds to "GetAllPersons.json" and the test goes green against a foreign snapshot.
    /// Both routes now call the same guard.
    /// </summary>
    internal static class SnapshotReferenceGuard
    {
        /// <summary>
        /// A payload can never be created on the fly - it is input, not a recording.
        /// </summary>
        public static void EnsurePayloadExists(EmbeddedFileInfo? file,
                                               string reference,
                                               string parameterName,
                                               Assembly callingAssembly)
        {
            if (file.IsNull() || file.Resolved)
            {
                return;
            }

            throw new SnapshotNotFoundException(reference,
                                                parameterName,
                                                callingAssembly,
                                                isPayload: true);
        }

        /// <summary>
        /// A missing snapshot is legitimate while recording - that is what write response is for.
        /// </summary>
        public static void EnsureSnapshotExists(EmbeddedFileInfo? file,
                                                string reference,
                                                string parameterName,
                                                Assembly callingAssembly,
                                                bool writeResponse)
        {
            if (file.IsNull() || file.Resolved || writeResponse)
            {
                return;
            }

            throw new SnapshotNotFoundException(reference,
                                                parameterName,
                                                callingAssembly,
                                                isPayload: false);
        }

        /// <summary>
        /// A file that exists but is not parseable json must say so. Otherwise the shape checks look at
        /// the first character only and report a structure mismatch for a plain syntax error.
        ///
        /// Validates the PARAMETER RESOLVED content: a parameterized snapshot legitimately carries bare
        /// placeholders ("age": $Age$) which are not json until the parameters are applied.
        /// </summary>
        public static void EnsureParseable(EmbeddedFileInfo? file,
                                           string? resolvedContent,
                                           bool isPayload)
        {
            // Only content that came from a *.json file is checked - inline json and text snapshots are
            // not this method's business, and an unresolved reference was already rejected above.
            if (file.IsNull() ||
                file.Resolved.IsFalse() ||
                resolvedContent.IsNullOrWhiteSpace() ||
                file.EmbeddedFileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase).IsFalse())
            {
                return;
            }

            try
            {
                // System.Text.Json is strict per RFC 8259 and reports line and position, where
                // Newtonsoft silently accepts several malformed shapes.
                using var _ = JsonDocument.Parse(resolvedContent);
            }
            catch (JsonException exception)
            {
                throw new InvalidSnapshotJsonException(file.EmbeddedFileName,
                                                       file.EmbeddedFile?.FullName,
                                                       resolvedContent!,
                                                       isPayload,
                                                       exception);
            }
        }
    }
}
