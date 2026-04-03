using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Context object for ObjectsAreEqual assertion methods.
    /// Provides a cleaner API compared to methods with many individual parameters.
    /// Base context that can be extended for specialized assertion scenarios (e.g., HTTP assertions).
    /// </summary>
    /// <typeparam name="T">The type being compared</typeparam>
    public record ObjectAssertContext<T>
    {
        /// <summary>
        /// The expected object as JSON string or file name.
        /// Can be a JSON string, a file name like "expected.json", or an embedded resource path.
        /// </summary>
        public required string ExpectedObjectAsJson { get; init; }

        /// <summary>
        /// The current/actual object to compare against the expected object.
        /// </summary>
        public required T? CurrentObject { get; init; }

        /// <summary>
        /// Optional ordering/transformation function to apply before comparison.
        /// Useful for sorting collections or normalizing data.
        /// </summary>
        public Func<T?, T?> OrderFunc { get; init; } = item => item;

        /// <summary>
        /// Optional function to filter differences found during comparison.
        /// Allows ignoring specific differences that are expected.
        /// </summary>
        public Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; init; } = item => item;

        /// <summary>
        /// Parameters to replace in JSON strings during comparison.
        /// Format: (Key, Value) tuples where Key is the placeholder and Value is the replacement.
        /// </summary>
        public (string Key, object? Value)[] Parameters { get; init; } = [];

        /// <summary>
        /// The calling assembly. If not provided, will be automatically determined.
        /// </summary>
        public required Assembly CallingAssembly { get; init; }

        /// <summary>
        /// Whether to write the response to disk when the assertion fails.
        /// Useful for updating test snapshots.
        /// </summary>
        public bool WriteResponse { get; init; }

        /// <summary>
        /// Title/description for the assertion output.
        /// Used in error messages to provide context.
        /// </summary>
        public string? Title { get; init; }

        /// <summary>
        /// The file path of the calling test method. Usually auto-filled by CallerFilePath.
        /// </summary>
        public required string CallerFilePath { get; init; }

        /// <summary>
        /// The parameter name of the expected object. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        public required string ExpectedResultParameterName { get; init; }

        /// <summary>
        /// The parameter name of the current object. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        public required string CurrentResultParameterName { get; init; }
    }
}
