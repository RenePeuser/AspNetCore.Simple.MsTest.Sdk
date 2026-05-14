using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddRequestResponseTestCreatorFilter
    {
        public static void AddTestCreator(this IServiceCollection services,
                                          IConfiguration configuration)
        {
            services.AddTestCreator(configuration, output => Debug.WriteLine(output));
        }

        public static void AddTestCreator(this IServiceCollection services,
                                          IConfiguration configuration,
                                          Action<string> logAction)
        {
            HttpClientAssertExtensions.LogAction = logAction;

            services.AddPostWithBodyTestCreator(configuration);
            services.AddRequestTestCreator();
            services.AddTestCreatorMiddleware();

            //services.AddTestCreatorActionFilter();
        }
    }
}
