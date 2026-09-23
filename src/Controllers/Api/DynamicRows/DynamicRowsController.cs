using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers.Api.DynamicRows
{
    /// <summary>
    /// An endpoint whose payload is not modelled by a dto but by a dictionary - the shape every api
    /// that returns database rows with dynamic columns ends up with. The keys are the column names,
    /// so they keep their own casing and are not touched by the property naming policy.
    /// </summary>
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Route("v{version:apiversion}/dynamic-rows")]
    public sealed class DynamicRowsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(DynamicRowsResponse), 200)]
        public DynamicRowsResponse GetRows()
        {
            return new DynamicRowsResponse
                   {
                       Page = new RowPage
                              {
                                  Items =
                                  [
                                      new Dictionary<string, object?>
                                      {
                                          ["Transition"] = "FC_TOP_GBS_SMO",
                                          ["Comment"] = "Volume increase ME",
                                          ["Amount"] = 30499.0
                                      }
                                  ],
                                  TotalItems = 1
                              }
                   };
        }
    }

    public sealed record DynamicRowsResponse
    {
        public required RowPage Page { get; init; }
    }

    public sealed record RowPage
    {
        public required IReadOnlyList<Dictionary<string, object?>> Items { get; init; }

        public int TotalItems { get; init; }
    }
}