using System;
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
            throw new NotImplementedException("Implementation is missing");
        }
    }
}
