using System.Collections.Generic;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddSnapshotPlaceholderGuardExtension
    {
        public static void AddSnapshotPlaceholderGuard(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ISnapshotPlaceholderGuard, SnapshotPlaceholderGuard>();
        }
    }

    /// <summary>
    ///     Guards against losing placeholders when overwriting snapshots.
    /// </summary>
    public interface ISnapshotPlaceholderGuard
    {
        /// <summary>
        ///     Throws when <paramref name="contentToWrite" /> would drop a placeholder that
        ///     <paramref name="existingContent" /> still has.
        /// </summary>
        void EnsureNoPlaceholderIsLost(EmbeddedFileInfo? expectedResult,
                                      string? existingContent,
                                      string contentToWrite,
                                      IEnumerable<(string Key, object? Value)> parameters);
    }

    /// <summary>
    ///     The last thing that happens before a snapshot is overwritten: does the text about to be written
    ///     still carry every placeholder the file has today?
    ///     This measures the damage rather than its cause, which is what keeps it quiet. A file that never
    ///     had <c>$name$</c> cannot lose it - so a shared helper calling the same endpoint with the same
    ///     parameters against twenty different snapshots stays silent for the nineteen that do not use one of
    ///     them. It only speaks when a placeholder that IS in the file would not come back, whatever the
    ///     reason: a parameter named after something that is not a property, a value that no longer matches,
    ///     a renamed field.
    /// </summary>
    internal sealed class SnapshotPlaceholderGuard : ISnapshotPlaceholderGuard
    {
        /// <summary>
        ///     Throws when <paramref name="contentToWrite" /> would drop a placeholder that
        ///     <paramref name="existingContent" /> still has.
        /// </summary>
        public void EnsureNoPlaceholderIsLost(EmbeddedFileInfo? expectedResult,
                                              string? existingContent,
                                              string contentToWrite,
                                              IEnumerable<(string Key, object? Value)> parameters)
        {
            if (existingContent.IsNullOrWhiteSpace())
            {
                // A snapshot created by this very run had no placeholders to begin with.
                return;
            }

            var before = PlaceholderJson.AllTokens(existingContent);

            if (before.IsEmpty)
            {
                return;
            }

            var lost = before.Except(PlaceholderJson.AllTokens(contentToWrite));

            if (lost.IsEmpty)
            {
                return;
            }

            var supplied = parameters.IsNull()
                               ? []
                               : parameters.Where(parameter => parameter.Key.IsNotNullOrWhiteSpace())
                                           .Select(parameter => parameter.Key.Trim('$'))
                                           .ToList();

            throw new SnapshotPlaceholderLostException(expectedResult?.EmbeddedFileName ?? "the snapshot",
                                                       expectedResult?.EmbeddedFile?.FullName,
                                                       lost,
                                                       supplied);
        }
    }
}