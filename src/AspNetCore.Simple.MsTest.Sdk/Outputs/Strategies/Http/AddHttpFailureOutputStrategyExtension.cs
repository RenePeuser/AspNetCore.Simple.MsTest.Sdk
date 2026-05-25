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
            // Register the helper
            services.AddHttpFailureOutputHelper();

            // Register all specific failure strategies
            services.AddStatusCodeMismatchOutputStrategy();
            services.AddSchemaMismatchOutputStrategy();
            services.AddSnapshotMismatchOutputStrategy();
            services.AddContentTypeMismatchOutputStrategy();

            // Register the default fallback strategy
            services.AddDefaultHttpFailureOutputStrategy();

            // Register the orchestrator builder
            services.AddHttpFailureOutputBuilder();
        }
    }
}