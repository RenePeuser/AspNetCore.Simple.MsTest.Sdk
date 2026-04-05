using System.Collections.Immutable;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.AssertableHttpClient;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddSnapshotTestOutputBuilderExtension
    {
        public static void AddSnapshotTestOutputBuilder(this IServiceCollection services)
        {
            // Register dependencies
            services.AddTestInfoBuilder();
            services.AddHttpCallInfoTableBuilder();
            services.AddDifferencesTableBuilder();
            services.AddJsonSectionBuilder();
            services.AddCurlBuilder();
            services.AddCurlFormatter();

            // Register service itself
            services.AddSingletonIfNotExists<ISnapshotTestOutputBuilder, SnapshotTestOutputBuilder>();
        }
    }

    public interface ISnapshotTestOutputBuilder
    {
        /// <summary>
        /// Builds complete snapshot test failure output.
        /// Combines all sections: TestInfo, HTTP Call, Differences, Expected/Current JSON, Curl.
        /// </summary>
        /// <param name="context">HTTP response context with all request/response data</param>
        /// <param name="differences">List of differences found (can be empty)</param>
        /// <param name="expectedJson">Expected JSON string (already prepared by step)</param>
        /// <param name="currentJson">Current/actual JSON string (already prepared by step)</param>
        string Build<TResult>(HttpResponseContext<TResult> context,
                              ImmutableList<Difference> differences,
                              string expectedJson,
                              string currentJson);
    }

    internal sealed class SnapshotTestOutputBuilder(ITestInfoBuilder testInfoBuilder,
                                                    IHttpCallInfoTableBuilder httpCallInfoTableBuilder,
                                                    IDifferencesTableBuilder differencesTableBuilder,
                                                    IJsonSectionBuilder jsonSectionBuilder,
                                                    ICurlBuilder curlBuilder,
                                                    ICurlFormatter curlFormatter) : ISnapshotTestOutputBuilder
    {
        public string Build<TResult>(HttpResponseContext<TResult> context,
                                     ImmutableList<Difference> differences,
                                     string expectedJson,
                                     string currentJson)
        {
            var stringBuilder = new StringBuilder();

            // Section 1: Test Info (Project, Class, Snapshot, Errors, ErrorTypes)
            var testInfo = testInfoBuilder.Build(context, differences);
            stringBuilder.AppendLine(testInfo);
            stringBuilder.AppendLine();

            // Section 2: HTTP Call Table
            var httpCallInfo = httpCallInfoTableBuilder.Build(context);
            stringBuilder.AppendLine(httpCallInfo);
            stringBuilder.AppendLine();

            // Section 3: Differences Table (if any)
            var differencesTable = differencesTableBuilder.Build(differences);

            if (differencesTable.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(differencesTable);
                stringBuilder.AppendLine();
            }

            // Section 4: Expected JSON
            var expectedSection = jsonSectionBuilder.BuildExpected(expectedJson);
            stringBuilder.AppendLine(expectedSection);
            stringBuilder.AppendLine();

            // Section 5: Current JSON
            var currentSection = jsonSectionBuilder.BuildCurrent(currentJson);
            stringBuilder.AppendLine(currentSection);
            stringBuilder.AppendLine();

            // Section 6: Curl
            var curl = curlBuilder.BuildFrom(context);

            if (curl.IsNotNullOrWhiteSpace())
            {
                var curlFormatted = curlFormatter.GetCurlAsFormattedString(curl);
                stringBuilder.AppendLine(curlFormatted);
            }

            return stringBuilder.ToString();
        }
    }
}
