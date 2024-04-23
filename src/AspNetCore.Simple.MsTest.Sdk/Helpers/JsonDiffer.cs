using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Extensions.Pack;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public sealed record Difference(string MemberPath, string? Value1, string? Value2);

    internal sealed class JsonDiffer
    {
        public IImmutableList<Difference> FindDifferences(string json1, string json2)
        {
            var differences = FindDifferencesNative(json1, json2);

            var simpleDifferences = differences.Select(item => new Difference(item.Key, item.Value.Item1?.ToString(), item.Value.Item2?.ToString()));

            return simpleDifferences.ToImmutableList();
        }

        public Dictionary<string, (JToken?, JToken?)> FindDifferencesNative(string json1, string json2)
        {
            var differences = new Dictionary<string, (JToken?, JToken?)>();
            CompareTokens(JToken.Parse(json1), JToken.Parse(json2), differences, "");
            return differences;
        }

        private void CompareTokens(JToken? token1, JToken? token2, Dictionary<string, (JToken?, JToken?)> differences, string path)
        {
            if (JToken.DeepEquals(token1, token2))
            {
                return;
            }

            if (token1.IsNull())
            {
                return;
            }

            if (token2.IsNull())
            {
                return;
            }

            switch (token1.Type)
            {
                case JTokenType.Object:
                    if (token2.Type != JTokenType.Object)
                    {
                        differences[path] = (token1, token2);
                        return;
                    }
                    var obj1 = (JObject)token1;
                    var obj2 = (JObject)token2;
                    foreach (var property in obj1)
                    {
                        string propertyPath = AppendPath(path, property.Key);
                        JToken? token2Value = obj2.GetValueOrDefault(property.Key);
                        if (token2Value == null)
                        {
                            differences[propertyPath] = (property.Value, null);
                        }
                        else
                        {
                            CompareTokens(property.Value, token2Value, differences, propertyPath);
                        }
                    }
                    foreach (var property in obj2)
                    {
                        if (obj1[property.Key] == null)
                        {
                            string propertyPath = AppendPath(path, property.Key);
                            differences[propertyPath] = (null, property.Value);
                        }
                    }
                    break;

                case JTokenType.Array:
                    if (token2.Type != JTokenType.Array)
                    {
                        differences[path] = (token1, token2);
                        return;
                    }
                    var array1 = (JArray)token1;
                    var array2 = (JArray)token2;
                    for (int i = 0; i < array1.Count || i < array2.Count; i++)
                    {
                        string indexPath = AppendPath(path, $"[{i}]");
                        if (i >= array1.Count)
                            differences[indexPath] = (null, array2[i]);
                        else if (i >= array2.Count)
                            differences[indexPath] = (array1[i], null);
                        else
                            CompareTokens(array1[i], array2[i], differences, indexPath);
                    }
                    break;

                default:
                    differences[path] = (token1, token2);
                    break;
            }
        }

        private string AppendPath(string path, string addition)
        {
            return string.IsNullOrEmpty(path) ? addition : $"{path}.{addition}";
        }
    }
}
