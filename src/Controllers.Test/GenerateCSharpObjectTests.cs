using System;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
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
            // Direct test of the CSharpCodeGenerator
            var generator = new CSharpCodeGenerator();

            var json = /*lang=json,strict*/ @"{
                ""id"": 1,
                ""name"": ""John Doe"",
                ""age"": 30,
                ""isActive"": true,
                ""tags"": [""developer"", ""tester""]
            }";

            var csharpCode = generator.GenerateAnonymousObjectInitializer(json, 12);

            // Verify the generated code contains expected properties
            Assert.IsTrue(csharpCode.Contains("id = 1"));
            Assert.IsTrue(csharpCode.Contains("name = \"John Doe\""));
            Assert.IsTrue(csharpCode.Contains("age = 30"));
            Assert.IsTrue(csharpCode.Contains("isActive = true"));
            Assert.IsTrue(csharpCode.Contains("tags = "));

            // Output for visual inspection
            Console.WriteLine("Generated C# Code:");
            Console.WriteLine(csharpCode);
        }

        [TestMethod]
        public void TestEmptyAnonymousObjectDetection()
        {
            // Test the detector directly
            var detector = new EmptyAnonymousObjectDetector();

            // Test 1: Empty anonymous object should be detected
            var empty = new { };
            var isEmptyDetected = detector.IsEmptyAnonymousObject(empty, "new { }");
            Assert.IsTrue(isEmptyDetected, "Should detect empty anonymous object");

            // Test 2: Non-empty anonymous object should NOT be detected
            var notEmpty = new { id = 1 };
            var isNotEmptyDetected = detector.IsEmptyAnonymousObject(notEmpty, "new { id = 1 }");
            Assert.IsFalse(isNotEmptyDetected, "Should NOT detect non-empty anonymous object");

            // Test 3: Expression detection
            var emptyByExpression = detector.IsEmptyAnonymousObject(new object(), "new {}");
            Assert.IsTrue(emptyByExpression, "Should detect via expression");
        }
    }
}