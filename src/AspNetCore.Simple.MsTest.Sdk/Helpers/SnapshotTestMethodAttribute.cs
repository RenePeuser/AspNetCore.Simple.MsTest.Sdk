using System.Runtime.CompilerServices;

namespace AspNetCore.Simple.MsTest.Sdk
{
#pragma warning disable CA1019 // Define accessors for attribute arguments
#pragma warning disable MSTEST0057 // TestMethodAttribute derived class should propagate source information
    public sealed class SnapshotTestMethodAttribute(int maxRetries,
#pragma warning restore MSTEST0057 // TestMethodAttribute derived class should propagate source information
                                                    [CallerFilePath] string callerFilePath = "",
                                                    [CallerLineNumber] int callerLineNumber = -1) : TestMethodAttribute(callerFilePath, callerLineNumber)
    {
        private static readonly WriteResponseService WriteResponseService = new WriteResponseService();

        public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
        {
            var callingAssembly = testMethod.MethodInfo.DeclaringType!.Assembly;

            var retries = WriteResponseService.ShouldWriteResponse(false, callingAssembly)
                              ? maxRetries
                              : 1;

            TestResult[]? lastResult = null;

            for (var attempt = 1; attempt <= retries; attempt++)
            {
                lastResult = await base.ExecuteAsync(testMethod).ConfigureAwait(false);

                if (lastResult.Length > 0 && lastResult[0].Outcome == UnitTestOutcome.Passed)
                {
                    return lastResult;
                }
            }

            return lastResult!;
        }
    }
#pragma warning restore CA1019 // Define accessors for attribute arguments
}