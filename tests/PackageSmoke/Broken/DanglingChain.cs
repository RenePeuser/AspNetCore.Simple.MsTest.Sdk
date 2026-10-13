using System.Net;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PackageSmoke
{
    /// <summary>
    /// Compiled only with -p:SmokeBroken=true: a chain without ExecuteAsync() sends nothing and would go green
    /// silently - the packaged analyzer has to stop the build with error MSTESTSDK001.
    /// </summary>
    [TestClass]
    public sealed class DanglingChain
    {
        [TestMethod]
        public void Chain_Without_ExecuteAsync()
        {
            using var client = new System.Net.Http.HttpClient();

            client.AssertGet("api/v1/persons")
                  .Produces(HttpStatusCode.OK);
        }
    }
}
