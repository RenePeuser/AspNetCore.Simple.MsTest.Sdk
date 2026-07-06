using AspNetCore.Simple.Sdk.ErrorHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Api.Errors
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/errors")]
    public class ErrorsController : ControllerBase
    {
        [HttpPost("not-implemented")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public void ThrowNotImplementedException()
        {
            throw new ProblemDetailsException("Implementation is missing",
                                              "Here are error details",
                                              ("PropertyA", "A"),
                                              ("PropertyB", "B"));
        }

        [AcceptVerbs("QUERY")]
        [Route("query-not-implemented")]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public void ThrowNotImplementedExceptionOnQuery()
        {
            throw new ProblemDetailsException("Query implementation is missing",
                                              "Error details for QUERY method",
                                              ("QueryPropertyA", "A"),
                                              ("QueryPropertyB", "B"));
        }
    }
}