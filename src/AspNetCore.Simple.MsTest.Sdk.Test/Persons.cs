using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    public class Persons : MsTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/tests/v1/persons",
                "[{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":99},{\"Id\":2,\"Name\":\"Vegeta\",\"FirstName\":\"Unknown\",\"Age\":77}]");
        }
    }

    [TestClass]
    public class UrlExists : MsTestBase
    {
        [TestMethod]
        public void Should_Return_True_If_Absolute_Url_Exists()
        {
            Assert.IsTrue(Client.UrlExists("https://www.google.de/"));
        }
    }
}
