using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Helpers
{
    public static class AddEmbeddedFileLocalizerExtension
    {
        public static void AddEmbeddedFileLocalizer(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<EmbeddedFileLocalizer>();
        }
    }

    public sealed class EmbeddedFileLocalizer(TestCreatorSettings testCreatorSettings)
    {
        public string LocalizeRequest(string embeddedFile,
                                      string callerFilePath,
                                      Assembly callingAssembly)
        {
            var allowedRequestFolders = testCreatorSettings.LegacyRequestFolderName.Concat(testCreatorSettings.RequestFolderName).ToImmutableHashSet();

            return GetLocalizedFile(embeddedFile, callerFilePath, allowedRequestFolders, callingAssembly);
        }

        public string LocalizeResponse(string embedddFile,
                                       string callerFilePath,
                                       Assembly callingAssembly)
        {
            var allowedRequestFolders = testCreatorSettings.LegacyResponseFolderNames.Concat(testCreatorSettings.ResponseFolderName).ToImmutableHashSet();

            return GetLocalizedFile(embedddFile, callerFilePath, allowedRequestFolders, callingAssembly);
        }

        private string GetLocalizedFile(string embedddFile,
                                        string callerFilePath,
                                        ImmutableHashSet<string> folderNames,
                                        Assembly callingAssembly)
        {
            // 1. If file is null, empty or white space we skipp it
            if (embedddFile.IsNullOrWhiteSpace())
            {
                return embedddFile;
            }

            // 2. We only can localize files, if we have no file we return origin
            var fileExtensions = Path.GetExtension(embedddFile);
            if (fileExtensions.IsNullOrWhiteSpace())
            {
                return embedddFile;
            }
            
            // 3. Detect if we really have a file
            var parts = embedddFile.Split('.');
            var filename = $"{parts[^2]}.{parts[^1]}";
            var trimmedFileName = filename.Trim('"');

            var fileInfo = new FileInfo(callerFilePath);

            // 2 To keep legacy code compatible we check for
            //   - Result, Results, Response

            var legacyFolder = fileInfo.Directory?.EnumerateDirectories().FirstOrDefault(d => folderNames.Contains(d.Name));

            var responseFolderName = legacyFolder.IsNotNull() ? legacyFolder.Name : testCreatorSettings.ResponseFolderName;

            // 3. Worst case if result is null
            responseFolderName ??= testCreatorSettings.ResponseFolderName;

            // 4. Define response or results folder
            //    We keep existing once compatible
            var targetResponseFile = new FileInfo(Path.Combine(fileInfo.DirectoryName!, responseFolderName, trimmedFileName));
            if (targetResponseFile.Directory!.NotExists())
            {
                targetResponseFile.Directory!.Create();
            }

            // 5. Identify the unique embedded file
            var assemblyName = callingAssembly.GetName().Name;
            var embeddedFileNames = callingAssembly.GetManifestResourceNames();
            var splittedPath = targetResponseFile.FullName.Split(assemblyName);

            if (splittedPath.Length < 2)
            {
                return embedddFile;
            }

            var relativePath = splittedPath.Last().Replace(Path.DirectorySeparatorChar.ToString(), ".").Trim('.');
            var match = embeddedFileNames.FirstOrDefault(e => e.Contains(relativePath));

            if (match.IsNotNull())
            {
                return match;
            }
            
            return embedddFile;
        }
    }
}
