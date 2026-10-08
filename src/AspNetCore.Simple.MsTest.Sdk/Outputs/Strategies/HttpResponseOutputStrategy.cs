using System.Collections.Immutable;
using System.Text;
using AspNetCore.Simple.MsTest.Sdk.Outputs.Strategies.Http;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.Strategies
{
    internal static class AddHttpResponseOutputStrategyExtension
    {
        public static void AddHttpResponseOutputStrategy(this IServiceCollection services)
        {
            // Register dependencies - all the builders needed for HTTP report
            services.AddHttpCallInfoTableBuilder();
            services.AddDifferencesTableBuilder();
            services.AddJsonSectionBuilder();
            services.AddUnresolvedParameterSectionBuilder();
            services.AddCurlBuilder();
            services.AddCurlFormatter();

            // Register HTTP failure output strategies (handles header building)
            services.AddHttpFailureOutputStrategy();

            // Note: ITextDecorator is registered separately based on build configuration

            // Register service itself
            services.AddSingletonIfNotExists<IAssertOutputStrategy, HttpResponseOutputStrategy>();
        }
    }

    /// <summary>
    /// Output strategy for HTTP response contexts.
    /// Builds comprehensive HTTP assertion failure output including test info,
    /// HTTP call details, differences, JSON comparison, and curl reproduction.
    /// </summary>
    internal sealed class HttpResponseOutputStrategy(IHttpFailureOutputBuilder httpFailureOutputBuilder,
                                                     IHttpCallInfoTableBuilder httpCallInfoTableBuilder,
                                                     IDifferencesTableBuilder differencesTableBuilder,
                                                     IJsonSectionBuilder jsonSectionBuilder,
                                                     IUnresolvedParameterSectionBuilder unresolvedParameterSectionBuilder,
                                                     ICurlBuilder curlBuilder,
                                                     ICurlFormatter curlFormatter)
        : AssertOutputStrategyBase<IHttpResponseContext>
    {
        protected override string BuildOutput(IHttpResponseContext context,
                                              ImmutableList<Difference> differences,
                                              string expectedJson,
                                              string currentJson)
        {
            var stringBuilder = new StringBuilder();

            // Section 1: Title + Test Information + Failure Details (delegated to failure-specific strategy)
            httpFailureOutputBuilder.BuildHeader(stringBuilder, context);

            // Section 2: HTTP Call Table
            var httpCallInfo = httpCallInfoTableBuilder.Build(context);
            stringBuilder.AppendLine(httpCallInfo);
            stringBuilder.AppendLine();

            // Section 3: Differences Table (if any)
            var differencesTable = differencesTableBuilder.Build(context, differences);

            if (differencesTable.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(differencesTable);
                stringBuilder.AppendLine();
            }

            // Section 3.1: A placeholder nobody supplied a parameter for explains a diff that otherwise
            // reads as a plain value mismatch - and a parse error that reads as nothing at all.
            var unresolvedParameters = unresolvedParameterSectionBuilder.Build(context, differences);

            if (unresolvedParameters.IsNotNullOrWhiteSpace())
            {
                stringBuilder.AppendLine(unresolvedParameters);
                stringBuilder.AppendLine();
            }

            // Section 4: Expected Result
            var expectedSection = jsonSectionBuilder.BuildExpected(context, expectedJson);
            stringBuilder.AppendLine(expectedSection);
            stringBuilder.AppendLine();

            // Section 5: Current Result
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