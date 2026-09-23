using System.Runtime.CompilerServices;
using System.Text.Json;
using Extensions.Pack;

namespace AspNetCore.Simple.MsTest.Sdk.Serializer.Json
{
    /// <summary>
    /// The two sides of a snapshot diff do not come from the same place: the current side is the
    /// response body exactly as it came off the wire (see <c>JsonComparisonStep.BuildCurrentResponse</c>,
    /// which only departs from that when the assert supplies a filter func), the expected side is the
    /// snapshot read into the response type and written out again. Every serializer setting that RENAMES
    /// something therefore only ever hits the expected side.
    ///
    /// <see cref="JsonSerializerOptions.PropertyNamingPolicy"/> is harmless here - it renames the same
    /// clr properties the api renamed when it produced the body, so both sides end up with the same
    /// spelling. <see cref="JsonSerializerOptions.DictionaryKeyPolicy"/> is not: dictionary keys are
    /// DATA. They are read back exactly as they stand in the json, so applying the policy on the way
    /// out rewrites keys that the wire side never had rewritten - a payload modelled as
    /// <c>Dictionary&lt;string, object&gt;</c> (database rows, dynamic columns) then differs in the
    /// casing of every single key.
    ///
    /// That mismatch cannot be recorded away either: write response stores the raw current body, and
    /// the next run camel cases the expected side again. The test stays red no matter how often it is
    /// re-recorded, which is exactly what the feature is supposed to fix.
    ///
    /// Comparing and recording therefore run with the api's options minus the dictionary key policy.
    /// Keys stay the way the json spells them, on both sides, in both directions.
    /// </summary>
    internal static class ComparisonJsonOptions
    {
        private static readonly ConditionalWeakTable<JsonSerializerOptions, JsonSerializerOptions> Cache = new();

        /// <summary>
        /// The options to serialize both sides of a comparison with. Returns the very same instance
        /// when there is nothing to neutralize, so the common case keeps the serializer's metadata
        /// cache instead of paying for a second one.
        /// </summary>
        public static JsonSerializerOptions ForComparison(this JsonSerializerOptions options)
        {
            if (options.IsNull() || options.DictionaryKeyPolicy.IsNull())
            {
                return options;
            }

            return Cache.GetValue(options, source => new JsonSerializerOptions(source) { DictionaryKeyPolicy = null });
        }
    }
}