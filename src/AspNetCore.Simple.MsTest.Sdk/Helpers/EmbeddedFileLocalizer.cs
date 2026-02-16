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
        // Core Pipeline
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

            var isRawJson = IsRawJson(input);
            if (isRawJson)
            {
                return new(input, input, null);
            }

            var isNoJsonFile = !IsJsonFile(input);
            if (isNoJsonFile)
            {
                return new(input, input, null);
            }

            var allowedSet = allowedFolders
                             .Where(f => !string.IsNullOrWhiteSpace(f))
                             .ToImmutableHashSet(StringComparer.OrdinalIgnoreCase);

            var embeddedResource = ResolveEmbeddedResource(input, assembly, allowedSet);

            if (embeddedResource is null)
            {
                return new(input, string.Empty, null);
            }

            var physicalFile = ResolvePhysicalFile(embeddedResource,
                                                   callerFilePath,
                                                   assembly);

            var content = assembly.GetFileContentFrom(embeddedResource);

            return new EmbeddedFileInfo(embeddedResource, content, physicalFile);
        }

        // ============================================================
        // Validation
        // ============================================================

        private static bool IsRawJson(string input)
        {
            var trimmed = input.TrimStart();
            return trimmed.StartsWith('{') || trimmed.StartsWith('[');
        }

        private static bool IsJsonFile(string input)
        {
            return Path.GetExtension(input)
                       .Equals(".json", StringComparison.OrdinalIgnoreCase);
        }

        // ============================================================
        // Embedded Resource Resolution (Scoped!)
        // ============================================================

        private static string? ResolveEmbeddedResource(string input,
                                                       Assembly assembly,
                                                       ImmutableHashSet<string> allowedFolders)
        {
            var resources = assembly.GetManifestResourceNames();
            if (resources.Length == 0)
            {
                return null;
            }

            var trimmed = input.Trim().Trim('"');
            var fileName = Path.GetFileName(trimmed);

            var scopedMatches = resources
                                .Where(r => EndsWithFileName(r, fileName))
                                .Where(r => ContainsFolderSegment(r, allowedFolders))
                                .ToList();

            if (scopedMatches.Count > 0)
            {
                return scopedMatches
                       .OrderBy(r => r.Length)
                       .ThenBy(r => r, StringComparer.OrdinalIgnoreCase)
                       .First();
            }

            // Fallback without folder scope (deterministic)
            return resources
                   .Where(r => EndsWithFileName(r, fileName))
                   .OrderBy(r => r.Length)
                   .ThenBy(r => r, StringComparer.OrdinalIgnoreCase)
                   .FirstOrDefault();
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
    }
}
