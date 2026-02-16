using System.Collections.Immutable;
using System.Reflection;
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
        string LocalizeRequest(string embeddedFile,
                               string callerFilePath,
                               Assembly callingAssembly);

        string LocalizeResponse(string embeddedFile,
                                string callerFilePath,
                                Assembly callingAssembly);

        EmbeddedFileInfo LocalizeRequestFile(string embeddedFile,
                                             string callerFilePath,
                                             Assembly callingAssembly);

        EmbeddedFileInfo LocalizeResponseFile(string embeddedFile,
                                              string callerFilePath,
                                              Assembly callingAssembly);
    }

    internal sealed class EmbeddedFileLocalizer(TestCreatorSettings settings)
        : IEmbeddedFileLocalizer
    {
        // ============================================================
        // Public API
        // ============================================================

        public string LocalizeRequest(string embeddedFile,
                                      string callerFilePath,
                                      Assembly callingAssembly)
        {
            return Localize(embeddedFile,
                            callerFilePath,
                            callingAssembly,
                            settings.LegacyRequestFolderName.Concat(settings.RequestFolderName))
                .EmbeddedFileName;
        }

        public string LocalizeResponse(string embeddedFile,
                                       string callerFilePath,
                                       Assembly callingAssembly)
        {
            return Localize(embeddedFile,
                            callerFilePath,
                            callingAssembly,
                            settings.LegacyResponseFolderNames.Concat(settings.ResponseFolderName))
                .EmbeddedFileName;
        }

        public EmbeddedFileInfo LocalizeRequestFile(string embeddedFile,
                                                    string callerFilePath,
                                                    Assembly callingAssembly)
        {
            return Localize(embeddedFile,
                            callerFilePath,
                            callingAssembly,
                            settings.LegacyRequestFolderName.Concat(settings.RequestFolderName));
        }

        public EmbeddedFileInfo LocalizeResponseFile(string embeddedFile,
                                                     string callerFilePath,
                                                     Assembly callingAssembly)
        {
            return Localize(embeddedFile,
                            callerFilePath,
                            callingAssembly,
                            settings.LegacyResponseFolderNames.Concat(settings.ResponseFolderName));
        }

        // ============================================================
        // Core Pipeline (Strict)
        // ============================================================

        private EmbeddedFileInfo Localize(string input,
                                          string callerFilePath,
                                          Assembly assembly,
                                          IEnumerable<string> allowedFolders)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new(input, input, null);
            }

            if (IsRawJson(input))
            {
                return new(input, input, null);
            }

            if (!IsJsonFile(input))
            {
                return new(input, input, null);
            }

            var allowedSet = allowedFolders
                             .Where(f => !string.IsNullOrWhiteSpace(f))
                             .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

            var embeddedResource = ResolveEmbeddedResource(input,
                                                           callerFilePath,
                                                           assembly,
                                                           allowedSet);

            var physicalFile = ResolvePhysicalFile(embeddedResource,
                                                   callerFilePath,
                                                   assembly);

            var content = assembly.GetFileContentFrom(embeddedResource);

            return new EmbeddedFileInfo(embeddedResource, content, physicalFile);
        }

        // ============================================================
        // Strict Embedded Resource Resolution
        // ============================================================

        private static string ResolveEmbeddedResource(string input,
                                                      string callerFilePath,
                                                      Assembly assembly,
                                                      ImmutableHashSet<string> allowedFolders)
        {
            var resources = assembly.GetManifestResourceNames();
            if (resources.Length == 0)
            {
                throw new InvalidOperationException($"Assembly '{assembly.GetName().Name}' contains no embedded resources.");
            }

            var fileName = Path.GetFileName(input.Trim('"'));
            var contextPrefix = BuildContextPrefix(callerFilePath, assembly);

            var matches = resources
                          .Where(r => r.StartsWith(contextPrefix, StringComparison.OrdinalIgnoreCase))
                          .Where(r => ContainsFolderSegment(r, allowedFolders))
                          .Where(r => EndsWithFileName(r, fileName))
                          .ToList();

            if (matches.Count == 0)
            {
                throw new InvalidOperationException($"""
                                                     No embedded resource match found.

                                                     Input File: {input}
                                                     File Name: {fileName}
                                                     Context Prefix: {contextPrefix}
                                                     Allowed Folders: {string.Join(", ", allowedFolders)}

                                                     Caller File:
                                                     {callerFilePath}

                                                     Available Resources:
                                                     {string.Join(Environment.NewLine, resources)}
                                                     """);
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

            return matches[0];
        }

        private static bool EndsWithFileName(string resourceName,
                                             string fileName)
        {
            return resourceName.EndsWith("." + fileName,
                                         StringComparison.OrdinalIgnoreCase)
                   || resourceName.EndsWith(fileName,
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

        private static bool IsJsonFile(string input)
        {
            return Path.GetExtension(input).Equals(".json", StringComparison.OrdinalIgnoreCase);
        }
    }
}
