using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddWriteResponseServiceExtension
    {
        public static void AddWriteResponseService(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<IWriteResponseService, WriteResponseService>();
        }
    }

    public interface IWriteResponseService
    {
        bool ShouldWriteResponse(bool scopedWriteResponse);
    }

    internal sealed class WriteResponseService : IWriteResponseService
    {
        public bool ShouldWriteResponse(bool scopedWriteResponse)
        {
            if (scopedWriteResponse)
            {
                return true;
            }

            if (AssertObjectExtensions.WriteResponse)
            {
                return true;
            }

            var envVariable = Environment.GetEnvironmentVariable("AspNetCoreSimpleMsTestSdk__WriteResponse")?.ToBool();
            if (envVariable.IsNull())
            {
                return false;
            }

            return envVariable.Value;
        }
    }
}
