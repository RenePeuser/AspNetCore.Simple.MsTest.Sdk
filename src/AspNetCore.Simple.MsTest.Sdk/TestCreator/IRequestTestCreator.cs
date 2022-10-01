namespace AspNetCore.Simple.MsTest.Sdk
{
    public interface IRequestTestCreator
    {
        string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfoUltra);
    }
}
