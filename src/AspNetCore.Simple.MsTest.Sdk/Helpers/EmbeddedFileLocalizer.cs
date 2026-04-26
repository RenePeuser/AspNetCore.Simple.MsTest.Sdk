using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
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
            services.AddTestCreatorSettings(configuration);

            services.AddSingletonIfNotExists<IEmbeddedFileLocalizer, EmbeddedFileLocalizer>();
        }
    }

    public sealed record EmbeddedFileInfo(string EmbeddedFileName,
                                          string Content,
                                          FileInfo? EmbeddedFile);

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

    internal sealed class EmbeddedFileLocalizer(TestCreatorSettings settings,
                                                JsonSerializerOptions jsonSerializerOptions)
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
                return new EmbeddedFileInfo(embeddedResource.EmbeddedFile, input, null);
            }

            if (physicalFile.NotExists())
            {
                return new EmbeddedFileInfo(embeddedResource.EmbeddedFile, input, physicalFile);
            }

            // NEW: When you are in snapshot mode, you are writing the json which are at that moment
            //      are not in the assembly
            var embeddedResourceExist = assembly.GetManifestResourceNames().Contains(embeddedResource.EmbeddedFile);

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
                    return (trimmed, false);

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

        private static string BuildContextPrefix(string callerFilePath,
                                                 Assembly assembly)
        {
            var projectFolder = FindProjectFolder(new FileInfo(callerFilePath).Directory,
                                                  assembly);

            if (projectFolder is null)
            {
                return assembly.GetName().Name + ".";
            }

            var relativePath = Path.GetRelativePath(projectFolder.FullName,
                                                    Path.GetDirectoryName(callerFilePath)!);

            var namespacePath = relativePath
                                .Replace(Path.DirectorySeparatorChar, '.')
                                .Trim('.');

            return assembly.GetName().Name + "." + namespacePath;
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
                ParseResourcePath(resourceName, assembly);

            if (relativeFolder is null)
            {
                return null;
            }

            var fullPath = Path.Combine(projectFolder.FullName,
                                        relativeFolder,
                                        fileName);

            var fileInfo = new FileInfo(fullPath);

            if (!fileInfo.Directory!.Exists)
            {
                fileInfo.Directory.Create();
            }

            return fileInfo;
        }

        private static (string? folder, string fileName)
            ParseResourcePath(string resourceName,
                              Assembly assembly)
        {
            var prefix = assembly.GetName().Name + ".";

            if (!resourceName.StartsWith(prefix,
                                         StringComparison.OrdinalIgnoreCase))
            {
                return (null, resourceName);
            }

            var relative = resourceName[prefix.Length..];
            var parts = relative.Split('.');

            if (parts.Length < 2)
            {
                return (null, resourceName);
            }

            var fileName = $"{parts[^2]}.{parts[^1]}";
            var folderSegments = parts[..^2];

            var folderPath = folderSegments.Length > 0
                                 ? Path.Combine(folderSegments)
                                 : string.Empty;

            return (folderPath, fileName);
        }

        // ============================================================
        // Project Folder Detection
        // ============================================================

        private static DirectoryInfo? FindProjectFolder(DirectoryInfo? dir,
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

            return FindProjectFolder(dir.Parent, assembly);
        }

        // ============================================================
        // Validation Helpers
        // ============================================================

        private static bool IsRawJson(string input)
        {
            var trimmed = input.TrimStart();

            return trimmed.StartsWith('{') || trimmed.StartsWith('[');
        }
    }
}
