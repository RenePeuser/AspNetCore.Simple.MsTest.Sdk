using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MinimalApi.ErrorHandling.Strategies.Specific
{
    public interface ISpecificErrorHandler
    {
        Task<bool> HandleExceptionAsync(HttpContext context,
                                        Exception exception,
                                        bool lastResult);
    }
}