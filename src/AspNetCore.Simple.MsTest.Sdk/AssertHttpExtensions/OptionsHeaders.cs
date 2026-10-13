using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using AspNetCore.Simple.MsTest.Sdk.AssertExtensions.Helpers;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk
{
    /// <summary>
    /// Header handling of the OPTIONS assertion. HTTP header names are case-insensitive, and HttpClient splits
    /// one answer into response headers and content headers - the comparison must see neither of these details.
    /// </summary>
    internal static class OptionsHeaders
    {
        // Header names travel exactly as written: no key policy (the app's camelCase turned
        // "Access-Control-Allow-Methods" into "access-Control-Allow-Methods", a snake_case policy would even
        // rewrite the hyphens), and no HTML escaping of values like "text/plain; q=0.5, */*+json".
        private static readonly JsonSerializerOptions HeaderJsonOptions = new()
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        internal static string Serialize(IImmutableDictionary<string, ImmutableList<string>> expectedHeaders)
        {
            return JsonSerializer.Serialize(expectedHeaders, HeaderJsonOptions);
        }

        /// <summary>
        /// All headers of the answer in one dictionary. <c>Allow</c> - the header OPTIONS exists for - and every
        /// <c>Content-*</c> header live in <see cref="HttpContent.Headers"/>, not in
        /// <see cref="HttpResponseMessage.Headers"/>; reading only the latter made them impossible to assert.
        ///
        /// <para>
        /// Content headers are read as SENT (<c>NonValidated</c>): the parsed view splits <c>Allow: POST, OPTIONS</c>
        /// into two values, which neither <c>curl -i</c> nor the <c>(name, "value")</c> overload would show. They
        /// were never compared before, so no existing expectation depends on the parsed form. Response headers keep
        /// the form they were always compared in.
        /// </para>
        /// </summary>
        internal static Dictionary<string, IEnumerable<string>> ReadFrom(HttpResponseMessage response)
        {
            var headers = response.Headers.ToDictionary(item => item.Key, item => item.Value, StringComparer.OrdinalIgnoreCase);

            foreach (var (name, values) in response.Content.Headers.NonValidated)
            {
                headers.TryAdd(name, values.ToList());
            }

            return headers;
        }

        /// <summary>
        /// Rewrites every expected header name to the spelling the server used, so the case-sensitive json
        /// comparison only reports real differences. A name listed twice (in any spelling) fails: it is one
        /// header, and no comparison can tell which of the two expectations the test meant.
        /// </summary>
        internal static string AlignNames(string expectedHeadersJson,
                                          IEnumerable<string> actualNames,
                                          Assembly callingAssembly,
                                          string callerFilePath,
                                          string callerMemberName,
                                          int callerLineNumber)
        {
            JsonDocument document;

            try
            {
                document = JsonDocument.Parse(expectedHeadersJson);
            }
            catch (JsonException)
            {
                // Not json at all - the comparison reports that with its own, detailed output.
                return expectedHeadersJson;
            }

            using (document)
            {
                if (document.RootElement.ValueKind != JsonValueKind.Object)
                {
                    return expectedHeadersJson;
                }

                var properties = document.RootElement.EnumerateObject().ToList();

                FailOnDuplicateNames(properties, callingAssembly, callerFilePath, callerMemberName, callerLineNumber);

                var actualSpelling = actualNames.ToDictionary(name => name, name => name, StringComparer.OrdinalIgnoreCase);

                var needsRewrite = properties.Exists(property => actualSpelling.TryGetValue(property.Name, out var actual) &&
                                                                 string.Equals(actual, property.Name, StringComparison.Ordinal).IsFalse());

                if (needsRewrite.IsFalse())
                {
                    return expectedHeadersJson;
                }

                using var stream = new MemoryStream();

                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }))
                {
                    writer.WriteStartObject();

                    foreach (var property in properties)
                    {
                        writer.WritePropertyName(actualSpelling.TryGetValue(property.Name, out var actual) ? actual : property.Name);
                        property.Value.WriteTo(writer);
                    }

                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        private static void FailOnDuplicateNames(List<JsonProperty> properties,
                                                 Assembly callingAssembly,
                                                 string callerFilePath,
                                                 string callerMemberName,
                                                 int callerLineNumber)
        {
            var duplicates = properties.GroupBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
                                       .Where(group => group.Count() > 1)
                                       .Select(group => string.Join(" / ", group.Select(property => $"\"{property.Name}\"")))
                                       .ToList();

            if (duplicates.Count == 0)
            {
                return;
            }

            var outputHelper = HttpClientAssertExtensions.GetService<IAssertOutputHelper>(callingAssembly);
            var sb = new StringBuilder();

            outputHelper.BuildHeader(sb, "EXPECTED HEADER LISTED TWICE");
            outputHelper.BuildTestInfoSection(sb, callerFilePath, callerMemberName, callerLineNumber, callingAssembly);
            outputHelper.BuildProblemSection(sb, "The expected headers name the same header more than once.");
            outputHelper.BuildDetailsSectionHeader(sb);

            foreach (var duplicate in duplicates)
            {
                sb.AppendLine($"{"Header",-10} : {duplicate}");
            }

            sb.AppendLine();
            outputHelper.BuildContextSection(sb, "HTTP header names are case-insensitive - every spelling above is ONE header. The comparison cannot know which of the expectations the test means.");
            outputHelper.BuildFixSection(sb, "List each header once. Several values of one header go into its array: { \"Allow\": [ \"GET, POST\" ] }.");
            outputHelper.BuildFooter(sb);

            throw new AssertFailedException(sb.ToString());
        }
    }
}
