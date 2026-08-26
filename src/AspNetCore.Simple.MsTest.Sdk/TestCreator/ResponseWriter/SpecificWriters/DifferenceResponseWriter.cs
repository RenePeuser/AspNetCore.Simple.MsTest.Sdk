using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            services.AddSingletonIfNotExists<ISpecificResponseWriter, DifferenceResponseWriter>();
        }
    }

    internal sealed class DifferenceResponseWriter(IJsonDiffer jsonDiffer,
                                                   IJsonPathWriter jsonPathWriter,
                                                   IParameterReplacer parameterReplacementService) : ISpecificResponseWriter
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

            // The snapshot being updated keeps the shape it has - see SnapshotShape. Both sides have to
            // be in that same shape here, otherwise every json path of the envelope would read as a
            // difference against a bare body and the file would be replaced wholesale.
            currentRootAsJson = SnapshotShape.MatchExisting(currentRootAsJson, context.ExpectedResult.Content);

            //// New we can have also indexer properties. Values[0] -> Values[$Index$]
            //foreach (var parameter in contextParameters)
            //{
            //    currentRootAsJson = GlobalRegex.IndexReplacement().Replace(currentRootAsJson, $"[{parameter.key}]");
            //}

            var currentRoot = JToken.Parse(currentRootAsJson);

            var expectedRoot = JToken.Parse(context.ExpectedResult.Content);

            var diffs = jsonDiffer.FindDifferences(expectedRoot.ToString(), currentRoot.ToString());

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
                if (ignoredDifference.MismatchType == MismatchType.MissingInFirst)
                {
                    jsonPathWriter.Remove(resultRoot, ignoredDifference.MemberPath);

                    continue;
                }

                var source = MemberPathQuery.SelectToken(expectedRoot, ignoredDifference.MemberPath);

                if (source != null)
                {
                    jsonPathWriter.AddOrUpdate(resultRoot, ignoredDifference.MemberPath, source);
                }
            }

            // Use custom serializer settings to handle currentValue un-escaping
            var serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new CurrentValueJsonConverter() }
            };

            var output = JsonConvert.SerializeObject(resultRoot, serializerSettings);

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName,
                              output);
        }
    }
}