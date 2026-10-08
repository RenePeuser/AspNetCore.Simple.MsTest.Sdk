using System;
using System.Collections.Immutable;
using System.Text.Json.Nodes;
using AspNetCore.Simple.MsTest.Sdk;
using Newtonsoft.Json.Linq;

namespace Core.Test.Core
{
    /// <summary>
    /// The differences the sdk finds between two json documents - read through the public contract only:
    /// <c>Assert.That.ObjectsAreEqual</c> hands every difference to the per-assert <c>differenceFunc</c>,
    /// which records them and drops them, so the assert itself never fails.
    /// </summary>
    internal static class JsonDifferences
    {
        public static ImmutableList<Difference> Of(string expectedJson,
                                                   string currentJson)
        {
            var differences = ImmutableList<Difference>.Empty;

            Assert.That.ObjectsAreEqual(JsonNode.Parse(expectedJson),
                                        JsonNode.Parse(currentJson),
                                        differenceFunc: found =>
                                        {
                                            differences = found;

                                            return [];
                                        });

            return differences;
        }

        public static ImmutableList<Difference> Of(JToken expected,
                                                   JToken current)
        {
            return Of(expected.ToString(), current.ToString());
        }

        /// <summary>
        /// Same, with the global <see cref="TestSdkSettings.OrderIndependentArrayFilter" /> set for this one
        /// comparison. Tests calling it must be <see cref="DoNotParallelizeAttribute" />.
        /// </summary>
        public static ImmutableList<Difference> Of(string expectedJson,
                                                   string currentJson,
                                                   Predicate<JsonArrayContext>? isOrderIndependentArray)
        {
            using var globalSettings = GlobalTestSdkSettings.Use(settings => settings.OrderIndependentArrayFilter = isOrderIndependentArray);

            return Of(expectedJson, currentJson);
        }
    }
}
