namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public interface IRequestTestCreator
    {
        string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfoUltra);
    }
}
