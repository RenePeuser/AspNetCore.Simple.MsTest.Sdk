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
        /// Builds expected JSON section with label.
        /// Format: "EXPECTED JSON (copy/paste):\n\n{json}"
        /// </summary>
        string BuildExpected(string expectedJson);

        /// <summary>
        /// Builds current JSON section with label.
        /// Format: "CURRENT JSON (copy/paste):\n\n{json}"
        /// </summary>
        string BuildCurrent(string currentJson);
    }

    internal sealed class JsonSectionBuilder : IJsonSectionBuilder
    {
        public string BuildExpected(string expectedJson)
        {
            return BuildSection("EXPECTED JSON (copy/paste)", expectedJson);
        }

        public string BuildCurrent(string currentJson)
        {
            return BuildSection("CURRENT JSON (copy/paste)", currentJson);
        }

        private static string BuildSection(string label,
                                           string json)
        {
            return $"{label}:\n\n{json}";
        }
    }
}
