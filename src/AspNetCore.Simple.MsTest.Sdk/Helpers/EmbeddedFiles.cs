namespace AspNetCore.Simple.MsTest.Sdk.Helpers
{
    public static class EmbeddedFile
    {
        public static string GetFileContentFrom(string fileName)
        {
            return typeof(EmbeddedFile).Assembly.GetJsonFileContentFrom(fileName);
        }
    }
}
