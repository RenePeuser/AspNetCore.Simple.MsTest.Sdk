using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// EXCEPTION assertions - Throws (Synchronous)
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the action throws an exception of the specified type.
        /// </summary>
        /// <typeparam name="TException">The expected exception type</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="action">The action to execute</param>
        /// <param name="because">Why this exception should be thrown (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actionName">Auto-captured action expression</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when action does not throw expected exception</exception>
        /// <returns>The caught exception of type TException for further processing</returns>
        public static TException Throws<TException>(this Assert _,
                                                    Action action,
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
                action();
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
        /// Asserts that the action throws an exception of the specified type with a specific message.
        /// </summary>
        /// <typeparam name="TException">The expected exception type</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="action">The action to execute</param>
        /// <param name="expectedMessage">The expected exception message</param>
        /// <param name="because">Why this exception should be thrown (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="actionName">Auto-captured action expression</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when action does not throw expected exception or message does not match</exception>
        /// <returns>The caught exception of type TException for further processing</returns>
        public static TException ThrowsWithMessage<TException>(this Assert _,
                                                               Action action,
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
                action();
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

        // Private helper for building throws assertion output
        private static string BuildThrowsOutput<TException>(string actionName,
                                                            Exception? caughtException,
                                                            string? expectedMessage,
                                                            string because,
                                                            string fix,
                                                            string callerFilePath,
                                                            string callerMemberName,
                                                            int callerLineNumber,
                                                            Assembly callingAssembly,
                                                            bool exactType = false) where TException : Exception
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator(callingAssembly);
            var sb = new StringBuilder();

            // A derived type only matters when the assertion demanded the exact one - for Throws it
            // is a pass, so it can never reach this output.
            var caughtDerivedType = exactType && caughtException is TException;

            // Header
            var title = expectedMessage != null
                            ? "EXCEPTION ASSERTION - TYPE AND MESSAGE MISMATCH"
                            : exactType
                                ? "EXCEPTION ASSERTION - EXACT TYPE MISMATCH"
                                : "EXCEPTION ASSERTION - TYPE MISMATCH";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            string problem;

            if (caughtException == null)
            {
                problem = $"Expected action to throw {typeof(TException).Name} but no exception was thrown.";
            }
            else if (caughtDerivedType)
            {
                problem = $"Expected action to throw exactly {typeof(TException).Name} but caught {caughtException.GetType().Name}, "
                          + $"which derives from it. ThrowsExactly does not accept a derived type.";
            }
            else if (caughtException is not TException)
            {
                problem = $"Expected action to throw {typeof(TException).Name} but caught {caughtException.GetType().Name} instead.";
            }
            else if (expectedMessage != null && caughtException.Message != expectedMessage)
            {
                problem = $"Expected exception message to match exactly but it differs.";
            }
            else
            {
                problem = "Unexpected exception assertion failure.";
            }

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Action",-15} : {actionName}");
            sb.AppendLine($"{"Expected Type",-15} : {typeof(TException).Name}");
            sb.AppendLine($"{"Match Mode",-15} : {(exactType ? "exact type only" : "type or any derived type")}");

            if (expectedMessage != null)
            {
                sb.AppendLine($"{"Expected Msg",-15} : {expectedMessage}");
            }

            if (caughtException != null)
            {
                sb.AppendLine($"{"Actual Type",-15} : {caughtException.GetType().Name}");
                sb.AppendLine($"{"Actual Message",-15} : {caughtException.Message}");
                sb.AppendLine($"{"Stack Trace",-15} : {caughtException.StackTrace ?? "(no stack trace)"}");
            }
            else
            {
                sb.AppendLine($"{"Actual Type",-15} : (no exception thrown)");
                sb.AppendLine($"{"Actual Message",-15} : N/A");
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            string[] additionalOptions;

            if (caughtException == null)
            {
                additionalOptions = new[] { $"Ensure the code in '{actionName}' throws {typeof(TException).Name}", $"Check if the exception is being caught and suppressed before this assertion", $"Verify that the conditions for throwing {typeof(TException).Name} are met" };
            }
            else if (caughtDerivedType)
            {
                additionalOptions = new[] { $"Expect the concrete type instead: ThrowsExactly<{caughtException.GetType().Name}>(...)", $"Switch to Throws<{typeof(TException).Name}>(...) if a derived type is acceptable here", $"Stop the code in '{actionName}' from throwing the more specific {caughtException.GetType().Name}" };
            }
            else if (caughtException is not TException)
            {
                additionalOptions = new[] { $"Change the expected exception type from {typeof(TException).Name} to {caughtException.GetType().Name}", $"Update the code in '{actionName}' to throw {typeof(TException).Name} instead of {caughtException.GetType().Name}", $"Check if {caughtException.GetType().Name} is wrapped or needs to be unwrapped" };
            }
            else if (expectedMessage != null)
            {
                additionalOptions = new[] { $"Update the expected message to match: \"{caughtException.Message}\"", $"Update the exception message in the code under test to: \"{expectedMessage}\"", "Consider using Contains assertion if exact message match is too strict" };
            }
            else
            {
                additionalOptions = new[] { $"Review the exception handling logic in '{actionName}'", "Check for unexpected exception types or missing throws" };
            }

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}