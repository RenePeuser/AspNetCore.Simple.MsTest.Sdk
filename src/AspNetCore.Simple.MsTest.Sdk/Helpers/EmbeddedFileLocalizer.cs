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

    internal sealed class EmbeddedFileLocalizer(TestCreatorSettings settings) : IEmbeddedFileLocalizer
    {
        public string LocalizeRequest(string embeddedFile,
                                      string callerFilePath,
                                      Assembly callingAssembly)
        {
            var allowedFolders = settings.LegacyRequestFolderName.Concat(settings.RequestFolderName);

            var localizedRequest = Localize(embeddedFile, callerFilePath, callingAssembly, allowedFolders).EmbeddedFileName;
            return localizedRequest;
        }

        public string LocalizeResponse(string embeddedFile,
                                       string callerFilePath,
                                       Assembly callingAssembly)
        {
            var allowedFolders = settings.LegacyResponseFolderNames.Concat(settings.ResponseFolderName);

            var localizedResponse = Localize(embeddedFile, callerFilePath, callingAssembly, allowedFolders).EmbeddedFileName;
            return localizedResponse;
        }

        public EmbeddedFileInfo LocalizeRequestFile(string embeddedFile,
                                                    string callerFilePath,
                                                    Assembly callingAssembly)
        {
            return Localize(embeddedFile, callerFilePath, callingAssembly,
                            settings.LegacyRequestFolderName.Concat(settings.RequestFolderName));
        }

        public EmbeddedFileInfo LocalizeResponseFile(string embeddedFile,
                                                     string callerFilePath,
                                                     Assembly callingAssembly)
        {
            return Localize(embeddedFile, callerFilePath, callingAssembly,
                            settings.LegacyResponseFolderNames.Concat(settings.ResponseFolderName));
        }

        // ==============================
        // Core Pipeline
        // ==============================

        private EmbeddedFileInfo Localize(string input,
                                          string callerFilePath,
                                          Assembly assembly,
                                          IEnumerable<string> allowedFolders)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new(input, string.Empty, null);
            }

            if (IsRawJson(input))
            {
                return new(input, string.Empty, null);
            }

            if (!IsJsonFile(input))
            {
                return new(input, string.Empty, null);
            }

            var embeddedResource = ResolveEmbeddedResource(input, assembly);
            if (embeddedResource is null)
            {
                return new(input, string.Empty, null);
            }

            var physicalFile = ResolvePhysicalFile(embeddedResource,
                                                   callerFilePath,
                                                   assembly,
                                                   allowedFolders.ToImmutableHashSet(StringComparer.OrdinalIgnoreCase));

            var content = assembly.GetFileContentFrom(embeddedResource);

            return new EmbeddedFileInfo(embeddedResource, content, physicalFile);
        }

        // ==============================
        // Validation Helpers
        // ==============================

        private static bool IsRawJson(string input)
        {
            return input.TrimStart().StartsWith('{') ||
                   input.TrimStart().StartsWith('[');
        }

        private static bool IsJsonFile(string input)
        {
            return Path.GetExtension(input)
                       .Equals(".json", StringComparison.OrdinalIgnoreCase);
        }

        // ==============================
        // Embedded Resource Resolution
        // ==============================

        private static string? ResolveEmbeddedResource(string fileName,
                                                       Assembly assembly)
        {
            var resources = assembly.GetManifestResourceNames();

            return resources.FirstOrDefault(r =>
                                                r.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));
        }

        // ==============================
        // Physical File Resolution
        // ==============================

        private FileInfo? ResolvePhysicalFile(string resourceName,
                                              string callerFilePath,
                                              Assembly assembly,
                                              ImmutableHashSet<string> allowedFolders)
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

        // ==============================
        // Resource Path Parsing
        // ==============================

        private static (string? folder, string fileName) ParseResourcePath(string resourceName, Assembly assembly)
        {
            var prefix = assembly.GetName().Name + ".";

            if (!resourceName.StartsWith(prefix, StringComparison.Ordinal))
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
            var folders = parts[..^2];

            var folderPath = Path.Combine(folders);

            return (folderPath, fileName);
        }

        // ==============================
        // Project Folder Detection
        // ==============================

        private static DirectoryInfo? FindProjectFolder(DirectoryInfo? dir,
                                                        Assembly assembly)
        {
            if (dir is null)
            {
                return null;
            }

            if (dir.Name.Equals(assembly.GetName().Name, StringComparison.OrdinalIgnoreCase))
            {
                return dir;
            }

            return FindProjectFolder(dir.Parent, assembly);
        }
    }
}
