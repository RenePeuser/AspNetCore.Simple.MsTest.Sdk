using System.Reflection;
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
        // Very important we do only write in DEBUG mode this is a pure Developer feature !
        bool ShouldWriteResponse(bool scopedWriteResponse,
                                 Assembly callingAssembly);
    }

    internal sealed class WriteResponseService : IWriteResponseService
    {
        public bool ShouldWriteResponse(bool scopedWriteResponse,
                                        Assembly callingAssembly)
        {
            if (callingAssembly.IsCompiledInDebug().IsFalse())
            {
                return false;
            }

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
