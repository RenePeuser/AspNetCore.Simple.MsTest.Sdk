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
        [ProducesResponseType(typeof(string), 200)]
        public string GetString()
        {
            return "String only";
        }

        [HttpGet("int")]
        [ProducesResponseType(typeof(int), 200)]
        public int GetInt()
        {
            return 42;
        }
    }
}
