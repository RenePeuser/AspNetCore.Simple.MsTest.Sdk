using System;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test
{
    // The C# code generation feature (CSharpObjectResponseWriter / RoslynCodeManipulator) is not
    // finished - it rewrites the test source in place and does not survive a re-run yet. These tests
    // describe the intended behaviour and are meant to be enabled together with that feature.
    [Ignore("C# code generation is unfinished - see CSharpObjectResponseWriter")]
    [TestClass]
    public class GenerateCSharpObjectTests : ApiTestBase
    {
        [TestMethod]
        public async Task TestGenerateCSharpObjectFromEmptyAnonymous()
        {
            // Arrange: Create a simple person object to POST
            var personToCreate = new { };

            // Act & Assert: Pass an empty anonymous object as expected response
            // This should trigger the CSharpObjectResponseWriter to generate code
            var result = await Client.AssertPostAsync("api/v1/persons", personToCreate, new
            {
                content = new
                {
                    headers = new[]
                                                                                                                    {
                                                                                                                        new
                                                                                                                        {
                                                                                                                            key = "Content-Type",
                                                                                                                            value = new[] { "application/json; charset=utf-8" }
                                                                                                                        }
                                                                                                                    },
                    value = new
                    {
                        id = 0,
                        name = "John Doe",
                        firstName = (string?)null,
                        age = 30,
                        emails = (string[]?)null
                    }
                },
                statusCode = "OK",
                headers = Array.Empty<object>(),
                trailingHeaders = Array.Empty<object>(),
                isSuccessStatusCode = true
            },
                                                      true,
                                                      skipEndpointValidation: true).ConfigureAwait(false); // Manually enable writeResponse for prototype

            // Verify the result is not null (the API returned something)
            Assert.That.IsNotNull(result,
                                  because: "An empty anonymous object as expected response is the trigger for CSharpObjectResponseWriter. The call still has to come back with a response - a null one means the assert pipeline gave up before the writer ever ran.",
                                  fix: "Check that AssertPostAsync returns the response even when writeResponse is on, and that CSharpObjectResponseWriter does not swallow it.");
        }

        [TestMethod]
        public void TestCSharpCodeGeneratorDirectly()
        {
            // Direct test of the CSharpCodeGenerator via DI
            var generator = Services.GetRequiredService<ICSharpCodeGenerator>();

            var json = /*lang=json,strict*/ @"{
                ""id"": 1,
                ""name"": ""John Doe"",
                ""age"": 30,
                ""isActive"": true,
                ""tags"": [""developer"", ""tester""]
            }";

            var csharpCode = generator.GenerateAnonymousObjectInitializer(json, 12);

            // Verify the generated code contains expected properties
            const string because = "The generated initializer is pasted straight into the test source, so every json property has to appear with C# syntax and the right literal form - a number unquoted, a string quoted, a bool lowercase.";
            const string fix = "Check the per-JTokenType branches in CSharpCodeGenerator.GenerateAnonymousObjectInitializer - a missing property means its token type has no branch.";

            Assert.That.Contains(csharpCode, "id = 1", because: because,
                                 fix: fix);

            Assert.That.Contains(csharpCode, "name = \"John Doe\"", because: because,
                                 fix: fix);

            Assert.That.Contains(csharpCode, "age = 30", because: because,
                                 fix: fix);

            Assert.That.Contains(csharpCode, "isActive = true", because: because,
                                 fix: fix);

            Assert.That.Contains(csharpCode, "tags = ", because: because,
                                 fix: fix);

            // Output for visual inspection
            Console.WriteLine("Generated C# Code:");
            Console.WriteLine(csharpCode);
        }

        [TestMethod]
        public void TestEmptyAnonymousObjectDetection()
        {
            // Test the detector via DI
            var detector = Services.GetRequiredService<IEmptyAnonymousObjectDetector>();

            // Test 1: Empty anonymous object should be detected
            var empty = new { };
            var isEmptyDetected = detector.IsEmptyAnonymousObject(empty, "new { }");

            Assert.That.IsTrue(isEmptyDetected,
                               because: "'new { }' is the opt-in signal for code generation. Not recognising it means the writer never runs and the test just compares against an empty object.",
                               fix: "EmptyAnonymousObjectDetector has to treat a type with zero properties as empty - check the reflection branch, not only the expression one.");

            // Test 2: Non-empty anonymous object should NOT be detected
            var notEmpty = new { id = 1 };
            var isNotEmptyDetected = detector.IsEmptyAnonymousObject(notEmpty, "new { id = 1 }");

            Assert.That.IsFalse(isNotEmptyDetected,
                                because: "A populated anonymous object is a real expectation the author wrote by hand. Mistaking it for the generation trigger would overwrite their test source.",
                                fix: "EmptyAnonymousObjectDetector must return false as soon as the object has at least one property or the expression contains anything between the braces.");

            // Test 3: Expression detection
            var emptyByExpression = detector.IsEmptyAnonymousObject(new object(), "new {}");

            Assert.That.IsTrue(emptyByExpression,
                               because: "The detector also has to recognise the trigger from the caller expression alone - 'new {}' without spaces is the same opt-in and reflection cannot tell a plain 'new object()' apart.",
                               fix: "Normalise whitespace before matching the expression in EmptyAnonymousObjectDetector; 'new {}' and 'new { }' have to be treated alike.");
        }
    }
}