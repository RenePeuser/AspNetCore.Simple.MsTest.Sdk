using System.Collections.Immutable;
using AspNetCore.Simple.MsTest.Sdk;
using MinimalApi.Api.Persons.V1;

namespace MinimalApi.Test.Api.Persons
{
    [TestClass]
    [TestCategory("Minimal Api")]
    public class PersonEndpointsTests : ApiTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload_Ignore_Id()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetPersonResponse.json",
                                                              DifferenceFunc);
        }

        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload_With_Post_Sort()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetPersonResponse.json",
                                                              FilterFunc);
        }

        [Ignore("Fails in Ci because of formatting")]
        [TestMethod]
        public async Task Invalid_Response_Type_Json_Exception()
        {
            var error = await Assert.ThrowsExactlyAsync<AssertFailedException>(() => Client.AssertGetAsync<UnknownResponse>("api/v1/persons",
                                                                                                                            "GetPersonResponse.json")).ConfigureAwait(false);

            Assert.That.ObjectsAreEqual(expectedObjectAsJson: "InvalidResponseType.txt", currentObject: error.Message);
        }

        private IEnumerable<Person>? FilterFunc(IEnumerable<Person>? arg)
        {
            return arg?.OrderBy(x => x.Id).ToImmutableList();
        }

        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", /*lang=json,strict*/ "{\"content\":{\"headers\":[{\"key\":\"Content-Type\",\"value\":[\"application/json; charset=utf-8\"]}],\"value\":[{\"id\":1,\"name\":\"Son\",\"firstName\":\"Goku\",\"age\":99,\"emails\":[{\"emailAddress\":\"alf@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"abc@hotmail.de\",\"type\":\"Microsoft\"}]},{\"id\":2,\"name\":\"Vegeta\",\"firstName\":\"Unknown\",\"age\":77,\"emails\":[{\"emailAddress\":\"abc@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"maxmustermann@hotmail.de\",\"type\":\"Microsoft\"}]}]},\"statusCode\":\"OK\",\"headers\":[],\"trailingHeaders\":[],\"isSuccessStatusCode\":true}");
        }

        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload_By_Embedded_File()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons",
                                                              "GetPersonResponse.json");
        }

        [TestMethod]
        public Task Should_Filter_Persons_By_Name_Query_Parameter()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons?name=Son",
                                                              "GetPersonFilteredResponse.json");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Post_A_Person_Object()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  new Person(1, "Son", "Goku",
                                                             42, ImmutableList<Email>.Empty),
                                                  "NewPerson.json");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Post_A_Person_Parameterized()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "NewPersonParameter.json",
                                                  "NewPersonParameter.json",
                                                  parameters:
                                                  [
                                                      ("$Name$", "Son"),
                                                      ("$Age$", 42)
                                                  ]);
        }

        [TestMethod]
        public Task Should_Be_Able_To_Post_A_Person_Parameterized_With_Absolute_Embedded_Filepath()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "AnyFolder.P.NewPersonParameter.json",
                                                  "AnyFolder.R.NewPersonParameter.json",
                                                  parameters: [("$Name$", "Son"), ("$Age$", 42)],
                                                  differenceFunc: DifferenceFunc);
        }

        [Ignore("Not supported ! if path is not correct it fails !")]
        [TestMethod]
        public Task Should_Be_Able_To_Post_A_Person_By_Json()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "Payloads.SonGoku.json",
                                                  "Results.SonGoku.json");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Put_A_Person_By_Json()
        {
            return Client.AssertPatchAsync<Person>("api/v1/persons",
                                                   "SonGoku.json",
                                                   "SonGoku.json");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Put_A_Patch_By_Json()
        {
            return Client.AssertPutAsync<Person>("api/v1/persons",
                                                 "SonGoku.json",
                                                 "SonGokuNewResponse.json");
        }

        [TestMethod]
        [DataRow("I am not a valid json")]
        [DataRow("1234")]
        [DataRow("@abc jnd")]
        [DataRow("{dsdmsd")]
        [DataRow("I am not a valid json}")]
        public async Task Should_Throw_Exception_If_Json_Is_Invalid(string invalidJson)
        {
            var exception = await Assert.ThrowsExactlyAsync<InvalidJsonException>(() => Client.AssertGetAsync<IEnumerable<Person>>("api/v1/persons", invalidJson)).ConfigureAwait(false);
            Assert.Contains(invalidJson, exception.Message);
        }

        [TestMethod]
        public Task Should_Be_Able_To_Put_A_Patch_By_Json_1()
        {
            return Client.AssertPutAsync<Person?>("api/v1/persons",
                                                  /*lang=json,strict*/
                                                  "{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":99,\"Emails\":[{\"EmailAddress\":\"alf@gmx.de\",\"Type\":\"GMX\"},{\"EmailAddress\":\"abc@hotmail.de\",\"Type\":\"Microsoft\"}]}",
                                                  /*lang=json,strict*/
                                                  "{\"id\":1,\"name\":\"Son\",\"firstName\":\"Goku\",\"age\":99,\"emails\":[{\"emailAddress\":\"alf@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"abc@hotmail.de\",\"type\":\"Microsoft\"}]}");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Put_A_Person_By_Json_1()
        {
            return Client.AssertPatchAsync<Person>("api/v1/persons",
                                                   "Persons.Requests.SonGoku.json",
                                                   "Persons.Responses.SonGoku.json");
        }

        [TestMethod]
        public Task Should_Be_Able_To_Post_A_Person_By_Json_1()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "Requests.SonGoku.json",
                                                  "Responses.SonGoku.json");
        }

        [Ignore("Not supported ! if path is not correct it fails !")]
        [TestMethod]
        public Task Should_Be_Able_Return_Validation_Infos_Of_Invalid_Payload()
        {
            return Client.AssertPostAsync<Person>("api/v1/persons",
                                                  "Payloads.InvalidSonGoku.json",
                                                  "Responses.InvalidSonGoku.json");
        }

        private IEnumerable<Difference> DifferenceFunc(ImmutableList<Difference> differences)
        {
            foreach (var difference in differences)
            {
                if (difference.MemberPath == nameof(Person.Id))
                {
                    continue;
                }

                yield return difference;
            }
        }
    }

    internal sealed record UnknownResponse
    {
        public required string Property { get; init; }
    }
}
