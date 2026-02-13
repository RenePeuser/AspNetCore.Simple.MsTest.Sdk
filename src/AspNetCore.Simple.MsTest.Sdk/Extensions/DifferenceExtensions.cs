using System.Collections.Immutable;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk;
using ConsoleTables;
using Extensions.Pack;

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

        if (differences.Count.EqualsTo(1) && differences[0].MemberPath.IsNullOrWhiteSpace())
        {
            var table = new ConsoleTable(objectName1, objectName2, "MismatchType") { Options = { EnableCount = false } };
            differences.ForEach(dif => table.AddRow(dif.Value1, dif.Value2, dif.MismatchType));

            return table.ToString();
        }

        var flattened = FlattenDifferences(differences).ToImmutableList();

        var fullTable = new ConsoleTable(nameof(Difference.MemberPath), objectName1, objectName2,
                                         "MismatchType") { Options = { EnableCount = false } };

        foreach (var dif in flattened)
        {
            switch (dif.MismatchType)
            {
                case MismatchType.ValueDifference:
                    fullTable.AddRow(dif.MemberPath, dif.Value1, dif.Value2,
                                     dif.MismatchType);

                    break;
                case MismatchType.MissingInFirst:
                    fullTable.AddRow(dif.MemberPath, "Property missing", dif.Value2,
                                     dif.MismatchType);

                    break;
                case MismatchType.MissingInSecond:
                    fullTable.AddRow(dif.MemberPath, dif.Value1, "Property missing",
                                     dif.MismatchType);

                    break;
            }
        }

        return fullTable.ToString();
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
