using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PackageSmoke
{
    /// <summary>The corrected chain: it ends with ExecuteAsync() and builds against the packed sdk.</summary>
    [TestClass]
    public sealed class SmokeTests
    {
        private static readonly HttpClient Client = new();

        [TestMethod]
        public Task Chain_Ends_With_ExecuteAsync()
        {
            return Client.AssertGet("api/v1/persons")
                         .Produces(HttpStatusCode.OK)
                         .ExecuteAsync();
        }
    }
}
