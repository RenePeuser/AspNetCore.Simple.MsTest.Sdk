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
    /// Snapshot writer in AUTHORITATIVE mode:
    /// - MissingInFirst  -> ADD property
    /// - MissingInSecond -> REMOVE property
    /// - ValueDifference -> IGNORED (filtered via differenceFunc)
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

            // Safety gate
            if (callingAssembly.IsCompiledInDebug().IsFalse())
            {
                Console.WriteLine("Snapshot writing disabled outside DEBUG mode.");
                return;
            }

            if (expectedResult.EmbeddedFileName.IsNullOrWhiteSpace())
            {
                return;
            }

            // Parse current
            var currentJson = JToken.Parse(currentResponseAsString);
            var formattedCurrent = currentJson.ToString(Formatting.Indented);

            // Parameter replacement (IDs, placeholders, etc.)
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

                File.WriteAllText(expectedResult.EmbeddedFile.FullName,
                                  mergedExpected);
            }
        }

        // ============================================================
        // Snapshot rewrite logic (authoritative)
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
                            // ADD
                            var source = current.SelectToken(diff.MemberPath);
                            if (source != null)
                            {
                                AddOrUpdateTokenAtPath(expected, diff.MemberPath, source);
                            }

                            break;
                        }

                    case MismatchType.MissingInSecond:
                        {
                            // REMOVE
                            RemoveTokenAtPath(expected, diff.MemberPath);
                            break;
                        }

                    case MismatchType.ValueDifference:
                        // intentionally ignored (IDs, timestamps, etc.)
                        break;
                }
            }

            mergedExpectedJson = expected.ToString(Formatting.Indented);
            return true;
        }

        // ============================================================
        // JSON path mutation helpers
        // ============================================================

        private static void AddOrUpdateTokenAtPath(JToken root, string path, JToken value)
        {
            var segments = ParsePath(path);
            var current = root;

            for (var i = 0; i < segments.Count - 1; i++)
            {
                var seg = segments[i];

                if (seg.IsArray)
                {
                    if (current[seg.Name] is not JArray arr)
                    {
                        arr = new JArray();
                        ((JObject)current)[seg.Name] = arr;
                    }

                    EnsureArraySize(arr, seg.Index);

                    if (arr[seg.Index].IsNull() || arr[seg.Index] is JValue)
                    {
                        var obj = new JObject();
                        arr[seg.Index] = obj;
                        current = obj;
                    }
                    else
                    {
                        current = arr[seg.Index]!;
                    }
                }
                else
                {
                    if (current[seg.Name] == null)
                    {
                        var obj = new JObject();
                        ((JObject)current)[seg.Name] = obj;
                        current = obj;
                    }
                    else
                    {
                        current = current[seg.Name]!;
                    }
                }
            }

            var leaf = segments[^1];

            if (leaf.IsArray)
            {
                var arr = current[leaf.Name] as JArray ?? new JArray();
                ((JObject)current)[leaf.Name] = arr;

                EnsureArraySize(arr, leaf.Index);
                arr[leaf.Index] = value.DeepClone();
            }
            else
            {
                ((JObject)current)[leaf.Name] = value.DeepClone();
            }
        }

        private static void RemoveTokenAtPath(JToken root, string path)
        {
            var lastDot = path.LastIndexOf('.');
            var parentPath = lastDot >= 0 ? path[..lastDot] : string.Empty;
            var leaf = lastDot >= 0 ? path[(lastDot + 1)..] : path;

            var parent = parentPath.IsNullOrEmpty()
                             ? root
                             : root.SelectToken(parentPath);

            if (parent == null)
            {
                return;
            }

            if (leaf.StartsWith('[') && leaf.EndsWith(']'))
            {
                if (parent is not JArray arr)
                {
                    return;
                }

                if (int.TryParse(leaf.Trim('[', ']'), out var index) &&
                    index >= 0 && index < arr.Count)
                {
                    arr.RemoveAt(index);
                }

                return;
            }

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
