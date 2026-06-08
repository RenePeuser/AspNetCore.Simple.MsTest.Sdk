using System;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// EXCEPTION assertions - Does Not Throw (Synchronous)
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the action does not throw any exception.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="action">The action to execute</param>
        /// <param name="because">Why this action should not throw (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actionName">Auto-captured action expression</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when action throws an exception</exception>
        public static void DoesNotThrow(this Assert _,
                                        Action action,
                                        string because,
                                        string fix,
                                        [CallerArgumentExpression(nameof(action))]
                                        string actionName = "",
                                        [CallerFilePath] string callerFilePath = "",
                                        [CallerMemberName] string callerMemberName = "",
                                        [CallerLineNumber] int callerLineNumber = 0
        )
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action), "Action cannot be null");
            }

            try
            {
                action();

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
                                                     callerLineNumber: callerLineNumber);

                throw new AssertFailedException(output);
            }
        }

        // Private helper for building DoesNotThrow assertion output
        private static string BuildDoesNotThrowOutput(
            string actionName,
            Exception thrownException,
            string because,
            string fix,
            string callerFilePath,
            string callerMemberName,
            int callerLineNumber
        )
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            AssertOutputHelper.BuildHeader(sb, "EXCEPTION THROWN - EXPECTED NO EXCEPTION", textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            AssertOutputHelper.BuildProblemSection(sb,
                                                   "Expected action to execute without throwing an exception, but an exception was thrown.",
                                                   textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Action",-15} : {actionName}");
            sb.AppendLine($"{"Exception Type",-15} : {thrownException.GetType().Name}");
            sb.AppendLine($"{"Message",-15} : {thrownException.Message}");

            if (thrownException.InnerException != null)
            {
                sb.AppendLine($"{"Inner Exception",-15} : {thrownException.InnerException.GetType().Name}");
                sb.AppendLine($"{"Inner Message",-15} : {thrownException.InnerException.Message}");
            }

            if (!string.IsNullOrEmpty(thrownException.StackTrace))
            {
                sb.AppendLine();
                sb.AppendLine("Stack Trace:");

                // Limit stack trace to first 5 lines for readability
#pragma warning disable CA1861 // Prefer static readonly fields
                var stackLines = thrownException.StackTrace.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
#pragma warning restore CA1861
                var lineCount = Math.Min(stackLines.Length, 5);

                for (var i = 0; i < lineCount; i++)
                {
                    sb.AppendLine($"  {stackLines[i].Trim()}");
                }

                if (stackLines.Length > 5)
                {
                    sb.AppendLine($"  ... ({stackLines.Length - 5} more lines)");
                }
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = new[]
                                    {
                                        $"Investigate the root cause of '{thrownException.GetType().Name}' in the action", $"Add proper error handling or validation before executing '{actionName}'", "Review the stack trace above to identify the failing code path",
                                        "Consider if this exception indicates a bug in the code under test", thrownException.InnerException != null
                                                                                                                 ? $"Check the inner exception: {thrownException.InnerException.GetType().Name}"
                                                                                                                 : "Add defensive checks to prevent this exception condition"
                                    };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}
