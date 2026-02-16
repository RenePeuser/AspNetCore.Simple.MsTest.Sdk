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

        public required (string key, object? Value)[] Parameters { get; init; }

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
    }
}
