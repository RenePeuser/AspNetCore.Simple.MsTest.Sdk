using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// A payload or snapshot reference could not be resolved to an embedded resource or a file on disk.
    ///
    /// This used to pass silently: the unresolved reference was handed on as if it were content, and a
    /// downstream substring lookup would happily bind "Persons.json" to "GetAllPersons.json". A test
    /// referencing a file that does not exist could therefore go green against a completely unrelated
    /// snapshot.
    /// </summary>
    public sealed class SnapshotNotFoundException : Exception
    {
        public SnapshotNotFoundException(string reference,
                                         string parameterName,
                                         Assembly callingAssembly,
                                         bool isPayload)
            : base($"Embedded file '{reference}' was not found in assembly '{callingAssembly.GetName().Name}'.")
        {
            Reference = reference;
            ParameterName = parameterName;
            IsPayload = isPayload;
            AssemblyName = callingAssembly.GetName().Name ?? string.Empty;
            Candidates = FindCandidates(reference, callingAssembly);
        }

        public SnapshotNotFoundException()
        {
            Reference = string.Empty;
            ParameterName = string.Empty;
            AssemblyName = string.Empty;
            Candidates = ImmutableList<string>.Empty;
        }

        public SnapshotNotFoundException(string message)
            : base(message)
        {
            Reference = string.Empty;
            ParameterName = string.Empty;
            AssemblyName = string.Empty;
            Candidates = ImmutableList<string>.Empty;
        }

        public SnapshotNotFoundException(string message,
                                         Exception innerException)
            : base(message, innerException)
        {
            Reference = string.Empty;
            ParameterName = string.Empty;
            AssemblyName = string.Empty;
            Candidates = ImmutableList<string>.Empty;
        }

        /// <summary>The reference exactly as written in the test.</summary>
        public string Reference { get; }

        /// <summary>Name of the argument that carried the reference.</summary>
        public string ParameterName { get; }

        /// <summary>True for a request payload, false for an expected response snapshot.</summary>
        public bool IsPayload { get; }

        public string AssemblyName { get; }

        /// <summary>Resources that most likely were meant, best first.</summary>
        public IImmutableList<string> Candidates { get; }

        /// <summary>
        /// Ranks the assembly's resources against the reference. A resource carrying the same file name
        /// in a different folder is the single most common cause (wrong folder prefix), followed by
        /// near misses on the file name itself (typos, singular/plural).
        /// </summary>
        private static ImmutableList<string> FindCandidates(string reference,
                                                             Assembly callingAssembly)
        {
            var resources = callingAssembly.GetManifestResourceNames();

            if (resources.Length.EqualsTo(0))
            {
                return ImmutableList<string>.Empty;
            }

            var wantedFileName = Path.GetFileName(reference.Replace('\\', '/').Trim().Trim('"'));

            var ranked = resources.Select(resource => new
                                                      {
                                                          Resource = resource,
                                                          Score = Score(ResourceFileName(resource), wantedFileName)
                                                      })
                                  .Where(entry => entry.Score <= MaxDistance(wantedFileName))
                                  .OrderBy(entry => entry.Score)
                                  .ThenBy(entry => entry.Resource.Length)
                                  .Select(entry => entry.Resource)
                                  .Take(5)
                                  .ToImmutableList();

            return ranked;
        }

        /// <summary>
        /// Lower is better. A file name that contains the wanted one ranks above any edit-distance hit:
        /// "Persons.json" against "GetAllPersons.json" is precisely the pair the old substring lookup
        /// would have bound silently, so it is the most useful thing to show first.
        /// </summary>
        private static int Score(string candidateFileName,
                                 string wantedFileName)
        {
            if (candidateFileName.Equals(wantedFileName, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            if (candidateFileName.Contains(wantedFileName, StringComparison.OrdinalIgnoreCase) ||
                wantedFileName.Contains(candidateFileName, StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }

            // +1 so a real edit-distance hit never outranks a containment hit.
            return Distance(candidateFileName, wantedFileName) + 1;
        }

        /// <summary>Last two dotted segments of a manifest name - "Folder.File.json" becomes "File.json".</summary>
        private static string ResourceFileName(string resourceName)
        {
            var parts = resourceName.Split('.');

            return parts.Length < 2 ? resourceName : $"{parts[^2]}.{parts[^1]}";
        }

        /// <summary>
        /// Allow roughly a third of the name to differ - enough for a typo or a singular/plural mix-up,
        /// tight enough to not list every snapshot of the project.
        /// </summary>
        private static int MaxDistance(string fileName)
        {
            return Math.Max(3, fileName.Length / 3);
        }

        private static int Distance(string left,
                                    string right)
        {
            if (left.Equals(right, StringComparison.OrdinalIgnoreCase))
            {
                return 0;
            }

            var previous = new int[right.Length + 1];
            var current = new int[right.Length + 1];

            for (var index = 0; index <= right.Length; index++)
            {
                previous[index] = index;
            }

            for (var leftIndex = 1; leftIndex <= left.Length; leftIndex++)
            {
                current[0] = leftIndex;

                for (var rightIndex = 1; rightIndex <= right.Length; rightIndex++)
                {
                    var substitutionCost = char.ToUpperInvariant(left[leftIndex - 1]) == char.ToUpperInvariant(right[rightIndex - 1])
                                               ? 0
                                               : 1;

                    current[rightIndex] = Math.Min(Math.Min(current[rightIndex - 1] + 1,
                                                            previous[rightIndex] + 1),
                                                   previous[rightIndex - 1] + substitutionCost);
                }

                (previous, current) = (current, previous);
            }

            return previous[right.Length];
        }
    }
}
