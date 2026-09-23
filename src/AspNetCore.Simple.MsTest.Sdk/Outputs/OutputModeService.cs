using System;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddOutputModeServiceExtension
    {
        public static void AddOutputModeService(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IOutputModeService, OutputModeService>();
        }
    }

    /// <summary>
    /// Service responsible for determining the output mode from environment configuration.
    /// </summary>
    public interface IOutputModeService
    {
        /// <summary>
        /// Gets the configured output mode (Human, Ai, or Hybrid).
        /// </summary>
        OutputMode GetOutputMode();
    }

    /// <summary>
    /// Implementation that reads the output mode from environment variable on initialization.
    /// The mode is read once and cached for the lifetime of the service.
    /// </summary>
    internal sealed class OutputModeService : IOutputModeService
    {
        private readonly OutputMode _mode;

        public OutputModeService()
        {
            // Read environment variable following the SDK naming convention
            var envVar = Environment.GetEnvironmentVariable("AspNetCoreSimpleMsTestSdk__OutputMode");

            // Parse the value (case-insensitive), default to Human mode
            _mode = envVar?.ToLower() switch
            {
                "ai" => OutputMode.Ai,
                "hybrid" => OutputMode.Hybrid,
                "human" => OutputMode.Human,
                null => OutputMode.Human,  // No env var set
                _ => DetermineDefaultForInvalidValue(envVar)
            };

            SdkTrace.WriteLine($"[OutputModeService] Initialized with mode: {_mode} (env var: '{envVar ?? "<not set>"}')");
        }

        public OutputMode GetOutputMode()
        {
            return _mode;
        }

        private static OutputMode DetermineDefaultForInvalidValue(string invalidValue)
        {
            SdkTrace.WriteLine($"[OutputModeService] Invalid value '{invalidValue}' for AspNetCoreSimpleMsTestSdk__OutputMode. Defaulting to Human mode. Valid values: Human, Ai, Hybrid");

            return OutputMode.Human;
        }
    }
}
