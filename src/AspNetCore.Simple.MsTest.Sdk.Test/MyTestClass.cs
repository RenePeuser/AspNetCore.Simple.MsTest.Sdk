using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AspNetCore.Simple.MsTest.Sdk.Test
{
    [TestClass]
    public class MyTestClass : MsTestBase
    {
        [TestMethod]
        public async Task Should_Return_My_Expected_Results()
        {
            var persons = await Client.GetAsAsync<IEnumerable<Person>>("/myApi/persons").ConfigureAwait(false);

            var expectedResult = EmbeddedFile.GetFileContentFrom("get-expected-person-result.json");

            Assert.That.ObjectsAreEqual(() => expectedResult, () => persons);
        }
    }
}