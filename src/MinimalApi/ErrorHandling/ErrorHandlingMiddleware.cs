using Extensions.Pack;
using MinimalApi.ErrorHandling.Strategies;

namespace MinimalApi.ErrorHandling
{
    public static class AddErrorHandlingMiddlewareExtension
    {
        public static void AddErrorHandlingMiddleware(this IServiceCollection services)
        {
            services.AddErrorHandlingStrategy();

            services.AddSingletonIfNotExists<ErrorHandlingMiddleware>();
        }
    }

    // this middle ware is just for handling the errors, not for logging !!
    internal sealed class ErrorHandlingMiddleware(IErrorHandlingStrategy errorHandlingStrategy) : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context,
                                      RequestDelegate next)
        {
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await errorHandlingStrategy.HandleAsync(context, exception).ConfigureAwait(false);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}
