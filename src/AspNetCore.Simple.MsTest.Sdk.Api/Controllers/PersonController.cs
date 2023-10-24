using System.Collections.Generic;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Simple.MsTest.Sdk.Api.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/persons")]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Person> Get()
        {
            return new List<Person> { new(1, "Son", "Goku", 99), new(2, "Vegeta", "Unknown", 77) };
        }

        [HttpGet("{id}")]
        public Task<Person> Get(long id)
        {
            var result = new Person(id, "son", "goku", 55);
            return Task.FromResult(result);
        }

        [HttpPost]
        public Task<Person> Post([FromBody] Person person)
        {
            return Task.FromResult(person);
        }
    }
}
