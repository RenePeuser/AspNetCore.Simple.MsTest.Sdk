namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public interface ISpecificTestCreator
    {
        bool CanCreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo);

        string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo);
    }
}
