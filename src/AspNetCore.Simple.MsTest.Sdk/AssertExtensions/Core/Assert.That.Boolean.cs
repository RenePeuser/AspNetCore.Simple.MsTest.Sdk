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
                                            callerLineNumber: callerLineNumber);

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
                                            callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private helper for building boolean assertion output
        private static string BuildBooleanOutput(bool expectTrue,
                                                 string conditionName,
                                                 string because,
                                                 string fix,
                                                 string callerFilePath,
                                                 string callerMemberName,
                                                 int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = expectTrue
                            ? "CONDITION FAILED - EXPECTED TRUE"
                            : "CONDITION FAILED - EXPECTED FALSE";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = expectTrue
                              ? "Expected condition to be TRUE but it was FALSE."
                              : "Expected condition to be FALSE but it was TRUE.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Condition",-10} : {conditionName}");
            sb.AppendLine($"{"Result",-10} : {!expectTrue}");
            sb.AppendLine($"{"Expected",-10} : {expectTrue}");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = expectTrue
                                        ? new[] { $"Review the logic in '{conditionName}' to ensure it returns true", "Check the values being compared in the condition", "Verify that prerequisites for this condition are met" }
                                        : new[] { $"Review the logic in '{conditionName}' to ensure it returns false", "Check if the condition should be inverted", "Verify the expected state for this test scenario" };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}