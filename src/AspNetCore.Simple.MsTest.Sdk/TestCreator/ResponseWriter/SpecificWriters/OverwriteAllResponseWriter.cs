using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCore.Simple.MsTest.Sdk
{
    internal static class AddOverwriteAllResponseWriterExtension
    {
        internal static void AddOverwriteAllResponseWriter(this IServiceCollection services)
        {
            services.AddSingletonIfNotExists<ISpecificResponseWriter, OverwriteAllResponseWriter>();
        }
    }

    internal sealed class OverwriteAllResponseWriter : ISpecificResponseWriter
    {
        public bool CanHandle(WriteResponseRequest context)
        {
            return context.Mode == ResponseWriteMode.OverwriteAll;
        }

        public void Write(WriteResponseRequest context)
        {
            if (context.CallingAssembly.IsCompiledInDebug().IsFalse())
            {
                return;
            }

            if (context.ExpectedResult.EmbeddedFileName.IsNullOrWhiteSpace())
            {
                return;
            }

            var currentJson = JToken.Parse(context.CurrentResponseAsString);
            var formattedCurrent = currentJson.ToString(Formatting.Indented);

            foreach (var (key, value) in context.Parameters)
            {
                var oldValue = value?.ToString();
                if (!oldValue.IsNullOrEmpty())
                {
                    formattedCurrent = formattedCurrent.Replace(oldValue, key);
                }
            }

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName, formattedCurrent);
        }
    }
}
