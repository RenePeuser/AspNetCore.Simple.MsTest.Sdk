using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http
{
    /// <summary>
    /// Extension method for registering HTTP failure output strategies.
    /// </summary>
    internal static class AddHttpFailureOutputStrategyExtension
    {
        public static void AddHttpFailureOutputStrategy(this IServiceCollection services)
        {
            // Register all specific failure strategies
            services.AddSingletonIfNotExists<IHttpFailureOutputStrategy, StatusCodeMismatchOutputStrategy>();
            services.AddSingletonIfNotExists<IHttpFailureOutputStrategy, SchemaMismatchOutputStrategy>();
            services.AddSingletonIfNotExists<IHttpFailureOutputStrategy, SnapshotMismatchOutputStrategy>();
            services.AddSingletonIfNotExists<IHttpFailureOutputStrategy, ContentTypeMismatchOutputStrategy>();

            // Register the default fallback strategy (injected separately, not via IEnumerable<T>)
            services.AddSingletonIfNotExists<DefaultHttpFailureOutputStrategy>();

            // Register the orchestrator builder
            services.AddSingletonIfNotExists<IHttpFailureOutputBuilder, HttpFailureOutputBuilder>();
        }
    }
}