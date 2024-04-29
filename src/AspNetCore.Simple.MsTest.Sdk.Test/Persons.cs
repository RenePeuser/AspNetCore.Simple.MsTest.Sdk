using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using Extensions.Pack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    public class Persons : MsTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/tests/v1/persons", /*lang=json,strict*/ "[{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":99,\"Emails\":[{\"EmailAddress\":\"alf@gmx.de\",\"Type\":\"GMX\"},{\"EmailAddress\":\"abc@hotmail.de\",\"Type\":\"Microsoft\"}]},{\"Id\":2,\"Name\":\"Vegeta\",\"FirstName\":\"Unknown\",\"Age\":77,\"Emails\":[{\"EmailAddress\":\"abc@gmx.de\",\"Type\":\"GMX\"},{\"EmailAddress\":\"maxmustermann@hotmail.de\",\"Type\":\"Microsoft\"}]}]");
        }

        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload_By_Embedded_File()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/tests/v1/persons", "Results.GetPersonResponse.json");
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
            Assert.IsTrue(exception.Message.Contains(invalidJson));
        }
    }
}
