using System;
using System.IO;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddOverwriteAllResponseWriterExtension
    {
        public static void AddOverwriteAllResponseWriter(this IServiceCollection services)
        {
            services.AddParameterReplacer();
            services.AddSingletonIfNotExists<ISpecificResponseWriter, OverwriteAllResponseWriter>();
        }
    }

    internal sealed class OverwriteAllResponseWriter(IParameterReplacer parameterReplacementService,
                                                     SnapshotPlaceholderGuard snapshotPlaceholderGuard) : ISpecificResponseWriter
    {
        public bool CanHandle(WriteResponseRequest context)
        {
            var canHandle = context.ExpectedResult.EmbeddedFileName.EndsWith(".json") &&
                            context.ExpectedResult.EmbeddedFile.IsNotNull() &&
                            (context.Mode == ResponseWriteMode.OverwriteAll || context.ExpectedResult.EmbeddedFile.NotExists());

            SdkTrace.WriteLine($"[OverwriteAllResponseWriter.CanHandle] Mode={context.Mode}, FileName={context.ExpectedResult.EmbeddedFileName}, CanHandle={canHandle}");

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

            var result = parameterReplacementService.ReplaceWithPlaceholders(context.CurrentResponseAsString, context.Parameters);

            // A numeric parameter stands bare in the snapshot and is not json Newtonsoft can read - see
            // PlaceholderJson. Without this the shape lookup below silently found "no shape" and the file
            // was replaced wholesale instead of keeping the bare body it already used.
            result = PlaceholderJson.MakeParseable(result);

            // An existing snapshot keeps its shape - see SnapshotShape. A file that is created right
            // here has no shape yet and gets the envelope.
            var targetExists = context.ExpectedResult.EmbeddedFile?.Exists ?? false;

            if (targetExists)
            {
                result = SnapshotShape.MatchExisting(result, PlaceholderJson.MakeParseable(context.ExpectedResult.Content));
            }

            //// New we can have also indexer properties. Values[0] -> Values[$Index$]
            //foreach (var parameter in context.Parameters)
            //{
            //    result = GlobalRegex.IndexReplacement().Replace(result, $"[{parameter.key}]");
            //}

            var targetFile = context.ExpectedResult.EmbeddedFile!;

            // A brand new snapshot can sit in a folder that does not exist yet.
            if (targetFile.Directory is { Exists: false })
            {
                targetFile.Directory.Create();
            }

            var output = RestoreSpelling(Indent(result), targetExists ? context.ExpectedResult.Content : null);

            // See SnapshotPlaceholderGuard. A file created by this run has no placeholders yet and passes.
            snapshotPlaceholderGuard.EnsureNoPlaceholderIsLost(context.ExpectedResult,
                                                               targetExists ? context.ExpectedResult.Content : null,
                                                               output,
                                                               context.Parameters);

            File.WriteAllText(targetFile.FullName, output);
        }

        /// <summary>
        /// Gives every placeholder the spelling the file being updated used for it - see
        /// SnapshotPlaceholderRestorer. A snapshot created by this very run has no spelling to preserve, so
        /// everything falls back to the quoted form.
        /// </summary>
        private static string RestoreSpelling(string json,
                                              string? existingContent)
        {
            if (existingContent.IsNotNullOrWhiteSpace())
            {
                try
                {
                    var tree = JToken.Parse(json);
                    SnapshotPlaceholderRestorer.ApplyOriginalSpelling(tree, existingContent);
                    json = tree.ToString(Formatting.Indented);
                }
#pragma warning disable CA1031
                catch (Exception)
#pragma warning restore CA1031
                {
                    // Not parseable as json (e.g. a text snapshot) - nothing to walk.
                }
            }

            return PlaceholderJson.RestoreBareMarkers(PlaceholderJson.RestoreRemainingSentinelsAsQuoted(json));
        }

        /// <summary>
        /// The current response arrives minified. Snapshots are read and reviewed by hand and every
        /// later write goes through the <see cref="DifferenceResponseWriter"/> which indents, so a newly
        /// created snapshot has to be indented as well - otherwise the first real diff rewrites the whole file.
        /// </summary>
        private static string Indent(string json)
        {
            try
            {
                using var reader = new JsonTextReader(new StringReader(json))
                {
                    // Keep dates, times and big numbers exactly as the api wrote them.
                    DateParseHandling = DateParseHandling.None,
                    FloatParseHandling = FloatParseHandling.Decimal
                };

                return JToken.Load(reader).ToString(Formatting.Indented);
            }
#pragma warning disable CA1031
            catch (Exception)
#pragma warning restore CA1031
            {
                // Not parseable as json (e.g. a text snapshot) - write it as it is.
                return json;
            }
        }
    }
}