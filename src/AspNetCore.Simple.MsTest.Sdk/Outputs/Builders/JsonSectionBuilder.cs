using System.IO;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddJsonSectionBuilderExtension
    {
        public static void AddJsonSectionBuilder(this IServiceCollection services)
        {
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

    internal sealed class JsonSectionBuilder : IJsonSectionBuilder
    {
        public string BuildExpected(IHttpResponseContext context,
                                   string expectedJson)
        {
            var responseFileName = GetResponseFileName(context);
            var label = $"EXPECTED RESULT ({responseFileName})";
            return BuildSection(label, expectedJson);
        }

        public string BuildCurrent(string currentJson)
        {
            return BuildSection("CURRENT RESULT", currentJson);
        }

        private static string BuildSection(string label,
                                           string json)
        {
            return $"{label}:\n\n{json}";
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
