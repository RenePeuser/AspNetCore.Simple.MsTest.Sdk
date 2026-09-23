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
    /// Implementation that reads the output mode from TestSdkSettings.
    /// The settings are injected via DI and the mode is resolved from configuration
    /// (environment variable, appsettings.json, etc.).
    /// </summary>
    internal sealed class OutputModeService : IOutputModeService
    {
        private readonly OutputMode _mode;

        public OutputModeService(TestSdkSettings settings)
        {
            _mode = settings.OutputMode;

            SdkTrace.WriteLine($"[OutputModeService] Initialized with mode: {_mode}");
        }

        public OutputMode GetOutputMode()
        {
            return _mode;
        }
    }
}
