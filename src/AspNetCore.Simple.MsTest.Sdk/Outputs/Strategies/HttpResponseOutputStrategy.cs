using System.Collections.Immutable;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpResponseOutputStrategyExtension
    {
        public static void AddHttpResponseOutputStrategy(this IServiceCollection services)
        {
            // Register dependencies
            services.AddSnapshotTestOutputBuilder();

            // Register service itself
            services.AddSingletonIfNotExists<IAssertOutputStrategy, HttpResponseOutputStrategy>();
        }
    }

    /// <summary>
    /// Output strategy for HTTP response contexts.
    /// Uses SnapshotTestOutputBuilder to create comprehensive HTTP assertion failure output
    /// including test info, HTTP call details, differences, JSON comparison, and curl reproduction.
    /// </summary>
    internal sealed class HttpResponseOutputStrategy(ISnapshotTestOutputBuilder snapshotTestOutputBuilder)
        : AssertOutputStrategyBase<IHttpResponseContext>
    {
        protected override string BuildOutput(IHttpResponseContext context,
                                              ImmutableList<Difference> differences,
                                              string expectedJson,
                                              string currentJson)
        {
            // Delegate to SnapshotTestOutputBuilder - now we have non-generic Build method!
            return snapshotTestOutputBuilder.Build(context, differences, expectedJson,
                                                   currentJson);
        }
    }
}
