using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class PostWithBodyTestCreator : ISpecificTestCreator
    {
        private readonly ILogger<PostWithBodyTestCreator> _logger;

        private readonly string TestTemplate = @"
[NUnit.Framework.Test]
public Task Should_Return_Expected_Result_For_Given_Payload()
{
    return Client.Assert$httpMethod$$error$Async<$responseType$>(""$url$"",
                                                                 Payload,
                                                                 Response);
}
";

        private readonly string NoPayloadTestTemplate = @"
[NUnit.Framework.Test]
public Task Should_Return_Expected_Result_For_Given_Payload()
{
    return Client.Assert$httpMethod$$error$Async<$responseType$>(""$url$"",                                       
                                                                 Response);
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
            var testParts = GeneratedTestParts().ToList();
            var maxCharsPerLine = testParts.SelectMany(line => line.Split(Environment.NewLine)).Max(line => line.Length);
            var separator = maxCharsPerLine.Times(() => "-").Flatten();


            var testOutput = testParts.Flatten($"{Environment.NewLine}");
            var outputWithSeparators = testOutput.Replace("$separator$", separator);

            Debug.WriteLine(outputWithSeparators);
            Console.WriteLine(outputWithSeparators);

            return outputWithSeparators;

            IEnumerable<string> GeneratedTestParts()
            {
                var typeName = GetTypeName(responseInfo.ResponseType);

                var errorPlaceHolder = responseInfo.StatusCode is >= 200 and < 300 ? string.Empty : "AsError";

                var template = requestInfo.Body.IsNullOrWhiteSpace() ? NoPayloadTestTemplate : TestTemplate;
                var httpMethodName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(requestInfo.HttpMethod.ToLowerInvariant());


                var test = template.Replace("$url$", requestInfo.RelativePath)
                                   .Replace("$payload$", ToLiteral(requestInfo.Body))
                                   .Replace("$response$", ToLiteral(responseInfo.Body))
                                   .Replace("$responseType$", typeName)
                                   .Replace("$httpMethod$", httpMethodName)
                                   .Replace("$error$", errorPlaceHolder);


                // Debug.WriteLine(test);
                //                 _logger.LogInformation(test);

                // First return test
                yield return "$separator$";
                yield return $"Http Call:     {requestInfo.HttpMethod} {requestInfo.AbsolutePath}";
                yield return "$separator$";
                yield return "Created test:";
                yield return "$separator$";
                yield return test;
                yield return "$separator$";

                if (requestInfo.Body.IsNotNullOrWhiteSpace())
                {
                    yield return "Payload:";
                    yield return "$separator$";
                    yield return JToken.Parse(requestInfo.Body).ToString(Formatting.Indented);
                    yield return "$separator$";
                }

                if (responseInfo.Body.IsNotNullOrWhiteSpace())
                {
                    yield return "Response:";
                    yield return "$separator$";
                    // Check it xml or html is returned
                    if (responseInfo.Body.StartWith("{"))
                    {
                        yield return JToken.Parse(responseInfo.Body).ToString(Formatting.Indented);
                    }
                    else
                    {
                        yield return responseInfo.Body;
                    }
                    yield return "$separator$";
                }
            }
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
