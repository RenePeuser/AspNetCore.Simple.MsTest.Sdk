using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MinimalApi.ErrorHandling.Strategies.Specific;

namespace MinimalApi.ErrorHandling
{
    public static class ErrorHandlingExtensions
    {
        public static void AddErrorHandling(this IServiceCollection services)
        {
            services.AddErrorHandlingMiddleware();

            services.AddSecurityProblemExceptionHandler();
            services.AddProblemDetailsExceptionHandler();
            services.AddValidationProblemDetailsExceptionHandler();

            // Must be the last one, order of registrations means priority of handling exception type !!!
            services.AddDefaultExceptionHandler();
        }

        public static void UseErrorHandling(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}