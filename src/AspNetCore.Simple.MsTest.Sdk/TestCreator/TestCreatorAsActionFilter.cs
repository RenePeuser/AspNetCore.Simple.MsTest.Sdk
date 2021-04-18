using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public class TestCreatorAsActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var returnType = context.ActionDescriptor.GetReturnType();

            context.HttpContext.Response.Headers.Add("returntype", returnType.ToString());

            //Continue down the Middleware pipeline, eventually returning to this class
            await next().ConfigureAwait(false);
        }
    }
}