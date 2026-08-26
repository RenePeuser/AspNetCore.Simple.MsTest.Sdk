using System;
using System.Collections.Generic;
using System.Linq;
using AspNetCore.Simple.MsTest.Sdk;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Test;

namespace Sdc.Console.Test.Startup
{
    [TestClass]
    [TestCategory("Startup - ServiceRegistrations")]
    public class ServiceRegistrationTests : ApiTestBase
    {
        [TestMethod]
        public void No_Multiple_Registrations_Should_Exists()
        {
            // MS have some internal registrations that we cannot change, so we need to whitelist them
            var whitelist = new[]
                            {
                                "IConfigureOptions`1", "IWebHostEnvironment", "IApplicationLifetime",
                                "ITelemetryModuleConfigurator", "IHostingEnvironment", "AuthorizationPolicyCache",
                                "UnexpectedDataChangeDetectedHandler", "StartupValidator", "IConfigureLoggerProviderBuilder",
                                "IConfigureMeterProviderBuilder", "IConfigureTracerProviderBuilder", "IDocumentProvider",
                                "NamedService`1"
                            };

            var duplicatedRegistrations = FindDuplicates().ToList();

            var filteredDuplicates = duplicatedRegistrations
                                     .Where(registration => whitelist.Contains(registration.Interface).IsFalse() &&
                                                            whitelist.Contains(registration.Implementation).IsFalse())
                                     .ToList();

            var table = ConsoleTable.From(filteredDuplicates).ToString();

            Assert.That.IsEmpty(filteredDuplicates,
                                because: "Every implementation must be registered exactly once. A duplicate registration means the container hands out a different instance than the one the code was written against - and with a scoped or singleton service that shows up much later as state that mysteriously does not persist.",
                                fix: $"Remove the duplicate registrations listed below, or add the type to the whitelist in this test if the framework registers it more than once on purpose.{Environment.NewLine}Total amount of service registrations: {ServiceCollection.Count}{Environment.NewLine}{Environment.NewLine}{table}");

            return;

            static IEnumerable<ServiceRegistrations> FindDuplicates()
            {
                var groupByType = ServiceCollection.GroupBy(s => s.ServiceType).ToList();
                var typesWithMultipleRegistrations = groupByType.Where(g => g.Count() > 1).ToList();

                foreach (var multipleRegistrations in typesWithMultipleRegistrations)
                {
                    // Case 1: ImplementationTypes exists
                    var implementationTypes = multipleRegistrations.Where(g => g.ImplementationType.IsNotNull()).GroupBy(g => g.ImplementationType!.FullName).Where(g => g.Count() > 1)
                                                                   .ToList();

                    if (implementationTypes.Count >= 1)
                    {
                        yield return new ServiceRegistrations(multipleRegistrations.Key.Name,
                                                              implementationTypes[0].Key,
                                                              implementationTypes[0].Count());
                    }

                    // Case 2: ImplementationInstances exists
                    var implementationInstances = multipleRegistrations.Where(g => g.ImplementationInstance.IsNotNull()).GroupBy(g => g.ImplementationInstance?.GetType().Name)
                                                                       .Where(g => g.Count() > 1).ToList();

                    if (implementationInstances.Count >= 1)
                    {
                        yield return new ServiceRegistrations(multipleRegistrations.Key.Name,
                                                              implementationInstances[0].Key,
                                                              implementationInstances[0].Count());
                    }
                }
            }
        }
    }

    internal sealed record ServiceRegistrations(string? Interface,
                                                string? Implementation,
                                                int Count);
}