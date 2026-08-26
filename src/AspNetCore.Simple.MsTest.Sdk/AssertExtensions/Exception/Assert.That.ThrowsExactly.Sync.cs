using System;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// EXCEPTION assertions - ThrowsExactly (Synchronous)
    ///
    /// Unlike Throws these do not accept a derived exception type. Use them whenever the test is
    /// about the exact type - a derived type slipping through would silently widen what the test
    /// proves.
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the action throws an exception of exactly the specified type.
        /// A derived exception type fails the assertion.
        /// </summary>
        /// <typeparam name="TException">The expected exception type (exact, not derived)</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="action">The action to execute</param>
        /// <param name="because">Why this exception should be thrown (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actionName">Auto-captured action expression</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when action does not throw exactly that exception type</exception>
        /// <returns>The caught exception of type TException for further processing</returns>
        public static TException ThrowsExactly<TException>(this Assert _,
                                                           Action action,
                                                           string because,
                                                           string fix,
                                                           [CallerArgumentExpression(nameof(action))]
                                                           string actionName = "",
                                                           [CallerFilePath] string callerFilePath = "",
                                                           [CallerMemberName] string callerMemberName = "",
                                                           [CallerLineNumber] int callerLineNumber = 0) where TException : Exception
        {
            Exception? caughtException = null;

            try
            {
                action();
            }
#pragma warning disable CA1031 // Do not catch general exception types - we need to catch all exceptions to verify the type
            catch (Exception ex)
            {
                caughtException = ex;
            }
#pragma warning restore CA1031

            if (caughtException is TException typedException && caughtException.GetType() == typeof(TException))
            {
                return typedException;
            }

            var output = BuildThrowsOutput<TException>(actionName: actionName,
                                                       caughtException: caughtException,
                                                       expectedMessage: null,
                                                       because: because,
                                                       fix: fix,
                                                       callerFilePath: callerFilePath,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber,
                                                       exactType: true);

            throw new AssertFailedException(output);
        }
    }
}