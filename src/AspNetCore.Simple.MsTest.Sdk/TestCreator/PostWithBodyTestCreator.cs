using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
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
        internal static void AddTestCreatorSettings(this IServiceCollection services,
                                                    IConfiguration configuration)
        {
            if (configuration.TryGetSettings<TestCreatorSettings>(out var settings).IsFalse())
            {
                settings = new TestCreatorSettings();
            }

            services.AddSingletonIfNotExists(settings);
        }
    }

#pragma warning disable CA1819 // Properties should not return arrays
    public record TestCreatorSettings
    {
        public string TestMethodAttribute { get; init; } = "[TestMethod]";

        public string ResponseFolderName { get; init; } = "Responses";

        public string RequestFolderName { get; init; } = "Requests";

        public string[] LegacyResponseFolderNames { get; init; } =
            [
                "Result",
                "Response",
                "Results",
                "Output"
            ];

        public string[] LegacyRequestFolderName { get; init; } =
            [
                "Payloads",
                "Payload",
                "Requests",
                "Request"
            ];

        /// <summary>
        /// Response headers that change on every single call. They carry no comparison value, but they
        /// used to be recorded into the snapshot envelope and then had to match - so a re-recorded
        /// snapshot showed up as noise in every diff and every review.
        ///
        /// They are dropped both when a snapshot is written and before it is compared, so existing
        /// snapshots that still carry one do not turn red.
        ///
        /// Override per project via configuration to add your own (a correlation id, a build stamp):
        /// <code>
        /// "TestCreatorSettings": { "VolatileHeaderNames": [ "traceparent", "X-My-Correlation-Id" ] }
        /// </code>
        /// Note that this REPLACES the defaults - list every name you want dropped.
        /// </summary>
        public string[] VolatileHeaderNames { get; init; } =
            [
                // W3C trace context - a new value per request by definition.
                "traceparent",
                "tracestate",
                "baggage",

                // Vendor tracing and correlation.
                "X-Amzn-Trace-Id",
                "X-Cloud-Trace-Context",
                "X-Correlation-Id",
                "X-Request-Id",
                "Request-Id",
                "Request-Context",

                // Wall clock and timing.
                "Date",
                "Age",
                "Server-Timing",
                "X-Runtime",

                // Changes with every build or host, never with the behaviour under test.
                "X-Powered-By"
            ];
    }
#pragma warning restore CA1819 // Properties should not return arrays

    internal static class AddPostWithBodyTestCreatorExtension
    {
        internal static void AddPostWithBodyTestCreator(this IServiceCollection services,
                                                        IConfiguration configuration)
        {
            services.AddTestCreatorSettings(configuration);

            services.AddSingleton<ISpecificTestCreator, PostWithBodyTestCreator>();
        }
    }

    internal sealed class PostWithBodyTestCreator(ILogger<PostWithBodyTestCreator> logger,
                                                  TestCreatorSettings testCreatorSettings) : ISpecificTestCreator
    {
        private readonly string NoPayloadTestTemplate = @"
$testattribute$
public Task $testmethodname$()
{
    return Client.Assert$httpMethod$$error$Async<$responseType$>(""$url$"",
                                                                 ""Response"");
}
";

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

        public bool CanCreateTestFor(RequestInfo requestInfo,
                                     ResponseInfoUltra responseInfo)
        {
            return true;
        }

        public string CreateTestFor(RequestInfo requestInfo,
                                    ResponseInfoUltra responseInfo)
        {
            var testParts = GeneratedTestParts().ToList();
            var maxCharsPerLine = testParts.SelectMany(line => line.Split(Environment.NewLine)).Max(line => line.Length);
            var separator = maxCharsPerLine.Times(() => "-").Flatten();

            var testOutput = testParts.Flatten($"{Environment.NewLine}");

            var outputWithSeparators = testOutput.Replace("$separator$", separator)
                                                 .Replace("$testattribute$", testCreatorSettings.TestMethodAttribute);

            HttpClientAssertExtensions.LogAction(outputWithSeparators);

            logger.LogDebug(outputWithSeparators);

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

                var httpMethodName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(requestInfo.HttpMethod.ToLowerInvariant());

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

        private string GetTemplate(RequestInfo requestInfo,
                                   ResponseInfoUltra responseInfo)
        {
            if (responseInfo.StatusCode.EqualsTo(401))
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

        private string GetTypeName(ResponseInfoUltra responseInfo)
        {
            var type = responseInfo.StatusCode switch
            {
                >= 200 and < 300 => responseInfo.ResponseType.First(rt => rt.StatusCode is >= 200 and < 300).Type,
                401 => null,
                _ => responseInfo.ResponseType.LastOrDefault(rt => rt.StatusCode.EqualsTo(responseInfo.StatusCode), responseInfo.ResponseType.MaxBy(rt => rt.StatusCode)!).Type
            };

            // If null check if we found explicit code declaration
            if (type.IsNull())
            {
                type = responseInfo.ResponseType.FirstOrDefault(rt => rt.StatusCode.EqualsTo(responseInfo.StatusCode))?.Type;
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
            provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, new CodeGeneratorOptions());

            return writer.ToString();
        }
    }
}