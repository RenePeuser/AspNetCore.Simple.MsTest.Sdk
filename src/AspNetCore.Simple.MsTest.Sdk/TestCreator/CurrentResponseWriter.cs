using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

    public sealed class CurrentResponseWriter(EmbeddedFileLocalizer embeddedFileLocalizer)
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

            var localizedFile = embeddedFileLocalizer.LocalizeResponseFile(expectedResponseFileName, callerFilePath,
                                                                           callingAssembly);

            if (localizedFile.EmbeddedFile.IsNull())
            {
                return;
            }
            
            // Parse the JSON string
            var parsedJson = JToken.Parse(currentResponseAsString);
            var formattedJson = parsedJson.ToString(Formatting.Indented);

            File.WriteAllText(localizedFile.EmbeddedFile.FullName, formattedJson);
        }
    }
}
