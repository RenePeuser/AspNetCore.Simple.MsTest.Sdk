using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public class PostWithBodyTestCreator : ISpecificTestCreator
    {
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

            Debug.WriteLine(test);

            return test;
        }

        private string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                return type.Name.Replace("`1", $"<{type.GenericTypeArguments.First().Name}>");
            }
            return type.Name;
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
