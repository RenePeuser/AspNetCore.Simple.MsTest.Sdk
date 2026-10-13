using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons.V1.Profile
{
    /// <summary>
    /// Consumer tests for the fluent comparison vocabulary — <c>IgnoreProperty</c>, <c>IgnorePaths</c>,
    /// <c>IgnoreDifferencesWhere</c> — against GET /api/v1/persons/1/profile.
    ///
    /// <para>
    /// The endpoint always returns
    /// <c>{ id: 1, name: "Son Goku", address: { name: "Home", street: "Kame House 1", zip_code: "12345" },
    /// contacts: [ { name: "Work", value: "goku@capsule.corp" }, { name: "Private", value: "goku@kame.house" } ] }</c>.
    /// <c>name</c> occurs on three levels, so every test that ignores ONE of them proves the others still count.
    /// </para>
    ///
    /// <para>
    /// Every ignore rule is paired with a test that MUST fail: an ignore rule that hides too much does not
    /// show up as a red test, it shows up as a green one that checks nothing.
    /// </para>
    /// </summary>
    [TestClass]
    [TestCategory("Minimal Api")]
    [TestCategory("Fluent")]
    public sealed class PersonProfileComparisonTests : ApiTestBase
    {
        private const string ProfileUrl = "api/v1/persons/1/profile";

        private static PersonProfile Expected()
        {
            return new PersonProfile(Id: 1,
                                     Name: "Son Goku",
                                     Address: new ProfileAddress("Home", "Kame House 1", "12345"),
                                     Contacts: ImmutableList.Create(new ProfileContact("Work", "goku@capsule.corp"),
                                                                    new ProfileContact("Private", "goku@kame.house")));
        }

        private static PersonProfile WithContact(PersonProfile profile,
                                                 int index,
                                                 ProfileContact contact)
        {
            return profile with { Contacts = profile.Contacts.SetItem(index, contact) };
        }

        private static IHttpComparisonConfiguring<PersonProfile> Compare(PersonProfile expected)
        {
            return Client.AssertGet(ProfileUrl)
                         .Produces<PersonProfile>(HttpStatusCode.OK)
                         .ExpectedResponse(expected);
        }

        [TestMethod]
        public Task Profile_Should_Match_Without_Ignore_Rules()
        {
            return Compare(Expected()).ExecuteAsync();
        }

        // ============================================================
        // IgnoreProperty — exactly the addressed position.
        // ============================================================

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public Task IgnoreProperty_Should_Ignore_Root_Property()
        {
            return Compare(Expected() with { Name = "Wrong" })
                   .IgnoreProperty(p => p.Name)
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public async Task IgnoreProperty_On_Root_Should_Not_Ignore_Same_Name_In_Nested_Object()
        {
            var expected = Expected() with { Address = Expected().Address with { Name = "Wrong" } };

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Compare(expected)
                                                                              .IgnoreProperty(p => p.Name)
                                                                              .ExecuteAsync(),
                                                                        because: "IgnoreProperty(p => p.Name) addresses the ROOT name only. address.name is a different property that merely shares the name - hiding it would silently switch off part of the comparison.",
                                                                        fix: "Check that IgnoreProperty matches the full member path built from the expression instead of the last segment's name.").ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public Task IgnoreProperty_Should_Ignore_Nested_Property()
        {
            return Compare(Expected() with { Address = Expected().Address with { Name = "Wrong" } })
                   .IgnoreProperty(p => p.Address.Name)
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public async Task IgnoreProperty_On_Nested_Should_Not_Ignore_Root_Property()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Compare(Expected() with { Name = "Wrong" })
                                                                              .IgnoreProperty(p => p.Address.Name)
                                                                              .ExecuteAsync(),
                                                                        because: "IgnoreProperty(p => p.Address.Name) addresses address.name only - the root name must still be compared.",
                                                                        fix: "Check that the member path built from the expression includes every member segment, not just the last one.").ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public Task IgnoreProperty_With_Select_Should_Ignore_Property_In_Every_Element()
        {
            var expected = Expected() with
            {
                Contacts = ImmutableList.Create(new ProfileContact("Wrong1", "goku@capsule.corp"),
                                                new ProfileContact("Wrong2", "goku@kame.house"))
            };

            return Compare(expected)
                   .IgnoreProperty(p => p.Contacts.Select(c => c.Name))
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public async Task IgnoreProperty_With_Select_Should_Not_Ignore_Other_Element_Properties()
        {
            var expected = WithContact(Expected(), 0, new ProfileContact("Work", "wrong@example.com"));

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Compare(expected)
                                                                              .IgnoreProperty(p => p.Contacts.Select(c => c.Name))
                                                                              .ExecuteAsync(),
                                                                        because: "Select(c => c.Name) ignores contacts[*].name only - a wrong contacts[0].value must still fail.",
                                                                        fix: "Check that Select maps to a [*] segment FOLLOWED by the inner member path, not to the whole collection.").ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public Task IgnoreProperty_With_Index_Should_Ignore_That_Element_Only()
        {
            return Compare(WithContact(Expected(), 1, new ProfileContact("Private", "wrong@example.com")))
                   .IgnoreProperty(p => p.Contacts[1].Value)
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public async Task IgnoreProperty_With_Index_Should_Not_Ignore_Other_Elements()
        {
            var expected = WithContact(Expected(), 0, new ProfileContact("Work", "wrong@example.com"));

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Compare(expected)
                                                                              .IgnoreProperty(p => p.Contacts[1].Value)
                                                                              .ExecuteAsync(),
                                                                        because: "Contacts[1].Value addresses the second element only - a wrong value in the first element must still fail.",
                                                                        fix: "Check that an indexer in the selector produces a concrete [n] segment, not a [*] wildcard.").ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public Task IgnoreProperty_Should_Honor_JsonPropertyName()
        {
            // ZipCode travels as "zip_code". Matching by the C# name would never hit and the test would fail.
            return Compare(Expected() with { Address = Expected().Address with { ZipCode = "99999" } })
                   .IgnoreProperty(p => p.Address.ZipCode)
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public Task IgnoreProperty_On_Object_Should_Ignore_Its_Whole_Subtree()
        {
            return Compare(Expected() with { Address = new ProfileAddress("Wrong", "Wrong Street", "00000") })
                   .IgnoreProperty(p => p.Address)
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnoreProperty")]
        public void IgnoreProperty_Should_Reject_Unsupported_Selector()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Compare(Expected()).IgnoreProperty(p => p.Name.ToUpperInvariant()),
                                                         because: "A selector that is not a member path cannot be turned into a difference path. Accepting it silently would ignore nothing - or the wrong thing.",
                                                         fix: "Check that MemberPathPattern.FromExpression throws for every expression node it does not translate.");
        }

        // ============================================================
        // IgnorePaths — difference path syntax, [*] for any index.
        // ============================================================

        [TestMethod]
        [TestCategory("IgnorePaths")]
        public Task IgnorePaths_Should_Ignore_Listed_Paths()
        {
            var expected = WithContact(Expected() with { Name = "Wrong" }, 0, new ProfileContact("Work", "wrong@example.com"));

            return Compare(expected)
                   .IgnorePaths("name", "contacts[*].value")
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnorePaths")]
        public Task IgnorePaths_Should_Accept_The_Path_As_Shown_In_The_Differences_Table()
        {
            // The failure table prints the envelope path; copying it from there has to work as well.
            return Compare(Expected() with { Address = Expected().Address with { Street = "Wrong" } })
                   .IgnorePaths("content.value.address.street")
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnorePaths")]
        public async Task IgnorePaths_Should_Not_Ignore_Unlisted_Paths()
        {
            var expected = WithContact(Expected(), 0, new ProfileContact("Wrong", "goku@capsule.corp"));

            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Compare(expected)
                                                                              .IgnorePaths("contacts[*].value")
                                                                              .ExecuteAsync(),
                                                                        because: "contacts[*].value must not hide contacts[0].name.",
                                                                        fix: "Check that a pattern matches segment by segment and only covers its own subtree.").ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("IgnorePaths")]
        public async Task IgnorePaths_Should_Not_Treat_Prefix_Of_A_Name_As_Match()
        {
            // "addr" is a string prefix of "address" but not the same property.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Compare(Expected() with { Address = Expected().Address with { Street = "Wrong" } })
                                                                              .IgnorePaths("addr")
                                                                              .ExecuteAsync(),
                                                                        because: "A path pattern matches whole segments. 'addr' must not ignore 'address.street'.",
                                                                        fix: "Check that IgnorePaths compares parsed segments instead of using StartsWith on the raw path.").ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("IgnorePaths")]
        public void IgnorePaths_Should_Reject_Empty_Path()
        {
            Assert.That.ThrowsExactly<ArgumentException>(() => Compare(Expected()).IgnorePaths(" "),
                                                         because: "An empty path pattern matches nothing useful - failing loudly beats an ignore rule that silently does nothing.",
                                                         fix: "Check MemberPathPattern.Parse rejects blank input.");
        }

        // ============================================================
        // IgnoreDifferencesWhere — true = ignore.
        // ============================================================

        [TestMethod]
        [TestCategory("IgnoreDifferencesWhere")]
        public Task IgnoreDifferencesWhere_Should_Ignore_Matching_Differences()
        {
            return Compare(Expected() with { Name = "Wrong" })
                   .IgnoreDifferencesWhere(d => d.MemberPath == "content.value.name")
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnoreDifferencesWhere")]
        public async Task IgnoreDifferencesWhere_Should_Not_Ignore_Non_Matching_Differences()
        {
            // The old IgnoreDifferences(diffs => diffs.Where(d => d.MemberPath == "content.value.name")) KEPT only the name
            // differences - and with a correct name, this exact test went green. The predicate now means
            // "ignore", so the wrong street has to fail.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Compare(Expected() with { Address = Expected().Address with { Street = "Wrong" } })
                                                                              .IgnoreDifferencesWhere(d => d.MemberPath == "content.value.name")
                                                                              .ExecuteAsync(),
                                                                        because: "IgnoreDifferencesWhere ignores what the predicate matches. Everything else - here address.street - must still count.",
                                                                        fix: "Check that the predicate result true means IGNORE and is inverted before it reaches the engine's keep-predicate.").ConfigureAwait(false);
        }

        [TestMethod]
        [TestCategory("IgnoreDifferencesWhere")]
        public Task IgnoreDifferencesWhere_Calls_Should_Combine()
        {
            var expected = Expected() with
            {
                Name = "Wrong",
                Address = Expected().Address with { Street = "Wrong" }
            };

            return Compare(expected)
                   .IgnoreDifferencesWhere(d => d.MemberPath == "content.value.name")
                   .IgnoreDifferencesWhere(d => d.MemberPath == "content.value.address.street")
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("IgnoreDifferencesWhere")]
        public async Task IgnoreDifferencesWhere_Second_Call_Should_Not_Replace_First()
        {
            var expected = Expected() with
            {
                Name = "Wrong",
                Address = Expected().Address with { Street = "Wrong" }
            };

            // Only the street is ignored here; the name must still fail - proving the rules are combined
            // per difference and the test above is green for the right reason.
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Compare(expected)
                                                                              .IgnoreDifferencesWhere(d => d.MemberPath == "content.value.address.street")
                                                                              .ExecuteAsync(),
                                                                        because: "Ignoring address.street must leave the wrong root name visible.",
                                                                        fix: "Check that IgnoreDifferencesWhere composes with OR per difference and does not drop the whole comparison.").ConfigureAwait(false);
        }

        // ============================================================
        // FilterDifferences — list-level escape hatch: returns what still COUNTS.
        // ============================================================

        [TestMethod]
        [TestCategory("FilterDifferences")]
        public Task FilterDifferences_Should_Drop_What_The_Filter_Removes()
        {
            return Compare(Expected() with { Address = Expected().Address with { Street = "Wrong" } })
                   .FilterDifferences(diffs => diffs.Where(d => d.MemberPath.EndsWith(".street", StringComparison.Ordinal).IsFalse()))
                   .ExecuteAsync();
        }

        [TestMethod]
        [TestCategory("FilterDifferences")]
        public async Task FilterDifferences_Should_Keep_What_The_Filter_Returns()
        {
            await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Compare(Expected() with { Name = "Wrong" })
                                                                              .FilterDifferences(diffs => diffs.Where(d => d.MemberPath.EndsWith(".street", StringComparison.Ordinal).IsFalse()))
                                                                              .ExecuteAsync(),
                                                                        because: "FilterDifferences keeps every difference the function returns - the wrong root name is returned and has to fail.",
                                                                        fix: "Check that the returned differences are the ones that COUNT, not the ones that are ignored.").ConfigureAwait(false);
        }
    }
}
