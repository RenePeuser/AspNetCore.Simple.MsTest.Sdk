using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class IntegrationTestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private readonly (string name, string value)[] _environmentVariables;
        private readonly IntegrationTestBase<TStartup> _testBase;

        public IntegrationTestWebApplicationFactory(IntegrationTestBase<TStartup> testBase, string environmentName, params (string name, string value)[] environmentVariables)
        {
            _testBase = testBase;
            EnvironmentName = environmentName;
            _environmentVariables = environmentVariables;
        }

        public string EnvironmentName { get; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            foreach (var environmentVariable in _environmentVariables)
            {
                Environment.SetEnvironmentVariable(environmentVariable.name, environmentVariable.value);
            }

            builder.ConfigureAppConfiguration(_testBase.ConfigureAppConfiguration);
            builder.ConfigureServices(_testBase.ConfigureServices);
            builder.UseEnvironment(EnvironmentName);
        }
    }
}
