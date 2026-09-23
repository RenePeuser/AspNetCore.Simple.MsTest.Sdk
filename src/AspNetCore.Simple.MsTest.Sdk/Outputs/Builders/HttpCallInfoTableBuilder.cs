using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using AspNetCore.Simple.MsTest.Sdk.Helpers;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpCallInfoTableBuilderExtension
    {
        public static void AddHttpCallInfoTableBuilder(this IServiceCollection services)
        {
            // ITextDecorator is registered separately
            services.AddEndpointSourceResolver();
            services.AddSingletonIfNotExists<IHttpCallInfoTableBuilder, HttpCallInfoTableBuilder>();
        }
    }

    public interface IHttpCallInfoTableBuilder
    {
        /// <summary>
        /// Builds HTTP call information table from HTTP response context.
        /// Includes: HttpMethod, Url, HttpStatusCode.
        /// Uses ConsoleTables for formatting.
        /// </summary>
        string Build<TResult>(HttpResponseContext<TResult> context);

        /// <summary>
        /// Builds HTTP call information table from HTTP response context interface.
        /// Non-generic overload for use with IHttpResponseContext.
        /// </summary>
        string Build(IHttpResponseContext context);
    }

    internal sealed class HttpCallInfoTableBuilder(ITextDecorator textDecorator,
                                                   IEndpointSourceResolver endpointSourceResolver) : IHttpCallInfoTableBuilder
    {
        public string Build<TResult>(HttpResponseContext<TResult> context)
        {
            // Delegate to non-generic implementation
            return Build((IHttpResponseContext)context);
        }

        public string Build(IHttpResponseContext context)
        {
            var httpMethod = context.HttpMethod.Method;
            var url = context.AbsoluteUrl;
            var statusCode = DecorateStatusCode(context.HttpStatusCode);
            var bodyContent = GetBodyContent(context);
            var responseFile = GetResponseFileName(context);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("🌍 HTTP"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{textDecorator.Highlight("Method")}   : {httpMethod}");
            stringBuilder.AppendLine($"{textDecorator.Highlight("Url")}      : {url}");
            stringBuilder.AppendLine($"{textDecorator.Highlight("Status")}   : {statusCode}");

            if (bodyContent.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine($"{textDecorator.Highlight("Body")}     : {bodyContent}");
            }

            var endpointSource = endpointSourceResolver.Resolve(context);

            if (endpointSource.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine($"{textDecorator.Highlight("Endpoint")} : {endpointSource}");
            }

            stringBuilder.Append($"{textDecorator.Highlight("Response")} : {responseFile}");

            return stringBuilder.ToString();
        }

        private static string GetBodyContent(IHttpResponseContext context)
        {
            if (context.PayloadFile.IsNull())
            {
                return string.Empty;
            }

            var content = context.PayloadFile.Content;

            if (content.IsNullOrWhiteSpace())
            {
                return string.Empty;
            }

            // Compress whitespace for display
            var compressed = content
                             .Replace("\r\n", " ")
                             .Replace("\n", " ")
                             .Replace("\r", " ")
                             .Replace("\t", " ");

            // Compress multiple spaces to single space
            while (compressed.Contains("  "))
            {
                compressed = compressed.Replace("  ", " ");
            }

            return compressed.Trim();
        }

        private static string GetResponseFileName(IHttpResponseContext context)
        {
            var expectedFileName = context.ExpectedResultFile.EmbeddedFile?.Name;

            if (expectedFileName.IsNotNullOrWhiteSpace())
            {
                return expectedFileName;
            }

            return "Expected";
        }

        private string DecorateStatusCode(System.Net.HttpStatusCode statusCode)
        {
            var statusCodeText = $"{(int)statusCode} {statusCode}";
            var statusCodeNumber = (int)statusCode;

            if (statusCodeNumber is >= 200 and < 300)
            {
                return textDecorator.Success(statusCodeText);
            }

            if (statusCodeNumber is >= 400 and < 500)
            {
                return textDecorator.SectionTitle(statusCodeText);
            }

            if (statusCodeNumber >= 500)
            {
                return textDecorator.Error(statusCodeText);
            }

            return textDecorator.Highlight(statusCodeText);
        }
    }
}