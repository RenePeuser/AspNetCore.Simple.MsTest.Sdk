using System.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// EXCEPTION assertions - Does Not Throw (Asynchronous)
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the async action does not throw any exception.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="action">The async action to execute</param>
        /// <param name="because">Why this action should not throw (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actionName">Auto-captured action expression</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when action throws an exception</exception>
        public static async Task DoesNotThrowAsync(this Assert _,
                                                   Func<Task> action,
                                                   string because,
                                                   string fix,
                                                   [CallerArgumentExpression(nameof(action))]
                                                   string actionName = "",
                                                   [CallerFilePath] string callerFilePath = "",
                                                   [CallerMemberName] string callerMemberName = "",
                                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Action cannot be null");
            }

            try
            {
                await action().ConfigureAwait(false);

                return; // Success - no exception thrown
            }
            catch (Exception ex)
            {
                var output = BuildDoesNotThrowOutput(actionName: actionName,
                                                     thrownException: ex,
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
}