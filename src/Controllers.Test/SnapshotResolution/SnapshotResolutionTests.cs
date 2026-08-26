using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Controllers.Api.Persons;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.SnapshotResolution
{
    /// <summary>
    /// Guards the resolution of snapshot references.
    ///
    /// The dangerous case is not a red test - it is a green one. Resolution used to fall back to a
    /// substring lookup over the manifest names, so "Persons.json" (a file that does not exist)
    /// silently bound to "GetAllPersons.json" and the assert passed against a foreign snapshot.
    /// </summary>
    [TestClass]
    [TestCategory("SnapshotResolution")]
    public sealed class SnapshotResolutionTests : ApiTestBase
    {
        [TestMethod]
        public async Task AReferenceThatOnlyMatchesBySubstringMustNotPass()
        {
            // "Persons.json" does not exist. "GetAllPersons.json" does - and ENDS with it.
            var failure = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(
                              () => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", "Persons.json"),
                              because: "The dangerous case here is not a red test but a green one: resolution used to fall back to a substring lookup, so a reference to a file that does not exist bound to a foreign snapshot and the assert passed.",
                              fix: "The snapshot lookup has to match the full resource name. If nothing throws, check whether the substring fallback is back in the resolution path.")
                                      .ConfigureAwait(false);

            Assert.That.Contains(failure.Message,
                                 "SNAPSHOT FILE NOT FOUND",
                                 because: "The failure has to name the real problem - a snapshot that is not there - instead of any downstream comparison error.",
                                 fix: "Check that the unresolved reference raises SnapshotNotFoundException and that its handler is selected.");

            // The near miss has to be offered, otherwise the error is not actionable.
            Assert.That.Contains(failure.Message,
                                 "GetAllPersons.json",
                                 because: "'Persons.json' is almost certainly a typo for the file that does exist. Refusing the substring match is only half the job - naming the near miss is what makes the error actionable.",
                                 fix: "Check that the snapshot handler still runs its suggestion search over the manifest names and prints the candidates it found.");
        }

        [TestMethod]
        public async Task AMissingPayloadMustNameTheFile()
        {
            var failure = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(
                              () => Client.AssertPostAsync<Person>("api/v1/persons",
                                                                   "Requests.DoesNotExistAtAll.json",
                                                                   "Responses.CreatePerson.json"),
                              because: "A request payload that cannot be resolved must stop the test. Posting an empty or substring-matched body instead would exercise the endpoint with the wrong input.",
                              fix: "Check that the payload reference goes through the same strict resolution as the response snapshot.")
                                      .ConfigureAwait(false);

            Assert.That.Contains(failure.Message,
                                 "PAYLOAD FILE NOT FOUND",
                                 because: "Payload and snapshot are two different files, and the author needs to know which of the two is missing before looking for it.",
                                 fix: "Check that the isPayload flag reaches SnapshotNotFoundException so the handler picks the payload wording.");

            Assert.That.Contains(failure.Message,
                                 "Requests.DoesNotExistAtAll.json",
                                 because: "The reference the author wrote is the one thing they can act on - without it they have to guess which of several payloads is meant.",
                                 fix: "Check that the handler prints the unresolved reference verbatim.");
        }

        [TestMethod]
        public async Task WriteResponseMustStillBeAbleToCreateANewSnapshot()
        {
            var snapshot = new FileInfo(Path.Combine(ProjectFolder(), "SnapshotResolution", "Responses", "ZzCreatedByWriteResponse.json"));

            if (snapshot.Exists)
            {
                snapshot.Delete();
            }

            try
            {
                // The snapshot does not exist - with writeResponse the sdk records it instead of failing.
                await Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                                 "Responses.ZzCreatedByWriteResponse.json",
                                                                 writeResponse: true)
                            .ConfigureAwait(false);

                Assert.That.IsTrue(File.Exists(snapshot.FullName),
                                   because: "The strict resolution must not break recording. With writeResponse the missing file is the point - the sdk has to create it instead of failing.",
                                   fix: $"Expected the snapshot at {snapshot.FullName}. Check that the resolution guard lets an unresolved reference through when writeResponse is on, and that a writer was selected at all.");

                Assert.That.IsGreaterThan(new FileInfo(snapshot.FullName).Length,
                                          0,
                                          because: "An empty file is worse than no file: the next run resolves it happily and compares against nothing.",
                                          fix: "Check that the response writer writes the recorded response before the stream is closed.");
            }
            finally
            {
                if (File.Exists(snapshot.FullName))
                {
                    File.Delete(snapshot.FullName);
                }
            }
        }

        [TestMethod]
        public async Task BrokenSnapshotJsonMustBeReportedAsASyntaxError()
        {
            var failure = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(
                              () => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", "Responses.BrokenJson.json"),
                              because: "A snapshot that cannot even be parsed has to stop the test - carrying on would compare against a half-read document.",
                              fix: "Check that the json parse error is raised as InvalidSnapshotJsonException instead of being swallowed.")
                                      .ConfigureAwait(false);

            Assert.That.Contains(failure.Message,
                                 "IS NOT VALID JSON",
                                 because: "The heading has to say that the file itself is broken - anything else sends the author looking at the api instead of at their fixture.",
                                 fix: "Check that InvalidSnapshotJsonErrorHandler is registered and selected for InvalidSnapshotJsonException.");

            // Must point at the offending line (1 based), not complain about object-vs-array shape.
            Assert.That.Contains(failure.Message,
                                 "BrokenJson.json:4",
                                 because: "file:line is what makes the message clickable in the terminal, which is the difference between fixing the typo in one jump and searching the file by hand.",
                                 fix: "Check that the handler renders the path and the line number in the file:line form, and that the line number is 1 based.");

            Assert.That.Contains(failure.Message,
                                 "line 4",
                                 because: "The parse position has to be carried over from the JsonException - a syntax error without a position leaves the author scanning the whole file.",
                                 fix: "Check that the JsonException's LineNumber reaches the handler and is converted from 0 based to 1 based.");

            Assert.That.DoesNotContain(failure.Message,
                                       "OBJECT {} TO ARRAY",
                                       because: "A file that does not parse has no shape yet, so it can never be a structure mismatch. Reporting one sends the author off to change their expected type for what is a stray character.",
                                       fix: "The shape comparison has to run only after parsing succeeded; check the order of the steps in the assert pipeline.");
        }

        /// <summary>
        /// The same protection has to hold for the direct object route. It ran without the guard for a
        /// while, so a typo there still bound to a foreign snapshot by substring and went green.
        /// </summary>
        [TestMethod]
        public void AnObjectAssertWithAReferenceThatOnlyMatchesBySubstringMustNotPass()
        {
            var current = new[] { new Person(1, "Goku", "Son", 42, ImmutableList<Email>.Empty) };

            var failure = Assert.That.ThrowsExactly<AssertFailedException>(
                () => Assert.That.ObjectsAreEqual("Persons.json", current),
                because: "The direct object route ran without the resolution guard for a while, so a typo there still bound to a foreign snapshot by substring and went green.",
                fix: "Check that ObjectsAreEqual runs the same strict snapshot resolution as the http asserts - see SnapshotReferenceGuard.");

            Assert.That.Contains(failure.Message,
                                 "SNAPSHOT FILE NOT FOUND",
                                 because: "The object route has to produce the same diagnosis as the http route - a missing file is a missing file either way.",
                                 fix: "Check that the object route raises SnapshotNotFoundException rather than a generic assertion failure.");

            Assert.That.Contains(failure.Message,
                                 "GetAllPersons.json",
                                 because: "The near miss is what turns 'not found' into a one-word fix, and the object route must not lose that hint.",
                                 fix: "Check that the object route reaches the same handler, which runs the suggestion search over the manifest names.");

            // The generic catch-all must not claim an sdk bug for what is a typo in the test.
            Assert.That.DoesNotContain(failure.Message,
                                       "UNEXPECTED ASSERTION ERROR",
                                       because: "That heading tells the author to suspect the sdk. For what is a typo in their own test it is the most expensive possible wrong turn.",
                                       fix: "The specific snapshot handler has to be selected before the catch-all on the object route too.");
        }

        [TestMethod]
        public void AnObjectAssertAgainstBrokenSnapshotJsonMustBeReportedAsASyntaxError()
        {
            var current = new[] { new Person(1, "Goku", "Son", 42, ImmutableList<Email>.Empty) };

            var failure = Assert.That.ThrowsExactly<AssertFailedException>(
                () => Assert.That.ObjectsAreEqual("Responses.BrokenJson.json", current),
                because: "An unparsable snapshot has to stop the object route as well - the two routes must not diagnose the same broken file differently.",
                fix: "Check that ObjectsAreEqual surfaces the parse error as InvalidSnapshotJsonException instead of swallowing it.");

            Assert.That.Contains(failure.Message,
                                 "IS NOT VALID JSON",
                                 because: "The heading has to say that the fixture itself is broken, on this route just as on the http one.",
                                 fix: "Check that the object route reaches InvalidSnapshotJsonErrorHandler.");

            Assert.That.Contains(failure.Message,
                                 "line 4",
                                 because: "Without the parse position the author has to scan the whole file for the stray character.",
                                 fix: "Check that the JsonException's position is carried over on the object route too, converted from 0 based to 1 based.");

            Assert.That.DoesNotContain(failure.Message,
                                       "OBJECT {} TO ARRAY",
                                       because: "A file that does not parse has no shape yet, so it can never be a structure mismatch.",
                                       fix: "The shape comparison has to run only after parsing succeeded; check the order of the steps on the object route.");
        }

        /// <summary>
        /// Writing a snapshot from the object route must still work - the guard lets an unresolved
        /// reference through exactly there, because the file is about to be created.
        /// </summary>
        [TestMethod]
        public void AnObjectAssertWithWriteResponseMustStillBeAbleToCreateANewSnapshot()
        {
            var snapshot = new FileInfo(Path.Combine(ProjectFolder(), "SnapshotResolution", "Responses", "ZzCreatedByObjectAssert.json"));

            if (snapshot.Exists)
            {
                snapshot.Delete();
            }

            try
            {
                var current = new[] { new Person(1, "Goku", "Son", 42, ImmutableList<Email>.Empty) };

                Assert.That.ObjectsAreEqual("Responses.ZzCreatedByObjectAssert.json", current, writeResponse: true);

                Assert.That.IsTrue(File.Exists(snapshot.FullName),
                                   because: "The guard deliberately lets an unresolved reference through when writeResponse is on, because the file is about to be created. Tightening the guard must not break recording from the object route.",
                                   fix: $"Expected the snapshot at {snapshot.FullName}. Check the writeResponse exemption in SnapshotReferenceGuard and that a writer was selected for the object route.");
            }
            finally
            {
                if (File.Exists(snapshot.FullName))
                {
                    File.Delete(snapshot.FullName);
                }
            }
        }

        private static string ProjectFolder([System.Runtime.CompilerServices.CallerFilePath] string callerFilePath = "")
        {
            return new FileInfo(callerFilePath).Directory!.Parent!.FullName;
        }
    }
}