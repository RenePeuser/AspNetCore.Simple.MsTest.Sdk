using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpCallInfoTableBuilderExtension
    {
        public static void AddHttpCallInfoTableBuilder(this IServiceCollection services)
        {
            // No dependencies - ITextDecorator is registered separately
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

    internal sealed class HttpCallInfoTableBuilder(ITextDecorator textDecorator) : IHttpCallInfoTableBuilder
    {
        public string Build<TResult>(HttpResponseContext<TResult> context)
        {
            // Delegate to non-generic implementation
            return Build((IHttpResponseContext)context);
        }

        public string Build(IHttpResponseContext context)
        {
            var table = new ConsoleTable { Options = { EnableCount = false } };

            // Add columns
            table.AddColumn(new[] { "HttpMethod", "Url", "HttpStatusCode" });

            // Add data row
            var httpMethod = context.HttpMethod.Method;
            var url = context.AbsoluteUrl;
            var statusCode = $"{(int)context.HttpStatusCode} {context.HttpStatusCode}";

            table.AddRow(httpMethod, url, statusCode);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(textDecorator.SectionTitle("HTTP CALL"));
            stringBuilder.AppendLine($"{textDecorator.Highlight("Outcome")} : {DecorateStatusCode(context.HttpStatusCode)}");
            stringBuilder.AppendLine();
            stringBuilder.Append(table.ToString().TrimEnd());

            return stringBuilder.ToString();
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
