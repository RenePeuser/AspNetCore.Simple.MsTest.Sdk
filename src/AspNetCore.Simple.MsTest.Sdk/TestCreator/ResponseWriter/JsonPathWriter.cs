using System;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddJsonPathWriterExtension
    {
        public static void AddJsonPathWriter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IJsonPathWriter, JsonPathWriter>();
        }
    }

    public interface IJsonPathWriter
    {
        void AddOrUpdate(JToken root,
                         string path,
                         JToken value);

        /// <summary>
        /// Drops the value at <paramref name="path"/>. Needed for an ignored difference that exists
        /// only in the response: there is no snapshot value to restore, so the only way not to record
        /// it is to remove it.
        /// </summary>
        void Remove(JToken root,
                    string path);
    }

    /// <summary>
    /// Writes single values into a <see cref="JToken"/> addressed by a
    /// <see cref="Difference.MemberPath"/>. That path is not plain JSONPath - see
    /// <see cref="MemberPathQuery"/> for the key-value array notation it can carry.
    /// </summary>
    internal sealed class JsonPathWriter : IJsonPathWriter
    {
        // =============================================================
        // ADD OR UPDATE
        // =============================================================

        public void AddOrUpdate(JToken root,
                                string path,
                                JToken value)
        {
            if (root == null || path.IsNullOrWhiteSpace())
            {
                return;
            }

            var parentPath = GetParentPath(path);
            var lastSegment = GetLastSegment(path);

            var parent = string.IsNullOrEmpty(parentPath)
                             ? root
                             : MemberPathQuery.SelectToken(root, parentPath);

            if (parent == null)
            {
                return;
            }

            if (lastSegment.IsIndex)
            {
                if (parent is not JArray array)
                {
                    return;
                }

                while (array.Count <= lastSegment.Index)
                {
                    array.Add(JValue.CreateNull());
                }

                array[lastSegment.Index] = value.DeepClone();
            }
            else if (lastSegment.IsKey)
            {
                // A key-value array is addressed by its Key, so the element to write is the one
                // carrying that key - its position in the array says nothing.
                if (parent is not JArray keyArray)
                {
                    return;
                }

                var element = FindByKey(keyArray, lastSegment.Name!);

                if (element == null)
                {
                    keyArray.Add(value.DeepClone());
                }
                else
                {
                    element.Replace(value.DeepClone());
                }
            }
            else
            {
                if (parent is not JObject obj)
                {
                    return;
                }

                obj[lastSegment.Name!] = value.DeepClone();
            }
        }

        // =============================================================
        // REMOVE
        // =============================================================

        public void Remove(JToken root,
                           string path)
        {
            if (root == null || path.IsNullOrWhiteSpace())
            {
                return;
            }

            var token = MemberPathQuery.SelectToken(root, path);

            if (token == null)
            {
                var lastDot = LastDotOutsideQuotes(path);
                var parentPath = lastDot >= 0 ? path[..lastDot] : string.Empty;
                var segment = lastDot >= 0 ? path[(lastDot + 1)..] : path;

                var parent = parentPath.IsNullOrEmpty() ? root : MemberPathQuery.SelectToken(root, parentPath);

                if (parent is JObject obj && obj.Property(segment) is not null)
                {
                    obj.Property(segment)!.Remove();
                }

                return;
            }

            if (token.Parent is JProperty prop)
            {
                prop.Remove();
            }
            else
            {
                token.Remove();
            }
        }

        // =============================================================
        // PATH HELPERS
        // =============================================================

        private static JToken? FindByKey(JArray array,
                                         string key)
        {
            foreach (var element in array)
            {
                if (element is JObject obj &&
                    string.Equals(obj.GetValue("Key", StringComparison.OrdinalIgnoreCase)?.ToString(),
                                  key,
                                  StringComparison.Ordinal))
                {
                    return element;
                }
            }

            return null;
        }

        /// <summary>
        /// The separating dot of the last segment. A quoted key may contain dots of its own
        /// (settings["a.b"].Value), and those must not split the path.
        /// </summary>
        private static int LastDotOutsideQuotes(string path)
        {
            var insideQuotes = false;

            for (var index = path.Length - 1; index >= 0; index--)
            {
                var character = path[index];

                if (character == '"')
                {
                    insideQuotes = insideQuotes.IsFalse();

                    continue;
                }

                if (character == '.' && insideQuotes.IsFalse())
                {
                    return index;
                }
            }

            return -1;
        }

        private static string GetParentPath(string path)
        {
            var lastDot = LastDotOutsideQuotes(path);
            var bracketIndex = path.IndexOf('[', lastDot < 0 ? 0 : lastDot);

            if (bracketIndex > 0)
            {
                return path[..bracketIndex];
            }

            return lastDot < 0 ? string.Empty : path[..lastDot];
        }

        private static PathSegment GetLastSegment(string path)
        {
            var lastDot = LastDotOutsideQuotes(path);
            var segment = lastDot < 0 ? path : path[(lastDot + 1)..];

            if (segment.Contains('['))
            {
                var start = segment.IndexOf('[');
                var end = segment.LastIndexOf(']');

                var inner = end > start ? segment[(start + 1)..end] : string.Empty;

                if (inner.Length > 1 && inner[0] == '"' && inner[^1] == '"')
                {
                    return PathSegment.Key(inner[1..^1]);
                }

                // Not an index and not a quoted key - treating it as a property name addresses
                // nothing and leaves the value alone, which beats throwing out of a snapshot write.
                return int.TryParse(inner, out var index)
                           ? PathSegment.AtIndex(index)
                           : PathSegment.Property(segment);
            }

            return PathSegment.Property(segment);
        }

        private sealed record PathSegment(bool IsIndex,
                                          bool IsKey,
                                          string? Name,
                                          int Index)
        {
            public static PathSegment Property(string name)
            {
                return new(false, false, name,
                           -1);
            }

            public static PathSegment AtIndex(int index)
            {
                return new(true, false, null,
                           index);
            }

            public static PathSegment Key(string key)
            {
                return new(false, true, key,
                           -1);
            }
        }
    }
}