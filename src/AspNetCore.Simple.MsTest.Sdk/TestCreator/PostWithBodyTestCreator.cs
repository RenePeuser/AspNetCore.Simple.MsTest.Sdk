using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Extensions.Pack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddTestCreatorSettingsExtension
    {
        internal static void AddTestCreatorSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = configuration.GetSection(nameof(TestCreatorSettings)).Get<TestCreatorSettings>();
            if (settings.IsNull())
            {
                settings = new TestCreatorSettings();
            }

            services.AddSingleton(settings);
        }
    }

    public record TestCreatorSettings
    {
        public string TestMethodAttribute { get; init; } = "[TestMethod]";
    }


    internal static class AddPostWithBodyTestCreatorExtension
    {
        internal static void AddPostWithBodyTestCreator(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTestCreatorSettings(configuration);

            services.AddSingleton<ISpecificTestCreator, PostWithBodyTestCreator>();
        }
    }


    internal sealed class PostWithBodyTestCreator : ISpecificTestCreator
    {
        // ToDo: Optimize template creation => Strategy :)

        private readonly string TestTemplate = @"
$testattribute$
public Task $testmethodname$()
{
    return Client.Assert$httpMethod$$error$Async<$responseType$>(""$url$"",
                                                                 ""Payload"",
                                                                 ""Response"");
}
";

        private readonly string NoPayloadTestTemplate = @"
$testattribute$
public Task $testmethodname$()
{
    return Client.Assert$httpMethod$$error$Async<$responseType$>(""$url$"",                                       
                                                                 ""Response"");
}
";

        private readonly string UrlOnlyTemplate = @"
$testattribute$
public Task $testmethodname$()
{
    return Client.Assert$httpMethod$$error$Async<$responseType$>(""$url$"");
}
";

        private readonly string UrlWithPayloadNoResponse = @"
$testattribute$
public Task $testmethodname$()
{
    return Client.Assert$httpMethod$$error$Async<$responseType$>(""$url$"",
                                                                 ""Payload"");
}
";

        private readonly ILogger<PostWithBodyTestCreator> _logger;
        private readonly TestCreatorSettings _testCreatorSettings;

        public PostWithBodyTestCreator(ILogger<PostWithBodyTestCreator> logger, TestCreatorSettings testCreatorSettings)
        {
            _logger = logger;
            _testCreatorSettings = testCreatorSettings;
        }

        public bool CanCreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo)
        {
            return true;
        }


        private string GetTemplate(RequestInfo requestInfo, ResponseInfoUltra responseInfo)
        {
            if (responseInfo.StatusCode == 401)
            {
                if (responseInfo.Body.IsNotNullOrWhiteSpace())
                {
                    return UrlOnlyTemplate.Replace("<$responseType$>", string.Empty)
                                          .Replace("$testmethodname$", "Should_Return_Unauthorized_If_Call_Is_Not_Authorized");
                }

                return UrlWithPayloadNoResponse.Replace("<$responseType$>", string.Empty)
                                               .Replace("$testmethodname$", "Should_Return_Unauthorized_If_Call_Is_Not_Authorized");
            }

            var template = requestInfo.Body.IsNullOrWhiteSpace() ? NoPayloadTestTemplate : TestTemplate;

            if (responseInfo.StatusCode is >= 200 and < 300)
            {
                template = template.Replace("$testmethodname$", "Should_Return_Ok_Result_When_Calling_Endpoint");
            }
            else
            {
                template = template.Replace("$testmethodname$", "Should_Return_Error_Result_When_Calling_Endpoint");
            }

            return template;
        }

        public string CreateTestFor(RequestInfo requestInfo, ResponseInfoUltra responseInfo)
        {
            var testParts = GeneratedTestParts().ToList();
            var maxCharsPerLine = testParts.SelectMany(line => line.Split(Environment.NewLine)).Max(line => line.Length);
            var separator = maxCharsPerLine.Times(() => "-").Flatten();


            var testOutput = testParts.Flatten($"{Environment.NewLine}");
            var outputWithSeparators = testOutput.Replace("$separator$", separator)
                                                 .Replace("$testattribute$", _testCreatorSettings.TestMethodAttribute);

            HttpClientAssertExtensions.LogAction(outputWithSeparators);

            _logger.LogDebug(outputWithSeparators);

            return outputWithSeparators;

            IEnumerable<string> GeneratedTestParts()
            {
                var typeName = GetTypeName(responseInfo);

                var errorPlaceHolder = responseInfo.StatusCode switch
                {
                    >= 200 and < 300 => string.Empty,
                    401 => "AsUnauthorized",
                    _ => "AsError"
                };


                var template = GetTemplate(requestInfo, responseInfo);

                var httpMethodName = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(requestInfo.HttpMethod.ToLowerInvariant());


                var test = string.Empty;
                test = typeName.IsNotNullOrWhiteSpace()
                    ? template.Replace("$url$", requestInfo.RelativePath)
                              .Replace("$payload$", ToLiteral(requestInfo.Body))
                              .Replace("$response$", ToLiteral(responseInfo.Body))
                              .Replace("$responseType$", typeName)
                              .Replace("$httpMethod$", httpMethodName)
                              .Replace("$error$", errorPlaceHolder)

                    : template.Replace("$url$", requestInfo.RelativePath)
                              .Replace("$payload$", ToLiteral(requestInfo.Body))
                              .Replace("$response$", ToLiteral(responseInfo.Body))
                              .Replace("<$responseType$>", typeName)
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

                    // Check it xml or html is returned
                    if (requestInfo.Body.StartWith("{"))
                    {
                        yield return JToken.Parse(requestInfo.Body).ToString(Formatting.Indented);
                    }
                    else
                    {
                        yield return requestInfo.Body;
                    }

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

        private string GetTypeName(ResponseInfoUltra responseInfo)
        {
            var type = responseInfo.StatusCode switch
            {
                >= 200 and < 300 => responseInfo.ResponseType.First(rt => rt.StatusCode is >= 200 and < 300).Type,
                401 => null,
                _ => responseInfo.ResponseType.LastOrDefault(rt => rt.StatusCode == responseInfo.StatusCode, responseInfo.ResponseType.MaxBy(rt => rt.StatusCode)!).Type
            };

            // If null check if we found explicit code declaration
            if (type.IsNull())
            {
                type = responseInfo.ResponseType.FirstOrDefault(rt => rt.StatusCode == responseInfo.StatusCode)?.Type;
            }

            if (type.IsNull())
            {
                return string.Empty;
            }

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
