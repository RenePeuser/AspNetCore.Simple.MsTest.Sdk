using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddCurrentResponseWriterExtension
    {
        internal static void AddCurrentResponseWriter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<CurrentResponseWriter>();
        }
    }

    internal sealed class CurrentResponseWriter(TestCreatorSettings testCreatorSettings)
    {
        internal void Write(string currentResponseAsString,
                                     string currentResponseFileName,
                                     string callerFilePath)
        {
            // 1. Detect if we really have a file
            var parts = currentResponseAsString.Split('.');
            var fileInfo = new FileInfo(callerFilePath);


            // 2 To keep legacy code compatible we check for
            //   - Result, Results, Response
            var responseFolderName = testCreatorSettings.LegacyFolderNames.Contains(fileInfo.DirectoryName) ?
                                         fileInfo.DirectoryName :
                                         testCreatorSettings.ResponseFolderName;

            // 3. Worst case if result is null
            responseFolderName ??= testCreatorSettings.ResponseFolderName;

            // 4. Define response or results folder
            //    We keep existing once compatible
            var targetResponseFile = new FileInfo(Path.Combine(fileInfo.DirectoryName!, responseFolderName, currentResponseFileName));
            if (targetResponseFile.Directory!.NotExists())
            {
                targetResponseFile.Directory!.Create();
            }

            File.WriteAllText(targetResponseFile.FullName, currentResponseAsString);
        }
    }
}
