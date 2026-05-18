using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Decorators;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddJsonSectionBuilderExtension
    {
        public static void AddJsonSectionBuilder(this IServiceCollection services)
        {
            // No dependencies - ITextDecorator is registered separately
            services.AddSingletonIfNotExists<IJsonSectionBuilder, JsonSectionBuilder>();
        }
    }

    public interface IJsonSectionBuilder
    {
        /// <summary>
        /// Builds expected result section with label including response filename.
        /// Format: "EXPECTED RESULT (NewPerson.json):\n\n{json}"
        /// </summary>
        string BuildExpected(IHttpResponseContext context,
                             string expectedJson);

        /// <summary>
        /// Builds current result section with label.
        /// Format: "CURRENT RESULT:\n\n{json}"
        /// </summary>
        string BuildCurrent(string currentJson);
    }

    internal sealed class JsonSectionBuilder(ITextDecorator textDecorator) : IJsonSectionBuilder
    {
        public string BuildExpected(IHttpResponseContext context,
                                    string expectedJson)
        {
            _ = GetResponseFileName(context);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("📄 Expected Snapshot"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.Append(expectedJson);

            return stringBuilder.ToString();
        }

        public string BuildCurrent(string currentJson)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(textDecorator.SectionTitle("📄 Current Result"));
            stringBuilder.AppendLine(textDecorator.Dim("──────────────────────────────────────────────────────────────"));
            stringBuilder.AppendLine();
            stringBuilder.Append(currentJson);

            return stringBuilder.ToString();
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
    }
}