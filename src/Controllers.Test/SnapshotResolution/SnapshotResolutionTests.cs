using System;
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
            var failure = await Assert.ThrowsExactlyAsync<AssertFailedException>(
                              () => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", "Persons.json"))
                                      .ConfigureAwait(false);

            StringAssert.Contains(failure.Message, "SNAPSHOT FILE NOT FOUND");

            // The near miss has to be offered, otherwise the error is not actionable.
            StringAssert.Contains(failure.Message, "GetAllPersons.json");
        }

        [TestMethod]
        public async Task AMissingPayloadMustNameTheFile()
        {
            var failure = await Assert.ThrowsExactlyAsync<AssertFailedException>(
                              () => Client.AssertPostAsync<Person>("api/v1/persons",
                                                                   "Requests.DoesNotExistAtAll.json",
                                                                   "Responses.CreatePerson.json"))
                                      .ConfigureAwait(false);

            StringAssert.Contains(failure.Message, "PAYLOAD FILE NOT FOUND");
            StringAssert.Contains(failure.Message, "Requests.DoesNotExistAtAll.json");
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

                Assert.IsTrue(File.Exists(snapshot.FullName),
                              $"write response did not create the snapshot at {snapshot.FullName}");

                Assert.IsTrue(new FileInfo(snapshot.FullName).Length > 0, "the created snapshot is empty");
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
            var failure = await Assert.ThrowsExactlyAsync<AssertFailedException>(
                              () => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", "Responses.BrokenJson.json"))
                                      .ConfigureAwait(false);

            StringAssert.Contains(failure.Message, "IS NOT VALID JSON");

            // Must point at the offending line (1 based), not complain about object-vs-array shape.
            StringAssert.Contains(failure.Message, "BrokenJson.json:4");
            StringAssert.Contains(failure.Message, "line 4");
            Assert.IsFalse(failure.Message.Contains("OBJECT {} TO ARRAY", StringComparison.Ordinal),
                           "a syntax error must not be reported as a structure mismatch");
        }

        /// <summary>
        /// The same protection has to hold for the direct object route. It ran without the guard for a
        /// while, so a typo there still bound to a foreign snapshot by substring and went green.
        /// </summary>
        [TestMethod]
        public void AnObjectAssertWithAReferenceThatOnlyMatchesBySubstringMustNotPass()
        {
            var current = new[] { new Person(1, "Goku", "Son", 42, ImmutableList<Email>.Empty) };

            var failure = Assert.ThrowsExactly<AssertFailedException>(
                () => Assert.That.ObjectsAreEqual("Persons.json", current));

            StringAssert.Contains(failure.Message, "SNAPSHOT FILE NOT FOUND");
            StringAssert.Contains(failure.Message, "GetAllPersons.json");

            // The generic catch-all must not claim an sdk bug for what is a typo in the test.
            Assert.IsFalse(failure.Message.Contains("UNEXPECTED ASSERTION ERROR", StringComparison.Ordinal),
                           "a missing snapshot must not be reported as an unexpected assertion error");
        }

        [TestMethod]
        public void AnObjectAssertAgainstBrokenSnapshotJsonMustBeReportedAsASyntaxError()
        {
            var current = new[] { new Person(1, "Goku", "Son", 42, ImmutableList<Email>.Empty) };

            var failure = Assert.ThrowsExactly<AssertFailedException>(
                () => Assert.That.ObjectsAreEqual("Responses.BrokenJson.json", current));

            StringAssert.Contains(failure.Message, "IS NOT VALID JSON");
            StringAssert.Contains(failure.Message, "line 4");

            Assert.IsFalse(failure.Message.Contains("OBJECT {} TO ARRAY", StringComparison.Ordinal),
                           "a syntax error must not be reported as a structure mismatch");
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

                Assert.IsTrue(File.Exists(snapshot.FullName),
                              $"write response did not create the snapshot at {snapshot.FullName}");
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
