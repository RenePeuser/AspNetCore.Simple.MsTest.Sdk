using System.Collections.Immutable;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public enum MismatchType
    {
        ValueDifference,
        MissingInFirst,
        MissingInSecond
    }

    public sealed record Difference
    {
        public required string MemberPath { get; init; }
        public required string? Value1 { get; init; }
        public required string? Value2 { get; init; }
        public required MismatchType MismatchType { get; init; }
    }

    public static class AddJsonSerializationExtensions
    {
        public static void AddJsonDiffer(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IJsonDiffer, JsonDiffer>();
        }
    }

    public interface IJsonDiffer
    {
        ImmutableList<Difference> FindDifferences(string json1, string json2);

        Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(string json1, string json2);

        ImmutableList<Difference> FindDifferences(JToken json1, JToken json2);

        Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(JToken json1, JToken json2);
    }

    internal sealed class JsonDiffer : IJsonDiffer
    {
        public ImmutableList<Difference> FindDifferences(string json1, string json2)
        {
            var left = json1.IsNullOrWhiteSpace() ? JToken.Parse("{}") : JToken.Parse(json1);
            var right = json2.IsNullOrWhiteSpace() ? JToken.Parse("{}") : JToken.Parse(json2);

            return FindDifferences(left, right);
        }

        public ImmutableList<Difference> FindDifferences(JToken json1, JToken json2)
        {
            var native = FindDifferencesNative(json1, json2);

            return native
                   .Select(d => new Difference
                   {
                       MemberPath = d.Key,
                       Value1 = d.Value.Item1?.ToString(),
                       Value2 = d.Value.Item2?.ToString(),
                       MismatchType = d.Value.Item3
                   })
                   .OrderBy(d => d.MemberPath)
                   .ToImmutableList();
        }

        public Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(string json1, string json2)
        {
            var left = json1.IsNullOrWhiteSpace() ? JToken.Parse("{}") : JToken.Parse(json1);
            var right = json2.IsNullOrWhiteSpace() ? JToken.Parse("{}") : JToken.Parse(json2);

            return FindDifferencesNative(left, right);
        }

        public Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(JToken json1, JToken json2)
        {
            var diffs = new Dictionary<string, (JToken?, JToken?, MismatchType)>();
            CompareTokens(json1, json2, diffs, string.Empty);
            return diffs;
        }

        // ============================
        // Core comparison logic
        // ============================

        private void CompareTokens(JToken? left,
                                   JToken? right,
                                   Dictionary<string, (JToken?, JToken?, MismatchType)> diffs,
                                   string path)
        {
            if (left == null && right == null)
            {
                return;
            }

            if (left == null)
            {
                AddDiff(diffs, path, null, right, MismatchType.MissingInFirst);
                return;
            }

            if (right == null)
            {
                AddDiff(diffs, path, left, null, MismatchType.MissingInSecond);
                return;
            }

            if (left.Type != right.Type)
            {
                AddDiff(diffs, path, left, right, MismatchType.ValueDifference);
                return;
            }

            if (JToken.DeepEquals(left, right))
            {
                return;
            }

            if (left is JObject obj1 && right is JObject obj2)
            {
                foreach (var prop in obj1.Properties())
                {
                    var childPath = AppendPath(path, prop.Name);

                    if (!obj2.TryGetValue(prop.Name, out var rightValue))
                    {
                        AddDiff(diffs, childPath, prop.Value, null, MismatchType.MissingInSecond);
                    }
                    else
                    {
                        CompareTokens(prop.Value, rightValue, diffs, childPath);
                    }
                }

                foreach (var prop in obj2.Properties())
                {
                    if (!obj1.ContainsKey(prop.Name))
                    {
                        var childPath = AppendPath(path, prop.Name);
                        AddDiff(diffs, childPath, null, prop.Value, MismatchType.MissingInFirst);
                    }
                }

                return;
            }

            if (left is JArray arr1 && right is JArray arr2)
            {
                var max = Math.Max(arr1.Count, arr2.Count);

                for (var i = 0; i < max; i++)
                {
                    var p = AppendPath(path, $"[{i}]");

                    var l = i < arr1.Count ? arr1[i] : null;
                    var r = i < arr2.Count ? arr2[i] : null;

                    CompareTokens(l, r, diffs, p);
                }

                return;
            }

            AddDiff(diffs, path, left, right, MismatchType.ValueDifference);
        }

        // ============================
        // Helpers
        // ============================

        private static void AddDiff(Dictionary<string, (JToken?, JToken?, MismatchType)> diffs,
                                    string path,
                                    JToken? left,
                                    JToken? right,
                                    MismatchType type)
        {
            if (path.IsNullOrWhiteSpace())
            {
                return;
            }

            diffs[path] = (left, right, type);
        }

        private static string AppendPath(string path, string addition)
        {
            return string.IsNullOrEmpty(path)
                       ? addition
                       : addition.StartsWith('[')
                           ? $"{path}{addition}"
                           : $"{path}.{addition}";
        }
    }
}
