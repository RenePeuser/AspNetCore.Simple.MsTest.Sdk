using System;
using System.Runtime.CompilerServices;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// TYPE CHECK assertions
    /// </summary>
    public static partial class AssertThat
    {
        /// <summary>
        /// Asserts that the object is of the specified type (exact match).
        /// </summary>
        /// <typeparam name="TExpected">The expected type</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="obj">The object to check</param>
        /// <param name="because">Why this type check is important (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="objName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when object is not of expected type</exception>
        public static void IsOfType<TExpected>(this Assert _,
                                               object? obj,
                                               string because,
                                               string fix,
                                               [CallerArgumentExpression(nameof(obj))]
                                               string objName = "",
                                               [CallerFilePath] string callerFilePath = "",
                                               [CallerMemberName] string callerMemberName = "",
                                               [CallerLineNumber] int callerLineNumber = 0)
        {
            var expectedType = typeof(TExpected);

            if (obj is not null && obj.GetType() == expectedType)
            {
                return;
            }

            var output = BuildTypeAssertionOutput(assertionType: TypeAssertionType.IsOfType,
                                                  expectedType: expectedType,
                                                  actualType: obj?.GetType(),
                                                  objName: objName,
                                                  obj: obj,
                                                  because: because,
                                                  fix: fix,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the object is NOT of the specified type (exact match).
        /// </summary>
        /// <typeparam name="TNotExpected">The type that should not match</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="obj">The object to check</param>
        /// <param name="because">Why this type check is important (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="objName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when object is of the specified type</exception>
        public static void IsNotOfType<TNotExpected>(this Assert _,
                                                     object? obj,
                                                     string because,
                                                     string fix,
                                                     [CallerArgumentExpression(nameof(obj))]
                                                     string objName = "",
                                                     [CallerFilePath] string callerFilePath = "",
                                                     [CallerMemberName] string callerMemberName = "",
                                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var notExpectedType = typeof(TNotExpected);

            if (obj is null || obj.GetType() != notExpectedType)
            {
                return;
            }

            var output = BuildTypeAssertionOutput(assertionType: TypeAssertionType.IsNotOfType,
                                                  expectedType: notExpectedType,
                                                  actualType: obj.GetType(),
                                                  objName: objName,
                                                  obj: obj,
                                                  because: because,
                                                  fix: fix,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the object is assignable to the specified type (including inheritance and interface implementation).
        /// </summary>
        /// <typeparam name="TExpected">The expected base type or interface</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="obj">The object to check</param>
        /// <param name="because">Why this type check is important (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="objName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when object is not assignable to expected type</exception>
        public static void IsAssignableTo<TExpected>(this Assert _,
                                                     object? obj,
                                                     string because,
                                                     string fix,
                                                     [CallerArgumentExpression(nameof(obj))]
                                                     string objName = "",
                                                     [CallerFilePath] string callerFilePath = "",
                                                     [CallerMemberName] string callerMemberName = "",
                                                     [CallerLineNumber] int callerLineNumber = 0)
        {
            var expectedType = typeof(TExpected);

            if (obj is not null && expectedType.IsAssignableFrom(obj.GetType()))
            {
                return;
            }

            var output = BuildTypeAssertionOutput(assertionType: TypeAssertionType.IsAssignableTo,
                                                  expectedType: expectedType,
                                                  actualType: obj?.GetType(),
                                                  objName: objName,
                                                  obj: obj,
                                                  because: because,
                                                  fix: fix,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        /// <summary>
        /// Asserts that the object is NOT assignable to the specified type - neither that type nor
        /// anything deriving from or implementing it.
        /// A null object passes: null is not an instance of anything.
        /// </summary>
        /// <typeparam name="TNotExpected">The type the object must not be assignable to</typeparam>
        /// <param name="_">Extension point (use Assert.That)</param>
        /// <param name="obj">The object to check</param>
        /// <param name="because">Why this object must not be of that type (context)</param>
        /// <param name="fix">How to fix if assertion fails (guidance)</param>
        /// <param name="objName">Auto-captured variable name</param>
        /// <param name="callerFilePath">Auto-captured file path</param>
        /// <param name="callerMemberName">Auto-captured method name</param>
        /// <param name="callerLineNumber">Auto-captured line number</param>
        /// <exception cref="AssertFailedException">Thrown when object is assignable to the type</exception>
        public static void IsNotAssignableTo<TNotExpected>(this Assert _,
                                                           object? obj,
                                                           string because,
                                                           string fix,
                                                           [CallerArgumentExpression(nameof(obj))]
                                                           string objName = "",
                                                           [CallerFilePath] string callerFilePath = "",
                                                           [CallerMemberName] string callerMemberName = "",
                                                           [CallerLineNumber] int callerLineNumber = 0)
        {
            var notExpectedType = typeof(TNotExpected);

            if (obj is null || !notExpectedType.IsAssignableFrom(obj.GetType()))
            {
                return;
            }

            var output = BuildTypeAssertionOutput(assertionType: TypeAssertionType.IsNotAssignableTo,
                                                  expectedType: notExpectedType,
                                                  actualType: obj.GetType(),
                                                  objName: objName,
                                                  obj: obj,
                                                  because: because,
                                                  fix: fix,
                                                  callerFilePath: callerFilePath,
                                                  callerMemberName: callerMemberName,
                                                  callerLineNumber: callerLineNumber);

            throw new AssertFailedException(output);
        }

        // Private enum for type assertion variants
        private enum TypeAssertionType
        {
            IsOfType,

            IsNotOfType,

            IsAssignableTo,

            IsNotAssignableTo
        }

        // Private helper for building type assertion output
        private static string BuildTypeAssertionOutput(TypeAssertionType assertionType,
                                                       Type expectedType,
                                                       Type? actualType,
                                                       string objName,
                                                       object? obj,
                                                       string because,
                                                       string fix,
                                                       string callerFilePath,
                                                       string callerMemberName,
                                                       int callerLineNumber)
        {
            var textDecorator = TextDecoratorHelper.GetTextDecorator();
            var sb = new StringBuilder();

            // Header
            var title = assertionType switch
            {
                TypeAssertionType.IsOfType => "TYPE MISMATCH - EXPECTED EXACT TYPE",
                TypeAssertionType.IsNotOfType => "TYPE MISMATCH - EXPECTED DIFFERENT TYPE",
                TypeAssertionType.IsAssignableTo => "TYPE MISMATCH - EXPECTED ASSIGNABLE TYPE",
                TypeAssertionType.IsNotAssignableTo => "TYPE MISMATCH - EXPECTED NON-ASSIGNABLE TYPE",
                _ => "TYPE MISMATCH"
            };

            AssertOutputHelper.BuildHeader(sb, title, textDecorator);

            // Test Information
            AssertOutputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName,
                                                    callerLineNumber, textDecorator);

            // Problem
            var problem = assertionType switch
            {
                TypeAssertionType.IsOfType => obj is null
                                                  ? $"Expected object to be of type '{expectedType.Name}' but received null."
                                                  : $"Expected object to be exactly of type '{expectedType.Name}' but received '{actualType?.Name}'.",
                TypeAssertionType.IsNotOfType => $"Expected object to NOT be of type '{expectedType.Name}' but it was.",
                TypeAssertionType.IsAssignableTo => obj is null
                                                        ? $"Expected object to be assignable to type '{expectedType.Name}' but received null."
                                                        : $"Expected object to be assignable to type '{expectedType.Name}' but received '{actualType?.Name}'.",
                TypeAssertionType.IsNotAssignableTo => $"Expected object to NOT be assignable to type '{expectedType.Name}' but '{actualType?.Name}' is.",
                _ => "Type mismatch detected."
            };

            AssertOutputHelper.BuildProblemSection(sb, problem, textDecorator);

            // Details
            AssertOutputHelper.BuildDetailsSectionHeader(sb, textDecorator);
            sb.AppendLine($"{"Variable",-15} : {objName}");
            sb.AppendLine($"{"Expected Type",-15} : {expectedType.FullName ?? expectedType.Name}");
            sb.AppendLine($"{"Actual Type",-15} : {(actualType is not null ? (actualType.FullName ?? actualType.Name) : "null")}");

            if (assertionType is TypeAssertionType.IsAssignableTo or TypeAssertionType.IsNotAssignableTo && actualType is not null)
            {
                sb.AppendLine($"{"Assignable",-15} : {(expectedType.IsAssignableFrom(actualType) ? "Yes" : "No")}");
            }

            if (obj is not null)
            {
                var valueStr = obj.ToString();

                if (valueStr != actualType?.FullName && valueStr != actualType?.Name)
                {
                    sb.AppendLine($"{"Value",-15} : {valueStr}");
                }
            }

            sb.AppendLine();

            // Context (Why)
            AssertOutputHelper.BuildContextSection(sb, because, textDecorator);

            // Fix (How)
            var additionalOptions = assertionType switch
            {
                TypeAssertionType.IsOfType => new[] { $"Ensure '{objName}' is instantiated as '{expectedType.Name}' rather than '{actualType?.Name}'", $"Check the factory or constructor creating '{objName}'", $"Verify that '{objName}' is not being cast or converted to a different type" },
                TypeAssertionType.IsNotOfType => new[] { $"Change the type of '{objName}' to something other than '{expectedType.Name}'", $"Review the logic that creates '{objName}' to return a different type", $"Consider using a derived or different type for '{objName}'" },
                TypeAssertionType.IsAssignableTo => new[] { $"Ensure '{actualType?.Name ?? "the type"}' inherits from '{expectedType.Name}' or implements it as an interface", $"Check that '{objName}' is created with the correct derived type", $"Verify the class hierarchy and interface implementations for '{actualType?.Name ?? "the type"}'" },
                TypeAssertionType.IsNotAssignableTo => new[] { $"Check what puts a '{actualType?.Name}' into '{objName}' - it was expected to be anything but a '{expectedType.Name}'", $"Verify the registration or selection order that produced '{objName}'", $"If '{actualType?.Name}' is legitimate here, relax the assertion to the concrete type you want to exclude" },
                _ => Array.Empty<string>()
            };

            AssertOutputHelper.BuildFixSection(sb, fix, textDecorator,
                                               additionalOptions);

            // Footer
            AssertOutputHelper.BuildFooter(sb, textDecorator);

            return sb.ToString();
        }
    }
}