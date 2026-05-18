using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class UseTestCreatorExtension
    {
        public static void UseTestCreator(this IApplicationBuilder app)
        {
            app.UseMiddleware<TestCreatorMiddleware>();
        }
    }
}