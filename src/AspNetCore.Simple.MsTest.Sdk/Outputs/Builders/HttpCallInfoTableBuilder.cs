using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using ConsoleTables;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddHttpCallInfoTableBuilderExtension
    {
        public static void AddHttpCallInfoTableBuilder(this IServiceCollection services)
        {
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

    internal sealed class HttpCallInfoTableBuilder : IHttpCallInfoTableBuilder
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
            stringBuilder.AppendLine("HTTP CALL");
            stringBuilder.Append(table.ToString().TrimEnd());

            return stringBuilder.ToString();
        }
    }
}
