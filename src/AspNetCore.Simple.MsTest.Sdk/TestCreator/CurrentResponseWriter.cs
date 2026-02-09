using System.Collections.Immutable;
using System.Reflection;
using Argument.Check;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddCurrentResponseWriterExtension
    {
        public static void AddCurrentResponseWriter(this IServiceCollection services)
        {
            services.AddJsonDiffer();
            services.AddSingletonIfNotExists<ICurrentResponseWriter, CurrentResponseWriter>();
        }
    }

    public sealed record WriteResponseRequest
    {
        public required string CurrentResponseAsString { get; init; }
        public required EmbeddedFileInfo expectedResult { get; init; }
        public required (string key, object? Value)[] parameters { get; init; }
        public required Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc { get; init; }
        public required Assembly callingAssembly { get; init; }
    }

    public interface ICurrentResponseWriter
    {
        void Write(WriteResponseRequest request);

        void Write(string currentResponseAsString,
                   EmbeddedFileInfo expectedResult,
                   (string key, object? Value)[] parameters,
                   Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                   Assembly callingAssembly);
    }

    /// <summary>
    /// AUTHORITATIVE snapshot writer (DEBUG only)
    ///
    /// Rules:
    /// - MissingInFirst  -> ADD from current
    /// - MissingInSecond -> REMOVE from expected
    /// - ValueDifference -> UPDATE from current (if allowed by differenceFunc)
    /// </summary>
    public sealed class CurrentResponseWriter(IJsonDiffer jsonDiffer) : ICurrentResponseWriter
    {
        public void Write(WriteResponseRequest request)
        {
            Write(request.CurrentResponseAsString,
                  request.expectedResult,
                  request.parameters,
                  request.differenceFunc,
                  request.callingAssembly);
        }

        public void Write(string currentResponseAsString,
                          EmbeddedFileInfo expectedResult,
                          (string key, object? Value)[] parameters,
                          Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                          Assembly callingAssembly)
        {
            Throw.IfNullOrWhiteSpace(currentResponseAsString);
            Throw.IfNull(expectedResult);
            Throw.IfNull(parameters);
            Throw.IfNull(differenceFunc);
            Throw.IfNull(callingAssembly);

            if (callingAssembly.IsCompiledInDebug().IsFalse())
            {
                Console.WriteLine("Snapshot writing disabled outside DEBUG mode.");
                return;
            }

            if (expectedResult.EmbeddedFileName.IsNullOrWhiteSpace())
            {
                return;
            }

            var currentJson = JToken.Parse(currentResponseAsString);
            var formattedCurrent = currentJson.ToString(Formatting.Indented);

            // Replace runtime parameters (IDs, placeholders, etc.)
            foreach (var (key, value) in parameters)
            {
                var oldValue = value?.ToString();
                if (!oldValue.IsNullOrEmpty())
                {
                    formattedCurrent = formattedCurrent.Replace(oldValue, key);
                }
            }

            var changed = RewriteExpectedSnapshot(expectedResult,
                                                  formattedCurrent,
                                                  differenceFunc,
                                                  out var mergedExpected);

            if (changed)
            {
                Assert.IsNotNull(expectedResult.EmbeddedFile,
                                 "Expected response file must be localized to be overwritten.");

                File.WriteAllText(expectedResult.EmbeddedFile.FullName, mergedExpected);
            }
        }

        // ============================================================
        // Snapshot rewrite logic
        // ============================================================

        private bool RewriteExpectedSnapshot(EmbeddedFileInfo existingResponseJson,
                                             string currentResponse,
                                             Func<ImmutableList<Difference>, IEnumerable<Difference>> differenceFunc,
                                             out string mergedExpectedJson)
        {
            mergedExpectedJson = string.Empty;

            var expected = JToken.Parse(existingResponseJson.Content);
            var current = JToken.Parse(currentResponse);

            var diffs = jsonDiffer.FindDifferences(expected, current);
            var relevantDiffs = differenceFunc(diffs).ToList();

            if (!relevantDiffs.Any())
            {
                mergedExpectedJson = expected.ToString(Formatting.Indented);
                return false;
            }

            foreach (var diff in relevantDiffs)
            {
                if (diff.MemberPath.IsNullOrWhiteSpace())
                {
                    continue;
                }

                switch (diff.MismatchType)
                {
                    case MismatchType.MissingInFirst:
                    {
                        var source = current.SelectToken(diff.MemberPath);
                        if (source != null)
                        {
                            AddOrUpdateTokenAtPath(expected, diff.MemberPath, source);
                        }

                        break;
                    }

                    case MismatchType.MissingInSecond:
                    {
                        RemoveTokenAtPath(expected, diff.MemberPath);
                        break;
                    }

                    case MismatchType.ValueDifference:
                    {
                        var source = current.SelectToken(diff.MemberPath);
                        if (source != null)
                        {
                            AddOrUpdateTokenAtPath(expected, diff.MemberPath, source);
                        }

                        break;
                    }
                }
            }

            mergedExpectedJson = expected.ToString(Formatting.Indented);
            return true;
        }

        // ============================================================
        // JSON mutation helpers (ROBUST)
        // ============================================================

        private static void AddOrUpdateTokenAtPath(JToken? root, string path, JToken value)
        {
            if (root == null || path.IsNullOrWhiteSpace())
            {
                return;
            }

            // 1️⃣ Direct replace if token exists (CRITICAL for arrays)
            var existing = root.SelectToken(path);
            if (existing != null)
            {
                existing.Replace(value.DeepClone());
                return;
            }

            // 2️⃣ Create missing path
            var segments = ParsePath(path);
            var current = root;

            for (var i = 0; i < segments.Count - 1; i++)
            {
                var seg = segments[i];

                if (!seg.IsArray)
                {
                    if (current is not JObject obj)
                    {
                        return;
                    }

                    if (obj[seg.Name] == null || obj[seg.Name]!.Type == JTokenType.Null)
                    {
                        obj[seg.Name] = new JObject();
                    }

                    current = obj[seg.Name]!;
                    continue;
                }

                // Array segment
                if (current is not JObject parentObj)
                {
                    return;
                }

                if (parentObj[seg.Name] is not JArray arr)
                {
                    arr = new JArray();
                    parentObj[seg.Name] = arr;
                }

                EnsureArraySize(arr, seg.Index);

                if (arr[seg.Index] == null || arr[seg.Index]!.Type == JTokenType.Null)
                {
                    arr[seg.Index] = new JObject();
                }

                current = arr[seg.Index]!;
            }

            var leaf = segments[^1];

            if (!leaf.IsArray)
            {
                if (current is JObject obj)
                {
                    obj[leaf.Name] = value.DeepClone();
                }

                return;
            }

            if (current is not JObject leafParent)
            {
                return;
            }

            if (leafParent[leaf.Name] is not JArray leafArr)
            {
                leafArr = new JArray();
                leafParent[leaf.Name] = leafArr;
            }

            EnsureArraySize(leafArr, leaf.Index);
            leafArr[leaf.Index] = value.DeepClone();
        }

        private static void RemoveTokenAtPath(JToken? root, string path)
        {
            if (root == null || path.IsNullOrWhiteSpace())
            {
                return;
            }

            var token = root.SelectToken(path);
            if (token == null)
            {
                return;
            }

            if (token.Parent is JProperty prop)
            {
                prop.Remove();
                return;
            }

            if (token.Parent is JArray)
            {
                token.Remove();
                return;
            }

            var lastDot = path.LastIndexOf('.');
            var parentPath = lastDot >= 0 ? path[..lastDot] : string.Empty;
            var leaf = lastDot >= 0 ? path[(lastDot + 1)..] : path;

            var parent = parentPath.IsNullOrWhiteSpace() ? root : root.SelectToken(parentPath);
            if (parent is JObject obj)
            {
                obj.Remove(leaf);
            }
        }

        // ============================================================
        // Path helpers
        // ============================================================

        private sealed record PathSegment(string Name,
                                          bool IsArray,
                                          int Index);

        private static List<PathSegment> ParsePath(string path)
        {
            var segments = new List<PathSegment>();

            foreach (var part in path.Split('.'))
            {
                if (part.Contains('['))
                {
                    var name = part[..part.IndexOf('[')];
                    var index = int.Parse(part[(part.IndexOf('[') + 1)..part.IndexOf(']')]);
                    segments.Add(new PathSegment(name, true, index));
                }
                else
                {
                    segments.Add(new PathSegment(part, false, -1));
                }
            }

            return segments;
        }

        private static void EnsureArraySize(JArray array, int index)
        {
            while (array.Count <= index)
            {
                array.Add(JValue.CreateNull());
            }
        }
    }
}
