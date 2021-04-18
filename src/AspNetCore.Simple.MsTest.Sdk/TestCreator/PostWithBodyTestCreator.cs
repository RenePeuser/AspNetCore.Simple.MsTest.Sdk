using System.CodeDom;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public class PostWithBodyTestCreator : ISpecificTestCreator
    {
        private readonly string TestTemplate = @"

        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertPostAsync<$responseType$>(""$url$"",
                                                          $payload$,
                                                          $response$);
        }
";
        public bool CanCreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo)
        {
            return requestInfo.HttpMethod == HttpMethods.Post && !string.IsNullOrWhiteSpace(requestInfo.Body);
        }

        public string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo)
        {
            var typeName = responseInfo.ResponseType.IsGenericType ? responseInfo.ResponseType.GetGenericArguments().First().Name : responseInfo.ResponseType.Name;

            var test = TestTemplate.Replace("$url$", requestInfo.Url)
                .Replace("$payload$", ToLiteral(requestInfo.Body))
                .Replace("$response$", ToLiteral(responseInfo.Body))
                .Replace("$responseType$", typeName);

            Debug.WriteLine(test);

            return test;

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
