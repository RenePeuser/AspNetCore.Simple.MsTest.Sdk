using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// EXCEPTION assertions - Throws (Asynchronous)
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the async action throws an exception of the specified type.
        /// </summary>
        /// <typeparam name="TException">The expected exception type</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="action">The async action to execute</param>
        /// <param name="because">Why this exception should be thrown (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actionName">Auto-captured action expression</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when action does not throw expected exception</exception>
        /// <returns>The caught exception of type TException for further processing</returns>
        public static async Task<TException> ThrowsAsync<TException>(this Assert _,
                                                                     Func<Task> action,
                                                                     string because,
                                                                     string fix,
                                                                     [CallerArgumentExpression(nameof(action))]
                                                                     string actionName = "",
                                                                     [CallerFilePath] string callerFilePath = "",
                                                                     [CallerMemberName] string callerMemberName = "",
                                                                     [CallerLineNumber] int callerLineNumber = 0) where TException : Exception
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            Exception? caughtException = null;

            try
            {
                await action().ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types - we need to catch all exceptions to verify the type
            catch (Exception ex)
            {
                caughtException = ex;
            }
#pragma warning restore CA1031

            if (caughtException is TException typedException)
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
                                                       callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the async action throws an exception of the specified type with a specific message.
        /// </summary>
        /// <typeparam name="TException">The expected exception type</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="action">The async action to execute</param>
        /// <param name="expectedMessage">The expected exception message</param>
        /// <param name="because">Why this exception should be thrown (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actionName">Auto-captured action expression</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when action does not throw expected exception or message does not match</exception>
        /// <returns>The caught exception of type TException for further processing</returns>
        public static async Task<TException> ThrowsWithMessageAsync<TException>(this Assert _,
                                                                                Func<Task> action,
                                                                                string expectedMessage,
                                                                                string because,
                                                                                string fix,
                                                                                [CallerArgumentExpression(nameof(action))]
                                                                                string actionName = "",
                                                                                [CallerFilePath] string callerFilePath = "",
                                                                                [CallerMemberName] string callerMemberName = "",
                                                                                [CallerLineNumber] int callerLineNumber = 0) where TException : Exception
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            Exception? caughtException = null;

            try
            {
                await action().ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types - we need to catch all exceptions to verify the type
            catch (Exception ex)
            {
                caughtException = ex;
            }
#pragma warning restore CA1031

            if (caughtException is TException typedException && caughtException.Message == expectedMessage)
            {
                return typedException;
            }

            var output = BuildThrowsOutput<TException>(actionName: actionName,
                                                       caughtException: caughtException,
                                                       expectedMessage: expectedMessage,
                                                       because: because,
                                                       fix: fix,
                                                       callerFilePath: callerFilePath,
                                                       callerMemberName: callerMemberName,
                                                       callerLineNumber: callerLineNumber,
                                                       callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }
    }
}