using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AspNetCore.Simple.MsTest.Sdk;

namespace Core.Test.Core
{
    /// <summary>
    /// An api may keep configuring its <see cref="JsonSerializerOptions"/> until they are first used -
    /// teaching STJ a polymorphic hierarchy through a type info modifier is the typical late step.
    /// The comparison must always see the options as they are NOW, never a copy taken before that.
    /// </summary>
    [TestClass]
    [TestCategory("JsonComparison")]
    [DoNotParallelize] // GlobalTestSdkSettings swaps the global settings.
    public sealed class JsonComparisonLateConfiguredOptionsTests
    {
        private const string Because = "The comparison serializes both sides through the declared base type. Without the api's polymorphism only the base properties are written, so every derived property silently drops out of the comparison.";

        private const string Fix = "Check ComparisonJsonOptions.ForComparison: it may only cache its copy once the source options are read only. A copy cached earlier is a snapshot that never sees a TypeInfoResolver configured afterwards.";

        [TestMethod]
        public void ShouldCompareDerivedProperties_WhenPolymorphismIsConfiguredAfterFirstComparison()
        {
            // A dictionary key policy forces ForComparison onto its copy path.
            var apiJsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };

            using var globalSettings = GlobalTestSdkSettings.Use(settings => settings.JsonSerializerOptions = apiJsonSerializerOptions);

            Animal expected = new Dog
            {
                Name = "Rex",
                Toy = null
            };

            Animal current = new Dog
            {
                Name = "Rex",
                Toy = "ball"
            };

            // 1. A comparison before the api has finished configuring its options - only the base
            //    properties are known, so it passes.
            Assert.That.ObjectsAreEqual(expected, current);

            // 2. The api adds its polymorphism afterwards - the options were never used for serialization yet
            apiJsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(AddAnimalPolymorphism);

            // 3. Every comparison from now on has to see the derived properties
            var differences = ImmutableList<Difference>.Empty;

            Assert.That.ObjectsAreEqual(expected,
                                        current,
                                        differenceFunc: found =>
                                        {
                                            differences = found;

                                            return [];
                                        });

            Assert.That.Any(differences,
                            difference => difference.MemberPath.Contains("toy", StringComparison.OrdinalIgnoreCase),
                            predicateDescription: "a difference on the derived property 'toy'",
                            because: Because,
                            fix: Fix);
        }

        private static void AddAnimalPolymorphism(JsonTypeInfo jsonTypeInfo)
        {
            if (jsonTypeInfo.Type != typeof(Animal))
            {
                return;
            }

            var polymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "kind"
            };

            polymorphismOptions.DerivedTypes.Add(new JsonDerivedType(typeof(Dog), "dog"));

            jsonTypeInfo.PolymorphismOptions = polymorphismOptions;
        }

        internal abstract record Animal
        {
            public required string Name { get; init; }
        }

        internal sealed record Dog : Animal
        {
            public string? Toy { get; init; }
        }
    }
}
