using System;
using System.Collections.Generic;
using AspNetCore.Simple.MsTest.Sdk.Validation;
using UnrelatedResponse = Controllers.Test.VersionedContracts.V1.UnrelatedResponse;
using V1Response = Controllers.Test.VersionedContracts.V1.InsertOrUpdateOrDeleteResponse;
using V2Response = Controllers.Test.VersionedContracts.V2.InsertOrUpdateOrDeleteResponse;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test
{
    /// <summary>
    /// A versioned api keeps the same contract name in V1 and V2. When a test declares the wrong
    /// version the validation table used to print the identical short name on both sides of a ✗,
    /// which reads like an sdk bug instead of the version mix-up it is.
    /// </summary>
    [TestClass]
    [TestCategory("TypeNameFormatter")]
    public sealed class TypeNameFormatterTests
    {
        [TestMethod]
        public void ShouldUseTheShortNameWhenNothingCollides()
        {
            IReadOnlyCollection<Type> peers = [typeof(V1Response), typeof(UnrelatedResponse)];

            Assert.AreEqual("InsertOrUpdateOrDeleteResponse",
                            TypeNameFormatter.Format(typeof(V1Response), peers));
        }

        [TestMethod]
        public void ShouldDisambiguateSameNamedTypesByTheirDifferingNamespaceSegment()
        {
            var v1 = typeof(V1Response);
            var v2 = typeof(V2Response);

            IReadOnlyCollection<Type> peers = [v1, v2];

            var v1Name = TypeNameFormatter.Format(v1, peers);
            var v2Name = TypeNameFormatter.Format(v2, peers);

            // Only the part the namespaces do NOT share is added - not the whole namespace.
            Assert.AreEqual("V1.InsertOrUpdateOrDeleteResponse", v1Name);
            Assert.AreEqual("V2.InsertOrUpdateOrDeleteResponse", v2Name);

            Assert.AreNotEqual(v1Name, v2Name, "A collision must never render as two identical strings.");
        }

        [TestMethod]
        public void ShouldStillRenderGenericArgumentsReadable()
        {
            Assert.AreEqual("IEnumerable<InsertOrUpdateOrDeleteResponse>",
                            TypeNameFormatter.Format(typeof(IEnumerable<V1Response>)));
        }
    }
}

namespace Controllers.Test.VersionedContracts.V1
{
    internal sealed class InsertOrUpdateOrDeleteResponse
    {
        public int Affected { get; init; }
    }

    internal sealed class UnrelatedResponse
    {
        public int Affected { get; init; }
    }
}

namespace Controllers.Test.VersionedContracts.V2
{
    internal sealed class InsertOrUpdateOrDeleteResponse
    {
        public int Affected { get; init; }

        public string? Etag { get; init; }
    }
}
