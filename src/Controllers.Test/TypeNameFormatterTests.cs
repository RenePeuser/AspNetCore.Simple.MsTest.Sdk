using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using V1 = Controllers.Api.SdkScenarios.V1;

namespace Controllers.Test
{
    /// <summary>
    /// A versioned api keeps the same contract name in V1 and V2. When a test declares the wrong
    /// version the validation table used to print the identical short name on both sides of a ✗,
    /// which reads like an sdk bug instead of the version mix-up it is.
    ///
    /// Readable generics and the plain short name are covered by the response-type mismatch tests in
    /// MinimalApi.Test (PersonGetTests_Native).
    /// </summary>
    [TestClass]
    [TestCategory("TypeNameFormatter")]
    public sealed class TypeNameFormatterTests : ApiTestBase
    {
        [TestMethod]
        public async Task SameNamedContractsMustBeToldApartByTheirDifferingNamespaceSegment()
        {
            // The endpoint returns the V2 contract, the test declares V1 - same short name.
            var error = await Assert.That.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<V1.InsertOrUpdateOrDeleteResponse>("api/v1/sdk-scenarios/versioned-contract",
                                                                                                                                                    "Responses.VersionedContract.json"),
                                                                                    because: "The declared V1 contract is not what the endpoint returns, so endpoint validation has to stop the test.",
                                                                                    fix: "Check that endpoint validation compares the declared type against the endpoint's real return type.")
                                    .ConfigureAwait(false);

            // Only the part the namespaces do NOT share is added - not the whole namespace.
            Assert.That.Contains(error.Message,
                                 "V1.InsertOrUpdateOrDeleteResponse",
                                 because: "V1 and V2 carry the same short name, so the declared type has to be prefixed with the one namespace segment that differs - otherwise both sides of the validation table read the same.",
                                 fix: "Check the common-prefix logic in TypeNameFormatter.Format: it has to strip the shared namespace part and keep the first differing segment.");

            Assert.That.Contains(error.Message,
                                 "V2.InsertOrUpdateOrDeleteResponse",
                                 because: "The endpoint's type needs the same disambiguation - a version mix-up must not render as two identical strings.",
                                 fix: "Make TypeNameFormatter.Format detect the collision - two peers with the same short name must both get their differing namespace segment.");

            Assert.That.DoesNotContain(error.Message,
                                       "Controllers.Api.SdkScenarios.V1.InsertOrUpdateOrDeleteResponse",
                                       because: "Printing the full namespace is noise - the differing segment is all the reader needs.",
                                       fix: "TypeNameFormatter.Format must only add the first differing namespace segment.");
        }
    }
}
