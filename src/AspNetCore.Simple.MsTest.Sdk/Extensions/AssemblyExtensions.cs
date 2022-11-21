using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AssemblyExtensions
    {
        public static T? ReadAs<T>(this object assembly, string fileName) where T : class
        {
            return assembly.GetType().Assembly.ReadAs<T>(fileName);
        }

        public static T? ReadAs<T>(this Assembly assembly, string fileName) where T : class
        {
            var result = assembly.GetFileAsByteArrayFrom(fileName);
            using var streamReader = new StreamReader(new MemoryStream(result.FileContent));
            var stringContent = streamReader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(stringContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public static string GetJsonFileContentFrom(this Assembly assembly, string fileName)
        {
            var result = assembly.GetFileAsByteArrayFrom(fileName);
            using var streamReader = new StreamReader(new MemoryStream(result.FileContent));
            return streamReader.ReadToEnd();
        }

        // very important try to avoid accessing file system during tests, so we fetch our data from
        // our assembly direct from the memory :-) 
        private static InMemoryFile GetFileAsByteArrayFrom(this Assembly assembly, string fileName)
        {
            var manifestResourceNames = assembly.GetManifestResourceNames();
            var name = manifestResourceNames.FirstOrDefault(name => name.ToLower(CultureInfo.InvariantCulture).Contains($"{fileName.ToLower(CultureInfo.InvariantCulture)}"));
            if (name is null)
            {
                throw new EmbededResuorceNotFoundException($"Embeded resource with name: '{fileName}' does not exists. Available for your assembly: '{assembly.GetName().Name}' are: {Environment.NewLine}{manifestResourceNames.Flatten(Environment.NewLine)}");
            }

            // steam can not be null check before validates that embedded resource exists.
            using var stream = assembly.GetManifestResourceStream(name);
            using var ms = new MemoryStream();
            stream!.CopyTo(ms);
            return new InMemoryFile(ms.ToArray(), name);
        }

        public static bool IsCompiledInDebug(this Assembly assembly)
        {
            return assembly.GetCustomAttribute<DebuggableAttribute>()?.IsJITTrackingEnabled ?? false;
        }
    }
}
