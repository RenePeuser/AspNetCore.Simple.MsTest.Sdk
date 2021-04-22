using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    public class UnitTest1 : MsTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/tests/v1/persons",
                "[{\"name\":\"Son\",\"firstName\":\"Goku\",\"age\":99},{\"name\":\"Vegeta\",\"firstName\":\"Unknown" +
                "\",\"age\":77}]");
        }
    }
}
