using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Api.Versioning
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 V1 AND V2 SIDE BY SIDE - VERSION VIA QUERY STRING OR HEADER
    // ═══════════════════════════════════════════════════════════════════════════════════
    //
    // Same pair as VersioningController, but the route carries no version:
    // - api/versioning-query/number?api-version=2
    // - api/versioning-header/number   +   x-api-version: 2
    //
    // The ApiVersionReader in Startup reads both. Endpoint validation has to read the version
    // from the same place, otherwise both "number" actions match (AMBIGUOUS ENDPOINT MATCH).
    public abstract class NumberPerVersionControllerBase : ControllerBase
    {
        [HttpGet("number")]
        [MapToApiVersion("1.0")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetNumberV1()
        {
            return new JsonResult(1);
        }

        [HttpGet("number")]
        [MapToApiVersion("2.0")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetNumberV2()
        {
            return new JsonResult("one");
        }
    }

    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("versioning-query")]
    public sealed class VersioningQueryController : NumberPerVersionControllerBase;

    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("versioning-header")]
    public sealed class VersioningHeaderController : NumberPerVersionControllerBase;
}
