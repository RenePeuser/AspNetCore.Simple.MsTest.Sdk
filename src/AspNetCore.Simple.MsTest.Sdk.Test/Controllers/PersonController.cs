using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ObjectsComparer;

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

        [TestMethod]
        public Task Should_Return_Expected_Result_For_Given_Payload_Ignore_Id()
        {
            return Client.AssertGetAsync<IEnumerable<Person>>("/api/tests/v1/persons",
                                                              /*lang=json,strict*/ "[{\"Id\":1,\"Name\":\"Son\",\"FirstName\":\"Goku\",\"Age\":99},{\"Id\":2,\"Name\":\"Vegeta\",\"FirstName\":\"Unknown\",\"Age\":77}]",
                                                              differenceFunc: DifferenceFunc);
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
