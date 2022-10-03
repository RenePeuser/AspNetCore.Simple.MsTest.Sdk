namespace AspNetCore.Simple.MsTest.Sdk
{
    internal interface ISpecificTestCreator
    {
        bool CanCreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo);

        string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo);
    }
}
