using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCore.Simple.MsTest.Sdk.Api.Controllers
{
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/native-types")]
    public class NativeTypeController : ControllerBase
    {
        [HttpGet("string")]
        public string GetString()
        {
            return "String only";
        }

        [HttpGet("int")]
        public int GetInt()
        {
            return 42;
        }
    }
}
