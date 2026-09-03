using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Recording would have written a snapshot that no longer carries a placeholder it had before.
    ///
    /// A parameterized snapshot is a template: <c>"resourceId": $resourceId$</c> stands for a value that
    /// differs on every run. Write response puts those placeholders back before it writes - but only
    /// where <c>ParameterReplacer</c> finds them, which is by property name plus an equal value. Where it
    /// finds nothing, the concrete value from this one run is written instead, and the template is gone.
    ///
    /// Nothing used to notice. The next run compares a hard coded id against a fresh one, fails, and the
    /// git diff shows a plausible looking value change rather than a destroyed template - so the search
    /// starts at the api instead of at the recording.
    /// </summary>
    public sealed class SnapshotPlaceholderLostException : Exception
    {
        public SnapshotPlaceholderLostException(string resourceName,
                                                string? filePath,
                                                IEnumerable<string> lostPlaceholders,
                                                IEnumerable<string> suppliedParameters)
            : base($"Recording '{resourceName}' would drop the placeholder(s) {Join(lostPlaceholders)}.")
        {
            ResourceName = resourceName;
            FilePath = filePath;
            LostPlaceholders = lostPlaceholders.OrderBy(name => name, StringComparer.Ordinal).ToImmutableList();
            SuppliedParameters = suppliedParameters.OrderBy(name => name, StringComparer.Ordinal).ToImmutableList();
        }

        public SnapshotPlaceholderLostException()
        {
            ResourceName = string.Empty;
            LostPlaceholders = ImmutableList<string>.Empty;
            SuppliedParameters = ImmutableList<string>.Empty;
        }

        public SnapshotPlaceholderLostException(string message)
            : base(message)
        {
            ResourceName = string.Empty;
            LostPlaceholders = ImmutableList<string>.Empty;
            SuppliedParameters = ImmutableList<string>.Empty;
        }

        public SnapshotPlaceholderLostException(string message,
                                                Exception innerException)
            : base(message, innerException)
        {
            ResourceName = string.Empty;
            LostPlaceholders = ImmutableList<string>.Empty;
            SuppliedParameters = ImmutableList<string>.Empty;
        }

        public string ResourceName { get; }

        /// <summary>Path on disk - this is the file the developer opens.</summary>
        public string? FilePath { get; }

        /// <summary>Placeholder names the file has today and the recording would not write back.</summary>
        public ImmutableList<string> LostPlaceholders { get; }

        /// <summary>Parameter names the assert supplied, to compare against the lost ones.</summary>
        public ImmutableList<string> SuppliedParameters { get; }

        private static string Join(IEnumerable<string> names)
        {
            return string.Join(", ", names.OrderBy(name => name, StringComparer.Ordinal).Select(name => $"${name}$"));
        }
    }
}
