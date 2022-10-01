using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class PostWithBodyTestCreator : ISpecificTestCreator
    {
        private readonly ILogger<PostWithBodyTestCreator> _logger;

        private readonly string TestTemplate = @"
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.Assert$httpMethod$$error$Async<$responseType$>(""$url$"",
                                                                         $payload$,
                                                                         $response$);
        }
";

        private readonly string NoPayloadTestTemplate = @"
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.Assert$httpMethod$$error$Async<$responseType$>(""$url$"",                                       
                                                                         $response$);
        }
";

        public PostWithBodyTestCreator(ILogger<PostWithBodyTestCreator> logger)
        {
            _logger = logger;
        }

        public bool CanCreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo)
        {
            return true;
        }

        public string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo)
        {
            var typeName = GetTypeName(responseInfo.ResponseType);

            var errorPlaceHolder = responseInfo.StatusCode is >= 200 and < 300 ? string.Empty : "Error";

            var template = string.IsNullOrWhiteSpace(requestInfo.Body) ? NoPayloadTestTemplate : TestTemplate;
            var httpMethodName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(requestInfo.HttpMethod.ToLowerInvariant());


            var test = template.Replace("$url$", requestInfo.Url)
                               .Replace("$payload$", ToLiteral(requestInfo.Body))
                               .Replace("$response$", ToLiteral(responseInfo.Body))
                               .Replace("$responseType$", typeName)
                               .Replace("$httpMethod$", httpMethodName)
                               .Replace("$error$", errorPlaceHolder);

            // Debug.WriteLine(test);
            _logger.LogInformation(test);


            return test;
            //if (!responseInfo.httpResponse.Headers.TryGetValue("assembly-location", out _))
            //{
            //    return string.Empty;
            //}

            //// Just test
            //var assembly = new FileInfo(responseInfo.httpResponse.Headers["assembly-location"]);
            //var csproj = new FileInfo(responseInfo.httpResponse.Headers["csproj"]);
            //var test1 = assembly.Directory.Parent.Parent.Parent.Parent;

            //var testProj = test1.EnumerateFiles("AspNetCore.Simple.MsTest.Sdk.Test.csproj",SearchOption.AllDirectories).First();
            //var testFile = new FileInfo(Path.Combine(testProj.Directory.FullName, "Controllers", "PersonController.cs"));
            //testFile.Directory.Create();

            //var className = ClassTemplate.Replace("$className$", "PersonController").Replace("$testMethod$", test);

            //File.WriteAllText(testFile.FullName, className);

            //var locationOfController = responseInfo.httpResponse.Headers["controller-name"];

            //return test;
        }

        private string GetTypeName(Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            if (typeof(Task).IsAssignableFrom(type))
            {
                return type.GenericTypeArguments.First().Name;
            }

            return type.Name.Replace("`1", $"<{type.GenericTypeArguments.First().Name}>");
        }

        private static string ToLiteral(string input)
        {
            using var writer = new StringWriter();
            using var provider = CodeDomProvider.CreateProvider("CSharp");
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
            return writer.ToString();
        }
    }
}
