namespace AspNetCore.Simple.MsTest.Sdk
{
    internal sealed record RequestInfo(string HttpMethod, string RelativePath, string AbsolutePath, string Body);
}
