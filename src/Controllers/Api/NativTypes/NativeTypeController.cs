using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Api.NativTypes
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/native-types")]
    public class NativeTypeController : ControllerBase
    {
        [HttpGet("string")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        public IActionResult GetString()
        {
            return new JsonResult("String only");
        }

        [HttpGet("int")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), 200)]
        public IActionResult GetInt()
        {
            return new JsonResult(42);
        }
    }
}