using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Console;

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
                             [CallerFilePath] string callerFilePath = "") where T : class;

        T LocalizeRequest<T>(string embeddedFile,
                             string callerFilePath,
                             Assembly callingAssembly) where T : class;

        T LocalizeResponse<T>(string embeddedFile,
                              [CallerFilePath] string callerFilePath = "") where T : class;

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

        IImmutableList<string> GetAllRequestFileNames(Assembly callingAssembly,
                                                      [CallerFilePath] string callerFilePath = "");

        IImmutableList<EmbeddedFileInfo> GetAllRequestFileInfos(Assembly callingAssembly,
                                                                [CallerFilePath] string callerFilePath = "");

        EmbeddedFileInfo LocalizeResponseFile(string embeddedFile,
                                              string callerFilePath,
                                              Assembly callingAssembly);
    }

    internal sealed class EmbeddedFileLocalizer(TestCreatorSettings settings,
                                                JsonSerializerOptions jsonSerializerOptions)
        : IEmbeddedFileLocalizer
    {
        // ============================================================
        // Public API
        // ============================================================

        public T LocalizeRequest<T>(string embeddedFile,
                                    [CallerFilePath] string callerFilePath = "") where T : class
        {
            var request = LocalizeRequest<T>(embeddedFile, callerFilePath, Assembly.GetCallingAssembly());
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
                                     [CallerFilePath] string callerFilePath = "") where T : class
        {
            var response = LocalizeResponse<T>(embeddedFile, callerFilePath, Assembly.GetCallingAssembly());
            return response;
        }

        public T LocalizeResponse<T>(string embeddedFile,
                                     string callerFilePath,
                                     Assembly callingAssembly) where T : class
        {
            var localizeResponseFile = LocalizeResponseFile(embeddedFile, callerFilePath, Assembly.GetCallingAssembly());
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

        public IImmutableList<string> GetAllRequestFileNames(Assembly callingAssembly,
                                                             [CallerFilePath] string callerFilePath = "")
        {
            var contextPrefix = BuildContextPrefix(callerFilePath, callingAssembly);
            var testCases = callingAssembly.GetManifestResourceNames().Where(r => r.Contains(contextPrefix)).ToImmutableList();
            return testCases;
        }

        public IImmutableList<EmbeddedFileInfo> GetAllRequestFileInfos(Assembly callingAssembly,
                                                                       [CallerFilePath] string callerFilePath = "")
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
                                      .Where(r => r.EndsWith(trimmed, StringComparison.OrdinalIgnoreCase))
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
                    throw new InvalidOperationException($"""
                                                         Absolute embedded resource ambiguous. Exactly ONE expected.

                                                         Requested:
                                                         {trimmed}

                                                         Matches:
                                                         {string.Join(Environment.NewLine, absoluteMatches)}
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
