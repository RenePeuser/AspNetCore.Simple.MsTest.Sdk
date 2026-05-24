using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Controllers.Test
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var testAppsettingsJson = Path.Combine(Environment.CurrentDirectory, "appsettings.test.json");

            builder.ConfigureAppConfiguration((_,
                                               configurationBuilder) => configurationBuilder.AddJsonFile(testAppsettingsJson, true));

            builder.ConfigureServices(services =>
            {
                // if we need to switch between services we have to do it here
            });
        }
    }

    public class IntegrationTestWebApplicationFactory<TStartup>(IntegrationTestBase<TStartup> testBase,
                                                                string environmentName,
                                                                params (string name, string value)[] environmentVariables) : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        public string EnvironmentName { get; } = environmentName;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            foreach (var environmentVariable in environmentVariables)
            {
                Environment.SetEnvironmentVariable(environmentVariable.name, environmentVariable.value);
            }

            builder.ConfigureAppConfiguration(testBase.ConfigureAppConfiguration);
            builder.ConfigureServices(testBase.ConfigureServices);
            builder.UseEnvironment(EnvironmentName);
        }
    }

    public abstract class IntegrationTestBase<TStartup> : DisposableBase
        where TStartup : class
    {
        private readonly IntegrationTestWebApplicationFactory<TStartup> _webApplicationFactory;

        protected IntegrationTestBase(string aspEnvironment,
                                      params (string name, string value)[] environmentVariables)
        {
            // Create this with new, is not a fault, the reason is to keep the test class more cleaner.
            EnvironmentName = aspEnvironment;
            _webApplicationFactory = new IntegrationTestWebApplicationFactory<TStartup>(this, EnvironmentName, environmentVariables);
            Client = _webApplicationFactory.CreateClient();
            ServiceProvider = _webApplicationFactory.Services;
        }

        protected string EnvironmentName { get; }

        protected IServiceProvider ServiceProvider { get; }

        protected HttpClient Client { get; }

        protected override void DisposeManagedResources()
        {
            _webApplicationFactory.Dispose();
            Client.Dispose();
        }

        public virtual void ConfigureAppConfiguration(WebHostBuilderContext webHostBuilderContext,
                                                      IConfigurationBuilder configurationBuilder)
        {
            // Gives the possibility to do test environment specific configurations
        }

        public virtual void ConfigureServices(IServiceCollection serviceCollection)
        {
            // Gives the possibility to do test environment specific configurations
        }
    }

    public abstract class DisposableBase : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

#pragma warning disable CA1063
        private void Dispose(bool disposing)
#pragma warning restore CA1063
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                DisposeManagedResources();
            }

            _disposed = true;
        }

        protected abstract void DisposeManagedResources();

        ~DisposableBase()
        {
            Dispose(false);
        }
    }
}