using System.Collections.Generic;
using System.Collections.Immutable;
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
            return Client.AssertGetAsync<IEnumerable<Person>>("api/tests/v1/persons",
                                                              "Results.GetPersonResponse.json");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Post_A_Person_Object()
        {
            return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                                  new Person(1, "Son", "Goku", 42, ImmutableList<Email>.Empty),
                                                  "Results.NewPerson.json");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Post_A_Person_Parameterized()
        {
            return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                                  "Payloads.NewPersonParameter.json",
                                                  "Results.NewPersonParameter.json",
                                                  [("$Name$", "Son"), ("$Age$", 42)]);
        }

        [TestMethod]
        public Task Should_Be_Able_To_Post_A_Person_By_Json()
        {
            return Client.AssertPostAsync<Person>("api/tests/v1/persons",
                                                  "Payloads.SonGoku.json",
                                                  "Results.SonGoku.json");
        }
        
        
        [TestMethod]
        public Task Should_Be_Able_To_Put_A_Person_By_Json()
        {
            return Client.AssertPatchAsync<Person>("api/tests/v1/persons",
                                                  "Payloads.SonGoku.json",
                                                  "Results.SonGoku.json");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Put_A_Patch_By_Json()
        {
            return Client.AssertPutAsync<Person>("api/tests/v1/persons",
                                                  "Payloads.SonGoku.json",
                                                  "Results.SonGokuNewResponse.json");
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
        
        //[TestMethod]
        //public Task Should_Be_Able_To_Put_A_Patch_By_Json_1()
        //{
        //    return Client.AssertPutAsync<Person?>("api/tests/v1/persons",
        //                                         "{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":99,\"Emails\":[{\"EmailAddress\":\"alf@gmx.de\",\"Type\":\"GMX\"},{\"EmailAddress\":\"abc@hotmail.de\",\"Type\":\"Microsoft\"}]}");
        //}
        
        //[TestMethod]
        //public Task Should_Be_Able_To_Put_A_Person_By_Json_1()
        //{
        //    return Client.AssertPatchAsync<Person>("api/tests/v1/persons",
        //                                           "Payloads.SonGoku.json");
        //}
        
        //[TestMethod]
        //public Task Should_Be_Able_To_Post_A_Person_By_Json_1()
        //{
        //    return Client.AssertPostAsync<Person>("api/tests/v1/persons",
        //                                          "Payloads.SonGoku.json");
        //}
    }
}
