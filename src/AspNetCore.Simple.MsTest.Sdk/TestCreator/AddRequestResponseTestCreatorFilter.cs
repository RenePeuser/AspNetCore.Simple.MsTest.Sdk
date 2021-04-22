using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public static class AddRequestResponseTestCreatorFilter
    {
        public static void AddTestCreator(this IServiceCollection services)
        {
            services.AddSingleton<ISpecificTestCreator, PostWithBodyTestCreator>();
            services.AddSingleton<IRequestTestCreator, RequestTestCreator>();

            services.AddSingleton<TestCreatorMiddleware>();

            services.AddMvc(options => options.Filters.Add<TestCreatorActionFilter>());
        }
    }
}