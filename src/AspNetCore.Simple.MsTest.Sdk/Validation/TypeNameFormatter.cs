using System;
using System.Collections.Generic;
using System.Linq;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk.Validation
{
    /// <summary>
    /// Renders type names for validation output.
    ///
    /// Short names alone are not enough: an api that versions its contracts keeps
    /// <c>Api.V1.InsertOrUpdateOrDeleteResponse</c> next to <c>Api.V2.InsertOrUpdateOrDeleteResponse</c>.
    /// Printing the short name for both puts the identical string on either side of a "does not match"
    /// marker, which reads like a bug in the sdk instead of the version mix-up it actually is.
    /// </summary>
    public static class TypeNameFormatter
    {
        /// <summary>
        /// Short, generic-aware name - e.g. <c>IEnumerable&lt;Person&gt;</c> instead of <c>IEnumerable`1</c>.
        /// </summary>
        public static string Format(Type type)
        {
            if (type.IsGenericType.IsFalse())
            {
                return type.Name;
            }

            var typeName = type.Name;
            var backtickIndex = typeName.IndexOf('`');

            if (backtickIndex > 0)
            {
                typeName = typeName.Substring(0, backtickIndex);
            }

            var genericArgNames = string.Join(", ", type.GetGenericArguments().Select(Format));

            return $"{typeName}<{genericArgNames}>";
        }

        /// <summary>
        /// Like <see cref="Format(Type)"/>, but guaranteed to be distinguishable from <paramref name="peers"/>.
        /// On a name collision the result is prefixed with just enough trailing namespace segments - the
        /// part the colliding types do NOT have in common - to tell them apart.
        /// </summary>
        public static string Format(Type type,
                                    IReadOnlyCollection<Type> peers)
        {
            var shortName = Format(type);

            var colliding = peers.Where(peer => peer != type &&
                                                Format(peer).Equals(shortName, StringComparison.Ordinal))
                                 .ToList();

            if (colliding.Count.EqualsTo(0))
            {
                return shortName;
            }

            var namespaces = colliding.Append(type)
                                      .Select(t => t.Namespace ?? string.Empty)
                                      .ToList();

            var commonSegments = CommonNamespaceSegmentCount(namespaces);

            var ownSegments = (type.Namespace ?? string.Empty).Split('.', StringSplitOptions.RemoveEmptyEntries);

            if (commonSegments >= ownSegments.Length)
            {
                // Identical namespace - only the assembly can still separate them.
                var assemblyName = type.Assembly.GetName().Name;

                return assemblyName.IsNullOrWhiteSpace() ? shortName : $"{shortName} ({assemblyName})";
            }

            var distinguishing = string.Join(".", ownSegments.Skip(commonSegments));

            return $"{distinguishing}.{shortName}";
        }

        /// <summary>
        /// Number of leading namespace segments all given namespaces share.
        /// </summary>
        private static int CommonNamespaceSegmentCount(IReadOnlyCollection<string> namespaces)
        {
            var split = namespaces.Select(ns => ns.Split('.', StringSplitOptions.RemoveEmptyEntries)).ToList();
            var shortest = split.Min(segments => segments.Length);

            for (var index = 0; index < shortest; index++)
            {
                var segment = split[0][index];

                if (split.Any(segments => segments[index].Equals(segment, StringComparison.Ordinal).IsFalse()))
                {
                    return index;
                }
            }

            return shortest;
        }
    }
}