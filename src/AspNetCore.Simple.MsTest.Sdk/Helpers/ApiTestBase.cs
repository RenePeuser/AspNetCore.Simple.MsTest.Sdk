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
                services.AddAssertableHttpClient(configuration);

                // NEW self setup
                using var serviceProvider = services.BuildServiceProvider();
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