using System;
using System.IO;
using System.Linq;
using System.Reflection;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class ApiTestBase<TStartup>(string environmentName,
                                       Action<IServiceCollection, IConfiguration> registerServices,
                                       params (string name, object? value)[] environmentVariables) : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        /// <summary>
        /// The test assembly that constructed this factory. Captured here because it is the last
        /// point where the consumer is still the caller - from ConfigureWebHost onwards every frame
        /// belongs to the sdk, and Assembly.GetCallingAssembly() would just answer "the sdk".
        /// </summary>
        private readonly Assembly _consumerAssembly = Assembly.GetCallingAssembly();

        private Assembly CallingAssembly => GetType().Assembly;

        public string EnvironmentName { get; } = environmentName;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            foreach (var environmentVariable in environmentVariables)
            {
                Environment.SetEnvironmentVariable(environmentVariable.name, environmentVariable.value?.ToString());
            }

            var testDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            var findAllTestSettings = testDirectory.EnumerateFiles("appsettings.*.json", SearchOption.AllDirectories).ToList();
            var environmentSpecificSettings = findAllTestSettings.Where(file => file.FullName.Contains(EnvironmentName)).ToList();
            var testSettings = findAllTestSettings.Where(file => file.Name.Contains("test", StringComparison.OrdinalIgnoreCase));
            var settingsToRegister = environmentSpecificSettings.Concat(testSettings);

            var embeddedAppSettings = CallingAssembly.GetManifestResourceNames().Where(item => item.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                                                                                               item.Contains("appsettings", StringComparison.OrdinalIgnoreCase)).ToList();

            IConfiguration configuration = null!;

            builder.ConfigureAppConfiguration((_,
                                               configurationBuilder) =>
            {
                foreach (var testSettingsFile in settingsToRegister)
                {
                    configurationBuilder.AddJsonFile(testSettingsFile.FullName, true);
                }

                configurationBuilder.AddUserSecrets(CallingAssembly);
                configurationBuilder.AddEnvironmentVariables();

                configuration = configurationBuilder.Build();
            });

            builder.ConfigureServices(services =>
            {
                registerServices(services, configuration);

                // NEW self registration
                services.AddAssertableHttpClient(configuration, _consumerAssembly);

                // NEW self setup - the static extensions resolve from this provider on every assert,
                // so it must outlive this callback and is deliberately never disposed here.
#pragma warning disable CA2000
                var serviceProvider = services.BuildServiceProvider();
#pragma warning restore CA2000
                HttpClientAssertExtensions.Setup(serviceProvider);

            });

            builder.UseEnvironment(EnvironmentName);
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var environmentVariable in environmentVariables)
            {
                Environment.SetEnvironmentVariable(environmentVariable.name, null);
            }

            base.Dispose(disposing);
        }
    }
}