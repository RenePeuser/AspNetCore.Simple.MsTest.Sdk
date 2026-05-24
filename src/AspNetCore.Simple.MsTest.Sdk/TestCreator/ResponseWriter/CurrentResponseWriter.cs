using System.Collections.Immutable;
using System.Reflection;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddResponseWriterExtension
    {
        public static void AddResponseWriter(this IServiceCollection services)
        {
            services.AddDifferenceResponseWriter();
            services.AddOverwriteAllResponseWriter();

            services.AddSingletonIfNotExists<IResponseWriter, ResponseWriter>();
        }
    }

    public enum ResponseWriteMode
    {
        DifferencesOnly,

        OverwriteAll
    }

    public sealed record WriteResponseRequest
    {
        public required string CurrentResponseAsString { get; init; }

        public required EmbeddedFileInfo ExpectedResult { get; init; }

#pragma warning disable CA1819
        public required (string key, object? Value)[] Parameters { get; init; }
#pragma warning restore CA1819

        public required Func<ImmutableList<Difference>, IEnumerable<Difference>> DifferenceFunc { get; init; }

        public required Assembly CallingAssembly { get; init; }

        public required ResponseWriteMode Mode { get; init; } = ResponseWriteMode.DifferencesOnly;
    }

    public interface ISpecificResponseWriter
    {
        bool CanHandle(WriteResponseRequest writeResponseRequest);

        void Write(WriteResponseRequest writeResponseRequest);
    }

    public interface IResponseWriter
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

    public sealed class ResponseWriter(IEnumerable<ISpecificResponseWriter> specificResponseWriters) : IResponseWriter
    {
        public void Write(WriteResponseRequest writeResponseRequest)
        {
            var writersCanHandle = specificResponseWriters.Where(w => w.CanHandle(writeResponseRequest)).ToImmutableList();

            if (writersCanHandle.IsEmpty())
            {
                return;
            }

            if (writersCanHandle.Count > 1)
            {
                throw new InvalidOperationException($"Multiple ISpecificResponseWriter found for mode '{writeResponseRequest.Mode}'.");
            }

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
                              CurrentResponseAsString = currentResponseAsString,
                              ExpectedResult = expectedResult,
                              Parameters = context.Parameters,
                              Mode = mode
                          };

            Write(request);
        }
    }
}