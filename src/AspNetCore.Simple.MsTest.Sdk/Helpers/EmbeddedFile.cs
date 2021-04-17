using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk.Helpers
{
    public static class EmbeddedFile
    {
        public static string GetFileContentFrom(string fileName)
        {
            return GetFileContentFrom(Assembly.GetCallingAssembly(), fileName);
        }

        public static string GetFileContentFrom(this Assembly assembly, string fileName)
        {
            return assembly.GetJsonFileContentFrom(fileName);
        }
    }
}
