using System.Reflection;
using System.Runtime.CompilerServices;

namespace AspNetCore.Simple.MsTest.Sdk
{
#pragma warning disable CA1019 // Define accessors for attribute arguments
    public sealed class SnapshotTestMethodAttribute : TestMethodAttribute
    {
        private readonly int _maxRetries;
        private readonly Assembly _callingAssembly;

        public SnapshotTestMethodAttribute(int maxRetries,
                                           [CallerFilePath] string callerFilePath = "",
                                           [CallerLineNumber] int callerLineNumber = -1)
            : this(maxRetries, Assembly.GetExecutingAssembly(), callerFilePath, callerLineNumber)
        {
        }

        private SnapshotTestMethodAttribute(int maxRetries,
                                            Assembly callingAssembly,
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerLineNumber] int callerLineNumber = -1)
            : base(callerFilePath, callerLineNumber)
        {
            _maxRetries = maxRetries;
            _callingAssembly = callingAssembly;
        }

        private readonly WriteResponseService _writeResponseService = new WriteResponseService();

        public override async Task<TestResult[]> ExecuteAsync(ITestMethod testMethod)
        {
            var retries = _writeResponseService.ShouldWriteResponse(false, _callingAssembly)
                              ? _maxRetries
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
