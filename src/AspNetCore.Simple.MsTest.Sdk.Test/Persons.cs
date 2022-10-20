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
            return Client.AssertGetAsync<IEnumerable<Person>>("api/tests/v1/persons", "[{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":99},{\"Id\":2,\"Name\":\"Vegeta\",\"FirstName\":\"Unknown\",\"Age\":77}]");
        }

        [DataTestMethod]
        [DataRow("I am not a valid json")]
        [DataRow("1234")]
        [DataRow("@abc jnd")]
        [DataRow("{dsdmsd")]
        [DataRow("I am not a valid json}")]
        public async Task Should_Throw_Exception_If_Json_Is_Invalid(string invalidJson)
        {
            var exception = await Assert.ThrowsExceptionAsync<InvalidJsonException>(() => Client.AssertGetAsync<IEnumerable<Person>>("api/tests/v1/persons", invalidJson)).ConfigureAwait(false);
            Assert.AreEqual(exception.Message, $"Your given json string does not contains a valid json string. Json strings have to begin with '{{' and end with a '}}' or if you use an array notation then []\r\nYour invalid string is:\r\n{invalidJson}");
        }
    }
}
