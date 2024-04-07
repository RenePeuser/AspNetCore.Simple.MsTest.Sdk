using System;
using System.IO;
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


        public ApiTestBase() : this("Development", (_, _) => { })
        {
        }

        public ApiTestBase(Action<IServiceCollection, IConfiguration> registerServices) : this("Development", registerServices)
        {
        }

        public ApiTestBase(string environmentName) : this(environmentName, (_, _) => { })
        {
        }

        public ApiTestBase(string environmentName,
                           Action<IServiceCollection, IConfiguration> registerServices,
                           params (string name, string value)[] environmentVariables)
        {
            EnvironmentName = environmentName;
            _registerServices = registerServices;
            _environmentVariables = environmentVariables;
        }

        public string EnvironmentName { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            foreach (var environmentVariable in _environmentVariables)
            {
                Environment.SetEnvironmentVariable(environmentVariable.name, environmentVariable.value);
            }

            var testSettingsPath = Path.Combine(Environment.CurrentDirectory,
                                                "Environments",
                                                EnvironmentName,
                                                "appsettings.test.json");

            var testSettingsFileInfo = new FileInfo(testSettingsPath);

            IConfiguration configuration = null!;

            builder.ConfigureAppConfiguration((_, configurationBuilder) =>
            {
                configurationBuilder.AddJsonFile(testSettingsPath, true);

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
