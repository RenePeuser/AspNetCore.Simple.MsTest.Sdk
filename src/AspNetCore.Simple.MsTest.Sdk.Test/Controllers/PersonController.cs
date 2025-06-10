using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test.Controllers
{
    [TestClass]
    public class PersonController : ApiTestBase
    {
        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("/api/tests/v1/persons", /*lang=json,strict*/ "{\"content\":{\"headers\":[{\"key\":\"Content-Type\",\"value\":[\"application/json; charset=utf-8\"]}],\"value\":[{\"id\":1,\"name\":\"Son\",\"firstName\":\"Goku\",\"age\":99,\"emails\":[{\"emailAddress\":\"alf@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"abc@hotmail.de\",\"type\":\"Microsoft\"}]},{\"id\":2,\"name\":\"Vegeta\",\"firstName\":\"Unknown\",\"age\":77,\"emails\":[{\"emailAddress\":\"abc@gmx.de\",\"type\":\"GMX\"},{\"emailAddress\":\"maxmustermann@hotmail.de\",\"type\":\"Microsoft\"}]}]},\"statusCode\":\"OK\",\"headers\":[],\"trailingHeaders\":[],\"isSuccessStatusCode\":true}");
        }

        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload_Ignore_Id()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("/api/tests/v1/persons",
                                                              "GetPersonResponse.json",
                                                              DifferenceFunc);
        }

        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload_With_Post_Sort()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("/api/tests/v1/persons",
                                                              "GetPersonResponse.json",
                                                              FilterFunc);
        }

        // The filter func can be used to sort or do some custom filtering
        // Sample: You get unsorted results from API so each call will provide
        //         the users in different order. You can sort them before comparison
        //         Because if order is not matching the Assert will fail
        private IEnumerable<Person> FilterFunc(IEnumerable<Person> arg)
        {
            return arg.OrderBy(x => x.Id).ToImmutableList();
        }

        // Difference func can be used to ignore some properties inside the object comparison
        private IEnumerable<Difference> DifferenceFunc(IImmutableList<Difference> differences)
        {
            foreach (var difference in differences)
            {
                // Here we ignore the Id property. Real world scenario generated id by database as an example
                if (difference.MemberPath == nameof(Person.Id))
                {
                    continue;
                }

                yield return difference;
            }
        }
    }
}
