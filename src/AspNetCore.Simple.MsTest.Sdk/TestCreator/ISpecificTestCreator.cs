namespace AspNetCore.Simple.MsTest.Sdk
{
    public interface ISpecificTestCreator
    {
        bool CanCreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo);

        string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo);
    }
}
