using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test.Controllers
{
    [TestClass]
    public class PersonController : MsTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("/api/tests/v1/persons", /*lang=json,strict*/ "[{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":99},{\"Id\":2,\"Name\":\"Vegeta\",\"FirstName\":\"Unknown\",\"Age\":77}]");
        }
    }
}
