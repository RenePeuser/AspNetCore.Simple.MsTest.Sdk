using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// NULL CHECK assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the value is null.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="because">Why this value should be null (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is not null</exception>
        public static void IsNull<T>(this Assert _,
                                     T? value,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(value))]
                                     string valueName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0) where T : class
        {
            if (value is null)
            {
                return;
            }

            var output = BuildNullAssertionOutput(expectNull: true,
                                                  valueName: valueName,
                                                  actualValue: value,
                                                  actualType: typeof(T),
                                                  because: because,
                                                  fix: fix,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the value is NOT null.
        /// </summary>
        /// <typeparam name="T">The type of the value</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="because">Why this value must not be null (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value is null</exception>
        public static void IsNotNull<T>(this Assert _,
                                        [NotNull] T? value,
                                        string because,
                                        string fix,
                                        [CallerArgumentExpression(nameof(value))]
                                        string valueName = "",
                                        [CallerFilePath] string callerFilePath = "",
                                        [CallerMemberName] string callerMemberName = "",
                                        [CallerLineNumber] int callerLineNumber = 0) where T : class
        {
            if (value is not null)
            {
                return;
            }

            var output = BuildNullAssertionOutput<T>(expectNull: false,
                                                     valueName: valueName,
                                                     actualValue: null,
                                                     actualType: typeof(T),
                                                     because: because,
                                                     fix: fix,
                                                     callerFilePath: callerFilePath,
                                                     callerMemberName: callerMemberName,
                                                     callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private helper for building null assertion output
        private static string BuildNullAssertionOutput<T>(bool expectNull,
                                                          string valueName,
                                                          T? actualValue,
                                                          System.Type actualType,
                                                          string because,
                                                          string fix,
                                                          string callerFilePath,
                                                          string callerMemberName,
                                                          int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = expectNull
                            ? "NULL REFERENCE - EXPECTED NULL"
                            : "NULL REFERENCE - EXPECTED NON-NULL";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = expectNull
                              ? "Expected value to be null but received a non-null object."
                              : "Expected value to be non-null but received null.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {valueName}");
            sb.AppendLine($"{"Type",-10} : {actualType.Name}");
            sb.AppendLine($"{"Value",-10} : {(actualValue is null ? "null" : actualValue.ToString() ?? "(no ToString)")}");
            sb.AppendLine($"{"Expected",-10} : {(expectNull ? "null" : "Non-null object")}");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = expectNull
                                        ? new[] { $"Ensure the code that sets '{valueName}' returns null for this scenario", $"Review the logic that creates or assigns '{valueName}'" }
                                        : new[] { $"Verify that '{valueName}' is properly initialized before this assertion", $"Check for null returns in methods that populate '{valueName}'", "Add null checks or default values in the code under test" };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}