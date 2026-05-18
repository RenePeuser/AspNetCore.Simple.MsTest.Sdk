using Extensions.Pack;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.Simple.MsTest.Sdk
{
    public static class AddOverwriteAllResponseWriterExtension
    {
        public static void AddOverwriteAllResponseWriter(this IServiceCollection services)
        {
            services.AddParameterReplacer();
            services.AddSingletonIfNotExists<ISpecificResponseWriter, OverwriteAllResponseWriter>();
        }
    }

    internal sealed class OverwriteAllResponseWriter(IParameterReplacer parameterReplacementService) : ISpecificResponseWriter
    {
        public bool CanHandle(WriteResponseRequest context)
        {
            var canHandle = context.ExpectedResult.EmbeddedFileName.EndsWith(".json") &&
                            context.ExpectedResult.EmbeddedFile.IsNotNull() &&
                            (context.Mode == ResponseWriteMode.OverwriteAll || context.ExpectedResult.EmbeddedFile.NotExists());

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

            var result = parameterReplacementService.ReplaceWithPlaceholders(context.CurrentResponseAsString, context.Parameters);

            //// New we can have also indexer properties. Values[0] -> Values[$Index$]
            //foreach (var parameter in context.Parameters)
            //{
            //    result = GlobalRegex.IndexReplacement().Replace(result, $"[{parameter.key}]");
            //}

            File.WriteAllText(context.ExpectedResult.EmbeddedFile!.FullName, result);
        }
    }
}