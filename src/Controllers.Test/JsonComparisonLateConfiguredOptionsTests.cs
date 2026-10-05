using System;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using AspNetCore.Simple.MsTest.Sdk;
using AspNetCore.Simple.MsTest.Sdk.Comparison;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test
{
    /// <summary>
    /// An api may keep configuring its <see cref="JsonSerializerOptions"/> until they are first used -
    /// teaching STJ a polymorphic hierarchy through a type info modifier is the typical late step.
    /// The comparison must always see the options as they are NOW, never a copy taken before that.
    /// </summary>
    [TestClass]
    [TestCategory("JsonComparison")]
    public sealed class JsonComparisonLateConfiguredOptionsTests
    {
        private const string Because = "The comparison serializes both sides through the declared base type. Without the api's polymorphism only the base properties are written, so every derived property silently drops out of the diff and the assert passes on data it never looked at.";

        private const string Fix = "Check ComparisonJsonOptions.ForComparison: it may only cache its copy once the source options are read only. A copy cached earlier is a snapshot that never sees a TypeInfoResolver the api adds afterwards. Also check that no caller captures the result of ForComparison in a field.";

        [TestMethod]
        public void ShouldCompareDerivedProperties_WhenPolymorphismIsConfiguredAfterFirstComparison()
        {
            // A dictionary key policy forces ForComparison onto its copy path.
            var apiJsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };

            var services = new ServiceCollection();
            services.AddSingleton(apiJsonSerializerOptions);
            services.AddComparisonStrategy();

            var comparisonStrategy = services.BuildServiceProvider().GetRequiredService<IComparisonStrategy>();

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

            // 1. A comparison before the api has finished configuring its options
            comparisonStrategy.Compare(CreateContext(expected, current));

            // 2. The api adds its polymorphism afterwards - the options were never used for serialization yet
            apiJsonSerializerOptions.TypeInfoResolver = new DefaultJsonTypeInfoResolver().WithAddedModifier(AddAnimalPolymorphism);

            // 3. Every comparison from now on has to see the derived properties
            var result = comparisonStrategy.Compare(CreateContext(expected, current));

            Assert.That.Any(result.Differences,
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

        private static ObjectAssertContext<Animal> CreateContext(Animal expected,
                                                                 Animal current)
        {
            var context = new ObjectAssertContext<Animal>
            {
                CallerFilePath = string.Empty,
                CallerLineNumber = 0,
                CallerMemberName = string.Empty,
                CallingAssembly = typeof(JsonComparisonLateConfiguredOptionsTests).Assembly,
                Current = current,
                CurrentObject = current,
                CurrentResultParameterName = nameof(current),
                DifferenceFunc = differences => differences,
                Expected = expected,
                ExpectedObjectAsJson = string.Empty,
                ExpectedResultFile = new EmbeddedFileInfo(string.Empty, string.Empty, null),
                ExpectedResultParameterName = nameof(expected),
                ExpectedType = typeof(Animal),
                Parameters = [],
                ResolvedExpectedJson = null,
                TypeIsPrimitiveType = false,
                WriteResponse = false
            };

            return context;
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