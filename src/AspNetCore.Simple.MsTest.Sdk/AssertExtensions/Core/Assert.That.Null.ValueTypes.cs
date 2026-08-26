using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// NULL CHECK assertions for nullable value types.
    ///
    /// The overloads in Assert.That.Null.cs are constrained to 'where T : class', so an int?,
    /// a DateTime? or any nullable struct would not bind. These carry the same output, so a test
    /// does not have to fall back to a classic assert just because the value happens to be a struct.
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the nullable value type has no value.
        /// </summary>
        /// <typeparam name="T">The underlying value type</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="because">Why this value should be null (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value has a value</exception>
        public static void IsNull<T>(this Assert _,
                                     T? value,
                                     string because,
                                     string fix,
                                     [CallerArgumentExpression(nameof(value))]
                                     string valueName = "",
                                     [CallerFilePath] string callerFilePath = "",
                                     [CallerMemberName] string callerMemberName = "",
                                     [CallerLineNumber] int callerLineNumber = 0) where T : struct
        {
            if (!value.HasValue)
            {
                return;
            }

            var output = BuildNullAssertionOutput(expectNull: true,
                                                  valueName: valueName,
                                                  actualValue: value.Value,
                                                  actualType: typeof(T),
                                                  because: because,
                                                  fix: fix,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the nullable value type has a value.
        /// </summary>
        /// <typeparam name="T">The underlying value type</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="value">The value to check</param>
        /// <param name="because">Why this value should have a value (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="valueName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when value has no value</exception>
        public static void IsNotNull<T>(this Assert _,
                                        [NotNull] T? value,
                                        string because,
                                        string fix,
                                        [CallerArgumentExpression(nameof(value))]
                                        string valueName = "",
                                        [CallerFilePath] string callerFilePath = "",
                                        [CallerMemberName] string callerMemberName = "",
                                        [CallerLineNumber] int callerLineNumber = 0) where T : struct
        {
            if (value.HasValue)
            {
                return;
            }

            var output = BuildNullAssertionOutput<T?>(expectNull: false,
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
    }
}