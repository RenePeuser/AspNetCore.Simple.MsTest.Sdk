using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Extensions.Pack;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class ApiTestBase<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly Action<IServiceCollection, IConfiguration> _registerServices;

        private readonly (string name, string value)[] _environmentVariables;
        private readonly Assembly _callingAssembly;

        public ApiTestBase(string environmentName,
                           Action<IServiceCollection, IConfiguration> registerServices,
                           params (string name, string value)[] environmentVariables)
        {
            EnvironmentName = environmentName;
            _registerServices = registerServices;
            _environmentVariables = environmentVariables;
            _callingAssembly = Assembly.GetCallingAssembly();
        }

        public string EnvironmentName { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            foreach (var environmentVariable in _environmentVariables)
            {
                Environment.SetEnvironmentVariable(environmentVariable.name, environmentVariable.value);
            }

            var testDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            var findAllTestSettings = testDirectory.EnumerateFiles("appsettings.*.json", SearchOption.AllDirectories).ToList();
            var environmentSpecificSettings = findAllTestSettings.Where(file => file.FullName.Contains(EnvironmentName)).ToList();
            var testSettings = findAllTestSettings.Where(file => file.Name.Contains("test", StringComparison.OrdinalIgnoreCase));
            var settingsToRegister = environmentSpecificSettings.Concat(testSettings);


            var embeddedAppSettings = _callingAssembly.GetManifestResourceNames().Where(item => item.EndsWith(".json", StringComparison.OrdinalIgnoreCase) &&
                                                                                        item.Contains("appsettings", StringComparison.OrdinalIgnoreCase)).ToList();

            IConfiguration configuration = null!;

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                foreach (var testSettingsFile in settingsToRegister)
                {
                    configurationBuilder.AddJsonFile(testSettingsFile.FullName, true);
                }
                
                configurationBuilder.AddUserSecrets(_callingAssembly);
                configurationBuilder.AddEnvironmentVariables();

                configuration = configurationBuilder.Build();
            });


            builder.ConfigureServices(services =>
            {
                _registerServices(services, configuration);
            });

            builder.UseEnvironment(EnvironmentName);
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var environmentVariable in _environmentVariables)
            {
                Environment.SetEnvironmentVariable(environmentVariable.name, null);
            }

            base.Dispose(disposing);
        }
    }
}
