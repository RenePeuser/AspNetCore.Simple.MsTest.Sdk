using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Person = AspNetCore.Simple.MsTest.Sdk.Api.Models.Person;

namespace AspNetCore.Simple.MsTest.Sdk.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/persons")]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return new List<Person>() { new Person("Son", "Goku", 99), new Person("Vegeta", "Unknown", 77) };
        }

        [HttpPost]
        public IActionResult Post(Person person)
        {
            return Ok();
        }
    }
}
