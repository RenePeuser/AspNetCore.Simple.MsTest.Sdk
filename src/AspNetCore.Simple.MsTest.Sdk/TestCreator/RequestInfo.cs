namespace AspNetCore.Simple.MsTest.Sdk
{
    public record RequestInfo(string HttpMethod, string RelativePath, string AbsolutePath, string Body);
}
