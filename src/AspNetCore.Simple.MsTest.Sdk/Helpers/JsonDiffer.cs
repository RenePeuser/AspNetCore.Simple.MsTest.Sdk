using System.Collections.Immutable;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    // Introduce an enum to categorize the type of mismatch.
    public enum MismatchType
    {
        ValueDifference, // Both values exist but are not equal.

        MissingInFirst, // The value is missing in the first JSON.

        MissingInSecond // The value is missing in the second JSON.
    }

    // Update the Difference record to include the mismatch type.
    public sealed record Difference()
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
        ImmutableList<Difference> FindDifferences(JToken json1,
                                                  JToken json2);

        ImmutableList<Difference> FindDifferences(string json1,
                                                  string json2);

        // Updated native differences method to include mismatch type.
        Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(string json1,
                                                                                   string json2);
    }

    internal sealed class JsonDiffer : IJsonDiffer
    {
        public ImmutableList<Difference> FindDifferences(JToken json1,
                                                         JToken json2)
        {
            return FindDifferences(json1.ToString(), json2.ToString());
        }

        public ImmutableList<Difference> FindDifferences(string json1,
                                                         string json2)
        {
            var differences = FindDifferencesNative(json1, json2);

            var simpleDifferences = differences.Select(item =>
                                                           new Difference()
                                                           {
                                                               MemberPath = item.Key,
                                                               Value1 = item.Value.Item1?.ToString(),
                                                               Value2 = item.Value.Item2?.ToString(),
                                                               MismatchType = item.Value.Item3
                                                           });

            return simpleDifferences.ToImmutableList();
        }

        public Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(string json1,
                                                                                          string json2)
        {
            var normalizedJson1 = json1.Replace("\r\n", "\n");
            var normalizedJson2 = json2.Replace("\r\n", "\n");

            var differences = new Dictionary<string, (JToken?, JToken?, MismatchType)>();

            // Parse and normalize both tokens using CurrentValueJsonConverter
            // This ensures that stringified JSON values in 'currentValue' properties are compared correctly
            var token1 = JToken.Parse(normalizedJson1);
            var token2 = JToken.Parse(normalizedJson2);

            // Apply CurrentValue normalization to both tokens
            NormalizeCurrentValues(token1);
            NormalizeCurrentValues(token2);

            CompareTokens(token1, token2, differences,
                          "");

            return differences;
        }

        /// <summary>
        /// Normalizes 'currentValue' properties by converting stringified JSON to actual JSON values.
        /// This ensures consistent comparison between expected (from file) and actual (from API) responses.
        /// </summary>
        private static void NormalizeCurrentValues(JToken token)
        {
            if (token is JObject obj)
            {
                foreach (var property in obj.Properties().ToList())
                {
                    if (property.Name == "currentValue" && property.Value.Type == JTokenType.String)
                    {
                        var stringValue = property.Value.ToString();

                        // Try to parse the string as JSON
                        try
                        {
                            var parsed = Newtonsoft.Json.JsonConvert.DeserializeObject(stringValue);

                            if (parsed != null)
                            {
                                property.Value = JToken.FromObject(parsed);
                            }
                            else if (stringValue == "null")
                            {
                                property.Value = JValue.CreateNull();
                            }
                        }
                        catch (Newtonsoft.Json.JsonException)
                        {
                            // Keep original string value if parsing fails
                        }
                    }

                    // Recursively process nested values
                    NormalizeCurrentValues(property.Value);
                }
            }
            else if (token is JArray array)
            {
                foreach (var item in array)
                {
                    NormalizeCurrentValues(item);
                }
            }
        }

        private void CompareTokens(JToken? token1,
                                   JToken? token2,
                                   Dictionary<string, (JToken?, JToken?, MismatchType)> differences,
                                   string path)
        {
            if (JToken.DeepEquals(token1, token2))
            {
                return;
            }

            // Handle cases where one token is missing.
            if (token1.IsNull())
            {
                differences[path] = (null, token2, MismatchType.MissingInFirst);

                return;
            }

            if (token2.IsNull())
            {
                differences[path] = (token1, null, MismatchType.MissingInSecond);

                return;
            }

            switch (token1.Type)
            {
                case JTokenType.Object:
                    if (token2.Type.NotEqualsTo(JTokenType.Object))
                    {
                        differences[path] = (token1, token2, MismatchType.ValueDifference);

                        return;
                    }

                    var obj1 = (JObject)token1;
                    var obj2 = (JObject)token2;

                    // Compare all properties from the first object.
                    foreach (var property in obj1)
                    {
                        var propertyPath = AppendPath(path, property.Key);
                        var token2Value = obj2.GetValueOrDefault(property.Key);

                        if (token2Value.IsNull())
                        {
                            differences[propertyPath] = (property.Value, null, MismatchType.MissingInSecond);
                        }
                        else
                        {
                            CompareTokens(property.Value, token2Value, differences,
                                          propertyPath);
                        }
                    }

                    // Look for properties that are in the second object but not in the first.
                    foreach (var property in obj2)
                    {
                        var propertyPath = AppendPath(path, property.Key);

                        if (obj1[property.Key].IsNull())
                        {
                            differences[propertyPath] = (null, property.Value, MismatchType.MissingInFirst);
                        }
                    }

                    break;

                case JTokenType.Array:
                    if (token2.Type.NotEqualsTo(JTokenType.Array))
                    {
                        differences[path] = (token1, token2, MismatchType.ValueDifference);

                        return;
                    }

                    var array1 = (JArray)token1;
                    var array2 = (JArray)token2;

                    // Check if the array represents key-value pairs.
                    var isKeyValueArray = array1.Count > 0 &&
                                          array1.First is JObject firstElement &&
                                          firstElement.ContainsKey("Key");

                    for (var i = 0; i < array1.Count || i < array2.Count; i++)
                    {
                        string indexPath;

                        if (isKeyValueArray)
                        {
                            // Use the key value if available.
                            var key1 = i < array1.Count ? array1[i]["Key"]?.ToString() : null;
                            var key2 = i < array2.Count ? array2[i]["Key"]?.ToString() : null;
                            indexPath = AppendPath(path, $"""["{key1 ?? key2 ?? i.ToInvariantString()}"]""");
                        }
                        else
                        {
                            indexPath = AppendPath(path, $"[{i}]");
                        }

                        if (i >= array1.Count)
                        {
                            differences[indexPath] = (null, array2[i], MismatchType.MissingInFirst);
                        }
                        else if (i >= array2.Count)
                        {
                            differences[indexPath] = (array1[i], null, MismatchType.MissingInSecond);
                        }
                        else
                        {
                            CompareTokens(array1[i], array2[i], differences,
                                          indexPath);
                        }
                    }

                    break;

                default:
                    // For primitive types, record the difference as a value difference.
                    differences[path] = (token1, token2, MismatchType.ValueDifference);

                    break;
            }
        }

        private string AppendPath(string path,
                                  string addition)
        {
            if (path.IsNullOrEmpty())
            {
                return addition;
            }

            // If the addition represents an array index, don't add a dot.
            if (addition.First().EqualsTo('['))
            {
                return $"{path}{addition}";
            }

            return $"{path}.{addition}";
        }
    }
}