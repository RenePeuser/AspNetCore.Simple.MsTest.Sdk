using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddResponseWriterExtension
    {
        public static void AddResponseWriter(this IServiceCollection services)
        {
            services.AddDifferenceResponseWriter();
            services.AddOverwriteAllResponseWriter();
            services.AddCSharpObjectResponseWriter();

            services.AddSingletonIfNotExists<IResponseWriter, ResponseWriter>();
        }
    }

    internal enum ResponseWriteMode
    {
        DifferencesOnly,

        OverwriteAll,

        GenerateCSharpObject
    }

    internal sealed record WriteResponseRequest
    {
        public required string CurrentResponseAsString { get; init; }

        public required EmbeddedFileInfo ExpectedResult { get; init; }

#pragma warning disable CA1819
        public required (string key, object? Value)[] Parameters { get; init; }
#pragma warning restore CA1819

        public required Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; init; }

        public Predicate<Difference> DifferenceFilter { get; init; } = static _ => true;

        /// <summary>
        /// Arrays whose element order carries no meaning, so the merge sees the same set of
        /// differences the assert saw. Without it a snapshot whose array is merely ordered
        /// differently would be rewritten on every recording.
        /// </summary>
        public Predicate<JsonArrayContext>? OrderIndependentArrayFilter { get; init; }

        public required Assembly CallingAssembly { get; init; }

        public required ResponseWriteMode Mode { get; init; } = ResponseWriteMode.DifferencesOnly;

        public required string CallerFilePath { get; init; }

        public required int CallerLineNumber { get; init; }

        public required string ExpectedResultParameterName { get; init; }

        public required Type ExpectedType { get; init; }

        public required object? ExpectedObject { get; init; }
    }

    internal interface ISpecificResponseWriter
    {
        bool CanHandle(WriteResponseRequest writeResponseRequest);

        void Write(WriteResponseRequest writeResponseRequest);
    }

    internal interface IResponseWriter
    {
        void Write(WriteResponseRequest writeResponseRequest);

        /// <summary>
        /// Writes the response using the context's properties combined with computed values.
        /// This is the preferred method for context-based operations.
        /// </summary>
        /// <param name="context">The assertion context containing parameters, assembly, and difference function</param>
        /// <param name="currentResponseAsString">The current response as JSON string (computed)</param>
        /// <param name="expectedResult">The expected result file info (computed)</param>
        /// <param name="mode">The write mode (optional, defaults to DifferencesOnly)</param>
        void Write(IObjectAssertContext context,
                   string currentResponseAsString,
                   EmbeddedFileInfo expectedResult,
                   ResponseWriteMode mode = ResponseWriteMode.DifferencesOnly);
    }

    internal sealed class ResponseWriter(IEnumerable<ISpecificResponseWriter> specificResponseWriters) : IResponseWriter
    {
        public void Write(WriteResponseRequest writeResponseRequest)
        {
            var writersCanHandle = specificResponseWriters.Where(w => w.CanHandle(writeResponseRequest)).ToImmutableList();

            SdkTrace.WriteLine($"[ResponseWriter.Write] Mode={writeResponseRequest.Mode}, WritersCanHandle={writersCanHandle.Count}");

            if (writersCanHandle.IsEmpty())
            {
                // Reaching this method means write response was requested. Silently doing nothing is how
                // a broken snapshot pipeline hides for months: the developer sees a red diff, re-runs with
                // write response on, nothing changes, and there is no hint why. For a *.json snapshot the
                // expectation is unambiguous - some writer has to claim it.
                var isSnapshotFile = writeResponseRequest.ExpectedResult
                                                         .EmbeddedFileName
                                                         .EndsWith(".json", StringComparison.OrdinalIgnoreCase);

                if (isSnapshotFile)
                {
                    var declined = specificResponseWriters.Select(writer => $"  - {writer.GetType().Name}")
                                                          .ToImmutableList();

                    throw new InvalidOperationException($"""
                                                         Write response was requested but no writer accepted the snapshot.

                                                         Snapshot : {writeResponseRequest.ExpectedResult.EmbeddedFileName}
                                                         File     : {writeResponseRequest.ExpectedResult.EmbeddedFile?.FullName ?? "<not resolved>"}
                                                         Mode     : {writeResponseRequest.Mode}

                                                         Writers that declined:
                                                         {string.Join(Environment.NewLine, declined)}

                                                         A null file means the snapshot path could not be resolved - that is the usual cause.
                                                         """);
                }

                // Inline expectations (an anonymous object, raw json) have no file to write to.
                SdkTrace.WriteLine("[ResponseWriter.Write] No writers can handle this request");

                return;
            }

            if (writersCanHandle.Count > 1)
            {
                throw new InvalidOperationException($"Multiple ISpecificResponseWriter found for mode '{writeResponseRequest.Mode}'.");
            }

            SdkTrace.WriteLine($"[ResponseWriter.Write] Calling Write on {writersCanHandle[0].GetType().Name}");
            writersCanHandle[0].Write(writeResponseRequest);
        }

        public void Write(IObjectAssertContext context,
                          string currentResponseAsString,
                          EmbeddedFileInfo expectedResult,
                          ResponseWriteMode mode = ResponseWriteMode.DifferencesOnly)
        {
            var request = new WriteResponseRequest
            {
                CallingAssembly = context.CallingAssembly,
                DifferenceFunc = context.DifferenceFunc,
                DifferenceFilter = context.DifferenceFilter,
                OrderIndependentArrayFilter = context.OrderIndependentArrayFilter,
                CurrentResponseAsString = currentResponseAsString,
                ExpectedResult = expectedResult,
                Parameters = context.Parameters,
                Mode = mode,
                CallerFilePath = context.CallerFilePath,
                CallerLineNumber = context.CallerLineNumber,
                ExpectedResultParameterName = context.ExpectedResultParameterName,
                ExpectedType = context.ExpectedType,
                ExpectedObject = null // We don't have access to Expected here in the non-generic interface
            };

            Write(request);
        }
    }
}