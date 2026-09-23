using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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

        /// <summary>
        /// Where the very same element sits in the CURRENT document, when that is not
        /// <see cref="MemberPath"/>. Null for every ordinary difference, because both documents
        /// address the element identically there. It is only ever set inside an order-independent
        /// array, where a matched pair can live at different indices on the two sides.
        /// Anything that PATCHES the current document has to use
        /// <c>CurrentMemberPath ?? MemberPath</c>; anything that READS the expected document uses
        /// <see cref="MemberPath"/>.
        /// </summary>
        public string? CurrentMemberPath { get; init; }

        public required string? Value1 { get; init; }

        public required string? Value2 { get; init; }

        public required MismatchType MismatchType { get; init; }
    }

    /// <summary>
    /// Describes one array while the differ walks a document. Handed to the order-independent
    /// predicate so a caller can decide per array instead of per bare name - a global "tags"
    /// would otherwise make every tags array in every payload order blind.
    /// </summary>
    public sealed record JsonArrayContext
    {
        /// <summary>
        /// Name of the property this array is the value of, e.g. anyOf. Empty for a root array and
        /// for an array nested directly inside another array.
        /// </summary>
        public required string PropertyName { get; init; }

        /// <summary>
        /// Index free path of the array, e.g. components.schemas.Pet.anyOf. Array elements
        /// contribute no segment, so the path describes the position in the SCHEMA rather than in
        /// one concrete document. That is what makes it usable in a predicate at all, and what
        /// keeps the answer identical for the expected and the current side.
        /// </summary>
        public required string Path { get; init; }
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

        /// <summary>
        /// Compares two documents. <paramref name="isOrderIndependentArray"/> decides per array
        /// whether its element order carries meaning; a set-like array has its elements MATCHED
        /// instead of compared index by index, and neither document is reordered to achieve that.
        /// </summary>
        ImmutableList<Difference> FindDifferences(JToken json1,
                                                  JToken json2,
                                                  Predicate<JsonArrayContext>? isOrderIndependentArray);

        ImmutableList<Difference> FindDifferences(string json1,
                                                  string json2);

        /// <summary>
        /// Compares two documents. <paramref name="isOrderIndependentArray"/> decides per array
        /// whether its element order carries meaning; a set-like array has its elements MATCHED
        /// instead of compared index by index, and neither document is reordered to achieve that.
        /// </summary>
        ImmutableList<Difference> FindDifferences(string json1,
                                                  string json2,
                                                  Predicate<JsonArrayContext>? isOrderIndependentArray);

        // Updated native differences method to include mismatch type.
        Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(string json1,
                                                                                   string json2);

        /// <summary>
        /// Native differences, keyed by the path in the EXPECTED document. A path collision - the
        /// same index holding a surplus element on both sides - collapses here; use
        /// <see cref="FindDifferences(string,string,Predicate{JsonArrayContext})"/> when every
        /// entry has to survive.
        /// </summary>
        Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(string json1,
                                                                                   string json2,
                                                                                   Predicate<JsonArrayContext>? isOrderIndependentArray);
    }

    internal sealed class JsonDiffer : IJsonDiffer
    {
        public ImmutableList<Difference> FindDifferences(JToken json1,
                                                         JToken json2)
        {
            return FindDifferences(json1, json2, isOrderIndependentArray: null);
        }

        public ImmutableList<Difference> FindDifferences(JToken json1,
                                                         JToken json2,
                                                         Predicate<JsonArrayContext>? isOrderIndependentArray)
        {
            return FindDifferences(json1.ToString(), json2.ToString(), isOrderIndependentArray);
        }

        public ImmutableList<Difference> FindDifferences(string json1,
                                                         string json2)
        {
            return FindDifferences(json1, json2, isOrderIndependentArray: null);
        }

        public ImmutableList<Difference> FindDifferences(string json1,
                                                         string json2,
                                                         Predicate<JsonArrayContext>? isOrderIndependentArray)
        {
            var simpleDifferences = Compare(json1, json2, isOrderIndependentArray)
                .Select(entry =>
                            new Difference()
                            {
                                MemberPath = entry.Path,
                                CurrentMemberPath = entry.CurrentPath,
                                Value1 = entry.Value1?.ToString(),
                                Value2 = entry.Value2?.ToString(),
                                MismatchType = entry.MismatchType
                            });

            return simpleDifferences.ToImmutableList();
        }

        public Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(string json1,
                                                                                          string json2)
        {
            return FindDifferencesNative(json1, json2, isOrderIndependentArray: null);
        }

        public Dictionary<string, (JToken?, JToken?, MismatchType)> FindDifferencesNative(string json1,
                                                                                          string json2,
                                                                                          Predicate<JsonArrayContext>? isOrderIndependentArray)
        {
            var differences = new Dictionary<string, (JToken?, JToken?, MismatchType)>(StringComparer.Ordinal);

            foreach (var entry in Compare(json1, json2, isOrderIndependentArray))
            {
                differences[entry.Path] = (entry.Value1, entry.Value2, entry.MismatchType);
            }

            return differences;
        }

        private static List<DifferenceEntry> Compare(string json1,
                                                     string json2,
                                                     Predicate<JsonArrayContext>? isOrderIndependentArray)
        {
            var normalizedJson1 = json1.Replace("\r\n", "\n");
            var normalizedJson2 = json2.Replace("\r\n", "\n");

            // Parse and normalize both tokens using CurrentValueJsonConverter
            // This ensures that stringified JSON values in 'currentValue' properties are compared correctly
            var token1 = JToken.Parse(normalizedJson1);
            var token2 = JToken.Parse(normalizedJson2);

            // Apply CurrentValue normalization to both tokens
            NormalizeCurrentValues(token1);
            NormalizeCurrentValues(token2);

            // Normalize line endings in all string values
            // This ensures consistent comparison between Windows (\r\n) and Unix (\n) line endings
            NormalizeStringValues(token1);
            NormalizeStringValues(token2);

            return new DiffWalker(isOrderIndependentArray).Compare(token1, token2);
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
                            var parsed = JsonConvert.DeserializeObject(stringValue);

                            if (parsed != null)
                            {
                                property.Value = JToken.FromObject(parsed);
                            }
                            else if (stringValue == "null")
                            {
                                property.Value = JValue.CreateNull();
                            }
                        }
                        catch (JsonException)
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

        /// <summary>
        /// Normalizes all string values in the JSON by replacing \r\n with \n.
        /// This ensures consistent comparison between files with different line endings.
        /// </summary>
        private static void NormalizeStringValues(JToken token)
        {
            if (token is JObject obj)
            {
                foreach (var property in obj.Properties().ToList())
                {
                    if (property.Value.Type == JTokenType.String)
                    {
                        var stringValue = property.Value.ToString();
                        var normalized = stringValue.Replace("\r\n", "\n");

                        if (normalized != stringValue)
                        {
                            property.Value = new JValue(normalized);
                        }
                    }
                    else
                    {
                        // Recursively process nested values
                        NormalizeStringValues(property.Value);
                    }
                }
            }
            else if (token is JArray array)
            {
                for (var i = 0; i < array.Count; i++)
                {
                    var item = array[i];

                    if (item.Type == JTokenType.String)
                    {
                        var stringValue = item.ToString();
                        var normalized = stringValue.Replace("\r\n", "\n");

                        if (normalized != stringValue)
                        {
                            array[i] = new JValue(normalized);
                        }
                    }
                    else
                    {
                        // Recursively process nested values
                        NormalizeStringValues(item);
                    }
                }
            }
        }

        private static string AppendPath(string path,
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

        /// <summary>
        /// One difference while it is still being collected. Carries both addresses, which the
        /// public <see cref="Difference"/> only keeps when they actually differ.
        /// </summary>
        private sealed record DifferenceEntry(string Path,
                                              string? CurrentPath,
                                              JToken? Value1,
                                              JToken? Value2,
                                              MismatchType MismatchType);

        /// <summary>
        /// The three addresses of one node. <see cref="Expected"/> and <see cref="Current"/> only
        /// drift apart below a matched pair inside an order-independent array; <see cref="Schema"/>
        /// never carries an index, so it stays identical on both sides and is the only one a
        /// predicate can sensibly be asked about.
        /// </summary>
        private readonly record struct NodePath(string Expected,
                                                string Current,
                                                string Schema)
        {
            public static NodePath Root => new(string.Empty, string.Empty, string.Empty);

            public NodePath Property(string name)
            {
                return new NodePath(AppendPath(Expected, name),
                                    AppendPath(Current, name),
                                    AppendPath(Schema, name));
            }

            public NodePath Index(string expectedIndex,
                                  string currentIndex)
            {
                return new NodePath(AppendPath(Expected, expectedIndex),
                                    AppendPath(Current, currentIndex),
                                    Schema);
            }
        }

        /// <summary>
        /// Walks two documents side by side. Holds the order-independent predicate so the recursion
        /// does not have to thread it through every call.
        /// </summary>
        private sealed class DiffWalker(Predicate<JsonArrayContext>? isOrderIndependentArray)
        {
            /// <summary>
            /// Properties tried, in order, to recognise two leftover elements as the same thing.
            /// This is presentation only: pass one below has already settled the verdict, so a
            /// wrong guess here can never turn a red assert green - it only decides whether the
            /// output reads "field x changed" or "one element gone, one new".
            /// </summary>
            private static readonly string[] IdentityPropertyNames = ["$ref", "id", "key", "name"];

            private readonly List<DifferenceEntry> _differences = [];

            public List<DifferenceEntry> Compare(JToken token1,
                                                 JToken token2)
            {
                CompareTokens(token1, token2, NodePath.Root);

                return _differences;
            }

            private void CompareTokens(JToken? token1,
                                       JToken? token2,
                                       NodePath path)
            {
                if (JToken.DeepEquals(token1, token2))
                {
                    return;
                }

                // Handle cases where one token is missing.
                if (token1.IsNull())
                {
                    Add(null, token2, path,
                        MismatchType.MissingInFirst);

                    return;
                }

                if (token2.IsNull())
                {
                    Add(token1, null, path,
                        MismatchType.MissingInSecond);

                    return;
                }

                switch (token1.Type)
                {
                    case JTokenType.Object:
                        if (token2.Type.NotEqualsTo(JTokenType.Object))
                        {
                            Add(token1, token2, path,
                                MismatchType.ValueDifference);

                            return;
                        }

                        CompareObjects((JObject)token1, (JObject)token2, path);

                        break;

                    case JTokenType.Array:
                        if (token2.Type.NotEqualsTo(JTokenType.Array))
                        {
                            Add(token1, token2, path,
                                MismatchType.ValueDifference);

                            return;
                        }

                        CompareArrays((JArray)token1, (JArray)token2, path);

                        break;

                    default:
                        // For primitive types, record the difference as a value difference.
                        Add(token1, token2, path,
                            MismatchType.ValueDifference);

                        break;
                }
            }

            private void CompareObjects(JObject obj1,
                                        JObject obj2,
                                        NodePath path)
            {
                // Compare all properties from the first object.
                foreach (var property in obj1)
                {
                    var propertyPath = path.Property(property.Key);
                    var token2Value = obj2.GetValueOrDefault(property.Key);

                    if (token2Value.IsNull())
                    {
                        Add(property.Value, null, propertyPath,
                            MismatchType.MissingInSecond);
                    }
                    else
                    {
                        CompareTokens(property.Value, token2Value, propertyPath);
                    }
                }

                // Look for properties that are in the second object but not in the first.
                foreach (var property in obj2)
                {
                    if (obj1[property.Key].IsNull())
                    {
                        Add(null, property.Value, path.Property(property.Key),
                            MismatchType.MissingInFirst);
                    }
                }
            }

            private void CompareArrays(JArray array1,
                                       JArray array2,
                                       NodePath path)
            {
                if (IsOrderIndependent(path))
                {
                    CompareAsSet(array1, array2, path);

                    return;
                }

                // Check if the array represents key-value pairs.
                var isKeyValueArray = array1.Count > 0 &&
                                      array1.First is JObject firstElement &&
                                      firstElement.ContainsKey("Key");

                for (var i = 0; i < array1.Count || i < array2.Count; i++)
                {
                    string index;

                    if (isKeyValueArray)
                    {
                        // Use the key value if available.
                        var key1 = i < array1.Count ? array1[i]["Key"]?.ToString() : null;
                        var key2 = i < array2.Count ? array2[i]["Key"]?.ToString() : null;
                        index = $"""["{key1 ?? key2 ?? i.ToInvariantString()}"]""";
                    }
                    else
                    {
                        index = $"[{i}]";
                    }

                    var indexPath = path.Index(index, index);

                    if (i >= array1.Count)
                    {
                        Add(null, array2[i], indexPath,
                            MismatchType.MissingInFirst);
                    }
                    else if (i >= array2.Count)
                    {
                        Add(array1[i], null, indexPath,
                            MismatchType.MissingInSecond);
                    }
                    else
                    {
                        CompareTokens(array1[i], array2[i], indexPath);
                    }
                }
            }

            /// <summary>
            /// Compares a set-like array by MATCHING its elements instead of reordering either
            /// document. Two arrays holding the same multiset consume each other completely in the
            /// first pass, so order cannot produce a difference; whatever is left over is a real
            /// difference and keeps the index it actually has in its own document.
            /// </summary>
            private void CompareAsSet(JArray expected,
                                      JArray current,
                                      NodePath path)
            {
                // Pass 1 - exact matches. A queue per signature keeps duplicates honest: three
                // identical elements on the left need three on the right.
                var pendingCurrent = new Dictionary<string, Queue<int>>(StringComparer.Ordinal);

                for (var index = 0; index < current.Count; index++)
                {
                    var signature = CanonicalSignature(current[index], path.Schema);

                    if (pendingCurrent.TryGetValue(signature, out var queue).IsFalse())
                    {
                        queue = new Queue<int>();
                        pendingCurrent[signature] = queue;
                    }

                    queue!.Enqueue(index);
                }

                var matchedCurrent = new bool[current.Count];
                var leftoverExpected = new List<int>();

                for (var index = 0; index < expected.Count; index++)
                {
                    var signature = CanonicalSignature(expected[index], path.Schema);

                    if (pendingCurrent.TryGetValue(signature, out var queue) && queue.Count > 0)
                    {
                        matchedCurrent[queue.Dequeue()] = true;

                        continue;
                    }

                    leftoverExpected.Add(index);
                }

                var leftoverCurrent = Enumerable.Range(0, current.Count)
                                                .Where(index => matchedCurrent[index].IsFalse())
                                                .ToList();

                // Everything from here only decides how a difference is DESCRIBED. The verdict was
                // settled above: equal multisets left nothing over.
                PairLeftovers(expected, current, leftoverExpected,
                              leftoverCurrent, path);
            }

            private void PairLeftovers(JArray expected,
                                       JArray current,
                                       List<int> leftoverExpected,
                                       List<int> leftoverCurrent,
                                       NodePath path)
            {
                // Pass 2 - same identity, changed content. Reporting "description changed" beats
                // reporting one element gone plus one element appeared.
                foreach (var expectedIndex in leftoverExpected.ToList())
                {
                    var identity = IdentityOf(expected[expectedIndex]);

                    if (identity.IsNull())
                    {
                        continue;
                    }

                    var currentIndex = leftoverCurrent.FirstOrDefault(index => IdentityOf(current[index]) == identity, -1);

                    if (currentIndex < 0)
                    {
                        continue;
                    }

                    leftoverExpected.Remove(expectedIndex);
                    leftoverCurrent.Remove(currentIndex);

                    CompareTokens(expected[expectedIndex],
                                  current[currentIndex],
                                  path.Index($"[{expectedIndex}]", $"[{currentIndex}]"));
                }

                // Pass 3 - whatever is left, paired in document order so a single changed element
                // still reads as a value difference rather than as two structural ones.
                var pairedCount = Math.Min(leftoverExpected.Count, leftoverCurrent.Count);

                for (var i = 0; i < pairedCount; i++)
                {
                    CompareTokens(expected[leftoverExpected[i]],
                                  current[leftoverCurrent[i]],
                                  path.Index($"[{leftoverExpected[i]}]", $"[{leftoverCurrent[i]}]"));
                }

                for (var i = pairedCount; i < leftoverExpected.Count; i++)
                {
                    var index = $"[{leftoverExpected[i]}]";

                    Add(expected[leftoverExpected[i]], null, path.Index(index, index),
                        MismatchType.MissingInSecond);
                }

                for (var i = pairedCount; i < leftoverCurrent.Count; i++)
                {
                    var index = $"[{leftoverCurrent[i]}]";

                    Add(null, current[leftoverCurrent[i]], path.Index(index, index),
                        MismatchType.MissingInFirst);
                }
            }

            /// <summary>
            /// A stable, culture free fingerprint of a token that agrees with
            /// <see cref="JToken.DeepEquals(JToken,JToken)"/>: object properties are written in
            /// ordinal name order, because property order is not a difference, and a nested
            /// order-independent array has its element fingerprints sorted - which happens AFTER
            /// the recursion, so the fingerprint of a child is already canonical when its parent
            /// uses it. Only fingerprints are ever sorted here; both documents stay untouched.
            /// </summary>
            private string CanonicalSignature(JToken token,
                                              string schemaPath)
            {
                switch (token)
                {
                    case JObject obj:
                        var properties = obj.Properties()
                                            .OrderBy(property => property.Name, StringComparer.Ordinal)
                                            .Select(property => $"{JsonConvert.ToString(property.Name)}:{CanonicalSignature(property.Value, AppendPath(schemaPath, property.Name))}");

                        return $"{{{string.Join(",", properties)}}}";

                    case JArray array:
                        // Elements do not add a segment - see JsonArrayContext.Path.
                        var items = array.Select(item => CanonicalSignature(item, schemaPath)).ToList();

                        if (IsOrderIndependent(schemaPath))
                        {
                            items.Sort(StringComparer.Ordinal);
                        }

                        return $"[{string.Join(",", items)}]";

                    default:
                        // JsonTextWriter writes numbers and dates invariantly, so this does not
                        // depend on the culture the test happens to run under.
                        return token.ToString(Formatting.None);
                }
            }

            private static string? IdentityOf(JToken token)
            {
                if (token is not JObject obj)
                {
                    return null;
                }

                foreach (var candidate in IdentityPropertyNames)
                {
                    var property = obj.Properties()
                                      .FirstOrDefault(item => string.Equals(item.Name, candidate, StringComparison.OrdinalIgnoreCase));

                    if (property?.Value is JValue { Value: not null })
                    {
                        return $"{candidate}={property.Value.ToString(Formatting.None)}";
                    }
                }

                return null;
            }

            private bool IsOrderIndependent(NodePath path)
            {
                return IsOrderIndependent(path.Schema);
            }

            private bool IsOrderIndependent(string schemaPath)
            {
                if (isOrderIndependentArray.IsNull())
                {
                    return false;
                }

                var lastSeparator = schemaPath.LastIndexOf('.');

                var propertyName = lastSeparator < 0
                                       ? schemaPath
                                       : schemaPath[(lastSeparator + 1)..];

                return isOrderIndependentArray.Invoke(new JsonArrayContext
                {
                    PropertyName = propertyName,
                    Path = schemaPath
                });
            }

            private void Add(JToken? value1,
                             JToken? value2,
                             NodePath path,
                             MismatchType mismatchType)
            {
                var currentPath = string.Equals(path.Expected, path.Current, StringComparison.Ordinal)
                                      ? null
                                      : path.Current;

                _differences.Add(new DifferenceEntry(path.Expected, currentPath, value1,
                                                     value2, mismatchType));
            }
        }
    }
}