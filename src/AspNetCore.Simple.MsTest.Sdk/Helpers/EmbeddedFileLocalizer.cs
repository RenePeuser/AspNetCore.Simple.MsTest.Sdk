using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddEmbeddedFileLocalizerExtension
    {
        public static void AddEmbeddedFileLocalizer(this IServiceCollection services,
                                                    IConfiguration configuration)
        {
            services.AddTestSdkSettings(configuration);
            services.AddSourceCodeExtractor();
            services.AddResourceRootNamespaceResolver();

            services.AddSingletonIfNotExists<IEmbeddedFileLocalizer, EmbeddedFileLocalizer>();
        }
    }

    /// <param name="EmbeddedFileName">Manifest resource name - synthesized when nothing was found.</param>
    /// <param name="Content">The snapshot content, or the raw input when it is inline json.</param>
    /// <param name="EmbeddedFile">The file on disk, if it could be located.</param>
    /// <param name="Resolved">
    /// False when the input was a file reference that matched neither an embedded resource nor a file
    /// on disk. Callers must not silently continue with such a reference: <see cref="Content"/> then
    /// still holds the file NAME, and comparing against a file name is how a typo turns into a green
    /// test. Only snapshot writing may proceed - there the file is about to be created.
    /// </param>
    public sealed record EmbeddedFileInfo(string EmbeddedFileName,
                                          string Content,
                                          FileInfo? EmbeddedFile,
                                          bool Resolved = true);

    public interface IEmbeddedFileLocalizer
    {
        T LocalizeRequest<T>(string embeddedFile,
                             [CallerFilePath] string callerFilePath = "",
                             [CallerMemberName] string callerMemberName = "") where T : class;

        T LocalizeRequest<T>(string embeddedFile,
                             string callerFilePath,
                             Assembly callingAssembly) where T : class;

        T LocalizeResponse<T>(string embeddedFile,
                              [CallerFilePath] string callerFilePath = "",
                              [CallerMemberName] string callerMemberName = "") where T : class;

        T LocalizeResponse<T>(string embeddedFile,
                              string callerFilePath,
                              Assembly callingAssembly) where T : class;

        string LocalizeRequest(string embeddedFile,
                               string callerFilePath,
                               Assembly callingAssembly);

        string LocalizeResponse(string embeddedFile,
                                string callerFilePath,
                                Assembly callingAssembly);

        EmbeddedFileInfo LocalizeRequestFile(string embeddedFile,
                                             string callerFilePath,
                                             Assembly callingAssembly);

        /// <summary>
        /// Localizes a request file using the HTTP context's payload properties.
        /// This is the preferred method for context-based operations.
        /// </summary>
        EmbeddedFileInfo LocalizeRequestFile(IHttpAssertContext context);

        IImmutableList<string> GetAllRequestFileNames(Assembly callingAssembly,
                                                      [CallerFilePath] string callerFilePath = "",
                                                      [CallerMemberName] string callerMemberName = "");

        IImmutableList<EmbeddedFileInfo> GetAllRequestFileInfos(Assembly callingAssembly,
                                                                [CallerFilePath] string callerFilePath = "",
                                                                [CallerMemberName] string callerMemberName = "");

        EmbeddedFileInfo LocalizeResponseFile(string embeddedFile,
                                              string callerFilePath,
                                              Assembly callingAssembly);

        /// <summary>
        /// Localizes a response file using the context's properties.
        /// This is the preferred method for context-based operations.
        /// </summary>
        EmbeddedFileInfo LocalizeResponseFile(IObjectAssertContext context);
    }

    internal sealed class EmbeddedFileLocalizer(TestSdkSettings settings,
                                                JsonSerializerOptions jsonSerializerOptions,
                                                ITextDecorator textDecorator,
                                                ISourceCodeExtractor sourceCodeExtractor,
                                                IResourceRootNamespaceResolver rootNamespaceResolver)
        : IEmbeddedFileLocalizer
    {
        // ============================================================
        // Public API
        // ============================================================

        public T LocalizeRequest<T>(string embeddedFile,
                                    [CallerFilePath] string callerFilePath = "",
                                    [CallerMemberName] string callerMemberName = "") where T : class
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var request = LocalizeRequest<T>(embeddedFile, callerFilePath, callingAssembly);

            return request;
        }

        public T LocalizeRequest<T>(string embeddedFile,
                                    string callerFilePath,
                                    Assembly callingAssembly) where T : class
        {
            var localizeRequestFile = LocalizeRequestFile(embeddedFile, callerFilePath, callingAssembly);
            var request = localizeRequestFile.Content.FromJsonStringAs<T>(jsonSerializerOptions);

            return request;
        }

        public T LocalizeResponse<T>(string embeddedFile,
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "") where T : class
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            var response = LocalizeResponse<T>(embeddedFile, callerFilePath, callingAssembly);

            return response;
        }

        public T LocalizeResponse<T>(string embeddedFile,
                                     string callerFilePath,
                                     Assembly callingAssembly) where T : class
        {
            var localizeResponseFile = LocalizeResponseFile(embeddedFile, callerFilePath, callingAssembly);
            var response = localizeResponseFile.Content.FromJsonStringAs<T>(jsonSerializerOptions);

            return response;
        }

        public string LocalizeRequest(string embeddedFile,
                                      string callerFilePath,
                                      Assembly callingAssembly)
        {
            return Localize(embeddedFile,
                            callerFilePath,
                            callingAssembly,
                            settings.LegacyRequestFolderName.Concat(settings.RequestFolderName),
                            settings.RequestFolderName).EmbeddedFileName;
        }

        public string LocalizeResponse(string embeddedFile,
                                       string callerFilePath,
                                       Assembly callingAssembly)
        {
            return Localize(embeddedFile,
                            callerFilePath,
                            callingAssembly,
                            settings.LegacyResponseFolderNames.Concat(settings.ResponseFolderName),
                            settings.ResponseFolderName)
                .EmbeddedFileName;
        }

        public EmbeddedFileInfo LocalizeRequestFile(string embeddedFile,
                                                    string callerFilePath,
                                                    Assembly callingAssembly)
        {
            return Localize(embeddedFile,
                            callerFilePath,
                            callingAssembly,
                            settings.LegacyRequestFolderName.Concat(settings.RequestFolderName),
                            settings.RequestFolderName);
        }

        public EmbeddedFileInfo LocalizeRequestFile(IHttpAssertContext context)
        {
            return LocalizeRequestFile(context.PayloadAsJson ?? string.Empty,
                                       context.CallerFilePath,
                                       context.CallingAssembly);
        }

        public IImmutableList<string> GetAllRequestFileNames(Assembly callingAssembly,
                                                             [CallerFilePath] string callerFilePath = "",
                                                             [CallerMemberName] string callerMemberName = "")
        {
            var contextPrefix = BuildContextPrefix(callerFilePath, callingAssembly);
            var testCases = callingAssembly.GetManifestResourceNames().Where(r => r.Contains(contextPrefix)).ToImmutableList();

            return testCases;
        }

        public IImmutableList<EmbeddedFileInfo> GetAllRequestFileInfos(Assembly callingAssembly,
                                                                       [CallerFilePath] string callerFilePath = "",
                                                                       [CallerMemberName] string callerMemberName = "")
        {
            var contextPrefix = BuildContextPrefix(callerFilePath, callingAssembly);
            var testCases = callingAssembly.GetManifestResourceNames().Where(r => r.Contains(contextPrefix)).ToImmutableList();
            var fileInfos = testCases.Select(t => LocalizeRequestFile(t, callerFilePath, callingAssembly)).ToImmutableList();

            return fileInfos;
        }

        public EmbeddedFileInfo LocalizeResponseFile(string embeddedFile,
                                                     string callerFilePath,
                                                     Assembly callingAssembly)
        {
            return Localize(embeddedFile,
                            callerFilePath,
                            callingAssembly,
                            settings.LegacyResponseFolderNames.Concat(settings.ResponseFolderName),
                            settings.ResponseFolderName);
        }

        public EmbeddedFileInfo LocalizeResponseFile(IObjectAssertContext context)
        {
            return LocalizeResponseFile(context.ExpectedObjectAsJson,
                                        context.CallerFilePath,
                                        context.CallingAssembly);
        }

        // ============================================================
        // Core Pipeline (Strict)
        // ============================================================

        private EmbeddedFileInfo Localize(string input,
                                          string callerFilePath,
                                          Assembly assembly,
                                          IEnumerable<string> allowedFolders,
                                          string defaultFolder)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new(input, input, null);
            }

            if (IsRawJson(input))
            {
                return new(input, input, null);
            }

            // Validate that file references have .json extension
            ValidateJsonFileExtension(input, callerFilePath);

            var allowedSet = allowedFolders
                             .Where(f => !string.IsNullOrWhiteSpace(f))
                             .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

            var embeddedResource = ResolveEmbeddedResource(input,
                                                           callerFilePath,
                                                           assembly,
                                                           allowedSet,
                                                           defaultFolder);

            var physicalFile = ResolvePhysicalFile(embeddedResource.EmbeddedFile,
                                                   callerFilePath,
                                                   assembly);

            if (physicalFile.IsNull())
            {
                // No source folder on this machine (or the name does not belong to this assembly).
                // The embedded copy is still the better content than echoing the file name back.
                var embeddedContent = embeddedResource.Exist
                                          ? assembly.GetFileContentOrDefaultFrom(embeddedResource.EmbeddedFile)
                                          : null;

                return new EmbeddedFileInfo(embeddedResource.EmbeddedFile,
                                            embeddedContent ?? input,
                                            null,
                                            embeddedResource.Exist);
            }

            if (physicalFile.NotExists())
            {
                // The source file is gone but the assembly still carries the snapshot - that content
                // is authoritative. Only when neither side has it is the reference unresolved.
                var embeddedContent = embeddedResource.Exist
                                          ? assembly.GetFileContentOrDefaultFrom(embeddedResource.EmbeddedFile)
                                          : null;

                return new EmbeddedFileInfo(embeddedResource.EmbeddedFile,
                                            embeddedContent ?? input,
                                            physicalFile,
                                            embeddedResource.Exist);
            }

            // NEW: When you are in snapshot mode, you are writing the json which are at that moment
            //      are not in the assembly
            var embeddedResourceExist = Enumerable.Contains(assembly.GetManifestResourceNames(), embeddedResource.EmbeddedFile);

            if (embeddedResourceExist.IsFalse())
            {
                var contentFromFile = File.ReadAllText(physicalFile.FullName);

                return new EmbeddedFileInfo(embeddedResource.EmbeddedFile, contentFromFile, physicalFile);
            }

            var content = assembly.GetFileContentOrDefaultFrom(embeddedResource.EmbeddedFile);

            return new EmbeddedFileInfo(embeddedResource.EmbeddedFile, content, physicalFile);
        }

        // ============================================================
        // Strict Embedded Resource Resolution
        // ============================================================

        private (string EmbeddedFile, bool Exist) ResolveEmbeddedResource(string input,
                                                                          string callerFilePath,
                                                                          Assembly assembly,
                                                                          ImmutableHashSet<string> allowedFolders,
                                                                          string defaultFolder)
        {
            var resources = assembly.GetManifestResourceNames();

            if (resources.Length == 0)
            {
                throw new InvalidOperationException($"Assembly '{assembly.GetName().Name}' contains no embedded resources.");
            }

            var trimmed = input.Trim().Trim('"');
            var fileName = Path.GetFileName(trimmed);

            // ------------------------------------------------------------
            // 🔥 NEW FEATURE: Absolute dotted resource path support
            // ------------------------------------------------------------
            // If user passes something like:
            // "AnyFolder.P.NewPersonParameter.json"
            // we treat it as full suffix and DO NOT use context or folder scope
            // ------------------------------------------------------------

            var isAbsoluteDottedPath = trimmed.Contains('.') &&
                                       trimmed.Count(c => c == '.') >= 2 &&
                                       !trimmed.StartsWith('{') &&
                                       !trimmed.StartsWith('[');

            var contextPrefix = BuildContextPrefix(callerFilePath, assembly);

            if (isAbsoluteDottedPath)
            {
                var absoluteMatches = resources
                                      .Where(r => MatchesAsSegment(r, trimmed))
                                      .ToList();

                if (absoluteMatches.Count == 0)
                {
                    // Nothing embedded under that name (yet). Returning the raw input would leave the
                    // writer without a target path, so a brand new snapshot referenced in dotted form
                    // could never be created. Qualify it with the caller context instead - exactly what
                    // the plain file name branch below does.
                    return ($"{contextPrefix}.{trimmed}", false);

                    //    throw new InvalidOperationException($"""
                    //                                         Absolute embedded resource not found.

                    //                                         Requested:
                    //                                         {trimmed}

                    //                                         Available Resources:
                    //                                         {string.Join(Environment.NewLine, resources)}
                    //                                         """);
                }

                if (absoluteMatches.Count > 1)
                {
                    // ============================================================
                    // 🎯 Context-aware disambiguation
                    // ============================================================
                    // If multiple matches exist, prefer the one matching the calling context
                    // Example:
                    //   Test in: Controllers.Test.Api.Persons.PersonController
                    //   Pattern: Requests.SonGoku.json
                    //   Matches:
                    //     - Controllers.Test.Api.Errors.Requests.SonGoku.json
                    //     - Controllers.Test.Api.Persons.Requests.SonGoku.json  ← Prefer this
                    //     - Controllers.Test.Api.NativTypes.Requests.SonGoku.json
                    // ============================================================

                    var contextMatches = absoluteMatches
                                         .Where(r => r.StartsWith(contextPrefix, StringComparison.OrdinalIgnoreCase))
                                         .ToList();

                    if (contextMatches.Count == 1)
                    {
                        // Found exactly one match in the same context - use it
                        return (contextMatches[0], true);
                    }

                    // Still ambiguous or no context match - fail with clear error
                    throw new InvalidOperationException($"""
                                                         Absolute embedded resource ambiguous. Exactly ONE expected.

                                                         Requested:
                                                         {trimmed}

                                                         Caller Context:
                                                         {contextPrefix}

                                                         Matches:
                                                         {string.Join(Environment.NewLine, absoluteMatches)}

                                                         Tip: Use a more specific path to disambiguate (e.g., "Api.Persons.Requests.SonGoku.json")
                                                         """);
                }

                return (absoluteMatches[0], true);
            }

            // ------------------------------------------------------------
            // 🔒 Normal Strict Context-aware resolution
            // ------------------------------------------------------------

            var matches = resources
                          .Where(r => r.StartsWith(contextPrefix, StringComparison.OrdinalIgnoreCase))
                          .Where(r => ContainsFolderSegment(r, allowedFolders))
                          .Where(r => EndsWithFileName(r, fileName))
                          .ToList();

            if (matches.Count == 0)
            {
                var expectedResource = $"{contextPrefix}.{defaultFolder}.{fileName}";

                return (expectedResource, false);

                //throw new InvalidOperationException($"""
                //                                     No embedded resource match found.

                //                                     Input File: {input}
                //                                     File Name: {fileName}
                //                                     Context Prefix: {contextPrefix}
                //                                     Allowed Folders: {string.Join(", ", allowedFolders)}

                //                                     Caller File:
                //                                     {callerFilePath}

                //                                     Available Resources:
                //                                     {string.Join(Environment.NewLine, resources)}
                //                                     """);
            }

            if (matches.Count > 1)
            {
                throw new InvalidOperationException($"""
                                                     Multiple embedded resource matches found. Exactly ONE expected.

                                                     Input File: {input}
                                                     Context Prefix: {contextPrefix}

                                                     Matches:
                                                     {string.Join(Environment.NewLine, matches)}
                                                     """);
            }

            return (matches[0], true);
        }

        private static bool EndsWithFileName(string resourceName,
                                             string fileName)
        {
            // Must match exact last segment
            var lastDotIndex = resourceName.LastIndexOf('.');

            if (lastDotIndex < 0)
            {
                return false;
            }

            var secondLastDotIndex = resourceName.LastIndexOf('.', lastDotIndex - 1);

            if (secondLastDotIndex < 0)
            {
                return false;
            }

            var actualFileName = resourceName[(secondLastDotIndex + 1)..];

            return actualFileName.Equals(fileName,
                                         StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Matches a resource name against a pattern as complete namespace segments.
        /// Ensures that the pattern is not just a suffix, but a complete segment match.
        /// </summary>
        /// <param name="resourceName">Full resource name, e.g., "Controllers.Test.Api.Persons.Requests.SonGoku.json"</param>
        /// <param name="pattern">Pattern to match, e.g., "Requests.SonGoku.json"</param>
        /// <returns>True if the pattern matches as complete segments</returns>
        /// <example>
        /// MatchesAsSegment("Controllers.Test.Api.Persons.Requests.SonGoku.json", "Requests.SonGoku.json") → true
        /// MatchesAsSegment("Controllers.Test.Api.Errors.Requests.SonGoku.json", "Requests.SonGoku.json") → true
        /// MatchesAsSegment("Controllers.Test.Api.ErrorsRequests.SonGoku.json", "Requests.SonGoku.json") → false (no dot before Requests)
        /// </example>
        private static bool MatchesAsSegment(string resourceName,
                                             string pattern)
        {
            // First check: must end with the pattern
            if (!resourceName.EndsWith(pattern, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Second check: ensure it's a complete segment match
            // The character before the pattern must be a dot (or it's the start of string)
            var startIndex = resourceName.Length - pattern.Length;

            // If pattern starts at beginning, it's a match
            if (startIndex == 0)
            {
                return true;
            }

            // Otherwise, the character before must be a dot (namespace separator)
            return resourceName[startIndex - 1] == '.';
        }

        private static bool ContainsFolderSegment(string resourceName,
                                                  ImmutableHashSet<string> folders)
        {
            foreach (var folder in folders)
            {
                if (resourceName.Contains("." + folder + ".",
                                          StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        // ============================================================
        // Context Prefix Builder
        // ============================================================

        private string BuildContextPrefix(string callerFilePath,
                                          Assembly assembly)
        {
            var projectFolder = FindProjectFolder(new FileInfo(callerFilePath).Directory,
                                                  assembly);

            // Resource names are built from the RootNamespace, which is only by default the
            // assembly name - see IResourceRootNamespaceResolver.
            var rootNamespace = rootNamespaceResolver.ResolveForAssembly(assembly, projectFolder);

            if (projectFolder is null)
            {
                return rootNamespace;
            }

            var relativePath = Path.GetRelativePath(projectFolder.FullName,
                                                    Path.GetDirectoryName(callerFilePath)!);

            var namespacePath = relativePath
                                .Replace(Path.DirectorySeparatorChar, '.')
                                .Trim('.');

            // The caller sits in the project root - there is no folder part to append.
            return namespacePath.IsNullOrWhiteSpace()
                       ? rootNamespace
                       : rootNamespace + "." + namespacePath;
        }

        // ============================================================
        // Physical File Mapping
        // ============================================================

        private FileInfo? ResolvePhysicalFile(string resourceName,
                                              string callerFilePath,
                                              Assembly assembly)
        {
            var projectFolder = FindProjectFolder(new FileInfo(callerFilePath).Directory,
                                                  assembly);

            if (projectFolder is null)
            {
                return null;
            }

            var (relativeFolder, fileName) =
                ParseResourcePath(resourceName, assembly, projectFolder);

            if (relativeFolder is null)
            {
                return null;
            }

            var fullPath = Path.Combine(projectFolder.FullName,
                                        relativeFolder,
                                        fileName);

            // The directory is created by the writer that actually needs it - looking a snapshot up
            // must not litter the source tree with empty folders.
            return new FileInfo(fullPath);
        }

        private (string? folder, string fileName)
            ParseResourcePath(string resourceName,
                              Assembly assembly,
                              DirectoryInfo? projectFolder)
        {
            var rootNamespace = rootNamespaceResolver.ResolveForResource(resourceName, assembly, projectFolder);

            if (rootNamespace.IsNullOrWhiteSpace())
            {
                return (null, resourceName);
            }

            var relative = resourceName[(rootNamespace.Length + 1)..];
            var parts = relative.Split('.');

            if (parts.Length < 2)
            {
                return (null, resourceName);
            }

            // A manifest name is one flat dotted string, so it cannot say on its own where a folder
            // ends and the file begins: "Api.V3.1.Responses.My.File.json" reads just as well as
            // folder "V3" + folder "1" and as file "File.json" inside a folder "My". Splitting blindly
            // on '.' picked exactly those wrong readings - a folder like "V3.1" mapped to V3\1 and a
            // file like "my.file.json" lost its first segment to the folder path.
            // The source tree is the authority, so walk it as far as it goes.
            var (matchedFolders, consumed, reachedFolder) = MatchFoldersOnDisk(parts, projectFolder);

            var remaining = parts[consumed..];

            // The file itself settles the remaining dots when it is already there.
            var joinedRemainder = string.Join('.', remaining);

            if (reachedFolder.IsNotNull() &&
                File.Exists(Path.Combine(reachedFolder.FullName, joinedRemainder)))
            {
                return (BuildFolderPath(matchedFolders), joinedRemainder);
            }

            // Nothing on disk to go by - a snapshot about to be created, or sources that are not on
            // this machine. The last two segments are the file name, the rest are folders.
            if (remaining.Length < 2)
            {
                return (BuildFolderPath(matchedFolders), joinedRemainder);
            }

            var fileName = $"{remaining[^2]}.{remaining[^1]}";
            var allFolders = matchedFolders.Concat(remaining[..^2]).ToArray();

            return (BuildFolderPath(allFolders), fileName);
        }

        private static string BuildFolderPath(IReadOnlyCollection<string> folderSegments)
        {
            return folderSegments.Count > 0
                       ? Path.Combine([.. folderSegments])
                       : string.Empty;
        }

        /// <summary>
        /// Consumes as many leading segments as map to folders that really exist. The folders on disk
        /// are matched by their RESOURCE form, not by their name: msbuild runs every folder name
        /// through MakeValidEverettIdentifier before it becomes part of a manifest name, so the folder
        /// "V3.1" appears as "V3._1" and could never be found by looking for a folder called "V3._1".
        ///
        /// The last two segments are never consumed - "Name.json" is the shortest a file name can be.
        /// </summary>
        private static (ImmutableList<string> Folders, int Consumed, DirectoryInfo? ReachedFolder)
            MatchFoldersOnDisk(string[] parts,
                               DirectoryInfo? projectFolder)
        {
            if (projectFolder.IsNull() || projectFolder.Exists.IsFalse())
            {
                return (ImmutableList<string>.Empty, 0, null);
            }

            var folders = ImmutableList<string>.Empty;
            var current = projectFolder;
            var index = 0;

            while (index < parts.Length - 2)
            {
                DirectoryInfo? bestMatch = null;
                var bestLength = 0;

                foreach (var candidate in current.EnumerateDirectories())
                {
                    var segments = ToResourceSegments(candidate.Name);

                    // Longest match wins: with both "V3" and "V3.1" on disk, "V3.1" is the one meant.
                    if (segments.Length <= bestLength ||
                        index + segments.Length > parts.Length - 2 ||
                        MatchesAt(parts, index, segments).IsFalse())
                    {
                        continue;
                    }

                    bestMatch = candidate;
                    bestLength = segments.Length;
                }

                if (bestMatch.IsNull())
                {
                    break;
                }

                current = bestMatch;
                folders = folders.Add(bestMatch.Name);
                index += bestLength;
            }

            return (folders, index, current);
        }

        private static bool MatchesAt(string[] parts,
                                      int index,
                                      string[] segments)
        {
            for (var offset = 0; offset < segments.Length; offset++)
            {
                if (parts[index + offset].Equals(segments[offset], StringComparison.OrdinalIgnoreCase).IsFalse())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The dotted segments a folder name contributes to a manifest resource name. Mirrors msbuild's
        /// MakeValidEverettIdentifier: every dot separated piece has to start with a letter or an
        /// underscore, and anything that is not a letter, digit or underscore is replaced.
        /// </summary>
        private static string[] ToResourceSegments(string folderName)
        {
            return folderName.Split('.')
                             .Select(ToIdentifier)
                             .ToArray();
        }

        private static string ToIdentifier(string piece)
        {
            if (piece.Length.EqualsTo(0))
            {
                return "_";
            }

            var builder = new StringBuilder(piece.Length + 1);

            if (char.IsLetter(piece[0]).IsFalse() && piece[0].EqualsTo('_').IsFalse())
            {
                builder.Append('_');
            }

            foreach (var character in piece)
            {
                builder.Append(char.IsLetterOrDigit(character) || character.EqualsTo('_')
                                   ? character
                                   : '_');
            }

            return builder.ToString();
        }

        // ============================================================
        // Project Folder Detection
        // ============================================================

        private static DirectoryInfo? FindProjectFolder(DirectoryInfo? dir,
                                                        Assembly assembly)
        {
            var byName = FindProjectFolderByName(dir, assembly);

            if (byName.IsNotNull())
            {
                return byName;
            }

            if (dir is null)
            {
                return null;
            }

            // Legacy projects can have a folder name that differs from the assembly name. The nearest
            // folder holding a csproj is the project root by definition.
            var csprojFolder = SourceLocationHelper.FindFirstCsprojDirectory(dir.FullName);

            return csprojFolder.IsNullOrWhiteSpace() ? null : new DirectoryInfo(csprojFolder);
        }

        private static DirectoryInfo? FindProjectFolderByName(DirectoryInfo? dir,
                                                              Assembly assembly)
        {
            if (dir is null)
            {
                return null;
            }

            if (dir.Name.Equals(assembly.GetName().Name,
                                StringComparison.OrdinalIgnoreCase))
            {
                return dir;
            }

            return FindProjectFolderByName(dir.Parent, assembly);
        }

        // ============================================================
        // Validation Helpers
        // ============================================================

        /// <summary>
        /// Validates that a file reference has a .json extension.
        /// Rule: If the input is NOT raw JSON, it MUST end with .json
        /// </summary>
        /// <param name="input">The input string to validate</param>
        /// <param name="callerFilePath">The caller file path for error reporting</param>
        /// <exception cref="InvalidOperationException">Thrown when input is not raw JSON and doesn't end with .json</exception>
        private void ValidateJsonFileExtension(string input,
                                               string callerFilePath)
        {
            var trimmed = input.Trim().Trim('"');

            // Skip validation if empty
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                return;
            }

            // Check if it's raw JSON - if yes, validation passes
            if (IsRawJson(trimmed))
            {
                return;
            }

            // Check if it looks like a file reference (contains path separators or dots suggesting a file extension)
            // If it doesn't look like a file path, assume it's a literal value and skip validation
            if (!LooksLikeFileReference(trimmed))
            {
                return;
            }

            // Looks like a file reference - must end with .json
            var fileExtension = Path.GetExtension(trimmed);

            if (fileExtension.IsNullOrWhiteSpace())
            {
                var suggestedFix = $"{trimmed}.json";

                var errorMessage = BuildMissingJsonExtensionError(trimmed, suggestedFix, callerFilePath,
                                                                  0);

                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Builds a formatted error message for missing .json extension with suggested fix.
        /// </summary>
        private string BuildMissingJsonExtensionError(string invalidInput,
                                                      string suggestedFix,
                                                      string callerFilePath,
                                                      int callerLineNumber)
        {
            var sb = new StringBuilder();

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine(textDecorator.Error("❌ INVALID FILE REFERENCE: MISSING .json EXTENSION"));
            sb.AppendLine(textDecorator.Error("══════════════════════════════════════════════════════════════"));
            sb.AppendLine();

            // File Information (only if we have line number)
            if (callerLineNumber > 0 && callerFilePath.IsNotNullOrWhiteSpace())
            {
                sb.AppendLine(textDecorator.SectionTitle("📦 File Information"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                var fileUri = $"file:///{callerFilePath.Replace('\\', '/')}:{callerLineNumber}";
                sb.AppendLine($"{"File",-10} : {fileUri}");
                sb.AppendLine($"{"Line",-10} : {callerLineNumber}");
                sb.AppendLine();
            }

            // Failure Details
            sb.AppendLine(textDecorator.SectionTitle("⚠️ Problem"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine("File references for payloads and expected responses must end with " + textDecorator.Success(".json"));
            sb.AppendLine();
            sb.AppendLine($"You provided: {textDecorator.Error($"\"{invalidInput}\"")}");
            sb.AppendLine($"Expected:     {textDecorator.Success($"\"{suggestedFix}\"")}");
            sb.AppendLine();

            // What's Valid
            sb.AppendLine(textDecorator.SectionTitle("✅ Valid Inputs"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.Success("  • File reference:  \"MyPayload.json\""));
            sb.AppendLine(textDecorator.Success("  • Dotted path:     \"Requests.MyPayload.json\""));
            sb.AppendLine(textDecorator.Success("  • Inline JSON:     \"{}\" or \"[]\""));
            sb.AppendLine();

            // What's Invalid
            sb.AppendLine(textDecorator.SectionTitle("❌ Invalid Inputs"));
            sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            sb.AppendLine();
            sb.AppendLine(textDecorator.Error("  • Missing .json:   \"MyPayload\""));
            sb.AppendLine(textDecorator.Error("  • Missing .json:   \"Requests.MyPayload\""));
            sb.AppendLine();

            // Assert Call - Original source code (only if line number is available)
            if (callerLineNumber > 0 && callerFilePath.IsNotNullOrWhiteSpace())
            {
                var sourceCode = sourceCodeExtractor.ExtractCallCode(callerFilePath, callerLineNumber);

                if (sourceCode.IsNotNullOrWhiteSpace())
                {
                    sb.AppendLine(textDecorator.SectionTitle("📝 Assert Call"));
                    sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                    sb.AppendLine();
                    sb.AppendLine(sourceCode);
                    sb.AppendLine();

                    // Suggested Fix - Generate corrected code
                    var suggestedFixCode = sourceCode.Replace($"\"{invalidInput}\"", $"\"{suggestedFix}\"");

                    sb.AppendLine(textDecorator.SectionTitle("✅ Suggested Fix"));
                    sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                    sb.AppendLine();
                    sb.AppendLine(textDecorator.Success(suggestedFixCode));
                    sb.AppendLine();
                }
            }
            else
            {
                // Fallback if no source code is available
                sb.AppendLine(textDecorator.SectionTitle("✅ Suggested Fix"));
                sb.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
                sb.AppendLine();
                sb.AppendLine($"Change {textDecorator.Error($"\"{invalidInput}\"")} to {textDecorator.Success($"\"{suggestedFix}\"")}");
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private static bool IsRawJson(string input)
        {
            var trimmed = input.TrimStart();

            // Check for JSON objects and arrays
            if (trimmed.StartsWith('{') || trimmed.StartsWith('['))
            {
                return true;
            }

            // Check for JSON strings (must start and end with quotes)
            if (trimmed.StartsWith('"') && trimmed.EndsWith('"') && trimmed.Length >= 2)
            {
                return true;
            }

            // Check for JSON numbers (integers or decimals, positive or negative)
            if (IsJsonNumber(trimmed))
            {
                return true;
            }

            // Check for JSON booleans and null
            if (trimmed.Equals("true", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("false", StringComparison.OrdinalIgnoreCase) ||
                trimmed.Equals("null", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static bool LooksLikeFileReference(string input)
        {
            // Contains path separators - definitely a file path
            if (input.Contains('/') || input.Contains('\\'))
            {
                return true;
            }

            // Contains dots (file extension or dotted path like "Requests.MyPayload")
            // But exclude simple decimals like "3.14"
            if (input.Contains('.'))
            {
                // If it's a pure number, it's not a file reference
                if (IsJsonNumber(input))
                {
                    return false;
                }

                return true;
            }

            // Contains neither separators nor dots - treat it as a literal value
            return false;
        }

        private static bool IsJsonNumber(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            // Simple check: starts with digit or minus, and contains only valid number characters
            var firstChar = input[0];

            if (firstChar != '-' && !char.IsDigit(firstChar))
            {
                return false;
            }

            // Check if it can be parsed as a number
            return double.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture,
                                   out _);
        }
    }
}