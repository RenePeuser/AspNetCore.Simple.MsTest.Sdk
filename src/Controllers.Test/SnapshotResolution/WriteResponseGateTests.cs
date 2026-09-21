using AspNetCore.Simple.MsTest.Sdk;
using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Controllers.Test.SnapshotResolution
{
    /// <summary>
    ///     Snapshot recording is a Debug-only feature, and the gate that enforces it used to return false
    ///     without a trace of why. That is the shape of failure this class exists for: the author passes
    ///     writeResponse, no file appears, nothing is logged, and the identical not-found error comes
    ///     back on the next run - so the sdk looks broken while it is working exactly as designed.
    /// </summary>
    [TestClass]
    [TestCategory("SnapshotResolution")]
    public sealed class WriteResponseGateTests
    {
        [TestMethod]
        public void AnExplicitRequestMustStillBeRefusedForANonDebugAssembly()
        {
            // Any assembly shipped in Release does - the framework's own is guaranteed to be there.
            var releaseAssembly = typeof(string).Assembly;

            Assert.That.IsFalse(releaseAssembly.IsCompiledInDebug(),
                                because: "The whole test rests on this assembly standing in for a Release build. If it ever reported Debug, the assertion below would pass for the wrong reason.",
                                fix: "Pick a different assembly that is genuinely shipped in Release.");

            Assert.That.IsFalse(Service().ShouldWriteResponse(true, releaseAssembly),
                                because: "Writing into a source tree from an optimized build is not something the sdk should do on its own - the Debug gate is deliberate and has to hold even when the caller asks explicitly.",
                                fix: "Check the IsCompiledInDebug guard at the top of WriteResponseService.ShouldWriteResponse.");
        }

        /// <summary>
        ///     The gate is the ONLY reason an explicit request may be refused. Pinning it against the
        ///     current build keeps the test honest in a Debug run and in a Release run alike - and it is
        ///     what fails if someone adds a second, quieter condition in front of it.
        /// </summary>
        [TestMethod]
        public void TheDebugBuildMustBeTheOnlyThingThatDecidesAnExplicitRequest()
        {
            var thisAssembly = typeof(WriteResponseGateTests).Assembly;

            Assert.That.AreEqual(thisAssembly.IsCompiledInDebug(),
                                 Service().ShouldWriteResponse(true, thisAssembly),
                                 because: "An explicit writeResponse has to be honoured in a Debug build and refused in a Release one - nothing else may enter that decision. An extra condition would silence recording for reasons no message ever mentions.",
                                 fix: "Check WriteResponseService.ShouldWriteResponse - after the Debug gate an explicit request must map straight through.");
        }

        [TestMethod]
        public void WithoutAnyRequestNothingIsWritten()
        {
            Assert.That.IsFalse(Service().ShouldWriteResponse(false, typeof(WriteResponseGateTests).Assembly),
                                because: "Recording has to stay opt-in. A default that writes would rewrite every snapshot of an ordinary run and hide real differences behind a clean diff.",
                                fix: "Check that WriteResponseService returns false when neither the parameter, the static flag nor the environment variable asked for it.");
        }

        private static IWriteResponseService Service()
        {
            var services = new ServiceCollection();

            services.AddWriteResponseService();

            return services.BuildServiceProvider().GetRequiredService<IWriteResponseService>();
        }
    }
}
