using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Simple.MsTest.Sdk.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/persons")]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return new List<Person>() { new Person(1, "Son", "Goku", 99), new Person(2, "Vegeta", "Unknown", 77) };
        }

        [HttpGet("/{id}")]
        public Task<Person> Get(long id)
        {
            return Task.FromResult(new Person(id, "son", "goku", 55));
        }

        [HttpPost]
        public Task<Person> Post([FromBody] Person person)
        {
            return Task.FromResult(person);
        }
    }
}
