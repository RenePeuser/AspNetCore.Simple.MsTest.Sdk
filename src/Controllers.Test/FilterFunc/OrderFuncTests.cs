using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.FilterFunc
{
    /// <summary>
    /// Pins down that the filter func handed to an assert reaches BOTH sides of the snapshot diff.
    ///
    /// The two sides are not built the same way. The expected side is read from the snapshot into the
    /// response type and written out again, so the filter func naturally applies to it. The current side
    /// is deliberately the raw response body - that is what still surfaces a property the api returns but
    /// the response type does not model.
    ///
    /// For a while the filter func only ever reached the expected side. That made it useless for the one
    /// thing it exists for: normalizing volatile data. An assert that blanked an execution arn or pinned a
    /// start date compared the normalized snapshot against the untouched wire values and failed - and
    /// re-recording it did not help either, because the recorder writes the current side, so the next run
    /// normalized the expected side again and the diff came back. A test in that state can never go green.
    ///
    /// Both directions are pinned here on purpose: with a filter func both sides must be normalized, and
    /// without one the raw body must still be what is compared.
    /// </summary>
    [TestClass]
    [TestCategory("Controller")]
    [TestCategory("FilterFunc")]
    public sealed class OrderFuncTests : ApiTestBase
    {
        private const string NormalizedName = "<normalized>";

        private const string Snapshot = "Responses.NormalizedPerson.json";

        private const string PersonUrl = "api/v1/persons/1";

        /// <summary>
        /// Replaces the two fields the fixture holds normalized. Stands in for the volatile data a real
        /// filter func deals with - an arn, a generated name, a start date.
        /// </summary>
        private static Person? NormalizeVolatileFields(Person? person)
        {
            if (person.IsNull())
            {
                return person;
            }

            return person with { Name = NormalizedName, Age = 0 };
        }

        [TestMethod]
        [TestCategory("GET")]
        public Task AFilterFuncMustNormalizeTheCurrentSideAsWell()
        {
            return Client.AssertGetAsync<Person>(PersonUrl,
                                                 Snapshot,
                                                 NormalizeVolatileFields);
        }

        [TestMethod]
        [TestCategory("GET")]
        public Task WithoutAFilterFuncTheRawResponseBodyMustStillBeCompared()
        {
            return Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<Person>(PersonUrl, Snapshot),
                                                                         because: "The same snapshot without a filter func has to fail. It holds normalized values the api never sends, so if this passes the current side is no longer the raw response body and a property the api returns but the response type does not model would stop showing up in the diff.",
                                                                         fix: "Check JsonComparisonStep.BuildCurrentResponse: it may only swap the raw body for the normalized object when the caller actually supplied a filter func (ObjectAssertContext.HasOrderFunc), never by default.");
        }
    }
}
