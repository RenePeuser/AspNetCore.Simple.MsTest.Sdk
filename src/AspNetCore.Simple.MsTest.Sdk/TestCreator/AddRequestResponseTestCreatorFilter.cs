using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddRequestResponseTestCreatorFilter
    {
        public static void AddTestCreator(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPostWithBodyTestCreator(configuration);
            services.AddRequestTestCreator();
            services.AddTestCreatorMiddleware();
            services.AddTestCreatorActionFilter();
        }
    }
}
