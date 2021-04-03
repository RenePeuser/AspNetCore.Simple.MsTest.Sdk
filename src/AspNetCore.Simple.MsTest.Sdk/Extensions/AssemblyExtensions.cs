using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AssemblyExtensions
    {
        public static T ReadAs<T>(this object assembly, string fileName) where T : class
        {
            return assembly.GetType().Assembly.ReadAs<T>(fileName);
        }

        public static T ReadAs<T>(this Assembly assembly, string fileName) where T : class
        {
            var result = assembly.GetFileAsByteArrayFrom(fileName);
            using var streamReader = new StreamReader(new MemoryStream(result.FileContent));
            var stringContent = streamReader.ReadToEnd();
            return JsonSerializer.Deserialize<T>(stringContent, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        }

        public static string GetJsonFileContentFrom(this Assembly assembly, string fileName)
        {
            var result = assembly.GetFileAsByteArrayFrom(fileName);
            using var streamReader = new StreamReader(new MemoryStream(result.FileContent));
            return streamReader.ReadToEnd();
        }

        public static InMemoryFile GetValidExcelFileAsByteArray(this Assembly assembly)
        {
            return assembly.GetFileAsByteArrayFrom("co2-sample.xlsx");
        }

        public static InMemoryFile GetInValidExcelFile(this Assembly assembly)
        {
            return assembly.GetFileAsByteArrayFrom("co2-sample-invalid.xlsx");
        }


        // very important try to avoid accessing file system during tests, so we fetch our data from
        // our assembly direct from the memory :-) 
        private static InMemoryFile GetFileAsByteArrayFrom(this Assembly assembly, string fileName)
        {
            var name = assembly.GetManifestResourceNames().FirstOrDefault(name => name.Contains($".{fileName}"));
            using var stream = assembly.GetManifestResourceStream(name);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return new InMemoryFile(ms.ToArray(), name);
        }
    }
}
