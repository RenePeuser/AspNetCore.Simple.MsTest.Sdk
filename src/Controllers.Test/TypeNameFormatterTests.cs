using System;
using System.Collections.Generic;
using AspNetCore.Simple.MsTest.Sdk;
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

            Assert.That.AreEqual("InsertOrUpdateOrDeleteResponse",
                                 TypeNameFormatter.Format(typeof(V1Response), peers),
                                 because: "Nothing in the peer set shares this short name, so the reader gains nothing from a namespace prefix - the plain name is the most readable form.",
                                 fix: "TypeNameFormatter.Format must only add the disambiguating namespace segment when a peer actually carries the same short name.");
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
            Assert.That.AreEqual("V1.InsertOrUpdateOrDeleteResponse",
                                 v1Name,
                                 because: "V1 and V2 carry the same short name, so the formatter has to prefix the one namespace segment that differs - and only that segment, printing the full namespace would drown the table.",
                                 fix: "Check the common-prefix logic in TypeNameFormatter.Format: it has to strip the shared namespace part and keep the first differing segment.");

            Assert.That.AreEqual("V2.InsertOrUpdateOrDeleteResponse",
                                 v2Name,
                                 because: "V1 and V2 carry the same short name, so the formatter has to prefix the one namespace segment that differs - and only that segment, printing the full namespace would drown the table.",
                                 fix: "Check the common-prefix logic in TypeNameFormatter.Format: it has to strip the shared namespace part and keep the first differing segment.");

            Assert.That.AreNotEqual(v1Name,
                                    v2Name,
                                    because: "This is the whole point of the formatter: a version mix-up must not render as two identical strings on both sides of the validation table, because that reads like an sdk bug instead of the wrong-version test it is.",
                                    fix: "Make TypeNameFormatter.Format detect the collision - two peers with the same short name must both get their differing namespace segment.");
        }

        [TestMethod]
        public void ShouldStillRenderGenericArgumentsReadable()
        {
            Assert.That.AreEqual("IEnumerable<InsertOrUpdateOrDeleteResponse>",
                                 TypeNameFormatter.Format(typeof(IEnumerable<V1Response>)),
                                 because: "Endpoints commonly return collections, so the generic has to render in C# syntax - the CLR form 'IEnumerable`1[[...]]' is unreadable in a failure table.",
                                 fix: "TypeNameFormatter.Format has to recurse into GetGenericArguments() and join them with '<' and '>' instead of falling back to Type.Name.");
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
