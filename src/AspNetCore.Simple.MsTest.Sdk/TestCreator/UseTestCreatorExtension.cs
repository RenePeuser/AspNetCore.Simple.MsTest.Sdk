using Microsoft.AspNetCore.Builder;

namespace AspNetCore.Simple.MsTest.Sdk.TestCreator
{
    public static class UseTestCreatorExtension
    {
        public static void UseTestCreator(this IApplicationBuilder app)
        {
            app.UseMiddleware<TestCreatorMiddleware>();
        }
    }
}
