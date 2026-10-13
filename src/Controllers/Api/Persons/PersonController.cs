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
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
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
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public Task<Person> GetPersonByIdAsync(long id)
        {
            var person = _persons.FirstOrDefault(x => x.Id == id);

            if (person.IsNull())
            {
                throw new ProblemDetailsException(System.Net.HttpStatusCode.NotFound,
                                                  "Person not found",
                                                  $"The person with the Id: {id} does not exist",
                                                  ("Id", id));
            }

            return Task.FromResult(person);
        }

        /// <summary>
        /// Declares the lean <see cref="PersonWithoutEmails"/> as its response type but writes a full
        /// <see cref="Person"/>, so <c>emails</c> only ever exists on the wire. Fixture for the snapshot
        /// diff: the current side has to stay the raw body, or that field silently disappears.
        /// </summary>
        [HttpGet("{id}/lean")]
        [ProducesResponseType(typeof(PersonWithoutEmails), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult GetPersonLean(long id)
        {
            var person = _persons.FirstOrDefault(x => x.Id == id);

            if (person.IsNull())
            {
                throw new ProblemDetailsException(System.Net.HttpStatusCode.NotFound,
                                                  "Person not found",
                                                  $"The person with the Id: {id} does not exist",
                                                  ("Id", id));
            }

            return Ok(person);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Person), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult UpdateAsync([FromBody] Person person)
        {
            ArgumentNullException.ThrowIfNull(person);

            if (person.Id == 999)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Person not found"
                });
            }

            return Ok(person);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Person), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult UpdateByIdAsync(long id,
                                             [FromBody] Person person)
        {
            ArgumentNullException.ThrowIfNull(person);

            if (id == 999)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Person not found"
                });
            }

            return Ok(person);
        }

        [HttpPatch]
        [ProducesResponseType(typeof(Person), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        [ProducesResponseType(typeof(ProblemDetails), 409)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public Task<Person> PatchAsync()
        {
            return Task.FromResult(_persons.First());
        }

        [HttpPost]
        [ProducesResponseType(typeof(Person), 201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult AddAsync([FromBody] Person person)
        {
            ArgumentNullException.ThrowIfNull(person);

            if (string.IsNullOrWhiteSpace(person.Name))
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Invalid request"
                });
            }

            return Ok(person);
        }

        /// <summary>
        /// Reflects a custom request header back in the response body.
        /// Exists so a test can PROVE that a request header actually left the client — custom headers
        /// used to be dropped silently on the typed assertion path, and no test could catch it because
        /// no action ever looked at one.
        /// </summary>
        [HttpGet("echo-header")]
        [ProducesResponseType(typeof(EchoHeaderResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public EchoHeaderResponse EchoHeader([FromHeader(Name = "X-Correlation-Id")] string? correlationId = null)
        {
            return new EchoHeaderResponse(correlationId ?? "(absent)");
        }

        /// <summary>
        /// Echoes the query string back, raw and as model binding decoded it. Exists so a test can PROVE how
        /// <c>WithQueryParameter</c> encodes against a controller action. GET for the body-less request stage,
        /// POST for the body stage (the body is ignored).
        /// </summary>
        [AcceptVerbs("GET", "POST", Route = "echo-query")]
        [ProducesResponseType(typeof(EchoQueryResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public EchoQueryResponse EchoQuery()
        {
            return new EchoQueryResponse(Request.QueryString.Value ?? string.Empty,
                                         Request.Query.ToDictionary(entry => entry.Key,
                                                                    entry => entry.Value.Select(value => value ?? string.Empty).ToArray()));
        }

        /// <summary>
        /// Echoes the <c>Authorization</c> request header back. Exists so a test can PROVE which credentials left the
        /// client: the per-request token of <c>WithBearerToken</c>, the client's default, or none after
        /// <c>WithoutAuthorization</c>.
        /// </summary>
        [AcceptVerbs("GET", "POST", Route = "echo-authorization")]
        [ProducesResponseType(typeof(EchoAuthorizationResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public EchoAuthorizationResponse EchoAuthorization()
        {
            return new EchoAuthorizationResponse(Request.Headers.Authorization.Count == 0
                                                     ? "(absent)"
                                                     : Request.Headers.Authorization.ToString());
        }

        /// <summary>
        /// 401 unless the request carries exactly <c>Authorization: Bearer valid-token</c> - a stand-in for a
        /// protected action without wiring a real authentication scheme into the sample.
        /// </summary>
        [AcceptVerbs("GET", "POST", "PUT", "PATCH", "DELETE", "QUERY", Route = "requires-token")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        public IActionResult RequiresToken()
        {
            return Request.Headers.Authorization.ToString() == "Bearer valid-token"
                       ? NoContent()
                       : Unauthorized();
        }

        /// <summary>Echoes the <c>X-Correlation-Id</c> request header and the request body back.</summary>
        [HttpPost("echo-request")]
        [ProducesResponseType(typeof(EchoRequestResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public EchoRequestResponse EchoRequest([FromHeader(Name = "X-Correlation-Id")] string? correlationId,
                                               [FromBody] EchoRequestBody body)
        {
            ArgumentNullException.ThrowIfNull(body);

            return new EchoRequestResponse(correlationId ?? "(absent)", body.Name);
        }

        /// <summary>
        /// The positive OPTIONS sample: 204 without a body, the advertised verbs in a CONTENT header (<c>Allow</c>)
        /// AND a RESPONSE header (<c>Access-Control-Allow-Methods</c>) - one OPTIONS assert has to see both.
        /// </summary>
        [HttpOptions("echo-request")]
        [ProducesResponseType(204)]
        public IActionResult EchoRequestOptions()
        {
            Response.Headers.Allow = "POST, OPTIONS";
            Response.Headers.AccessControlAllowMethods = "POST, OPTIONS";

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(statusCode: 204)]
        [ProducesResponseType(typeof(ProblemDetails), statusCode: 404)]
        public IActionResult Delete(long id)
        {
            if (id == 999)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Person not found"
                });
            }

            return NoContent();
        }

        [HttpDelete("{id}/with-response")]
        [ProducesResponseType(typeof(DeletePersonResponse), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public IActionResult DeleteWithResponse(long id)
        {
            // Simple test implementation - always returns success response
            return Ok(new DeletePersonResponse(id, "Son", "Goku",
                                               true, DateTime.UtcNow));
        }

        [AcceptVerbs("QUERY")]
        [Route("")]
        [ProducesResponseType(typeof(IEnumerable<Person>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IEnumerable<Person> QueryPersons([FromQuery] string name = "")
        {
            if (name.IsNotNullOrWhiteSpace())
            {
                return _persons.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            return _persons;
        }

        [AcceptVerbs("QUERY")]
        [Route("search")]
        [ProducesResponseType(typeof(IEnumerable<Person>), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public IActionResult QueryPersonsWithBody([FromBody] PersonSearchRequest searchRequest)
        {
            ArgumentNullException.ThrowIfNull(searchRequest);

            var results = _persons.AsEnumerable();

            if (searchRequest.Name.IsNotNullOrWhiteSpace())
            {
                results = results.Where(p => p.Name.Contains(searchRequest.Name, StringComparison.OrdinalIgnoreCase));
            }

            if (searchRequest.MinAge.HasValue)
            {
                results = results.Where(p => p.Age >= searchRequest.MinAge.Value);
            }

            if (searchRequest.MaxAge.HasValue)
            {
                results = results.Where(p => p.Age <= searchRequest.MaxAge.Value);
            }

            return Ok(results);
        }
    }

    public record DeletePersonResponse(long Id,
                                       string Name,
                                       string FirstName,
                                       bool Deleted,
                                       DateTime DeletedAt);

    public record PersonSearchRequest(string? Name,
                                      int? MinAge,
                                      int? MaxAge);
}