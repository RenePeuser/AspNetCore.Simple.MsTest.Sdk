using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Api.Versioning
{
    // ═══════════════════════════════════════════════════════════════════════════════════
    // 🎯 V1 AND V2 SIDE BY SIDE
    // ═══════════════════════════════════════════════════════════════════════════════════
    //
    // The typical migration phase of an API: V1 and V2 run at the same time.
    // - "versioning/number" exists once per version - the same route, two actions
    // - "versioning/shared" is one action mapped to V1 and V2 explicitly
    // - "versioning/implicit" has no MapToApiVersion and serves every version of the controller
    //
    // The controller declares 1 and 2 for every action in it. Endpoint validation has to use
    // the version each action is mapped to (MapToApiVersion), otherwise both "number" actions
    // look like V1 and the tests fail with AMBIGUOUS ENDPOINT MATCH / ENDPOINT NOT FOUND.
    //
    // [ApiController] is required: Microsoft.AspNetCore.Mvc.Versioning (UseApiBehavior = true)
    // only applies versioning to api controllers. Without it [MapToApiVersion] is ignored by
    // routing and the app itself answers 500 AmbiguousMatchException.
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    [Route("v{version:apiversion}/versioning")]
    public sealed class VersioningController : ControllerBase
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

        [HttpGet("shared")]
        [MapToApiVersion("1.0")]
        [MapToApiVersion("2.0")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetShared()
        {
            return new JsonResult(7);
        }

        [HttpGet("implicit")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(int), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        public IActionResult GetImplicit()
        {
            return new JsonResult(9);
        }
    }
}
