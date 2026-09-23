using System.Collections.Generic;
using System.IO;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk.Converters;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddDifferenceResponseWriterExtension
    {
        internal static void AddDifferenceResponseWriter(this IServiceCollection services)
        {
            services.AddJsonDiffer();
            services.AddJsonPathWriter();
            services.AddParameterReplacer();
            services.AddSnapshotPlaceholderGuard();

            services.AddSingletonIfNotExists<ISpecificResponseWriter, DifferenceResponseWriter>();
        }
    }

    internal sealed class DifferenceResponseWriter(IJsonDiffer jsonDiffer,
                                                   IJsonPathWriter jsonPathWriter,
                                                   IParameterReplacer parameterReplacementService,
                                                   SnapshotPlaceholderGuard snapshotPlaceholderGuard) : ISpecificResponseWriter
    {
        public bool CanHandle(WriteResponseRequest context)
        {
            var canHandle = context.ExpectedResult.EmbeddedFileName.EndsWith(".json") &&
                            context.Mode == ResponseWriteMode.DifferencesOnly &&
                            context.ExpectedResult.EmbeddedFile.IsNotNull() &&
                            context.ExpectedResult.EmbeddedFile.Exists;

            SdkTrace.WriteLine($"[DifferenceResponseWriter.CanHandle] Mode={context.Mode}, FileName={context.ExpectedResult.EmbeddedFileName}, CanHandle={canHandle}");

            return canHandle;
        }

        public void Write(WriteResponseRequest context)
        {
            if (context.CallingAssembly.IsCompiledInDebug().IsFalse())
            {
                return;
            }

            if (context.ExpectedResult.EmbeddedFileName.IsNullOrWhiteSpace())
            {
                return;
            }

            var contextParameters = context.Parameters.OrderByDescending(p => p.Value?.ToString()?.Length).ToArray();
            var currentRootAsJson = parameterReplacementService.ReplaceWithPlaceholders(context.CurrentResponseAsString, contextParameters);

            // A numeric parameter stands bare in the snapshot, which is not json Newtonsoft can read - see
            // PlaceholderJson. Both sides move into the sentinel form for the whole merge and come back at
            // the very end, so the file keeps the spelling its author chose.
            var expectedAsJson = PlaceholderJson.MakeParseable(context.ExpectedResult.Content);
            currentRootAsJson = PlaceholderJson.MakeParseable(currentRootAsJson);

            // The snapshot being updated keeps the shape it has - see SnapshotShape. Both sides have to
            // be in that same shape here, otherwise every json path of the envelope would read as a
            // difference against a bare body and the file would be replaced wholesale.
            currentRootAsJson = SnapshotShape.MatchExisting(currentRootAsJson, expectedAsJson);

            //// New we can have also indexer properties. Values[0] -> Values[$Index$]
            //foreach (var parameter in contextParameters)
            //{
            //    currentRootAsJson = GlobalRegex.IndexReplacement().Replace(currentRootAsJson, $"[{parameter.key}]");
            //}

            var currentRoot = JToken.Parse(currentRootAsJson);

            var expectedRoot = JToken.Parse(expectedAsJson);

            // The same array semantics the assert used. An array that only got reordered produces no
            // difference at all here, so the snapshot keeps the order its author chose instead of
            // being rewritten on every recording.
            var diffs = jsonDiffer.FindDifferences(expectedRoot.ToString(),
                                                   currentRoot.ToString(),
                                                   context.OrderIndependentArrayFilter ??
                                                   AssertObjectExtensions.OrderIndependentArrayFilter);

            if (!diffs.Any())
            {
                return;
            }

            var finalDiffs = AssertObjectExtensions.ApplyDifferenceFiltering(diffs,
                                                                             context.DifferenceFunc,
                                                                             context.DifferenceFilter);

            var ignoredDifferences = diffs.Except(finalDiffs)
                                          .Where(difference => difference.MemberPath.IsNullOrWhiteSpace().IsFalse());

            var resultRoot = currentRoot.DeepClone();

            // An ignored difference is not compared, so its current value must not reach the file -
            // it would produce a diff on every single re-record for a property the author declared
            // uninteresting, and that noise is what buries the real changes in a review.
            foreach (var ignoredDifference in ignoredDifferences)
            {
                // The response carries a property the snapshot never had. There is no snapshot value
                // to keep, so the only way not to record it is to drop it from the result.
                // resultRoot is a clone of the CURRENT document, expectedRoot is the snapshot. Inside
                // an order-independent array a matched pair can sit at different indices on the two
                // sides, so each side has to be addressed with its own path - see
                // Difference.CurrentMemberPath.
                var currentPath = ignoredDifference.CurrentMemberPath ?? ignoredDifference.MemberPath;

                if (ignoredDifference.MismatchType == MismatchType.MissingInFirst)
                {
                    jsonPathWriter.Remove(resultRoot, currentPath);

                    continue;
                }

                var source = MemberPathQuery.SelectToken(expectedRoot, ignoredDifference.MemberPath);

                if (source != null)
                {
                    jsonPathWriter.AddOrUpdate(resultRoot, currentPath, source);
                }
            }

            // Use custom serializer settings to handle currentValue un-escaping
            var serializerSettings = new JsonSerializerSettings
                                     {
                                         Formatting = Formatting.Indented,
                                         Converters = new List<JsonConverter> { new CurrentValueJsonConverter() }
                                     };

            // Which placeholder belongs where, and how it was spelled, is read off the file being updated
            // - per json path, so the same name can be a bare number here and a quoted string there. See
            // SnapshotPlaceholderRestorer.
            SnapshotPlaceholderRestorer.ApplyOriginalSpelling(resultRoot, context.ExpectedResult.Content);

            var serialized = JsonConvert.SerializeObject(resultRoot, serializerSettings);

            // A sentinel left over here sits at a path the snapshot never had, so nothing is known about
            // its spelling and quoted is the only safe form.
            var output = PlaceholderJson.RestoreBareMarkers(PlaceholderJson.RestoreRemainingSentinelsAsQuoted(serialized));

            // A merge that silently drops a placeholder turns the template into a hard coded snapshot -
            // see SnapshotPlaceholderGuard. Checked before the write, so a refusal leaves the file intact.
            snapshotPlaceholderGuard.EnsureNoPlaceholderIsLost(context.ExpectedResult,
                                                               context.ExpectedResult.Content,
                                                               output,
                                                               context.Parameters);

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName,
                              output);
        }
    }
}