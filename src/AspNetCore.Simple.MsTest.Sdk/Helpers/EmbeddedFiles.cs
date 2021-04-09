using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk.Helpers
{
    public static class EmbeddedFile
    {
        public static string GetFileContentFrom(string fileName)
        {
            return Assembly.GetCallingAssembly().GetJsonFileContentFrom(fileName);
        }
    }
}
