using System;
using System.Threading;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Microsoft.Extensions.Hosting;

namespace Core.Test.Core
{
    /// <summary>
    /// Swaps the global <see cref="TestSdkSettings"/> for the duration of one test - through the public
    /// contract only: a started host registered via <c>AddAssertableHttpClient</c> hands its provider to
    /// the static asserts. Disposing starts a host with the default settings, so later tests see defaults.
    /// Tests using it must be <see cref="DoNotParallelizeAttribute"/>.
    /// </summary>
    internal sealed class GlobalTestSdkSettings : IDisposable
    {
        private static readonly Lock Gate = new();

        private static IHost? _current;

        private GlobalTestSdkSettings(Action<TestSdkSettings>? configureSettings)
        {
            StartHost(configureSettings);
        }

        public static GlobalTestSdkSettings Use(Action<TestSdkSettings> configureSettings)
        {
            return new GlobalTestSdkSettings(configureSettings);
        }

        public void Dispose()
        {
            StartHost(configureSettings: null);
        }

        private static void StartHost(Action<TestSdkSettings>? configureSettings)
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Services.AddAssertableHttpClient(builder.Configuration, configureSettings);

            var host = builder.Build();

            host.Start();

            lock (Gate)
            {
                // The previous host is no longer handed out - the new one replaced it on start.
                _current?.Dispose();
                _current = host;
            }
        }
    }
}
