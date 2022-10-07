using System;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public abstract class IntegrationTestBase<TStartup> : DisposableBase
        where TStartup : class
    {
        private readonly IntegrationTestWebApplicationFactory<TStartup> _webApplicationFactory;

        protected IntegrationTestBase(params (string name, string value)[] environmentVariables) : this("Development", environmentVariables)
        {
        }

        protected IntegrationTestBase(string environmentName, params (string name, string value)[] environmentVariables)
        {
            // Create this with new, is not a fault, the reason is to keep the test class more cleaner.
            _webApplicationFactory = new IntegrationTestWebApplicationFactory<TStartup>(this, environmentName, environmentVariables);
            Client = _webApplicationFactory.CreateClient();
            ServiceProvider = _webApplicationFactory.Services;
        }

        protected IServiceProvider ServiceProvider { get; }

        protected HttpClient Client { get; }

        protected abstract Task<string> GetAuthTokenAsync();

        protected override void DisposeManagedResources()
        {
            _webApplicationFactory.Dispose();
            Client.Dispose();
        }

        public virtual void ConfigureAppConfiguration(WebHostBuilderContext webHostBuilderContext, IConfigurationBuilder configurationBuilder)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddMediatR(this.GetType().Assembly);
            // Gives the possibility to do test environment specific configurations
        }
    }
}
