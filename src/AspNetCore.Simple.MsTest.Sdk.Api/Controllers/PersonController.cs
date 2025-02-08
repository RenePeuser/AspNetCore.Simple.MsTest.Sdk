using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Simple.MsTest.Sdk.Api.Models;
using AspNetCore.Simple.Sdk.ErrorHandling;
using Extensions.Pack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Simple.MsTest.Sdk.Api.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/persons")]
    public class PersonController : ControllerBase
    {
        private readonly List<Person> _persons =
        [
            new(1, "Son", "Goku",
                99, ImmutableList.Create(new Email("alf@gmx.de", "GMX"), new Email("abc@hotmail.de", "Microsoft"))),
            new(2, "Vegeta", "Unknown",
                77, ImmutableList.Create(new Email("abc@gmx.de", "GMX"), new Email("maxmustermann@hotmail.de", "Microsoft")))
        ];

        [HttpGet]
        public IEnumerable<Person> GetAllPersons()
        {
            return _persons;
        }

        [HttpGet("{id}")]
        public Task<Person> GetPersonByIdAsync(long id)
        {
            var person = _persons.FirstOrDefault(x => x.Id == id);

            if (person.IsNull())
            {
                throw new ProblemDetailsException("Person for given Id does not exist",
                                                  $"The person with the Id: {id} does not exist",
                                                  ("Id", id));
            }

            return Task.FromResult(person);
        }

        [HttpPut]
        public Task<Person> UpdateAsync([FromBody] Person person)
        {
            return Task.FromResult(person);
        }

        [HttpPatch]
        public Task<Person> PatchAsync()
        {
            return Task.FromResult(_persons.First());
        }

        [HttpPost]
        public Task<Person> AddAsync([FromBody] Person person)
        {
            return Task.FromResult(person);
        }
    }
}
