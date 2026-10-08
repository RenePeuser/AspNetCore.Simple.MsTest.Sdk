using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Api.SdkScenarios
{
    /// <summary>
    /// Endpoints that exist only to drive sdk behaviour from the outside - each one reproduces a shape
    /// a real api produces and the sdk once got wrong. See the tests named after each action.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/sdk-scenarios")]
    public sealed class SdkScenariosController : ControllerBase
    {
        /// <summary>Answers with headers that change on every single call - plus one that does not.</summary>
        [HttpGet("volatile-headers")]
        [ProducesResponseType(typeof(BlogPost), 200)]
        public BlogPost GetWithVolatileHeaders()
        {
            Response.Headers.TraceParent = $"00-{Guid.NewGuid():N}-{Guid.NewGuid().ToString("N")[..16]}-01";
            Response.Headers["X-AMZN-TRACE-ID"] = $"Root=1-{Guid.NewGuid():N}";
            Response.Headers["X-Business-Relevant"] = "keep me";

            return new BlogPost(new BlogContent("a blog post"), "Fresh");
        }

        /// <summary>A body that happens to carry 'content.value' - the envelope's own property names.</summary>
        [HttpGet("blog-post")]
        [ProducesResponseType(typeof(BlogPost), 200)]
        public BlogPost GetBlogPost()
        {
            return new BlogPost(new BlogContent("a blog post"), "Fresh");
        }

        /// <summary>The v2 contract - same short name as the v1 one.</summary>
        [HttpGet("versioned-contract")]
        [ProducesResponseType(typeof(V2.InsertOrUpdateOrDeleteResponse), 200)]
        public V2.InsertOrUpdateOrDeleteResponse GetVersionedContract()
        {
            return new V2.InsertOrUpdateOrDeleteResponse(1, "etag");
        }

        /// <summary>An awaitable without a result - there is no response body at all.</summary>
        [HttpDelete("bodyless/{id}")]
        [ProducesResponseType(204)]
#pragma warning disable IDE0060 // The route needs the id, the scenario does not.
        public Task DeleteAsync(long id)
#pragma warning restore IDE0060
        {
            Response.StatusCode = 204;

            return Task.CompletedTask;
        }
    }

    public sealed record BlogPost(BlogContent Content,
                                  string Title);

    public sealed record BlogContent(string Value);
}

namespace Controllers.Api.SdkScenarios.V1
{
    public sealed record InsertOrUpdateOrDeleteResponse(int Affected);
}

namespace Controllers.Api.SdkScenarios.V2
{
    public sealed record InsertOrUpdateOrDeleteResponse(int Affected,
                                                        string Etag);
}
