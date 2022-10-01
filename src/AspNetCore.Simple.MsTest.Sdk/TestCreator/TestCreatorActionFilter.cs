using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public class TestCreatorActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var returnType = context.ActionDescriptor.GetReturnType();

            context.HttpContext.Response.Headers.Add("returntype", returnType.ToString());
            context.HttpContext.Response.Headers.Add("returntype-assembly", returnType.AssemblyQualifiedName);
            context.HttpContext.Response.Headers.Add("csproj", $"{context.Controller.GetType().Assembly.GetName().Name}.csproj");
            context.HttpContext.Response.Headers.Add("controller-name", context.Controller.GetType().FullName);
            context.HttpContext.Response.Headers.Add("assembly-location", context.Controller.GetType().Assembly.Location);

            //Continue down the Middleware pipeline, eventually returning to this class
            await next().ConfigureAwait(false);
        }
    }
}
