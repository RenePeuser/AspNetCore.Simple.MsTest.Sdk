using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// STRING NULL/EMPTY/WHITESPACE assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the string is null or empty.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The string to check</param>
        /// <param name="because">Why this string should be null or empty (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when string is not null or empty</exception>
        public static void IsNullOrEmpty(this Assert _,
                                         string? value,
                                         string because,
                                         string fix,
                                         [CallerArgumentExpression(nameof(value))]
                                         string valueName = "",
                                         [CallerFilePath] string callerFilePath = "",
                                         [CallerMemberName] string callerMemberName = "",
                                         [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            var output = BuildStringNullEmptyOutput(checkType: StringNullCheckType.IsNullOrEmpty,
                                                    expectNullOrEmpty: true,
                                                    valueName: valueName,
                                                    actualValue: value,
                                                    because: because,
                                                    fix: fix,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber,
                                                    callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the string is NOT null or empty.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The string to check</param>
        /// <param name="because">Why this string must not be null or empty (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when string is null or empty</exception>
        public static void IsNotNullOrEmpty(this Assert _,
                                            string? value,
                                            string because,
                                            string fix,
                                            [CallerArgumentExpression(nameof(value))]
                                            string valueName = "",
                                            [CallerFilePath] string callerFilePath = "",
                                            [CallerMemberName] string callerMemberName = "",
                                            [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (!string.IsNullOrEmpty(value))
            {
                return;
            }

            var output = BuildStringNullEmptyOutput(checkType: StringNullCheckType.IsNullOrEmpty,
                                                    expectNullOrEmpty: false,
                                                    valueName: valueName,
                                                    actualValue: value,
                                                    because: because,
                                                    fix: fix,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber,
                                                    callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the string is null or whitespace.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The string to check</param>
        /// <param name="because">Why this string should be null or whitespace (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when string is not null or whitespace</exception>
        public static void IsNullOrWhiteSpace(this Assert _,
                                              string? value,
                                              string because,
                                              string fix,
                                              [CallerArgumentExpression(nameof(value))]
                                              string valueName = "",
                                              [CallerFilePath] string callerFilePath = "",
                                              [CallerMemberName] string callerMemberName = "",
                                              [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var output = BuildStringNullEmptyOutput(checkType: StringNullCheckType.IsNullOrWhiteSpace,
                                                    expectNullOrEmpty: true,
                                                    valueName: valueName,
                                                    actualValue: value,
                                                    because: because,
                                                    fix: fix,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber,
                                                    callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the string is NOT null or whitespace.
        /// </summary>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The string to check</param>
        /// <param name="because">Why this string must not be null or whitespace (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when string is null or whitespace</exception>
        public static void IsNotNullOrWhiteSpace(this Assert _,
                                                 string? value,
                                                 string because,
                                                 string fix,
                                                 [CallerArgumentExpression(nameof(value))]
                                                 string valueName = "",
                                                 [CallerFilePath] string callerFilePath = "",
                                                 [CallerMemberName] string callerMemberName = "",
                                                 [CallerLineNumber] int callerLineNumber = 0)
        {
            var callingAssembly = Assembly.GetCallingAssembly();

            if (!string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var output = BuildStringNullEmptyOutput(checkType: StringNullCheckType.IsNullOrWhiteSpace,
                                                    expectNullOrEmpty: false,
                                                    valueName: valueName,
                                                    actualValue: value,
                                                    because: because,
                                                    fix: fix,
                                                    callerFilePath: callerFilePath,
                                                    callerMemberName: callerMemberName,
                                                    callerLineNumber: callerLineNumber,
                                                    callingAssembly: callingAssembly);

            throw new AssertFailedException(output);
        }

        // Private enum for string null check types
        private enum StringNullCheckType
        {
            IsNullOrEmpty,

            IsNullOrWhiteSpace
        }

        // Private helper for building string null/empty assertion output
        private static string BuildStringNullEmptyOutput(StringNullCheckType checkType,
                                                         bool expectNullOrEmpty,
                                                         string valueName,
                                                         string? actualValue,
                                                         string because,
                                                         string fix,
                                                         string callerFilePath,
                                                         string callerMemberName,
                                                         int callerLineNumber,
                                                         Assembly callingAssembly)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator(callingAssembly);
            var sb = new StringBuilder();

            // Determine check type labels
            var checkTypeName = checkType == StringNullCheckType.IsNullOrEmpty
                                    ? "null or empty"
                                    : "null or whitespace";

            var checkTypeNameUpper = checkType == StringNullCheckType.IsNullOrEmpty
                                         ? "NULL OR EMPTY"
                                         : "NULL OR WHITESPACE";

            // Header
            var title = expectNullOrEmpty
                            ? $"STRING CHECK FAILED - EXPECTED {checkTypeNameUpper}"
                            : $"STRING CHECK FAILED - EXPECTED NON-{checkTypeNameUpper}";

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = expectNullOrEmpty
                              ? $"Expected string to be {checkTypeName} but it contains content."
                              : $"Expected string to have content but it was {checkTypeName}.";

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-10} : {valueName}");
            sb.AppendLine($"{"Type",-10} : string");

            // Format actual value with visual indicators
            string displayValue;

            if (actualValue is null)
            {
                displayValue = "null";
            }
            else if (actualValue.Length == 0)
            {
                displayValue = "\"\" (empty string)";
            }
            else if (string.IsNullOrWhiteSpace(actualValue))
            {
                displayValue = $"\"{actualValue}\" (whitespace only, length: {actualValue.Length})";
            }
            else
            {
                displayValue = actualValue.Length > 50
                                   ? $"\"{actualValue[..50]}...\" (length: {actualValue.Length})"
                                   : $"\"{actualValue}\" (length: {actualValue.Length})";
            }

            sb.AppendLine($"{"Value",-10} : {displayValue}");
            sb.AppendLine($"{"Expected",-10} : {(expectNullOrEmpty ? checkTypeNameUpper : $"Non-{checkTypeName} string with content")}");
            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            string[] additionalOptions;

            if (expectNullOrEmpty)
            {
                // Expected null/empty but got content
                additionalOptions = checkType == StringNullCheckType.IsNullOrEmpty
                                        ? new[] { $"Ensure the code that sets '{valueName}' returns null or empty string for this scenario", $"Review the logic that populates '{valueName}' - it may be receiving unexpected data", $"Check if '{valueName}' needs to be cleared or reset before this assertion" }
                                        : new[] { $"Ensure the code that sets '{valueName}' returns null or whitespace for this scenario", $"Review the logic that populates '{valueName}' - it may be receiving unexpected content", $"Check if '{valueName}' needs to be trimmed or cleared before this assertion" };
            }
            else
            {
                // Expected content but got null/empty
                if (actualValue is null)
                {
                    additionalOptions = new[]
                                        {
                                            $"Verify that '{valueName}' is properly initialized before this assertion", $"Check for null returns in methods that populate '{valueName}'", $"Add null checks or default values in the code under test",
                                            $"Review the data source for '{valueName}' - it may not be providing expected values"
                                        };
                }
                else if (actualValue.Length == 0)
                {
                    additionalOptions = new[]
                                        {
                                            $"Verify that '{valueName}' is populated with actual content", $"Check the data source for '{valueName}' - it may be returning empty strings", $"Review the logic that sets '{valueName}' to ensure it receives valid data",
                                            $"Add validation to prevent empty strings from being assigned to '{valueName}'"
                                        };
                }
                else
                {
                    // Whitespace only
                    additionalOptions = new[]
                                        {
                                            $"Verify that '{valueName}' contains actual content, not just whitespace", $"Check the data source for '{valueName}' - it may be returning whitespace-only strings", $"Add .Trim() validation to ensure '{valueName}' has meaningful content",
                                            $"Review input validation for '{valueName}' to reject whitespace-only values"
                                        };
                }
            }

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}