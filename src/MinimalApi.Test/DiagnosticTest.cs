using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StrategyPattern.Evolution;

namespace MinimalApi.Test
{
    [TestClass]
    [TestCategory("Minimal Api")]
    public class DiagnosticTest : ApiTestBase
    {
        [TestMethod]
        public void Check_Endpoints_Registered()
        {
            // Get the services from the test server
            var services = Client.GetType().GetProperty("Handler", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(Client);
            
            Console.WriteLine($"Services type: {services?.GetType().Name}");
            
            // Try to access the service provider through reflection or other means
            // This is just for debugging
            Assert.IsNotNull(services);
        }
    }
}
