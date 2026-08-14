using System;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public enum ObjectConstructionType
    {
        Unknown,

        AnonymousObject,

        RecordPositional,

        RecordNominal,

        ClassNominal
    }

    public interface IObjectTypeDetector
    {
        ObjectConstructionType DetectConstructionType<T>(T obj,
                                                         string expressionText);
    }

    public sealed class ObjectTypeDetector : IObjectTypeDetector
    {
        public ObjectConstructionType DetectConstructionType<T>(T obj,
                                                                string expressionText)
        {
            if (obj == null)
            {
                return ObjectConstructionType.Unknown;
            }

            var type = typeof(T);
            var trimmedExpression = expressionText.Trim();

            // Check 1: Anonymous object (compiler-generated type name)
            if (type.Name.Contains("AnonymousType", StringComparison.Ordinal))
            {
                return ObjectConstructionType.AnonymousObject;
            }

            // Check 2: Expression-based detection
            // Pattern: "new TypeName(...)" = positional constructor
            // Pattern: "new TypeName { ... }" = nominal/object initializer
            // Pattern: "new { ... }" = anonymous object

            // Anonymous object pattern
            if (trimmedExpression.StartsWith("new {", StringComparison.Ordinal) ||
                trimmedExpression.StartsWith("new{", StringComparison.Ordinal))
            {
                return ObjectConstructionType.AnonymousObject;
            }

            // Extract constructor call pattern
            var newIndex = trimmedExpression.IndexOf("new ", StringComparison.Ordinal);

            if (newIndex >= 0)
            {
                var afterNew = trimmedExpression.Substring(newIndex + 4).TrimStart();

                // Find the opening bracket
                var parenIndex = afterNew.IndexOf('(');
                var braceIndex = afterNew.IndexOf('{');

                if (parenIndex >= 0 && (braceIndex < 0 || parenIndex < braceIndex))
                {
                    // Positional constructor: new Person(...)
                    // Check if it's a record by reflection
                    if (IsRecord(type))
                    {
                        return ObjectConstructionType.RecordPositional;
                    }

                    // Classes can also use positional constructors, but for generation purposes
                    // we'll treat them as nominal (more common pattern)
                    return ObjectConstructionType.ClassNominal;
                }

                if (braceIndex >= 0)
                {
                    // Object initializer: new Person { ... }
                    if (IsRecord(type))
                    {
                        return ObjectConstructionType.RecordNominal;
                    }

                    return ObjectConstructionType.ClassNominal;
                }
            }

            // Fallback: Use reflection to detect record vs class
            if (IsRecord(type))
            {
                // Default to positional for records (most common)
                return ObjectConstructionType.RecordPositional;
            }

            // Default to nominal for classes
            return ObjectConstructionType.ClassNominal;
        }

        private static bool IsRecord(Type type)
        {
            // Records have a compiler-generated EqualityContract property
            // and implement IEquatable<T>
            var hasEqualityContract = type.GetProperty("EqualityContract",
                                                       System.Reflection.BindingFlags.NonPublic |
                                                       System.Reflection.BindingFlags.Instance) != null;

            if (hasEqualityContract)
            {
                return true;
            }

            // Alternative: Check for <Clone>$ method (records have this)
            var hasCloneMethod = type.GetMethods(System.Reflection.BindingFlags.NonPublic |
                                                 System.Reflection.BindingFlags.Public |
                                                 System.Reflection.BindingFlags.Instance)
                                     .Any(m => m.Name.Contains("Clone", StringComparison.Ordinal));

            return hasCloneMethod;
        }
    }

    public static class AddObjectTypeDetectorExtension
    {
        public static void AddObjectTypeDetector(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IObjectTypeDetector, ObjectTypeDetector>();
        }
    }
}