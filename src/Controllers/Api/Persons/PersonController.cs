using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Simple.Sdk.ErrorHandling;
using Extensions.Pack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Api.Persons
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/persons")]
    public sealed class PersonController : ControllerBase
    {
        private readonly List<Person> _persons =
        [
            new(1, "Son", "Goku",
                99, ImmutableList.Create(new Email("alf@gmx.de", "GMX"), new Email("abc@hotmail.de", "Microsoft"))),
            new(2, "Vegeta", "Unknown",
                77, ImmutableList.Create(new Email("abc@gmx.de", "GMX"), new Email("maxmustermann@hotmail.de", "Microsoft")))
        ];

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Person>), 200)]
        public IEnumerable<Person> GetAllPersons([FromQuery] string name = "")
        {
            if (name.IsNotNullOrWhiteSpace())
            {
                return _persons.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            return _persons;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Person), 200)]
        [ProducesResponseType(typeof(Person), 404)]
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
        [ProducesResponseType(typeof(Person), 200)]
        public Task<Person> UpdateAsync([FromBody] Person person)
        {
            return Task.FromResult(person);
        }

        [HttpPatch]
        [ProducesResponseType(typeof(Person), 200)]
        public Task<Person> PatchAsync()
        {
            return Task.FromResult(_persons.First());
        }

        [HttpPost]
        [ProducesResponseType(typeof(Person), 200)]
        public Task<Person> AddAsync([FromBody] Person person)
        {
            return Task.FromResult(person);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        public IActionResult Delete(long id)
        {
            // Simple test implementation - always succeeds
            Console.WriteLine(id);

            return NoContent();
        }

        [HttpDelete("{id}/with-response")]
        [ProducesResponseType(typeof(DeletePersonResponse), 200)]
        public IActionResult DeleteWithResponse(long id)
        {
            // Simple test implementation - always returns success response
            return Ok(new DeletePersonResponse(id, "Son", "Goku",
                                               true, DateTime.UtcNow));
        }
    }

    public record DeletePersonResponse(long Id,
                                       string Name,
                                       string FirstName,
                                       bool Deleted,
                                       DateTime DeletedAt);
}