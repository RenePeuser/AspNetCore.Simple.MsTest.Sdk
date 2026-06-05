using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Provides assertion methods with cleaner output compared to standard Assert.Fail().
    /// Throws AssertFailedException directly without the "Assert.Fail failed." prefix.
    /// </summary>
    public static class AssertThat
    {
        /// <summary>
        /// Fails the test with the specified message.
        /// Unlike Assert.Fail(), this throws AssertFailedException directly without additional prefix.
        /// Output: "SNAPSHOT TEST FAILED..." instead of "Assert.Fail failed. SNAPSHOT TEST FAILED..."
        /// </summary>
        /// <param name="_">just placeholder to use it as extension</param>
        /// <param name="message">The error message to display</param>
        public static void Fail(this Assert _,
                                string message)
        {
            throw new AssertFailedException(message);
        }

        /// <summary>
        /// Fails the test with a beautifully formatted JSON type mismatch error (object to array).
        /// Uses the globally initialized JsonTypeMismatchOutputBuilder from HttpClientAssertExtensions.Setup().
        /// </summary>
        /// <param name="_">just placeholder to use it as extension</param>
        /// <param name="expectedResultParameterName">Name of the parameter that contains the expected result</param>
        /// <param name="targetTypeFullName">Full name of the target type</param>
        /// <param name="expectedJson">The expected JSON string</param>
        /// <param name="currentJson">The current/actual JSON string</param>
        /// <param name="sourceFilePath">Auto-captured: source file path</param>
        /// <param name="sourceLineNumber">Auto-captured: source line number</param>
        /// <param name="memberName">Auto-captured: calling member name</param>
        public static void FailWithObjectToArrayMismatch(this Assert _,
                                                         string expectedResultParameterName,
                                                         string targetTypeFullName,
                                                         string expectedJson,
                                                         string currentJson,
                                                         [CallerFilePath] string sourceFilePath = "",
                                                         [CallerLineNumber] int sourceLineNumber = 0,
                                                         [CallerMemberName] string memberName = "")
        {
            var message = HttpClientAssertExtensions.JsonTypeMismatchOutputBuilder.BuildObjectToArrayMismatch(
                expectedResultParameterName,
                targetTypeFullName,
                expectedJson,
                currentJson,
                sourceFilePath,
                sourceLineNumber,
                memberName);

            throw new AssertFailedException(message);
        }

        /// <summary>
        /// Fails the test with a beautifully formatted JSON type mismatch error (array to object).
        /// Uses the globally initialized JsonTypeMismatchOutputBuilder from HttpClientAssertExtensions.Setup().
        /// </summary>
        /// <param name="_">just placeholder to use it as extension</param>
        /// <param name="expectedResultParameterName">Name of the parameter that contains the expected result</param>
        /// <param name="targetTypeFullName">Full name of the target type</param>
        /// <param name="expectedJson">The expected JSON string</param>
        /// <param name="currentJson">The current/actual JSON string</param>
        /// <param name="sourceFilePath">Auto-captured: source file path</param>
        /// <param name="sourceLineNumber">Auto-captured: source line number</param>
        /// <param name="memberName">Auto-captured: calling member name</param>
        public static void FailWithArrayToObjectMismatch(this Assert _,
                                                         string expectedResultParameterName,
                                                         string targetTypeFullName,
                                                         string expectedJson,
                                                         string currentJson,
                                                         [CallerFilePath] string sourceFilePath = "",
                                                         [CallerLineNumber] int sourceLineNumber = 0,
                                                         [CallerMemberName] string memberName = "")
        {
            var message = HttpClientAssertExtensions.JsonTypeMismatchOutputBuilder.BuildArrayToObjectMismatch(
                expectedResultParameterName,
                targetTypeFullName,
                expectedJson,
                currentJson,
                sourceFilePath,
                sourceLineNumber,
                memberName);

            throw new AssertFailedException(message);
        }
    }
}
