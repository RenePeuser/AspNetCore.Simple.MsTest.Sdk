using System.Collections.Immutable;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.Tables;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class DifferenceExtensions
    {
        private static readonly TestCreatorSettings TestCreatorSettings = new();

        internal static string ToResultTable(this ImmutableList<Difference> differences,
                                             string objectName1,
                                             string objectName2)
        {
            if (differences.IsEmpty())
            {
                return string.Empty;
            }

            if (!AssertObjectExtensions.ResponseFileFullPath)
            {
                var matched = TestCreatorSettings.ResponseFolderName;

                foreach (var legacyResponseFolderName in TestCreatorSettings.LegacyResponseFolderNames)
                {
                    if (objectName1.Contains($".{legacyResponseFolderName}."))
                    {
                        matched = legacyResponseFolderName;

                        break;
                    }
                }

                objectName1 = objectName1.Split($".{matched}.").Last();
            }

            // Build table using static helper
            return BuildDifferenceTable(differences, objectName1, objectName2);
        }

        private static string BuildDifferenceTable(ImmutableList<Difference> differences,
                                                   string objectName1,
                                                   string objectName2)
        {
            var tableBuilder = new TableBuilder();

            if (differences.Count.EqualsTo(1) && differences[0].MemberPath.IsNullOrWhiteSpace())
            {
                var columns = new[] { objectName1, objectName2, "MismatchType" };
                var rows = new List<object[]>();

                foreach (var dif in differences)
                {
                    rows.Add(new object[] { dif.Value1 ?? "null", dif.Value2 ?? "null", dif.MismatchType });
                }

                return tableBuilder.BuildTable(columns, rows, enableCount: false);
            }

            var flattened = FlattenDifferences(differences).ToImmutableList();

            var fullColumns = new[]
                              {
                                  nameof(Difference.MemberPath), objectName1, objectName2,
                                  "MismatchType"
                              };

            var fullRows = new List<object[]>();

            foreach (var dif in flattened)
            {
                switch (dif.MismatchType)
                {
                    case MismatchType.ValueDifference:
                        fullRows.Add(new object[]
                                     {
                                         dif.MemberPath, dif.Value1 ?? "null", dif.Value2 ?? "null",
                                         dif.MismatchType
                                     });

                        break;
                    case MismatchType.MissingInFirst:
                        fullRows.Add(new object[]
                                     {
                                         dif.MemberPath, "Property missing", dif.Value2 ?? "null",
                                         dif.MismatchType
                                     });

                        break;
                    case MismatchType.MissingInSecond:
                        fullRows.Add(new object[]
                                     {
                                         dif.MemberPath, dif.Value1 ?? "null", "Property missing",
                                         dif.MismatchType
                                     });

                        break;
                }
            }

            return tableBuilder.BuildTable(fullColumns, fullRows, enableCount: false);
        }

        private static IEnumerable<Difference> FlattenDifferences(IEnumerable<Difference> diffs)
        {
            foreach (var diff in diffs)
            {
                foreach (var fd in Flatten(diff))
                {
                    yield return fd;
                }
            }
        }

        private static IEnumerable<Difference> Flatten(Difference diff,
                                                       string prefix = "")
        {
            var path = prefix.IsNullOrEmpty() ? diff.MemberPath : $"{prefix}.{diff.MemberPath}";

            if (IsJson(diff.Value1) || IsJson(diff.Value2))
            {
                var obj1 = TryParseJson(diff.Value1);
                var obj2 = TryParseJson(diff.Value2);

                var allKeys = obj1.Keys.Union(obj2.Keys).Distinct().ToImmutableList();

                if (allKeys.IsEmpty())
                {
                    yield return diff;
                }

                foreach (var key in allKeys)
                {
                    yield return new Difference
                                 {
                                     MemberPath = $"{path}.{key}",
                                     Value1 = obj1.TryGetValue(key, out var v1) ? v1 : "Property missing",
                                     Value2 = obj2.TryGetValue(key, out var v2) ? v2 : "Property missing",
                                     MismatchType = ResolveMismatchType(v1, v2)
                                 };
                }
            }
            else
            {
                yield return diff;
            }
        }

        private static bool IsJson(object? value)
        {
            if (value is not string s)
            {
                return false;
            }

            s = s.Trim();

            return (s.StartsWith('{') && s.EndsWith('}')) || (s.StartsWith('[') && s.EndsWith(']'));
        }

        private static Dictionary<string, string?> TryParseJson(object? value)
        {
            var result = new Dictionary<string, string?>();

            if (value is not string json || json.IsNullOrWhiteSpace())
            {
                return result;
            }

            try
            {
                using var doc = JsonDocument.Parse(json);
                FlattenJson(doc.RootElement, result, "");
            }
#pragma warning disable CA1031
            catch
#pragma warning restore CA1031
            {
                // Ignore parsing errors, treat as primitive
            }

            return result;
        }

        private static void FlattenJson(JsonElement element,
                                        IDictionary<string, string?> dict,
                                        string prefix)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    foreach (var prop in element.EnumerateObject())
                    {
                        var propName = prefix.IsNullOrEmpty() ? prop.Name : $"{prefix}.{prop.Name}";
                        FlattenJson(prop.Value, dict, propName);
                    }

                    break;

                case JsonValueKind.Array:
                    var index = 0;

                    foreach (var item in element.EnumerateArray())
                    {
                        var itemName = $"{prefix}[{index++}]";
                        FlattenJson(item, dict, itemName);
                    }

                    break;

                case JsonValueKind.String:
                    dict[prefix] = element.GetString();

                    break;

                case JsonValueKind.Number:
                    dict[prefix] = element.GetRawText();

                    break;

                case JsonValueKind.True:
                case JsonValueKind.False:
                    dict[prefix] = element.GetBoolean().ToString();

                    break;

                case JsonValueKind.Null:
                    dict[prefix] = "null";

                    break;
            }
        }

        private static MismatchType ResolveMismatchType(object? v1,
                                                        object? v2)
        {
            if (v1.IsNull())
            {
                return MismatchType.MissingInFirst;
            }

            if (v2.IsNull())
            {
                return MismatchType.MissingInSecond;
            }

            return MismatchType.ValueDifference;
        }
    }
}