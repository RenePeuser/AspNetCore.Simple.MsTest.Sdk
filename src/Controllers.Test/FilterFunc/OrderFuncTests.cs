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

        private const string UnmodelledSnapshot = "Responses.UnmodelledPerson.json";

        private const string PersonUrl = "api/v1/persons/1";

        private const string LeanPersonUrl = "api/v1/persons/1/lean";

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

            return person with
                   {
                       Name = NormalizedName,
                       Age = 0
                   };
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
                                                                         fix: "Check JsonComparisonStep.BuildCurrentValue: it may only swap the raw body for the normalized object when ObjectAssertContext.OrderFunc is non null, never by default.");
        }

        /// <summary>
        /// The twin of the test above, and the one that actually bites.
        ///
        /// <see cref="PersonWithoutEmails"/> does not model <c>emails</c>, and the lean endpoint writes
        /// them anyway. Both sides of the diff are built differently on purpose: the expected side is the
        /// snapshot read through the response type, which drops them, while the current side is the raw
        /// response body, which keeps them. So the assert HAS to report a difference - that asymmetry is
        /// the whole reason the current side stays untyped, and the sdk documents it as surfacing "a
        /// property the api returns but the response type does not model".
        ///
        /// It goes quiet the moment the current side is built from the response type as well: both sides
        /// are then equally lossy, the field is gone from the diff, and nobody is told. That is not
        /// hypothetical. A separate HasOrderFunc flag was derived from a null check on the filter func,
        /// while every overload taking no filter func forwarded an identity lambda - so the flag was true
        /// for practically every assert in the sdk. The identity normalized nothing; the data was already
        /// gone by then, lost on the typed round trip the flag had switched on.
        ///
        /// Catching that needs a deliberately lossy response type. The tests above use <see cref="Person"/>,
        /// which models every field, so a typed round trip loses nothing and the regression walks straight
        /// past them.
        /// </summary>
        [TestMethod]
        [TestCategory("GET")]
        public Task WithoutAFilterFuncAFieldTheResponseTypeDoesNotModelMustStillShowUpInTheDiff()
        {
            return Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<PersonWithoutEmails>(LeanPersonUrl, UnmodelledSnapshot),
                                                                         because: "The lean endpoint writes `emails`, PersonWithoutEmails does not model them, and the snapshot is read through that type - so only the current side can still carry them. If this passes, the current side is no longer the raw response body and every field a response type does not model has silently dropped out of the diff.",
                                                                         fix: "Check JsonComparisonStep.BuildCurrentValue: it may only swap the raw body for the typed object when ObjectAssertContext.OrderFunc is non null. Keep that field nullable and never default it to an identity lambda - an overload that takes no filter func has to forward filterFunc: null, or the swap happens for every assert in the sdk.");
        }
    }
}