using System.Text.RegularExpressions;
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
            var canHandle = context.Mode == ResponseWriteMode.OverwriteAll ||
                            // A response file have to be exists
                            context.ExpectedResult.EmbeddedFile.IsNull() ||
                            context.ExpectedResult.EmbeddedFile.NotExists();

            return canHandle;
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

            var sortedParameters = context.Parameters.Select(p => new
            {
                Key = p.key,
                Value = p.Value?.ToString()
            }).OrderByDescending(p => p.Value?.Length).ToList();

            foreach (var parameter in sortedParameters)
            {
                var oldValue = parameter.Value;
                if (!oldValue.IsNullOrEmpty())
                {
                    formattedCurrent = Regex.Replace(formattedCurrent,
                                                     $@"\b{Regex.Escape(oldValue)}\b",
                                                     parameter.Key);
                }
            }

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName, formattedCurrent);
        }
    }
}
