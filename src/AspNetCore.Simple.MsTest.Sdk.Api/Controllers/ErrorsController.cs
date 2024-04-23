using AspNetCore.Simple.Sdk.ErrorHandling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Simple.MsTest.Sdk.Api.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/errors")]
    public class ErrorsController : ControllerBase
    {
        [HttpPost("not-implemented")]
        public void ThrowNotImplementedException()
        {
            throw new ProblemDetailsException("Implementation is missing",
                                              "Here are error details",
                                              ("PropertyA", "A"),
                                              ("PropertyB", "B"));

        }
    }
}
