using System;
using System.Text.Json;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// A payload or snapshot file was found, but its content is not parseable json.
    ///
    /// Nothing used to verify this. The broken content simply reached the shape checks, which look at
    /// the first character only - so <c>{ "a": }</c> was reported as
    /// "JSON TYPE MISMATCH: OBJECT {} TO ARRAY [] CONVERSION" together with the advice to wrap it in
    /// brackets. Following that advice does nothing, because the file is not valid json in the first place.
    /// </summary>
    public sealed class InvalidSnapshotJsonException : Exception
    {
        public InvalidSnapshotJsonException(string resourceName,
                                            string? filePath,
                                            string content,
                                            bool isPayload,
                                            Exception parseError)
            : base($"'{resourceName}' does not contain valid json: {parseError.Message}", parseError)
        {
            ResourceName = resourceName;
            FilePath = filePath;
            Content = content;
            IsPayload = isPayload;
            ParseMessage = parseError.Message;

            if (parseError is JsonException jsonException && jsonException.LineNumber.HasValue)
            {
                // LineNumber is zero based.
                LineNumber = (int)jsonException.LineNumber.Value + 1;
                Position = jsonException.BytePositionInLine.HasValue
                               ? (int)jsonException.BytePositionInLine.Value + 1
                               : null;
            }
        }

        public InvalidSnapshotJsonException()
        {
            ResourceName = string.Empty;
            Content = string.Empty;
            ParseMessage = string.Empty;
        }

        public InvalidSnapshotJsonException(string message)
            : base(message)
        {
            ResourceName = string.Empty;
            Content = string.Empty;
            ParseMessage = message;
        }

        public InvalidSnapshotJsonException(string message,
                                            Exception innerException)
            : base(message, innerException)
        {
            ResourceName = string.Empty;
            Content = string.Empty;
            ParseMessage = message;
        }

        public string ResourceName { get; }

        /// <summary>Path on disk, when the file could be located - this is what the developer opens.</summary>
        public string? FilePath { get; }

        public string Content { get; }

        public bool IsPayload { get; }

        /// <summary>Parser message, which carries the line and position of the offending token.</summary>
        public string ParseMessage { get; }

        /// <summary>One based line of the offending token, when the parser reported one.</summary>
        public int? LineNumber { get; }

        /// <summary>One based column of the offending token, when the parser reported one.</summary>
        public int? Position { get; }
    }
}
