namespace AspNetCore.Simple.MsTest.Sdk
{
    internal record RequestInfo(string HttpMethod, string RelativePath, string AbsolutePath, string Body);
}
