//using System.Collections.Immutable;
//using System.IO;
//using System.Linq;
//using System.Text;
//using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
//using Extensions.Pack;
//using Microsoft.Extensions.DependencyInjection;

//namespace AspNetCore.Simple.MsTest.Sdk
//{
//    public static class AddTestInfoBuilderExtension
//    {
//        public static void AddTestInfoBuilder(this IServiceCollection services)
//        {
//            services.AddSingletonIfNotExists<ITestInfoBuilder, TestInfoBuilder>();
//        }
//    }

//    public interface ITestInfoBuilder
//    {
//        /// <summary>
//        /// Builds test information section from HTTP response context and differences.
//        /// Includes: Project, Class, Request, Response, Errors count, ErrorTypes.
//        /// </summary>
//        string Build<TResult>(HttpResponseContext<TResult> context,
//                              ImmutableList<Difference> differences);

//        /// <summary>
//        /// Builds test information section from HTTP response context interface.
//        /// Non-generic overload for use with IHttpResponseContext.
//        /// </summary>
//        string Build(IHttpResponseContext context,
//                     ImmutableList<Difference> differences);
//    }

//    internal sealed class TestInfoBuilder : ITestInfoBuilder
//    {
//        public string Build<TResult>(HttpResponseContext<TResult> context,
//                                     ImmutableList<Difference> differences)
//        {
//            return Build((IHttpResponseContext)context, differences);
//        }

//        public string Build(IHttpResponseContext context,
//                            ImmutableList<Difference> differences)
//        {
//            var projectName = context.CallingAssembly.GetName().Name ?? "Unknown";
//            var classPath = Path.GetFileName(context.CallerFilePath);
//            var requestName = GetRequestName(context);
//            var responseName = GetResponseName(context);
//            var errorCount = differences.Count;
//            var errorTypes = differences.Select(d => d.MismatchType).Distinct().ToList();

//            var stringBuilder = new StringBuilder();
//            stringBuilder.AppendLine("══════════════════════════════════════════════════════════════════════════════");
//            stringBuilder.AppendLine("SNAPSHOT TEST FAILED");
//            stringBuilder.AppendLine("══════════════════════════════════════════════════════════════════════════════");
//            stringBuilder.AppendLine();
//            stringBuilder.AppendLine($"Project   : {projectName}");
//            stringBuilder.AppendLine($"Class     : {classPath}");

//            // Method name is not available from context - would need StackTrace or additional parameter
//            // stringBuilder.AppendLine($"Method    : {methodName}");

//            stringBuilder.AppendLine($"Expected  : {responseName}");
//            stringBuilder.AppendLine($"Current   : {requestName}");
//            stringBuilder.AppendLine($"Errors    : {errorCount}");

//            if (errorTypes.Any())
//            {
//                var errorTypesStr = string.Join(", ", errorTypes);
//                stringBuilder.AppendLine($"ErrorTypes: {errorTypesStr}");
//            }

//            return stringBuilder.ToString();
//        }

//        private static string GetRequestName(IHttpResponseContext context)
//        {
//            var payloadFileName = context.PayloadFile?.EmbeddedFileName;

//            if (payloadFileName.IsNotNullOrWhiteSpace())
//            {
//                return Path.GetFileName(payloadFileName);
//            }

//            return "N/A";
//        }

//        private static string GetResponseName(IHttpResponseContext context)
//        {
//            var expectedFileName = context.ExpectedResultFile.EmbeddedFileName;

//            if (expectedFileName.IsNotNullOrWhiteSpace())
//            {
//                return Path.GetFileName(expectedFileName);
//            }

//            return "N/A";
//        }
//    }
//}


