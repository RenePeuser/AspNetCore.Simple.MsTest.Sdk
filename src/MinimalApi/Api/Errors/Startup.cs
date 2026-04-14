using MinimalApi.Api.Errors.V1;

namespace MinimalApi.Api.Errors
{
    internal static class Startup
    {
        internal static void AddErrors(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddErrorsV1();
        }
    }
}
