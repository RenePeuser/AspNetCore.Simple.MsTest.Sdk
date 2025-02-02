using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddCurrentResponseWriterExtension
    {
        public static void AddCurrentResponseWriter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<CurrentResponseWriter>();
        }
    }

    public sealed class CurrentResponseWriter(TestCreatorSettings testCreatorSettings)
    {
        public void Write(string currentResponseAsString,
                          string expectedResponseFileName,
                          string callerFilePath,
                          Assembly callingAssembly)
        {
            if (callingAssembly.IsCompiledInDebug().IsFalse())
            {
                Console.WriteLine("We are not writing into test results file only in DEBUG mode. Cause of safety reasons.");
                return;
            }
            
            // ToDo: We just support .json file at the moment
            if (expectedResponseFileName.Trim('"').EndsWith(".json", StringComparison.OrdinalIgnoreCase).IsFalse())
            {
                return;
            }

            // 1. Detect if we really have a file
            var parts = expectedResponseFileName.Split('.');
            var filename = $"{parts[^2]}.{parts[^1]}";
            var trimmedFileName = filename.Trim('"');

            var fileInfo = new FileInfo(callerFilePath);

            // 2 To keep legacy code compatible we check for
            //   - Result, Results, Response
            var legacyFolder = fileInfo.Directory?.EnumerateDirectories().FirstOrDefault(d => testCreatorSettings.LegacyFolderNames.Contains(d.Name));


            var responseFolderName = legacyFolder.IsNotNull() ?
                                         legacyFolder.Name :
                                         testCreatorSettings.ResponseFolderName;

            // 3. Worst case if result is null
            responseFolderName ??= testCreatorSettings.ResponseFolderName;

            // 4. Define response or results folder
            //    We keep existing once compatible
            var targetResponseFile = new FileInfo(Path.Combine(fileInfo.DirectoryName!, responseFolderName, trimmedFileName));
            if (targetResponseFile.Directory!.NotExists())
            {
                targetResponseFile.Directory!.Create();
            }
            
            // Parse the JSON string
            var parsedJson = JToken.Parse(currentResponseAsString); 
            var formattedJson = parsedJson.ToString(Formatting.Indented);

            File.WriteAllText(targetResponseFile.FullName, formattedJson);
        }
    }
}
