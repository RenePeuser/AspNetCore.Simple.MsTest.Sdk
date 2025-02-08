//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.Extensions.DependencyInjection;

//namespace AspNetCore.Simple.MsTest.Sdk
//{
//    internal static class AddTestCreatorActionFilterExtension
//    {
//        internal static void AddTestCreatorActionFilter(this IServiceCollection services)
//        {
//            services.AddMvc(options => options.Filters.Add<TestCreatorActionFilter>());
//        }
//    }

//    internal record ReturnTypeInfo(int StatusCode, string TypeName, string FullQualifiedName);

//    internal class TestCreatorActionFilter : IAsyncActionFilter
//    {
//        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//        {
//            var returnType = context.ActionDescriptor.GetReturnType();
//            var producesResponseTypes = context.ActionDescriptor.EndpointMetadata.OfType<ProducesResponseTypeAttribute>();

//            var returnTypes = producesResponseTypes.Select(pr => new ReturnTypeInfo(pr.StatusCode, pr.Type.Name, pr.Type.AssemblyQualifiedName!)).ToList();
//            // For case produce response types are missing we add the default, OK from return type and Error as ProblemDetails
//            if (returnTypes.IsEmpty())
//            {
//                returnTypes = new List<ReturnTypeInfo>()
//                {
//                    new ReturnTypeInfo(200, returnType.ToString(), returnType.AssemblyQualifiedName!),
//                    new ReturnTypeInfo(400, typeof(ProblemDetails).ToString(), typeof(ProblemDetails).AssemblyQualifiedName!)
//                };
//            }

//            context.HttpContext.Response.Headers.Add("returntypes", returnTypes.ToJson());
//            context.HttpContext.Response.Headers.Add("csproj", $"{context.Controller.GetType().Assembly.GetName().Name}.csproj");
//            context.HttpContext.Response.Headers.Add("controller-name", context.Controller.GetType().FullName);
//            context.HttpContext.Response.Headers.Add("assembly-location", context.Controller.GetType().Assembly.Location);

//            //Continue down the Middleware pipeline, eventually returning to this class
//            await next().ConfigureAwait(false);
//        }
//    }
//}


