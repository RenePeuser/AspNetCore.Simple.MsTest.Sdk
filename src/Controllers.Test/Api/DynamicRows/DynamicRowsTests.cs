using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.DynamicRows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.Api.DynamicRows
{
    /// <summary>
    /// A payload that is not modelled by a dto but by a <c>Dictionary&lt;string, object&gt;</c> - the
    /// shape every api ends up with that returns database rows with dynamic columns. The keys are
    /// data, and asp.net core writes them exactly as they stand in the dictionary: a property naming
    /// policy renames clr properties, never dictionary keys.
    ///
    /// The comparison has to keep that promise. The current side of a snapshot diff is the raw
    /// response body, the expected side is the snapshot read into the response type and written out
    /// again - so a <see cref="System.Text.Json.JsonSerializerOptions.DictionaryKeyPolicy"/> would
    /// rename the keys on the expected side ONLY, and every key of every row would differ in nothing
    /// but its first letter.
    ///
    /// That is the failure this fixture pins down, and the reason it cannot be shrugged off: write
    /// response records the raw body, the next run renames the expected side again, and the test
    /// stays red however often it is re-recorded. See <c>ComparisonJsonOptions</c>.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("DictionaryPayload")]
    public sealed class DynamicRowsTests : ApiTestBase
    {
        [TestMethod]
        public Task DictionaryKeysMustBeComparedAsTheyAreWritten()
        {
            // The snapshot carries the keys the api really sends ("Transition", not "transition").
            return Client.AssertGetAsync<DynamicRowsResponse>("api/v1/dynamic-rows",
                                                              "Responses.DynamicRows.json");
        }
    }
}
