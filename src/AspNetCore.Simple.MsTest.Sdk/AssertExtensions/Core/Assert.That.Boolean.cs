using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// BOOLEAN assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the condition is true.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="condition">The condition to check</param>
        /// <param name="because">Why this condition should be true (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="conditionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when condition is false</exception>
        public static void IsTrue(this Assert _,
                                  bool condition,
                                  string because,
                                  string fix,
                                  [CallerArgumentExpression(nameof(condition))]
                                  string conditionName = "",
                                  [CallerFilePath] string callerFilePath = "",
                                  [CallerMemberName] string callerMemberName = "",
                                  [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (condition)
            {
                return;
            }

            var output = BuildBooleanOutput(expectTrue: true,
                                            conditionName: conditionName,
                                            because: because,
                                            fix: fix,
                                            callerFilePath: callerFilePath,
                                            callerMemberName: callerMemberName,
                                            callerLineNumber: callerLineNumber,
                                            callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the condition is false.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="condition">The condition to check</param>
        /// <param name="because">Why this condition should be false (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="conditionName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when condition is true</exception>
        public static void IsFalse(this Assert _,
                                   bool condition,
                                   string because,
                                   string fix,
                                   [CallerArgumentExpression(nameof(condition))]
                                   string conditionName = "",
                                   [CallerFilePath] string callerFilePath = "",
                                   [CallerMemberName] string callerMemberName = "",
                                   [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (!condition)
            {
                return;
            }

            var output = BuildBooleanOutput(expectTrue: false,
                                            conditionName: conditionName,
                                            because: because,
                                            fix: fix,
                                            callerFilePath: callerFilePath,
                                            callerMemberName: callerMemberName,
                                            callerLineNumber: callerLineNumber,
                                            callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        // Private helper for building boolean assertion output
        private static string BuildBooleanOutput(bool expectTrue,
                                                 string conditionName,
                                                 string because,
                                                 string fix,
                                                 string callerFilePath,
                                                 string callerMemberName,
                                                 int callerLineNumber,
                                                 Assembly callingAssembly)
        {
            var assertOutputHelper = HttpClientAssertExtensions.GetService<IAssertOutputHelper>(callingAssembly);
            var sb = new StringBuilder();

            // Header
            var title = expectTrue
                            ? "CONDITION FAILED - EXPECTED TRUE"
                            : "CONDITION FAILED - EXPECTED FALSE";

            assertOutputHelper.BuildHeader(sb, title);

            // Test Information
            assertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber);

            // Problem
            var problem = expectTrue
                              ? "Expected condition to be TRUE but it was FALSE."
                              : "Expected condition to be FALSE but it was TRUE.";

            assertOutputHelper.BuildProblemSection(sb, problem);

            // Details
            assertOutputHelper.BuildDetailsSectionHeader(sb);
            sb.AppendLine($"{"Condition",-10} : {conditionName}");
            sb.AppendLine($"{"Result",-10} : {!expectTrue}");
            sb.AppendLine($"{"Expected",-10} : {expectTrue}");
            sb.AppendLine();

            // Context (Why)
            assertOutputHelper.BuildContextSection(sb, because);

            // Fix (How)
            var additionalOptions = expectTrue
                                        ? new[] { $"Review the logic in '{conditionName}' to ensure it returns true", "Check the values being compared in the condition", "Verify that prerequisites for this condition are met" }
                                        : new[] { $"Review the logic in '{conditionName}' to ensure it returns false", "Check if the condition should be inverted", "Verify the expected state for this test scenario" };

            assertOutputHelper.BuildFixSection(sb, fix,
                                               additionalOptions);

            // Footer
            assertOutputHelper.BuildFooter(sb);

            return sb.ToString();
        }
    }
}