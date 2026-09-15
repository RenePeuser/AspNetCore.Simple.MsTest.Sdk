using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public interface IObjectAssertContext
    {
        /// <summary>
        /// The expected type of the result/response.
        /// Defaults to the generic type parameter TResult, but can be overridden.
        /// Use typeof(void) for operations that return no content (e.g., 204 NoContent, DELETE with no response body).
        /// </summary>
        Type ExpectedType { get; init; }

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
        EmbeddedFileInfo ExpectedResultFile { get; init; }

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
        /// Optional per-difference predicate. Return <c>true</c> to keep a difference,
        /// <c>false</c> to ignore it. The SDK iterates internally, so callers only
        /// describe the condition instead of writing loops.
        /// Applied in addition to (and after) <see cref="DifferenceFunc"/>.
        /// </summary>
        Predicate<Difference> DifferenceFilter { get; init; }

        /// <summary>
        /// Optional per-assert override for
        /// <see cref="AssertObjectExtensions.OrderIndependentArrayFilter"/>. Null falls back to
        /// that global filter.
        /// </summary>
        Predicate<JsonArrayContext>? OrderIndependentArrayFilter { get; init; }

        /// <summary>
        /// Parameters to replace in JSON strings during comparison.
        /// Format: (Key, Value) tuples where Key is the placeholder and Value is the replacement.
        /// </summary>
#pragma warning disable CA1819
        (string Key, object? Value)[] Parameters { get; init; }
#pragma warning restore CA1819

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
        /// The file path of the calling test method. Usually auto-filled by CallerFilePath.
        /// </summary>
        string CallerFilePath { get; init; }

        /// <summary>
        /// The name of the calling test method. Usually auto-filled by CallerMemberName.
        /// </summary>
        string CallerMemberName { get; init; }

        /// <summary>
        /// The line number in the source file where the assertion was called.
        /// Captured via CallerLineNumberAttribute for debugging purposes.
        /// </summary>
        int CallerLineNumber { get; init; }

        /// <summary>
        /// The parameter name of the expected object. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        string ExpectedResultParameterName { get; init; }

        /// <summary>
        /// The parameter name of the current object. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        string CurrentResultParameterName { get; init; }

        /// <summary>
        /// The expected JSON content with all preprocessing applied (parameter replacement, etc.).
        /// This is the ready-to-use JSON that can be directly deserialized or compared.
        /// All data preparation happens before context creation - the service receives only processed data.
        /// </summary>
        string? ResolvedExpectedJson { get; init; }

        /// <summary>
        /// Indicates whether the expected object is an empty anonymous object that should trigger C# code generation.
        /// When true, the framework will generate C# code with actual response properties in the test file.
        /// </summary>
        bool IsEmptyAnonymousObjectForCodeGeneration { get; init; }
    }

    /// <summary>
    /// Non-generic base context for object assertions.
    /// Contains all properties that don't depend on the object type.
    /// Enables polymorphism and type-safe handling of assertion contexts.
    /// </summary>
    public abstract record ObjectAssertContext : IObjectAssertContext
    {
        /// <summary>
        /// The expected type of the result/response.
        /// Defaults to the generic type parameter TResult, but can be overridden.
        /// Use typeof(void) for operations that return no content (e.g., 204 NoContent, DELETE with no response body).
        /// </summary>
        public required Type ExpectedType { get; init; }

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
        public required EmbeddedFileInfo ExpectedResultFile { get; init; }

        /// <summary>
        /// The current/actual object to compare against the expected object (untyped).
        /// Use the strongly-typed CurrentObject property in derived generic classes when possible.
        /// This property is set automatically by the derived generic class.
        /// </summary>
        public required object? CurrentObject { get; init; }

        /// <summary>
        /// Optional function to filter differences found during comparison.
        /// Allows ignoring specific differences that are expected.
        /// </summary>
        public required Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; init; } = item => item;

        /// <summary>
        /// Optional per-difference predicate. Return <c>true</c> to keep a difference,
        /// <c>false</c> to ignore it. The SDK iterates internally, so callers only
        /// describe the condition instead of writing loops.
        /// Applied in addition to (and after) <see cref="DifferenceFunc"/>.
        /// Defaults to keeping every difference.
        /// </summary>
        public Predicate<Difference> DifferenceFilter { get; init; } = static _ => true;

        /// <summary>
        /// Optional per-assert override for
        /// <see cref="AssertObjectExtensions.OrderIndependentArrayFilter"/>. Null falls back to
        /// that global filter, so a test only sets this when it needs different array semantics
        /// than the rest of the suite.
        /// </summary>
        public Predicate<JsonArrayContext>? OrderIndependentArrayFilter { get; init; }

        /// <summary>
        /// Parameters to replace in JSON strings during comparison.
        /// Format: (Key, Value) tuples where Key is the placeholder and Value is the replacement.
        /// </summary>
#pragma warning disable CA1819
        public required (string Key, object? Value)[] Parameters { get; init; } = [];
#pragma warning restore CA1819

        /// <summary>
        /// The calling assembly. If not provided, will be automatically determined.
        /// </summary>
        public required Assembly CallingAssembly { get; init; }

        /// <summary>
        /// Whether to write the response to disk when the assertion fails.
        /// Useful for updating test snapshots.
        /// </summary>
        public required bool WriteResponse { get; init; }

        /// <summary>
        /// The file path of the calling test method. Usually auto-filled by CallerFilePath.
        /// </summary>
        public required string CallerFilePath { get; init; }

        /// <summary>
        /// The name of the calling test method. Usually auto-filled by CallerMemberName.
        /// </summary>
        public required string CallerMemberName { get; init; }

        /// <summary>
        /// The line number in the source file where the assertion was called.
        /// Captured via CallerLineNumberAttribute for debugging purposes.
        /// </summary>
        public required int CallerLineNumber { get; init; }

        /// <summary>
        /// The parameter name of the expected object. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        public required string ExpectedResultParameterName { get; init; }

        /// <summary>
        /// The parameter name of the current object. Usually auto-filled by CallerArgumentExpression.
        /// </summary>
        public required string CurrentResultParameterName { get; init; }

        /// <summary>
        /// The expected JSON content with all preprocessing applied (parameter replacement, etc.).
        /// This is the ready-to-use JSON that can be directly deserialized or compared.
        /// All data preparation happens before context creation - the service receives only processed data.
        /// </summary>
        public required string? ResolvedExpectedJson { get; init; }

        public required bool TypeIsPrimitiveType { get; init; }

        /// <summary>
        /// Indicates whether the expected object is an empty anonymous object that should trigger C# code generation.
        /// When true, the framework will generate C# code with actual response properties in the test file.
        /// </summary>
        public bool IsEmptyAnonymousObjectForCodeGeneration { get; init; }
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
        /// The expected object (strongly-typed and already deserialized).
        /// When available, this avoids unnecessary JSON serialization → deserialization → serialization cycles.
        /// If null, the comparison strategy will deserialize from ResolvedExpectedJson.
        /// </summary>
        public required T? Expected { get; init; }

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
        /// Optional ordering/transformation function to apply before comparison. Useful for sorting
        /// collections or normalizing volatile data - an execution arn, a start date, a generated name.
        /// Must handle nullable inputs/outputs. Null means the caller supplied none.
        ///
        /// Null is load bearing, so do NOT default it to an identity lambda. It is the only thing that
        /// says whether a caller asked for normalization, and the http snapshot diff steers on exactly
        /// that: the expected side always goes through the response type, while the current side stays
        /// the raw response body on purpose, because that is what still surfaces a property the api
        /// returns but the type does not model. Only an assert that actually normalizes may give that
        /// up and compare the typed object on both sides - otherwise it would compare normalized
        /// against volatile and could never go green, no matter how often the snapshot is re-recorded.
        ///
        /// This used to be a separate `HasOrderFunc` flag next to a non-nullable func, and the two drifted
        /// apart immediately: every overload taking no filter func forwarded an identity lambda, so the
        /// flag was true for practically every assert in the sdk and every field a response type did not
        /// model silently dropped out of the diff. One nullable field cannot drift.
        /// </summary>
        public Func<T?, T?>? OrderFunc { get; init; }
    }
}