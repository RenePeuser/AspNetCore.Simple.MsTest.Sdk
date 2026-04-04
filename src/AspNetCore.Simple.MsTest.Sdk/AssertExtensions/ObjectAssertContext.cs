using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public interface IObjectAssertContext
    {
        /// <summary>
        /// The expected object as JSON string or file name.
        /// Can be a JSON string, a file name like "expected.json", or an embedded resource path.
        /// </summary>
        string ExpectedObjectAsJson { get; init; }

        /// <summary>
        /// The localized expected result file info.
        /// Contains the resolved embedded file information including content and physical file location.
        /// This is resolved once during context creation and reused throughout the assertion pipeline.
        /// </summary>
        EmbeddedFileInfo? ExpectedResultFile { get; init; }

        /// <summary>
        /// The current/actual object to compare against the expected object (untyped).
        /// Use the strongly-typed CurrentObject property in derived generic classes when possible.
        /// This property is set automatically by the derived generic class.
        /// </summary>
        object? CurrentObject { get; init; }

        /// <summary>
        /// Optional function to filter differences found during comparison.
        /// Allows ignoring specific differences that are expected.
        /// </summary>
        Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; init; }

        /// <summary>
        /// Parameters to replace in JSON strings during comparison.
        /// Format: (Key, Value) tuples where Key is the placeholder and Value is the replacement.
        /// </summary>
        (string Key, object? Value)[] Parameters { get; init; }

        /// <summary>
        /// The calling assembly. If not provided, will be automatically determined.
        /// </summary>
        Assembly CallingAssembly { get; init; }

        /// <summary>
        /// Whether to write the response to disk when the assertion fails.
        /// Useful for updating test snapshots.
        /// </summary>
        bool WriteResponse { get; init; }

        /// <summary>
        /// Title/description for the assertion output.
        /// Used in error messages to provide context.
        /// </summary>
        string? Title { get; init; }

        /// <summary>
        /// The file path of the calling test method. Usually auto-filled by CallerFilePath.
        /// </summary>
        string CallerFilePath { get; init; }

        /// <summary>
        /// The parameter name of the expected object. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        string ExpectedResultParameterName { get; init; }

        /// <summary>
        /// The parameter name of the current object. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        string CurrentResultParameterName { get; init; }
    }

    /// <summary>
    /// Non-generic base context for object assertions.
    /// Contains all properties that don't depend on the object type.
    /// Enables polymorphism and type-safe handling of assertion contexts.
    /// </summary>
    public abstract record ObjectAssertContext : IObjectAssertContext
    {
        /// <summary>
        /// The expected object as JSON string or file name.
        /// Can be a JSON string, a file name like "expected.json", or an embedded resource path.
        /// </summary>
        public required string ExpectedObjectAsJson { get; init; }

        /// <summary>
        /// The localized expected result file info.
        /// Contains the resolved embedded file information including content and physical file location.
        /// This is resolved once during context creation and reused throughout the assertion pipeline.
        /// </summary>
        public EmbeddedFileInfo? ExpectedResultFile { get; init; }

        /// <summary>
        /// The current/actual object to compare against the expected object (untyped).
        /// Use the strongly-typed CurrentObject property in derived generic classes when possible.
        /// This property is set automatically by the derived generic class.
        /// </summary>
        public object? CurrentObject { get; init; }

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

    /// <summary>
    /// Generic context object for ObjectsAreEqual assertion methods.
    /// Provides a cleaner API compared to methods with many individual parameters.
    /// Can be extended for specialized assertion scenarios (e.g., HTTP assertions).
    /// </summary>
    /// <typeparam name="T">The type being compared</typeparam>
    public record ObjectAssertContext<T> : ObjectAssertContext
    {
        /// <summary>
        /// The current/actual object to compare against the expected object (strongly-typed).
        /// When set, this also sets the base CurrentObject property for polymorphic access.
        /// </summary>
        public required T? Current
        {
            get => (T?)CurrentObject;

            init => CurrentObject = value;
        }

        /// <summary>
        /// Optional ordering/transformation function to apply before comparison.
        /// Useful for sorting collections or normalizing data.
        /// Note: Function must handle nullable inputs/outputs.
        /// </summary>
        public Func<T?, T?> OrderFunc { get; init; } = item => item;
    }
}
