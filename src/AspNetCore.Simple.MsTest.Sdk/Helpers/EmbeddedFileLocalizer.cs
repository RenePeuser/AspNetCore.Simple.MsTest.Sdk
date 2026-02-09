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

    public record EmbeddedFileInfo(string EmbeddedFileName,
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

        EmbeddedFileInfo LocalizeResponseFile(string embedddFile,
                                              string callerFilePath,
                                              Assembly callingAssembly);
    }

    internal sealed class EmbeddedFileLocalizer(TestCreatorSettings testCreatorSettings) : IEmbeddedFileLocalizer
    {
        public string LocalizeRequest(string embeddedFile,
                                      string callerFilePath,
                                      Assembly callingAssembly)
        {
            var allowedRequestFolders = testCreatorSettings.LegacyRequestFolderName.Concat(testCreatorSettings.RequestFolderName).ToImmutableHashSet();

            return GetLocalizedFile(embeddedFile, callerFilePath, allowedRequestFolders,
                                    callingAssembly).EmbeddedFileName;
        }

        public string LocalizeResponse(string embeddedFile,
                                       string callerFilePath,
                                       Assembly callingAssembly)
        {
            var allowedRequestFolders = testCreatorSettings.LegacyResponseFolderNames.Concat(testCreatorSettings.ResponseFolderName).ToImmutableHashSet();

            return GetLocalizedFile(embeddedFile, callerFilePath, allowedRequestFolders,
                                    callingAssembly).EmbeddedFileName;
        }

        public EmbeddedFileInfo LocalizeRequestFile(string embeddedFile,
                                                    string callerFilePath,
                                                    Assembly callingAssembly)
        {
            if (embeddedFile.EndsWith(".json").IsFalse())
            {
                return new EmbeddedFileInfo(string.Empty, embeddedFile, null);
            }

            var allowedRequestFolders = testCreatorSettings.LegacyRequestFolderName.Concat(testCreatorSettings.RequestFolderName).ToImmutableHashSet();

            var localizeRequestFile = GetLocalizedFile(embeddedFile, callerFilePath, allowedRequestFolders,
                                                       callingAssembly);

            return localizeRequestFile;
        }

        public EmbeddedFileInfo LocalizeResponseFile(string embedddFile,
                                                     string callerFilePath,
                                                     Assembly callingAssembly)
        {
            if (embedddFile.EndsWith(".json").IsFalse())
            {
                return new EmbeddedFileInfo(string.Empty, embedddFile, null);
            }

            var allowedRequestFolders = testCreatorSettings.LegacyResponseFolderNames.Concat(testCreatorSettings.ResponseFolderName).ToImmutableHashSet();

            return GetLocalizedFile(embedddFile, callerFilePath, allowedRequestFolders,
                                    callingAssembly);
        }

        private EmbeddedFileInfo GetLocalizedFile(string embedddFile,
                                                  string callerFilePath,
                                                  ImmutableHashSet<string> folderNames,
                                                  Assembly callingAssembly)
        {
            // 1. If file is null, empty or white space we skipp it
            if (embedddFile.IsNullOrWhiteSpace())
            {
                return new EmbeddedFileInfo(embedddFile, string.Empty, null);
            }

            // 3. If embedded file is raw json return
            if (embedddFile.StartsWith('{') ||
                embedddFile.StartsWith('['))
            {
                return new EmbeddedFileInfo(embedddFile, string.Empty, null);
            }

            // 2. We only can localize files, if we have no file we return origin
            var fileExtensions = Path.GetExtension(embedddFile);
            if (fileExtensions.IsNullOrWhiteSpace())
            {
                return new EmbeddedFileInfo(embedddFile, string.Empty, null);
            }

            // In case we have a native string separated by .
            if (fileExtensions.DoesNotContain(".json"))
            {
                return new EmbeddedFileInfo(embedddFile, string.Empty, null);
            }

            // 3. Get assembly infos
            var assemblyName = callingAssembly.GetName().Name;
            var embeddedFileNames = callingAssembly.GetManifestResourceNames();

            // 4. Detect if we really have a file
            var parts = embedddFile.Split('.');

            // 4. Detect if we already have an absolute path
            var matchingFiles = embeddedFileNames.Where(file => file.Contains(embedddFile, StringComparison.OrdinalIgnoreCase)).ToList();

            var filename = $"{parts[^2]}.{parts[^1]}";
            var trimmedFileName = filename.Trim('"');

            var fileInfo = new FileInfo(callerFilePath);

            // 2 To keep legacy code compatible we check for
            //   - Result, Results, Response

            // NewPersonParameter.json
            // Requests.NewPersonParameter.json
            // Responses.NewPersonParameter.json
            // AnyFolder.P.NewPersonParameter.json
            if (matchingFiles.Count.EqualsTo(1) &&
                parts.Length > 2)
            {
                var embeddedFileName = matchingFiles[0];

                // Go back to test folder which ends with .Test or Tests
                var projectFolder = FindProjectFolder(fileInfo.Directory, callingAssembly);

                if (projectFolder.IsNotNull())
                {
                    var relativePath2 = embeddedFileName.Replace(trimmedFileName, string.Empty)
                                                        .Replace(projectFolder.Name, string.Empty)
                                                        .Replace('.', Path.DirectorySeparatorChar)
                                                        .Trim(Path.DirectorySeparatorChar);

                    var filePath = Path.Combine(projectFolder.FullName, relativePath2.TrimStart('\''), filename);
                    var fileInfo2 = new FileInfo(filePath);

                    if (fileInfo2.Exists)
                    {
                        var fileContentEmbedded = callingAssembly.GetFileContentFrom(embedddFile);
                        return new EmbeddedFileInfo(embeddedFileName, fileContentEmbedded, fileInfo2);
                    }
                }

                return new EmbeddedFileInfo(embeddedFileName, string.Empty, null);
            }

            var legacyFolder = fileInfo.Directory?.EnumerateDirectories().FirstOrDefault(d => folderNames.Contains(d.Name));

            var responseFolderName = legacyFolder.IsNotNull() ? legacyFolder.Name : testCreatorSettings.ResponseFolderName;

            // 3. Worst case if result is null
            responseFolderName ??= testCreatorSettings.ResponseFolderName;

            if (embedddFile.Contains(responseFolderName))
            {
                var test = embedddFile.Replace(trimmedFileName, string.Empty);
                var split = test.Split(responseFolderName).LastOrDefault()?.TrimStart('.').Replace('.', Path.DirectorySeparatorChar);
                trimmedFileName = $"{split}{trimmedFileName}";
            }

            // 4. Define response or results folder
            //    We keep existing once compatible
            var targetResponseFile = new FileInfo(Path.Combine(fileInfo.DirectoryName!, responseFolderName, trimmedFileName));

            if (targetResponseFile.Directory!.NotExists())
            {
                targetResponseFile.Directory!.Create();
            }

            // 5. Identify the unique embedded file
            var splittedPath = targetResponseFile.FullName.Split(assemblyName);

            if (splittedPath.Length < 2)
            {
                var fileContentEmbedded = callingAssembly.GetFileContentFrom(embedddFile);
                return new EmbeddedFileInfo(embedddFile, fileContentEmbedded, targetResponseFile);
            }

            var relativePath = splittedPath.Last().Replace(Path.DirectorySeparatorChar.ToString(), ".").Trim('.');
            var match = embeddedFileNames.FirstOrDefault(e => e.Contains(relativePath));

            if (match.IsNotNull())
            {
                var fileContentFrom = callingAssembly.GetFileContentFrom(match);
                return new EmbeddedFileInfo(match, fileContentFrom, targetResponseFile);
            }

            var fileContentFromRelative = callingAssembly.GetFileContentFrom(relativePath);
            return new EmbeddedFileInfo(relativePath, fileContentFromRelative, targetResponseFile);
        }

        private DirectoryInfo? FindProjectFolder(DirectoryInfo? directoryInfo,
                                                 Assembly assembly)
        {
            if (directoryInfo.IsNull())
            {
                return directoryInfo;
            }

            if (directoryInfo.Name.EqualsTo(assembly.GetName().Name))
            {
                return directoryInfo;
            }

            return FindProjectFolder(directoryInfo.Parent, assembly);
        }
    }
}
