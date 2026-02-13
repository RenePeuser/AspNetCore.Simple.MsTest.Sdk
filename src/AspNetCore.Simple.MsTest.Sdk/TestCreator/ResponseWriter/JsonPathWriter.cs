using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddJsonPathWriterExtension
    {
        internal static void AddJsonPathWriter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<JsonPathWriter>();
        }
    }

    internal sealed class JsonPathWriter
    {
        // =============================================================
        // ADD OR UPDATE
        // =============================================================

        internal void AddOrUpdate(JToken root,
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
                             : root.SelectToken(parentPath);

            if (parent == null)
            {
                return;
            }

            if (lastSegment.IsArray)
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
            else
            {
                if (parent is not JObject obj)
                {
                    return;
                }

                obj[lastSegment.PropertyName!] = value.DeepClone();
            }
        }

        // =============================================================
        // REMOVE
        // =============================================================

        internal void Remove(JToken root,
                             string path)
        {
            if (root == null || path.IsNullOrWhiteSpace())
            {
                return;
            }

            var token = root.SelectToken(path);

            if (token == null)
            {
                var lastDot = path.LastIndexOf('.');
                var parentPath = lastDot >= 0 ? path[..lastDot] : string.Empty;
                var segment = lastDot >= 0 ? path[(lastDot + 1)..] : path;

                var parent = parentPath.IsNullOrEmpty() ? root : root.SelectToken(parentPath);

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

        private static string GetParentPath(string path)
        {
            var lastDot = path.LastIndexOf('.');
            var bracketIndex = path.IndexOf('[', lastDot < 0 ? 0 : lastDot);

            if (bracketIndex > 0)
            {
                return path[..bracketIndex];
            }

            return lastDot < 0 ? string.Empty : path[..lastDot];
        }

        private static PathSegment GetLastSegment(string path)
        {
            var lastDot = path.LastIndexOf('.');
            var segment = lastDot < 0 ? path : path[(lastDot + 1)..];

            if (segment.Contains('['))
            {
                var start = segment.IndexOf('[');
                var end = segment.IndexOf(']', start);

                var index = int.Parse(segment[(start + 1)..end]);

                return PathSegment.Array(index);
            }

            return PathSegment.Property(segment);
        }

        private sealed record PathSegment(bool IsArray,
                                          string? PropertyName,
                                          int Index)
        {
            public static PathSegment Property(string name) => new(false, name, -1);

            public static PathSegment Array(int index) => new(true, null, index);
        }
    }
}
