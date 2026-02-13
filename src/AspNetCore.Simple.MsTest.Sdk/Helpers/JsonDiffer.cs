using System.Collections.Immutable;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public enum MismatchType
    {
        ValueDifference,

        MissingInFirst, // left fehlt

        MissingInSecond // right fehlt
    }

    public sealed record Difference
    {
        public required string MemberPath { get; init; }

        public required string? Value1 { get; init; } // left

        public required string? Value2 { get; init; } // right

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
        ImmutableList<Difference> FindDifferences(JToken left,
                                                  JToken right);
    }

    internal sealed class JsonDiffer : IJsonDiffer
    {
        public ImmutableList<Difference> FindDifferences(JToken left,
                                                         JToken right)
        {
            var diffs = new Dictionary<string, (JToken?, JToken?, MismatchType)>();

            CompareTokens(left, right, diffs,
                          string.Empty);

            return diffs
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
                AddDiff(diffs, path, null,
                        right, MismatchType.MissingInFirst);

                return;
            }

            if (right == null)
            {
                AddDiff(diffs, path, left,
                        null, MismatchType.MissingInSecond);

                return;
            }

            if (left.Type != right.Type)
            {
                AddDiff(diffs, path, left,
                        right, MismatchType.ValueDifference);

                return;
            }

            if (JToken.DeepEquals(left, right))
            {
                return;
            }

            if (left is JObject objLeft && right is JObject objRight)
            {
                foreach (var prop in objLeft.Properties())
                {
                    var childPath = AppendPath(path, prop.Name);

                    if (!objRight.TryGetValue(prop.Name, out var rightValue))
                    {
                        AddDiff(diffs, childPath, prop.Value,
                                null, MismatchType.MissingInSecond);
                    }
                    else
                    {
                        CompareTokens(prop.Value, rightValue, diffs,
                                      childPath);
                    }
                }

                foreach (var prop in objRight.Properties())
                {
                    if (!objLeft.ContainsKey(prop.Name))
                    {
                        var childPath = AppendPath(path, prop.Name);

                        AddDiff(diffs, childPath, null,
                                prop.Value, MismatchType.MissingInFirst);
                    }
                }

                return;
            }

            if (left is JArray arrLeft && right is JArray arrRight)
            {
                var max = Math.Max(arrLeft.Count, arrRight.Count);

                for (var i = 0; i < max; i++)
                {
                    var childPath = AppendPath(path, $"[{i}]");

                    var l = i < arrLeft.Count ? arrLeft[i] : null;
                    var r = i < arrRight.Count ? arrRight[i] : null;

                    CompareTokens(l, r, diffs,
                                  childPath);
                }

                return;
            }

            AddDiff(diffs, path, left,
                    right, MismatchType.ValueDifference);
        }

        private static void AddDiff(Dictionary<string, (JToken?, JToken?, MismatchType)> diffs,
                                    string path,
                                    JToken? left,
                                    JToken? right,
                                    MismatchType type)
        {
            if (!path.IsNullOrWhiteSpace())
            {
                diffs[path] = (left, right, type);
            }
        }

        private static string AppendPath(string path,
                                         string addition)
        {
            return string.IsNullOrEmpty(path)
                       ? addition
                       : addition.StartsWith('[')
                           ? $"{path}{addition}"
                           : $"{path}.{addition}";
        }
    }
}
